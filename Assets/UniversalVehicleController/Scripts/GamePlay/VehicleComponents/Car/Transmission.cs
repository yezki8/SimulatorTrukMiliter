﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace PG
{
    //This part of the component contains the gear shift logic (automatic and manual), 
    //and the logic for transferring torque from the engine to the drive wheels.

    // heavily modified to include clutch and shifter logics
    public partial class CarController :VehicleController
    {
        public GearboxConfig Gearbox;
        public event System.Action<int> OnChangeGearAction;

        int _CurrentGear;
        public int CurrentGear              //Current gear, starting at -1 for reverse gear: -1 - reverse, 0 - neutral, 1 - 1st gear, etc.
        {
            get { return _CurrentGear; }

            set
            {
                if (_CurrentGear != value)
                {
                    _CurrentGear = value;
                    OnChangeGearAction.SafeInvoke (_CurrentGear);
                }
            }
        }

        public int CurrentGearIndex { get { return CurrentGear + 1; } }     //Current gear index, starting at 0 for reverse gear: 0 - reverse, 1 - neutral, 2 - 1st gear, etc.
        public bool InChangeGear { get { return ChangeGearTimer > 0; } }

        public float WheelTorque;
        public float CurrentMotorTorque;

        float ChangeGearTimer = 0;
        float[] AllGearsRatio;
        Wheel[] DriveWheels;

        [SerializeField] private TextMeshProUGUI _currentGearUI;                               // Current Gear UI


        void AwakeTransmition ()
        {
            //Calculated gears ratio with main ratio
            AllGearsRatio = new float[Gearbox.GearsRatio.Length + 2];
            AllGearsRatio[0] = -Gearbox.ReversGearRatio * Gearbox.MainRatio;
            AllGearsRatio[1] = 0;
            for (int i = 0; i < Gearbox.GearsRatio.Length; i++)
            {
                AllGearsRatio[i + 2] = Gearbox.GearsRatio[i] * Gearbox.MainRatio;
            }

            var driveWheels = new  List<Wheel>();
            foreach (var wheel in Wheels)
            {
                if (wheel.DriveWheel)
                {
                    driveWheels.Add (wheel);
                }
            }

            DriveWheels = driveWheels.ToArray ();
        }

        void FixedUpdateTransmition ()
        {
            if ((Gearbox.HasRGear || CurrentGear >= 0) && !float.IsNaN(EngineRPM))
            {
                // Calculate power transfer from motor to wheel, quadratic
                // add checks for automatic gearbox
                var powerTransfer = Gearbox.AutomaticGearBox ? 1 : Mathf.Pow(CarControl.Clutch, 2);
                // var powerTransfer = Gearbox.AutomaticGearBox ? 1 : CarControl.Clutch * 2;

                CurrentMotorTorque = (CurrentEngineTorque * (MaxMotorTorque * AllGearsRatio[CurrentGearIndex])) * CurrentAcceleration;

                if (InChangeGear)
                {
                    CurrentMotorTorque = 0;
                }

                // Calculate clutchSlipRatio here to modify motorTorque
                float clutchSlipRatio = Mathf.Abs(TargetRPM - EngineRPM) / Mathf.Max(TargetRPM, EngineRPM, 1f);
                CurrentMotorTorque *= (1 - clutchSlipRatio);

                //Calculation of target rpm for driving wheels.
                var targetWheelsRPM = AllGearsRatio[CurrentGearIndex] == 0? 0: EngineRPM / AllGearsRatio[CurrentGearIndex];
                var offset = (400 / AllGearsRatio[CurrentGearIndex]).Abs();

                for (int i = 0; i < DriveWheels.Length; i++)
                {
                    var wheel = DriveWheels[i];

                    // implement powerTransfer to wheel
                    WheelTorque = CurrentMotorTorque * powerTransfer;

                    //The torque transmitted to the wheels depends on the difference between the target RPM and the current RPM. 
                    //If the current RPM is greater than the target RPM, the wheel will brake. 
                    //If the current RPM is less than the target RPM, the wheel will accelerate.

                    if (targetWheelsRPM != 0 && Mathf.Sign (targetWheelsRPM * wheel.RPM) > 0)
                    {
                        var multiplier = wheel.RPM.Abs () / (targetWheelsRPM.Abs () + (IsPlayerVehicle ? ( offset / 32f ) : offset));
                        if (multiplier >= 1f)
                        {
                            WheelTorque *= (1 - multiplier);
                        }
                    }

                    //Apply of torque to the wheel.
                    wheel.SetMotorTorque (WheelTorque);
                    
                    // brake torque for weight
                    if (WheelTorque == 0 && powerTransfer > 0f) { wheel.SetBrakeTorque(1f * powerTransfer); }
                }
            }
            else
            {
                for (int i = 0; i < DriveWheels.Length; i++)
                {
                    DriveWheels[i].SetMotorTorque (0);
                }
            }

            if (InChangeGear)
            {
                ChangeGearTimer -= Time.fixedDeltaTime;
            }

            //Automatic gearbox logic. 
            if (!InChangeGear && Gearbox.AutomaticGearBox && IsLocalVehicle)
            {

                bool forwardIsSlip = false;
                bool anyWheelIsGrounded = false;
                float avgSign = 0;
                for (int i = 0; i < DriveWheels.Length; i++)
                {
                    forwardIsSlip |= DriveWheels[i].ForwardSlipNormalized > 0.9f;
                    anyWheelIsGrounded |= DriveWheels[i].IsGrounded;
                    avgSign += DriveWheels[i].RPM;
                }

                avgSign = Mathf.Sign (avgSign);

                if (anyWheelIsGrounded && !forwardIsSlip && EngineRPM > Engine.RPMToNextGear && CurrentGear >= 0 && CurrentGear < (AllGearsRatio.Length - 2))
                {
                    NextGear ();
                }
                else if (CurrentGear > 0 && (EngineRPM + 10 <= MinRPM || CurrentGear != 1) &&
                    Engine.RPMToNextGear > EngineRPM / AllGearsRatio[CurrentGearIndex] * AllGearsRatio[CurrentGearIndex - 1] + Engine.RPMToPrevGearDiff)
                {
                    PrevGear ();
                }

                //Switching logic from neutral gear.
                if (CurrentGear == 0 && CurrentBrake > 0)
                {
                    CurrentGear = -1;
                }
                else if (CurrentGear == 0 && CurrentAcceleration > 0)
                {
                    CurrentGear = 1;
                }
                else if ((avgSign > 0 && CurrentGear < 0 || VehicleDirection == 0) && Mathf.Approximately(CurrentAcceleration, 0))
                {
                    CurrentGear = 0;
                }
            }

            // update gear text
            if (_currentGearUI)
            {
                if (_CurrentGear == 0)
                {
                    _currentGearUI.text = "N";
                }
                else if (_CurrentGear == -1)
                {
                    _currentGearUI.text = "R";
                }
                else
                {
                    _currentGearUI.text = _CurrentGear.ToString();
                }
            }
        }

        // both function checks for automatic gearbox
        public void NextGear ()
        {
            if (!InChangeGear && CurrentGear < (AllGearsRatio.Length - 2))
            {
                CurrentGear++;
                ChangeGearTimer = Gearbox.AutomaticGearBox ? Gearbox.AutomaticGearBoxTime : Gearbox.ChangeUpGearTime;
                PlayBackfireWithProbability ();
            }
        }

        public void PrevGear ()
        {
            if (!InChangeGear && CurrentGear >= 0)
            {
                CurrentGear--;
                ChangeGearTimer = Gearbox.AutomaticGearBox ? Gearbox.AutomaticGearBoxTime : Gearbox.ChangeDownGearTime;
            }
        }

        // -1 = Reverse, 0 = Neutral, 2 = 1st Gear, etc.
        // Also add failed clutch handling, force stopping the engine
        public void SetGear(int gear)
        {
            if (CarControl.Clutch < 0.4)
            {
                if (!InChangeGear && (CurrentGear >= 0 || CurrentGear < (AllGearsRatio.Length - 2)))
                {
                    CurrentGear = gear;
                    ChangeGearTimer = Gearbox.ChangeClutchedGearTime;
                }
            }
            else
            {
                StopEngine();
            }
            
        }

        [System.Serializable]
        public class GearboxConfig
        {
            public float ChangeUpGearTime = 0.3f;                   // Delay after upshift.
            public float ChangeDownGearTime = 0.2f;                 // Delay after downshift.
            public float ChangeClutchedGearTime = 0.01f;            // Delay when using Clutch.
            public float AutomaticGearBoxTime = 0.7f;               // Delay for automatic gearbox.

            [Header("Automatic gearbox")]
            public bool AutomaticGearBox = true;

            [Header("Ratio")]
            public float[] GearsRatio;                              //Gear ratio. The values ​​are best take from the technical data of real transmissions.
            public float FinalDriveRatio;                           //Final drive ratio. The value is best taken from the technical data of the real transmission. (UNUSED)
            public float MainRatio;
            public bool HasRGear = true;
            [ShowInInspectorIf ("HasRGear")]
            public float ReversGearRatio;
        }
    }
}
