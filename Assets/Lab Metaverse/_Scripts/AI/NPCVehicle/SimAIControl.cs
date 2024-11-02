using UnityEngine;
using Random = UnityEngine.Random;

namespace PG
{

    /// <summary>
    /// AI for Race mode.
    /// </summary>
    public class SimAIControl :PositioningAIControl
    {
        SimAIConfig SimAIConfig;

        float Aggressiveness { get { return SimAIConfig.Aggressiveness; } }
        float ObstacleHitDistance { get { return SimAIConfig.ObstacleHitDistance; } }
        float HitPointHeight { get { return SimAIConfig.HitPointHeight; } }
        float ChangeHorizontalOffsetSpeed { get { return SimAIConfig.ChangeHorizontalOffsetSpeed; } }
        float OffsetForMainHitPoints { get { return SimAIConfig.OffsetForMainHitPoints; } }
        float OffsetForMainHitDirections { get { return SimAIConfig.OffsetForMainHitDirections; } }
        float OffsetForAdditionalHitPoints { get { return SimAIConfig.OffsetForAdditionalHitPoints; } }
        float OffsetForAdditionalHitDirections { get { return SimAIConfig.OffsetForAdditionalHitDirections; } }
        float MainHitDellayTime { get { return SimAIConfig.MainHitDellayTime; } }
        float AdditionalHitDellayTime { get { return SimAIConfig.AdditionalHitDellayTime; } }

        bool Reverse;
        float ReverseTimer = 0;
        float PrevSpeed = 0;
        float LastReverseTime;

        public float TargetDist { get; private set; }
        public AIPath.RoutePoint TargetPoint { get; private set; }
        public AIPath.RoutePoint TurnPredictionPoint { get; private set; }
        public float CurrentHorizontalOffset { get; set; }                      //Offset target point for overtaking.

        Vector3 HorizontalOffset;                                               //Offset target point for overtaking as Vector3, depends on the direction of the path.
        Rigidbody AheadRB;                                                      //Nearest ahead car.
        float DistanceToAheadCollider;                                          //Distance to the nearest car.
        float AheadRotationCheck;                                                    // Ahead vehicle rotation

        // lighting
        CarLighting CarLighting;

        float ResetTimer = 0;
        [SerializeField] private float ResetTimeInSeconds = 10;

        public override void Start ()
        {
            base.Start ();

            CarLighting = GetComponent<CarLighting> ();

            var selectedRaceAsset =  AIConfigAsset as SimAIConfigAsset;

            if (selectedRaceAsset)
            {
                SimAIConfig = selectedRaceAsset.SimAIConfig;
            }
            else
            {
                SimAIConfig = new SimAIConfig ();
            }

            StartHits ();
        }

        protected override void FixedUpdate ()
        {
            HandBrake = Finished;
            if (Finished)
            {
                Horizontal = 0;
                Vertical = 0;
                return;
            }

            base.FixedUpdate ();

            if (Reverse)
            {
                ReverseMove ();
            }
            else
            {
                ForwardMove ();
                UpdateMainHits ();
                UpdateAdditionalHits ();
            }

            // update car lighting when night
            float timeOfDay = DayTimeManager.Instance.getTimeOfDay();
            if (timeOfDay > 18 || timeOfDay < 6)
            {
                if (!CarLighting.MainLightsIsOn) CarLighting.SwitchMainLights();
            }
            else
            {
                if (CarLighting.MainLightsIsOn) CarLighting.SwitchMainLights();
            }
        }

        /// <summary>
        /// All behavior of AI is defined in this method.
        /// The desired speed is calculated based on the angle to the prediction point. 
        /// The overtaking offset is calculated.
        /// </summary>
        private void ForwardMove ()
        {
            TargetPoint = AIPath.GetRoutePoint (ProgressDistance + OffsetToTargetPoint + (SpeedFactorToTargetPoint * Car.CurrentSpeed));
            TurnPredictionPoint = AIPath.GetRoutePoint (ProgressDistance + OffsetTurnPrediction + (SpeedFactorToTurnPrediction * Car.CurrentSpeed));

            //The angle to TurnPredictionPoint is calculated to control the desired speed
            var angleToPredictionPoint = Vector3.SignedAngle (Vector3.forward,
                                                            transform.InverseTransformPoint ((TurnPredictionPoint.Position)).ZeroHeight(),
                                                            transform.up).Abs();
            
            float desiredSpeed = (1 - (angleToPredictionPoint / LookAngleSppedFactor)).AbsClamp ();
            desiredSpeed = desiredSpeed * (SpeedLimit - MinSpeed) + MinSpeed;
            desiredSpeed = desiredSpeed.Clamp (MinSpeed, MaxSpeed);

            if (AheadRB) {

                var vehicleDirection = transform.forward;
                var aheadRBSpeed = 0f;

                // compare the velocity direction of the current car and the ahead car
                AheadRotationCheck = Vector3.Dot(AheadRB.velocity.normalized, vehicleDirection);

                if (AheadRotationCheck > 0.3f)
                {
                    aheadRBSpeed = AheadRB.velocity.magnitude;
                }

                //Apply aggressiveness to the desired speed.
                float adjustedSpeed = Mathf.Lerp(aheadRBSpeed, desiredSpeed, Aggressiveness);

                desiredSpeed = Mathf.Min (desiredSpeed, Mathf.Lerp(adjustedSpeed, desiredSpeed, ((DistanceToAheadCollider - 2) / ObstacleHitDistance).Clamp()));
            }

            //Apply speed limit.
            desiredSpeed = Mathf.Min (SpeedLimit, desiredSpeed);

            // Acceleration and brake logic
            Vertical = ((desiredSpeed / Car.CurrentSpeed - 1)).Clamp (-1, 1);

            //Horizontal offset logic
            //Changing the offset for overtaking.
            float targetOffset = CurrentHorizontalOffset;

            //Left and Right rays have a hit.
            if (MainHits[0].collider && MainHits[1].collider)
            {
                // check if AheadRotation is less than 0.3f
                if (AheadRotationCheck < 0.3f)
                {
                    targetOffset = 0;
                }
                else
                {
                    if (OffsetToTargetPoint == 0)
                    {
                        if (Random.Range(0, 1) > 0.5f)
                        {
                            targetOffset = TargetPoint.OvertakeZoneRight;
                        }
                        else
                        {
                            targetOffset = -TargetPoint.OvertakeZoneLeft;
                        }
                    }
                    else if (MainHits[0].distance < MainHits[1].distance)
                    {
                        targetOffset = TargetPoint.OvertakeZoneRight;
                    }
                    else
                    {
                        targetOffset = -TargetPoint.OvertakeZoneLeft;
                    }
                }
            }
            //Left ray have a hit.
            else if (MainHits[0].collider && AheadRotationCheck > 0.3f)
            {
                targetOffset = TargetPoint.OvertakeZoneRight;
            }
            //Right ray have a hit.
            else if (MainHits[1].collider && AheadRotationCheck > 0.3f)
            {
                targetOffset = -TargetPoint.OvertakeZoneLeft;
            }
            //Center ray have a hit.
            else if (MainHits[2].collider && AheadRotationCheck > 0.3f)
            {
                if (CurrentHorizontalOffset > 0)
                {
                    targetOffset = TargetPoint.OvertakeZoneRight;
                }
                else
                {
                    targetOffset = -TargetPoint.OvertakeZoneLeft;
                }
            }
            //If overtaking is on the right and there is a car to be overtaken (and vice versa), the offset for overtaking will be unchanged.
            else if (!AdditionalHits[0].collider && CurrentHorizontalOffset > 0 || !AdditionalHits[1].collider && CurrentHorizontalOffset < 0)
            {
                targetOffset = 0;
            }

            //Applying an offset for overtaking.
            var distanceFactor = AheadRB? 1 - DistanceToAheadCollider / ObstacleHitDistance: 0.2f;
            CurrentHorizontalOffset = Mathf.MoveTowards (CurrentHorizontalOffset, targetOffset, Time.fixedDeltaTime * ChangeHorizontalOffsetSpeed * distanceFactor);
            CurrentHorizontalOffset = CurrentHorizontalOffset.Clamp (-TargetPoint.OvertakeZoneLeft, TargetPoint.OvertakeZoneRight);

            HorizontalOffset = Vector3.Cross(TargetPoint.Direction.normalized, transform.up) * -CurrentHorizontalOffset;

            //Steer angle logic
            var angleToTargetPoint = Vector3.SignedAngle (Vector3.forward,
                                                            transform.InverseTransformPoint ((TargetPoint.Position + HorizontalOffset)).ZeroHeight(),
                                                            Vector3.up);
            Horizontal = (angleToTargetPoint / Car.Steer.MaxSteerAngle * SetSteerAngleMultiplayer).Clamp (-1, 1);


            // Reset Car logic
            var deltaSpeed = Mathf.Abs (Car.CurrentSpeed - PrevSpeed);
            if (Vertical > 0.1f && deltaSpeed < 1 && Car.CurrentSpeed < 10)
            {
                if (ResetTimer < ResetTimeInSeconds)
                {
                    ResetTimer += Time.fixedDeltaTime;
                }
                else
                {
                    Horizontal = 0;
                    Vertical = 0;
                    // clear raycast parameters
                    DistanceToAheadCollider = float.MaxValue;
                    AheadRB = null;
                    // reset vehicle damage
                    Car.RestoreVehicle();
                    // reset location
                    ResetAIControl();
                    ResetTimer = 0;
                }
            }
            else
            {
                ResetTimer = 0;
            }
        }

        #region Hits

        RaycastHit[] MainHits = new RaycastHit[3];          //MainHits[0] - Left hit, MainHits[1] - Right hit, MainHits[2] - Center hit.
        RaycastHit[] AdditionalHits = new RaycastHit[2];    //AdditionalHits[0] - Left additional hit, AdditionalHits[1] - Right additional hit.

        Vector3[] MainHitPoints = new Vector3[3];           //Ray positions, calculated at the start.
        Vector3[] AdditionalHitPoints = new Vector3[2];     //Ray positions, calculated at the start.

        float LastMainHitsTime;
        float LastAdditionalHitsTime;

        void StartHits ()
        {
            //Calculation of ray positions
            var point = Car.Bounds.center;
            point.y = HitPointHeight;

            point.z = Car.Bounds.size.z * 0.5f;
            MainHitPoints[2] = point;

            point.z = Car.Bounds.size.z * 0.5f;
            point.x = -(Car.Bounds.size.x * 0.5f + OffsetForMainHitPoints);
            MainHitPoints[0] = point;

            point.x = -point.x;
            MainHitPoints[1] = point;

            point.x = -(Car.Bounds.size.x * 0.5f) - OffsetForAdditionalHitPoints;
            point.z = -Car.Bounds.size.z * 0.5f;

            AdditionalHitPoints[0] = point;

            point.x = -point.x;
            AdditionalHitPoints[1] = point;
        }

        void UpdateMainHits ()
        {

            if (Time.time - LastMainHitsTime < MainHitDellayTime)
            {
                return;
            }
            AheadRB = null;
            LastMainHitsTime = Time.time;
            DistanceToAheadCollider = float.MaxValue;
            Vector3 dir = Vector3.zero;

            for (int i = 0; i < MainHits.Length; i++)
            {
                dir.z = ObstacleHitDistance;
                dir.x = -MainHitPoints[i].x * OffsetForMainHitDirections;
                if (Physics.Raycast (transform.TransformPoint(MainHitPoints[i]), 
                                     transform.TransformDirection(dir.normalized), 
                                    out MainHits[i], i < 2? ObstacleHitDistance: ObstacleHitDistance * 0.8f,
                                    AiDetectionMask
                                    ) && 
                    MainHits[i].distance < DistanceToAheadCollider)
                {
                    DistanceToAheadCollider = MainHits[i].distance;
                    AheadRB = MainHits[i].rigidbody;
                }
            }
        }

        void UpdateAdditionalHits ()
        {
            if (Time.time - LastAdditionalHitsTime < AdditionalHitDellayTime)
            {
                return;
            }
            LastAdditionalHitsTime = Time.time;
            Vector3 dir = Vector3.zero;
            for (int i = 0; i < AdditionalHits.Length; i++)
            {
                // offset for additional hit points
                dir.z = ObstacleHitDistance;
                dir.x = AdditionalHitPoints[i].x * OffsetForAdditionalHitDirections;
                if (Physics.Raycast(transform.TransformPoint(AdditionalHitPoints[i]), 
                    transform.TransformDirection(dir.normalized), out AdditionalHits[i], 
                    ObstacleHitDistance + Car.Bounds.size.z, AiDetectionMask))
                {
                    DistanceToAheadCollider = AdditionalHits[i].distance;
                    AheadRB = AdditionalHits[i].rigidbody;
                }

            }
        }

        #endregion //Hits

        private void ReverseMove ()
        {
            if (ReverseTimer < ReverseTime)
            {
                ReverseTimer += Time.fixedDeltaTime;
            }
            else
            {
                LastReverseTime = Time.time;
                ReverseTimer = 0;
                Reverse = false;
            }
        }

        private void OnDrawGizmosSelected ()
        {
            if (Application.isPlaying && this.enabled)
            {

                Gizmos.color = Color.green;
                Gizmos.DrawLine (transform.position, TargetPoint.Position + HorizontalOffset);
                Gizmos.DrawWireSphere (TargetPoint.Position + HorizontalOffset, 0.5f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere (TargetPoint.Position, 0.5f);
                Gizmos.DrawWireSphere (TurnPredictionPoint.Position, 0.5f);

                Vector3 firstPoint;
                Vector3 dir = Vector3.zero;

                for (int i = 0; i < MainHits.Length; i++)
                {
                    dir.z = ObstacleHitDistance;
                    dir.x = -MainHitPoints[i].x * OffsetForMainHitDirections;

                    Gizmos.color = MainHits[i].collider != null ? Color.red : Color.blue;
                    firstPoint = transform.TransformPoint (MainHitPoints[i]);
                    Gizmos.DrawLine (firstPoint, firstPoint + transform.TransformDirection(dir.normalized) * (i < 2? ObstacleHitDistance: ObstacleHitDistance * 0.8f));
                }

                for (int i = 0; i < AdditionalHits.Length; i++)
                {
                    dir.z = ObstacleHitDistance;
                    dir.x = AdditionalHitPoints[i].x * OffsetForAdditionalHitDirections;

                    Gizmos.color = AdditionalHits[i].collider != null ? Color.red : Color.blue;
                    firstPoint = transform.TransformPoint (AdditionalHitPoints[i]);
                    Gizmos.DrawLine (firstPoint, firstPoint + transform.TransformDirection(dir.normalized) * (ObstacleHitDistance + Car.Bounds.size.z));
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //To Ensure these trigger calls only happen during gameplay
            if (GameStateController.Instance.GameState == StateOfGame.Match)
            {
                if (other.tag == "NPCFinish")
                {
                    SpecialNPCController.Instance.OnFinish(this);
                }
            }
        }
    }

    [System.Serializable]
    public class SimAIConfig
    {
        [Range(0f, 1f)] public float Aggressiveness = 0.0f;     //AI aggressiveness. 0 - AI will slow down in front of the car. 1 - AI will ram the car.
        public float ObstacleHitDistance = 25f;                 //Rays length for overtaking logic.
        public float HitPointHeight = 0.5f;                     //Rays height.
        public float ChangeHorizontalOffsetSpeed = 3f;          //The speed of change of offset for overtaking.
        public float OffsetForMainHitPoints = 0.7f;             //Offset of the main ray point from the edge of the car.
        public float OffsetForMainHitDirections = 0.8f;         //Offset of the direction of the main rays. 0 - straight rays, 1 - rays directed to the center.
        public float OffsetForAdditionalHitPoints = 1.2f;       //Offset of the additional ray point from the edge of the car.
        public float OffsetForAdditionalHitDirections = 1.2f;    //Offset of the direction of the additional rays. 0 - straight rays, 1 - rays directed outside.
        public float MainHitDellayTime = 0.4f;                  //Check MainHit interval for optimization.
        public float AdditionalHitDellayTime = 0.6f;            //Check AdditionalHit interval for optimization.
    }
}

