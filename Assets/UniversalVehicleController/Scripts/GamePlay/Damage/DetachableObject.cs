using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PG
{
    /// <summary>
    /// Detachable object (not damageable).
    /// </summary>
    [DisallowMultipleComponent]
    public class DetachableObject :MonoBehaviour
    {
        public float Mass = 0.1f;                                               //RidgidBody data is transferred to the created RB after the part is lost.
        public float Drag;                                                      //RidgidBody data is transferred to the created RB after the part is lost.
        public float AngularDrag = 0.05f;                                       //RidgidBody data is transferred to the created RB after the part is lost.

        public float LooseForce = 1;                                            //The force at which Joint is created. If it is <= 0 then the object will not detach.
        public float BreakForce = 25;                                           //Break force transferred to the created Joint.
        public PartJoint[] Joints;                                              //Possible Joints, when a part is lost, a random one is selected.
        public Vector3[] DamageCheckPoints = new Vector3[1] { Vector3.zero };   //Break check points, there must be at least one point.
        public bool UseLoseHealth = true;                                       //Use for the additional damage effect, if the flag is not set, then to lose the part, you need to get LooseForce in one collision
        public bool DisableCollidersBeetwenLoseAndDetach;                       //To temporarily disable the collider so that the wheels do not interact with this object.

        bool ChildsIsDestroyed;
        DamageableObject[] DestroyableChilds;
        Dictionary<Collider, bool> CollidersWithState = new Dictionary<Collider, bool>();

        Transform TR;
        Rigidbody RB;
        Rigidbody ParentBody;
        VehicleDamageController VehicleDamageController;
        VehicleController Vehicle;
        Vector3 OnStartJointPos;
        float LooseHealth;

        public bool IsLoose { get; private set; }                               //Lost part, that is, the part is associated with the Rigit Body of the car by a Joint.
        public bool IsDetached { get; private set; }                            //The part is completely detached from the car.
        public HingeJoint Hinge { get; private set; }
        public Vector3 InitialPos { get; private set; }
        PartJoint chosenJoint;
        float DetachTimer = 0;

        Quaternion InitialRot;
        Transform InitialParent;

        public event System.Action OnLoseAction;
        public event System.Action OnDetachAction;

        void Start ()
        {
            TR = transform;
            DestroyableChilds = GetComponentsInChildren<DamageableObject> ();

            var colliders = GetComponentsInChildren<Collider> (true);

            foreach (var c in colliders)
            {
                CollidersWithState.Add (c, c.enabled);
            }

            ParentBody = TR.GetTopmostParentComponent<Rigidbody> ();
            VehicleDamageController = GetComponentInParent<VehicleDamageController> ();
            if (VehicleDamageController)
            {
                Vehicle = VehicleDamageController.GetComponent<VehicleController> ();
            }

            InitialPos = TR.localPosition;
            InitialRot = TR.localRotation;
            InitialParent = TR.parent;

            LooseHealth = LooseForce;
            if (LooseForce == 0)
            {
                SetAsLoose ();
            }
        }

        void OnCollisionEnter (Collision col)
        {
            if (!IsDetached && IsLoose)
            {
                //Damage to a car if there is a connection with it through a joint.
                VehicleDamageController.OnCollisionEnter (col);
                if (Vehicle)
                {
                    Vehicle.OnCollisionEnter (col);
                }
            }
        }

        void Update ()
        {
            //Destruction of all damaged child objects.
            if (IsLoose)
            {
                if (!ChildsIsDestroyed)
                {
                    for (int i = 0; i < DestroyableChilds.Length; i++)
                    {
                        DestroyableChilds[i].Kill ();
                    }
                    ChildsIsDestroyed = true;
                }

                bool needJointBreak = !Hinge || transform.parent != null && (transform.parent.TransformPoint (OnStartJointPos) - transform.TransformPoint (Hinge.anchor)).sqrMagnitude > 0.25;

                if (Hinge && chosenJoint.UseDetachSpeed)
                {
                    //Applying a spring to a detachable part
                    float speedNormalize = Mathf.InverseLerp(0, chosenJoint.maxDetachSpeed, Vehicle.CurrentSpeed);
                    float signDirection = Mathf.Sign (chosenJoint.carDirection * Vehicle.VehicleDirection);
                    var spring = Hinge.spring;
                    spring.targetPosition = signDirection > 0? chosenJoint.springTargetPositionMaxSpeed: chosenJoint.springTargetPosition;
                    spring.spring = Mathf.Lerp (chosenJoint.springForce, chosenJoint.springForceMaxSpeed, speedNormalize);
                    Hinge.spring = spring;
                    Hinge.useSpring = true;

                    if (Vehicle.CurrentSpeed > chosenJoint.maxDetachSpeed && signDirection > 0)
                    {
                        DetachTimer += Time.deltaTime;
                        needJointBreak |= DetachTimer >= chosenJoint.DetachTime;
                    }
                    else
                    {
                        DetachTimer = 0;
                    }
                }


                if (needJointBreak)
                {
                    OnJointBreak ();
                }
            }
        }

        /// <summary>
        /// Forced Break Joint.
        /// </summary>
        public void OnJointBreak ()
        {
            this.enabled = false;
            IsDetached = true;
            if (Hinge)
            {
                Destroy (Hinge);
                RB.velocity *= 0.5f;
            }

            foreach (var cKV in CollidersWithState)
            {
                cKV.Key.enabled = true;
            }

            TR.parent = null;

            OnDetachAction.SafeInvoke ();
        }

        public void SetDamageForce (float force)
        {
            if (!IsLoose && LooseForce > 0)
            {
                if (UseLoseHealth)
                {
                    //Additional damage effect
                    LooseHealth -= force;
                    if (LooseHealth <= 0)
                    {
                        SetAsLoose ();
                    }
                }
                else
                {
                    //Check looseForce in one collision.
                    if (force >= LooseForce)
                    {
                        SetAsLoose ();
                    }
                }
            }
        }

        /// <summary>
        /// Loss of detail, here a ridge body is created, and a joint is created if the part is not completely lost and there is an available joint.
        /// </summary>
        public void SetAsLoose ()
        {
            if (!IsLoose)
            {
                IsLoose = true;
                RB = GetComponent<Rigidbody> ();
                if (!RB)
                {
                    RB = gameObject.AddComponent<Rigidbody> ();
                }
                RB.mass = Mass;
                RB.drag = Drag;
                RB.angularDrag = AngularDrag;

                if (DisableCollidersBeetwenLoseAndDetach)
                {
                    foreach (var cKV in CollidersWithState)
                    {
                        cKV.Key.enabled = false;
                    }
                }

                if (ParentBody)
                {
                    ParentBody.mass -= Mass;
                    RB.velocity = ParentBody.GetPointVelocity (TR.position);
                    RB.angularVelocity = ParentBody.angularVelocity;

                    if (Joints.Length > 0)
                    {
                        chosenJoint = Joints[Random.Range (0, Joints.Length)];

                        Hinge = gameObject.AddComponent<HingeJoint> ();
                        Hinge.autoConfigureConnectedAnchor = false;
                        Hinge.connectedBody = ParentBody;
                        Hinge.anchor = chosenJoint.hingeAnchor;
                        Hinge.axis = chosenJoint.hingeAxis;
                        Hinge.connectedAnchor = InitialPos + chosenJoint.hingeAnchor;
                        Hinge.enableCollision = false;
                        Hinge.useLimits = chosenJoint.useLimits;

                        JointLimits limits = new JointLimits();
                        limits.min = chosenJoint.minLimit;
                        limits.max = chosenJoint.maxLimit;
                        limits.bounciness = chosenJoint.bounciness;
                        Hinge.limits = limits;
                        Hinge.useSpring = chosenJoint.useSpring;

                        JointSpring spring = new JointSpring();
                        spring.targetPosition = chosenJoint.springTargetPosition;
                        spring.spring = chosenJoint.springForce;
                        spring.damper = chosenJoint.springDamper;
                        Hinge.spring = spring;
                        Hinge.breakForce = BreakForce;
                        Hinge.breakTorque = float.PositiveInfinity;
                        if (TR.parent)
                        {
                            OnStartJointPos = TR.parent.InverseTransformPoint (transform.TransformPoint (chosenJoint.hingeAnchor));
                        }
                    }
                }
                else
                {
                    TR.parent = null;
                }

                OnLoseAction.SafeInvoke ();
            }
        }

        public void RestoreObject ()
        {
            if (Hinge)
            {
                Destroy (Hinge);
            }
            if (RB && LooseForce > 0)
            {
                Destroy (RB);
            }

            enabled = true;
            IsLoose = false;
            ChildsIsDestroyed = false;
            DetachTimer = 0;
            TR.parent = InitialParent;
            IsDetached = false;

            foreach (var cKV in CollidersWithState)
            {
                cKV.Key.enabled = cKV.Value;
            }

            TR.localPosition = InitialPos;
            TR.localRotation = InitialRot;

            LooseHealth = LooseForce;
            if (LooseForce == 0)
            {
                SetAsLoose ();
            }
        }

        void OnDrawGizmosSelected ()
        {
            if (!TR)
            {
                TR = transform;
            }

            if (LooseForce >= 0 && Joints.Length > 0)
            {
                Gizmos.color = Color.yellow;
                foreach (PartJoint curJoint in Joints)
                {
                    var pos = TR.TransformPoint (curJoint.hingeAnchor);
                    var from = TR.TransformDirection (curJoint.hingeAxis);
                    var normal = from.normalized;
                    Gizmos.DrawRay (pos, TR.TransformDirection (curJoint.hingeAxis).normalized * 0.2f);
                    Gizmos.DrawWireSphere (pos, 0.02f);
                }
            }

            Gizmos.color = Color.green;
            foreach (var checkPoint in DamageCheckPoints)
            {
                Gizmos.DrawWireSphere (TR.TransformPoint (checkPoint), 0.05f);
            }
        }
    }

    [System.Serializable]
    public struct PartJoint             //Joint data, has the same variables as the standard joint.
    {
        public Vector3 hingeAnchor;
        public Vector3 hingeAxis;
        public bool useLimits;
        public float minLimit;
        public float maxLimit;
        public float bounciness;
        public bool useSpring;
        public float springTargetPosition;
        public float springForce;
        public float springDamper;
        public bool UseDetachSpeed;                     //For logic detachable part at speed (such as hood).
        [HideInInspectorIf("UseDetachSpeed", false)]
        public float springTargetPositionMaxSpeed;
        [HideInInspectorIf ("UseDetachSpeed", false)]
        public float springForceMaxSpeed;
        [HideInInspectorIf ("UseDetachSpeed", false)]
        public float maxDetachSpeed;
        [HideInInspectorIf ("UseDetachSpeed", false)]
        public int carDirection;
        [HideInInspectorIf ("UseDetachSpeed", false)]
        public float DetachTime;
    }
}

