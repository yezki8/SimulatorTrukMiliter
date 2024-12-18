﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PG
{
    // One of the most important parts of the component, it is responsible for management and management assistance.
    public partial class CarController :VehicleController
    {
        public SteerConfig Steer;

        protected float PrevSteerAngle;
        protected float CurrentSteerAngle;
        protected float WheelMaxSteerAngle;
        protected Wheel[] SteeringWheels;
        protected float HorizontalControl { get { return CarControl != null && !BlockControl? CarControl.Horizontal : 0; } }
        public bool ABSIsActive { get; private set; }

        void AwakeSteering ()
        {
            var steeringWheels = new List<Wheel>();
            foreach (var wheel in Wheels)
            {
                if (wheel.IsSteeringWheel)
                {
                    steeringWheels.Add (wheel);
                    if (wheel.SteerPercent.Abs() > WheelMaxSteerAngle)
                    {
                        WheelMaxSteerAngle = wheel.SteerPercent.Abs();
                    }
                }
            }
            // sort wheel so that the first wheel is the leftmost wheel
            SteeringWheels = steeringWheels.OrderBy (x => x.transform.localPosition.x).ToArray ();
        }

        /// <summary>
        /// Update all helpers logic.
        /// </summary>
        void FixedUpdateSteering ()
        {
            var needHelp = VelocityAngle.Abs() > 0.001f && VelocityAngle.Abs() < Steer.MaxVelocityAngleForHelp && CurrentSpeed > Steer.MinSpeedForHelp && CurrentGear > 0;
            float helpAngle = 0;
            var angularVelocity = RB.angularVelocity;

            if (needHelp)
            {
                for (int i = 0; i < SteeringWheels.Length; i++)
                {
                    if (Wheels[i].IsGrounded)
                    {
                        HelpAngularVelocity ();
                        break;
                    }
                }

                helpAngle = Mathf.Clamp (VelocityAngle * Steer.HelpDriftIntensity, -Steer.MaxSteerAngle, Steer.MaxSteerAngle);
            }
            else if (CurrentSpeed < Steer.MinSpeedForHelp && CurrentAcceleration > 0 && CurrentBrake > 0)
            {
                angularVelocity.y += Steer.HandBrakeAngularHelpCurve.Evaluate (angularVelocity.y.Abs()) * HorizontalControl * 5 * Time.fixedDeltaTime;
                RB.angularVelocity = angularVelocity;
            }

            float helpWhenChangeAngle = VehicleDirection == 1? (VelocityAngle - PrevVelocityAngle) * (Steer.MaxSteerAngle / 90): 0;

            var steerMultiplier = Steer.EnableSteerLimit && VehicleDirection != 0? Steer.SteerLimitCurve.Evaluate (CurrentSpeed): 1;

            if (ForceFeedback != null) ApplyForceFeedback(steerMultiplier);

            float targetSteerAngle = HorizontalControl * Steer.MaxSteerAngle;

            //Wheel turn limitation.
            var targetAngle = Mathf.Clamp (helpAngle + targetSteerAngle, -Steer.MaxSteerAngle, Steer.MaxSteerAngle);

            //Calculation of the steering speed. The steering wheel should turn faster towards the velocity angle.
            //More details (With images) are described in the documentation.
            float steerAngleChangeSpeed;

            float currentAngleDiff = (VelocityAngle - CurrentSteerAngle).Abs();

            if (!needHelp || PrevSteerAngle > CurrentSteerAngle && CurrentSteerAngle > VelocityAngle || PrevSteerAngle < CurrentSteerAngle && CurrentSteerAngle < VelocityAngle)
            {
                steerAngleChangeSpeed = Steer.SteerChangeSpeedToVelocity.Evaluate (currentAngleDiff);
            }
            else
            {
                steerAngleChangeSpeed = Steer.SteerChangeSpeedFromVelocity.Evaluate (currentAngleDiff);
            }

            PrevSteerAngle = CurrentSteerAngle;
            CurrentSteerAngle = Mathf.MoveTowards (CurrentSteerAngle, targetAngle, steerAngleChangeSpeed * steerMultiplier * Time.fixedDeltaTime);
            CurrentSteerAngle = (CurrentSteerAngle + helpWhenChangeAngle).Clamp (-Steer.MaxSteerAngle, Steer.MaxSteerAngle);

            //Apply a turn to the front wheels.
            for (int i = 0; i < SteeringWheels.Length; i++)
            {
                SteeringWheels[i].SetSteerAngle (CurrentSteerAngle);
            }
        }
        /// <summary>
        /// Apply force feedback to the steering wheel.
        /// </summary>
        void ApplyForceFeedback(float steerMultiplier)
        {
            // Simulate wheel turn when through potholes or bumps
            // check height difference between left and right wheels
            // steer the wheels to the side of the wheel with greater height

            // wheel count check
            float heightDifference = SteeringWheels.Length > 2 ? SteeringWheels[1].transform.position.y - SteeringWheels[0].transform.position.y : 0;
            ForceFeedback.SetSpringPosOffset(heightDifference);
            
            // Simulate dirt road effect, based on stiffness
            float avgMagnitude = 0;
            foreach (var wheel in Wheels)
            {
                
                if (wheel.IsGrounded)
                {
                    avgMagnitude += wheel.CurrentGroundConfig.WheelStiffness;
                }
            }
            avgMagnitude = (6 - avgMagnitude) * (Steer.SpeedToDirtRoadFFB.Evaluate(CurrentSpeed));
            // calculate dirt road effect based on stiffness
            ForceFeedback.SetDirtRoadEffect(avgMagnitude);

            // set spring force on steering wheel
            // add height difference to spring multiplier
            ForceFeedback.SetSpringMultiplier(Mathf.Clamp(1 - steerMultiplier + (heightDifference.Abs() * 2.5f), 0, 1));
        }

        /// <summary>
        /// A method to help resistance drift and rotate the car with the handbrake.
        /// </summary>
        void HelpAngularVelocity ()
        {
            var angularVelocity = RB.angularVelocity;

            float angularHelp = 0;
            float angularHelpMultiplier = HorizontalControl * (CurrentSpeed / Steer.MaxSpeedForMaxAngularHelp).Clamp() * Time.fixedDeltaTime;


            if (HorizontalControl * VelocityAngle >= 0 && VelocityAngle.Abs() < 150)
            {
                //Drift resistance help.
                angularHelp = Steer.DriftResistanceCurve.Evaluate (VelocityAngle < 0 ? RB.angularVelocity.y : -RB.angularVelocity.y) * (VelocityAngle.Abs() / 60).Clamp() * angularHelpMultiplier;
            }

            if (InHandBrake)
            {
                //Handbrake resistance help.
                angularHelp += Steer.HandBrakeAngularHelpCurve.Evaluate (angularVelocity.y * -Mathf.Sign(angularVelocity.y)) * angularHelpMultiplier;
            }

            if (Steer.DriftLimitAngle > 0)
            {
                if ((-VelocityAngle * RB.angularVelocity.y) > 0)
                {
                    float groundedWheels = 0;
                    for (int i = 0; i < Wheels.Length; i++)
                    {
                        if (Wheels[i].IsGrounded)
                        {
                            groundedWheels++;
                        }
                    }
                    float limitMultiplier = Mathf.InverseLerp(Steer.DriftLimitAngle, Steer.DriftLimitAngle * 0.5f, VelocityAngle.Abs());
                    angularVelocity.y = Mathf.Lerp(angularVelocity.y, angularVelocity.y * limitMultiplier, (groundedWheels / Wheels.Length) * Time.fixedDeltaTime * 10);
                }
            }

            angularVelocity.y += angularHelp;
            RB.angularVelocity = angularVelocity;
        }

        /// <summary>
        /// Braking logic.
        /// </summary>
        void FixedUpdateBrakeLogic (bool canBrake, bool canHandBrake)
        {
            ABSIsActive = false;
            //HandBrake
            if (InHandBrake)
            {
                if (canHandBrake)
                {
                    for (int i = 0; i < Wheels.Length; i++)
                    {
                        Wheels[i].SetHandBrake(true);
                    }
                }
            }
            //Brake and acceleration pressed at the same time for burnout.
            else if (CurrentAcceleration > 0 && CurrentBrake > 0 && CurrentSpeed < 5)
            {
                if (canBrake)
                {
                    for (int i = 0; i < Wheels.Length; i++)
                    {
                        Wheels[i].SetBrakeTorque(Wheels[i].DriveWheel ? 0 : CurrentBrake);
                    }
                }
            }
            //Just braking.
            else
            {
                if (canBrake)
                {
                    if (Steer.ABS > 0 && CurrentBrake > 0)
                    {
                        //ABS Logic.
                        float maxSlipForEnableAbs = 2.8f - Steer.ABS * 1.2f;
                        for (int i = 0; i < Wheels.Length; i++)
                        {
                            if (Wheels[i].ForwardSlipNormalized > maxSlipForEnableAbs)
                            {
                                Wheels[i].SetBrakeTorque(0);
                                ABSIsActive |= true;
                            }
                            else
                            {
                                Wheels[i].SetBrakeTorque(CurrentBrake);
                            }
                        }
                    }
                    else
                    {
                        //Without ABS Logic.
                        for (int i = 0; i < Wheels.Length; i++)
                        {
                            Wheels[i].SetBrakeTorque(CurrentBrake);
                        }
                    }
                }                
            }
        }

        [System.Serializable]
        public class SteerConfig
        {
            [Header("Steer settings")]
            public float MaxSteerAngle = 25;
            public bool EnableSteerLimit = true;                    //Enables limiting wheel turning based on car speed.
            public AnimationCurve SteerLimitCurve;                  //Limiting wheel turning if the EnableSteerLimit flag is enabled
            public AnimationCurve SteerChangeSpeedToVelocity;       //The speed of turn of the wheel in the direction of the velocity of the car.
            public AnimationCurve SteerChangeSpeedFromVelocity;     //The speed of turn of the wheel from the direction of the velocity of the car.
            public AnimationCurve SpeedToDirtRoadFFB;               //The magnitude of the dirt road effect based on the speed of the car.

            [Header("Steer assistance")]
            public float MaxVelocityAngleForHelp = 120;             //The maximum degree of angle of the car relative to the velocity, at which the steering assistance will be provided.
            public float MinSpeedForHelp = 1.5f;

            [Space(10)]
            [Range(0, 1)] public float HelpDriftIntensity = 0.8f;   //The intensity of the automatic steering while drifting.

            [Header("Angular help")]
            public AnimationCurve HandBrakeAngularHelpCurve;        //The power of assistance that turns the car with the hand brake.
            public AnimationCurve DriftResistanceCurve;
            public float MaxSpeedForMaxAngularHelp = 20;
            public float DriftLimitAngle = 0;

            [Header("Electronic assistants")]
            [Range(0, 1)]
            public float ABS;               //ABS to prevent wheel lock when braking.
            //[Range(0, 1)]
            //public float ESP;             TODO add ESP logic
            [Range(0, 1)]
            public float TCS;               //TCS to prevent wheel slip when accelerating.
        }
    }
}
