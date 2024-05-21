﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PG
{
    /// <summary>
    /// Main vegicle controller component. 
    /// </summary>
    [RequireComponent (typeof (Rigidbody))]
    public class VehicleController :MonoBehaviour
    {
        [Header("VehicleController")]
#pragma warning disable 0649

        [SerializeField] bool ShowBoundsGizmo = false;

#pragma warning restore 0649

        public string VehicleName;
        public Wheel[] Wheels = new Wheel[0];                                           //Wheel object references
        public Transform COM;                                                           //Center of Mass, assigned by Ridgidbody in Awake.
        public MeshRenderer[] BaseViews = new MeshRenderer[0];                          //To determine the visibility of the vehicle.
        public Transform FirstPersonCameraPos;

        public event System.Action<VehicleController, Collision> CollisionAction;       //Actions are performed at the moment of collision.
        public event System.Action<VehicleController, Collision> CollisionStayAction;   //Actions are performed at the moment of stay collision.
        public event System.Action BeforeResetVehicleAction;                            //Actions are performed before the vehicle is reset.
        public event System.Action AfterResetVehicleAction;                             //Actions are performed after the vehicle is reset.

        public Rigidbody RB { get; private set; }

        public Bounds Bounds { get; private set; }
        public float Size { get; private set; }
        public bool IsPlayerVehicle { get; set; }
        public bool IsLocalVehicle { get; set; } = true;                                //Need for multiplayer

        VehicleSFX _VehicleSFX;
        public VehicleSFX VehicleSFX 
        {
            get
            {
                if (!_VehicleSFX)
                {
                    _VehicleSFX = GetComponentInChildren<VehicleSFX> ();
                }
                return _VehicleSFX;
            }
        }

        float LastCheckVisibleTime;     //Time of last visibility check. To check the visibility every 0.5 seconds.
        bool _VehicleIsVisible;

        public bool VehicleIsVisible 
        { 
            get 
            {
                if (Time.time - LastCheckVisibleTime > 0.5f)
                {
                    _VehicleIsVisible = false;
                    for (int i = 0; i < BaseViews.Length; i++)
                    {
                        if (BaseViews[i].isVisible)
                        {
                            _VehicleIsVisible = true;
                            break;
                        }
                    }
                    LastCheckVisibleTime = Time.time;
                }

                return _VehicleIsVisible; 
            } 
        }

        public bool VehicleIsGrounded { get; private set; }
        public float CurrentSpeed { get; private set; }                                     //Vehicle speed, measured in "units per second".
        public float SpeedInHour { get { return CurrentSpeed * (B.GameSettings.EnumMeasurementSystem == MeasurementSystem.KM? C.KPHMult: C.MPHMult); } }    //Vehicle speed in selected units.
        public int VehicleDirection { get { return CurrentSpeed < 1 ? 0 : (VelocityAngle.Abs() < 90? 1 : -1); } }
        public float VelocityAngle { get; private set; }                                    //The angle of the vehicle body relative to the Velocity vector.
        public float PrevVelocityAngle { get; private set; }

        protected virtual void Awake ()
        {
            RB = GetComponent<Rigidbody> ();
            RB.centerOfMass = COM.localPosition;

            Quaternion startRotation = transform.rotation;
            transform.rotation = Quaternion.identity;

            bool needFindBaseView = BaseViews == null || BaseViews.Length == 0;
            MeshRenderer largestRenderer = null;

            var meshRenderers = GetComponentsInChildren<MeshRenderer>();
            var bounds = meshRenderers[0].bounds;
            foreach (var renderer in meshRenderers)
            {
                bounds.Encapsulate (renderer.bounds);

                if (needFindBaseView && (largestRenderer == null || renderer.bounds.size.sqrMagnitude > largestRenderer.bounds.size.sqrMagnitude))
                {
                    largestRenderer = renderer;
                }
            }
            bounds.center = transform.InverseTransformPoint (bounds.center);
            Bounds = bounds;
            Size = Mathf.Max (Bounds.size.x, Bounds.size.y, Bounds.size.z);

            var colliderGO = new GameObject ("Collider for AI detection");
            colliderGO.transform.SetParent (transform);
            colliderGO.transform.localPosition = Vector3.zero;
            colliderGO.transform.localRotation = Quaternion.identity;
            colliderGO.layer = B.GameSettings.LayerForAiDetection;

            var collider = colliderGO.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = Bounds.size;
            collider.center = Bounds.center;


            if (needFindBaseView && largestRenderer)
            {
                BaseViews = new MeshRenderer[1] { largestRenderer };
            }

            transform.rotation = startRotation;
        }

        protected virtual void FixedUpdate ()
        {
            //Calculating body speed and angle
            CurrentSpeed = RB.velocity.magnitude;
            PrevVelocityAngle = VelocityAngle;
            Vector3 horizontalLocalVelocity = transform.InverseTransformDirection(RB.velocity).ZeroHeight ();
            if (horizontalLocalVelocity.sqrMagnitude > 0.01f)
            {
                VelocityAngle = -Vector3.SignedAngle (horizontalLocalVelocity.normalized, Vector3.forward, Vector3.up);
            }
            else
            {
                VelocityAngle = 0;
            }

            VehicleIsGrounded = false;

            for (int i = 0; i < Wheels.Length; i++)
            {
                VehicleIsGrounded |= Wheels[i].IsGrounded;
            }
        }

        protected virtual void Update () { }

        protected virtual void LateUpdate () { }

        public virtual void OnCollisionEnter (Collision collision)
        {
            CollisionAction.SafeInvoke (this, collision);
        }

        public void OnCollisionStay (Collision collision)
        {
            CollisionStayAction.SafeInvoke (this, collision);
        }


        protected virtual void OnTriggerEnter (Collider other) { }

        protected virtual void OnTriggerExit (Collider other) { }

        /// <summary>
        /// Reset vehicle logic.
        /// TODO Add a vehicle reset on the way.
        /// </summary>
        public virtual void ResetVehicle ()
        {
            BeforeResetVehicleAction.SafeInvoke ();

            RB.velocity = Vector3.zero;
            RB.angularVelocity = Vector3.zero;

            float y = transform.rotation.eulerAngles.y;
            transform.position += Vector3.up * 2;
            transform.rotation = Quaternion.AngleAxis (y, Vector3.up);

            AfterResetVehicleAction.SafeInvoke ();
        }

        public virtual void RestoreVehicle ()
        {
            var damageController = GetComponent <VehicleDamageController>();
            if (damageController)
            {
                damageController.RestoreCar ();
            }
            else
            {
                Debug.LogError ("VehicleDamageController component not found");
            }
        }

        protected virtual void OnDrawGizmosSelected ()
        {
            if (RB == null)
            {
                return;
            }

            //The lines of the direction of the vehicle body and the movement of the vehicle are drawn.

            var centerPos = RB.worldCenterOfMass;
            var velocity = centerPos + (Vector3.ClampMagnitude (RB.velocity, 4));
            var forwardPos = transform.TransformPoint (RB.centerOfMass + Vector3.forward * 4);

            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere (centerPos, 0.2f);
            Gizmos.DrawLine (centerPos, velocity);
            Gizmos.DrawLine (centerPos, forwardPos);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere (forwardPos, 0.2f);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere (velocity, 0.2f);

            Gizmos.color = Color.red;
            if (ShowBoundsGizmo)
            {
                var halfSize = Bounds.size / 2;
                List<Vector3> points = new List<Vector3>()
                {
                    transform.TransformPoint(Bounds.center + new Vector3(-halfSize.x, halfSize.y, halfSize.z)),
                    transform.TransformPoint(Bounds.center + new Vector3(halfSize.x, halfSize.y, halfSize.z)),
                    transform.TransformPoint(Bounds.center + new Vector3(halfSize.x, -halfSize.y, halfSize.z)),
                    transform.TransformPoint(Bounds.center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z)),
                    transform.TransformPoint(Bounds.center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z)),
                    transform.TransformPoint(Bounds.center + new Vector3(halfSize.x, halfSize.y, -halfSize.z)),
                    transform.TransformPoint(Bounds.center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z)),
                    transform.TransformPoint(Bounds.center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z))
                };

                Gizmos.DrawSphere (transform.TransformPoint (Bounds.center), 0.5f);

                int halfCount = points.Count / 2;

                for (int i = 0; i < halfCount; i++)
                {
                    Gizmos.DrawLine (points[i], points[MathExtentions.Repeat (i + 1, 0, halfCount - 1)]);
                    Gizmos.DrawLine (points[i + halfCount], points[MathExtentions.Repeat (i + halfCount + 1, halfCount, points.Count - 1)]);
                    Gizmos.DrawLine (points[i], points[i + halfCount]);
                }

            }

        }

    }

    public enum VehicleType
    {
        None,
        Car,
        CarWithTrailer,
        Truck,
        Trailer,
        TruckWithTrailer,
    }
}
