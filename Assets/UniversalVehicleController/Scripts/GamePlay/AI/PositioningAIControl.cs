using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PG 
{
    /// <summary>
    /// For AI with track positioning functions (for Race or Drift mode).
    /// </summary>
    [RequireComponent (typeof(CarController))]
    public class PositioningAIControl :BaseAIControl
    {
        public AIPath AIPath;                                           //The path to which the AI is tied.

        protected float SpeedLimit;                                     //Current speed limit.

        public float ProgressDistance { get; set; }                     //Distance of progress along the AIPath
        public AIPath.RoutePoint ProgressPoint { get; private set; }

        protected Vector3 _initPosition = Vector3.zero;                   // initial position of the vehicle
        protected float _initRotation = 0;                                // initial rotation of the vehicle

        /// <summary>
        /// If the path is not looped, then the property returns true when the end of the path is reached.
        /// </summary>
        public bool Finished 
        { 
            get 
            {
                return !AIPath.LoopedPath && ProgressDistance >= AIPath.Length;
            } 
        }

        public void ResetAIControl()
        {
            this.GetComponent<CarController>().ResetVehicle();
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            this.GetComponent<Rigidbody>().isKinematic = true;
            this.GetComponent<Rigidbody>().position = _initPosition;
            this.GetComponent<Rigidbody>().rotation = Quaternion.Euler(0, _initRotation, 0);
            this.GetComponent<Rigidbody>().isKinematic = false;

            // reset progress
            ResetProgress();
        }

        public void ResetProgress()
        {
            ProgressDistance = 0;
            ProgressPoint = AIPath.GetRoutePoint(0);
        }

        public override void Start ()
        {
            //If path is null, then the first path of the scene is taken.
            if (!AIPath)
            {
                AIPath = AIPath.FirstPath;
            }

            if (!AIPath)
            {
                Debug.LogError ("AIPath not found");
                enabled = false;
                return;
            }

            if (!AIConfigAsset && !AIPath.AIConfigAsset)
            {
                Debug.LogError ("AIConfig not found");
                enabled = false;
                return;
            }

            if (!AIConfigAsset)
            {
                AIConfigAsset = AIPath.AIConfigAsset;
            }

            base.Start ();

            // Set the initial position and rotation of the vehicle
            _initPosition = this.GetComponent<Rigidbody>().position;
            _initRotation = this.GetComponent<Rigidbody>().rotation.eulerAngles.y;

            BaseAIConfig = AIConfigAsset.AIConfig;

            ProgressPoint = AIPath.GetRoutePoint (0);

            //Finding the closest waypoint at the start.
            float minProgress = 0;
            float curProgress = 0;
            float minDist = (AIPath.GetRoutePoint (0).Position - transform.position).sqrMagnitude;
            float curDist;
            while (curProgress < AIPath.Length)
            {
                curProgress += 0.5f;
                curDist = (AIPath.GetRoutePoint (curProgress).Position - transform.position).sqrMagnitude;
                if (curDist < minDist)
                {
                    minDist = curDist;
                    minProgress = curProgress;
                }
            }
            ProgressDistance = minProgress;
            ProgressPoint = AIPath.GetRoutePoint (ProgressDistance);
        }

        protected override void FixedUpdate ()
        {
            //Determination of the current progress distance on the way.

            Vector3 progressDelta = ProgressPoint.Position - transform.position;
            float dotProgressDelta = Vector3.Dot (progressDelta, ProgressPoint.Direction);

            if (dotProgressDelta < 0)
            {
                //Forward move direction logic
                while (dotProgressDelta < 0)
                {
                    ProgressDistance += Mathf.Max (0.5f, Car.CurrentSpeed * Time.fixedDeltaTime);
                    ProgressPoint = AIPath.GetRoutePoint (ProgressDistance);
                    progressDelta = ProgressPoint.Position - transform.position;
                    dotProgressDelta = Vector3.Dot (progressDelta, ProgressPoint.Direction);
                }
            }
            else if (ProgressDistance > 0 && progressDelta.sqrMagnitude < 0)
            {
                //Wrog move direction logic
                dotProgressDelta = Vector3.Dot (progressDelta, -ProgressPoint.Direction);

                if (dotProgressDelta < 0f)
                {
                    ProgressDistance -= progressDelta.magnitude * 0.5f;
                    ProgressPoint = AIPath.GetRoutePoint (ProgressDistance);
                }
            }

            SpeedLimit = ProgressPoint.SpeedLimit;
        }
    }
}
