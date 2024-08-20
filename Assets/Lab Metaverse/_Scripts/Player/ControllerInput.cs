using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Events;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PG
{
    /// <summary>
    /// For user multiplatform control. This way of implementing the input is chosen to be able to implement control of several players for one device.
    /// </summary>

    // Heavily modified by user
    public class ControllerInput :InitializePlayer, ICarControl
    {
        // target camera
        public ChaseController TargetCamera;

        // InputAction
        public SimulatorInputActions controls;

        public float HorizontalChangeSpeed = 10;            //To simulate the use of a keyboard trigger.


        #region String Binding
        [Header("Key binding settings")]
        public string SteerAxis = "Steer";
        public string AccelerationAxis = "Acceleration";
        public string BrakeReverseAxis = "BrakeReverse";
        public string HandBrakeButton = "HandBrake";
        // public string PitchAxis = "Pitch";
        
        public string NextGearButton = "NextGear";
        public string PrevGearButton = "PrevGear";

        public string ClutchButton = "Clutch";
        public string Gear1stButton = "Gear1st";
        public string Gear2ndButton = "Gear2nd";
        public string Gear3rdButton = "Gear3rd";
        public string Gear4thButton = "Gear4th";
        public string Gear5thButton = "Gear5th";
        public string Gear6thButton = "Gear6th";
        public string GearRevButton = "GearReverse";

        public string SwitchLightsButton = "SwitchLights";
        public string SwitchLeftTurnLightsButton=  "SwitchLeftTurnLights";
        public string SwitchRightTurnLightsButton = "SwitchRightTurnLights";
        public string SwitchAlarmButton = "SwitchAlarm";
        // public string ConnectTrailerButton = "ConnectTrailer";
        public string ResetCarButton = "ResetCar";
        public string RestoreCarButton = "RestoreCar";
        public string ChangeViewButton = "ChangeView";

        #endregion

        public float Horizontal { get; private set; }
        public float Acceleration { get; private set; }
        public float BrakeReverse { get; private set; }
        public float Clutch { get; private set; }
        public float Pitch { get; private set; }
        public bool HandBrake { get; private set; }
        public bool Boost { get; private set; }

        public UnityEvent OnChangeViewAction;

        float TargetHorizontal;

        CarLighting CarLighting;

        event System.Action OnDestroyAction;

        public bool IsFirstPlayer { get; private set; }


        // enabling and disabling control
        public void EnableControls()
        {
            controls.Enable();
        }
        public void DisableControls()
        {
            controls.Disable();
        }

        // create instance and hook up controls
        private void Awake()
        {
            controls = new SimulatorInputActions();

            controls.Player.ChangeView.performed += ctx => ChangeView();

            // call UpdateOffset() on camera gameobject
            controls.Player.Look.performed += ctx => UpdateCamera(controls.Player.Look.ReadValue<Vector2>());
            controls.Player.Look.canceled += ctx => UpdateCamera(Vector2.zero);

            // convert vector2 x to steer value
            controls.Player.Steer.performed += ctx => SetSteer(controls.Player.Steer.ReadValue<float>());
            controls.Player.Steer.canceled += ctx => SetSteer(0);

            // driving
            controls.Player.Acceleration.performed += ctx => SetAcceleration(controls.Player.Acceleration.ReadValue<float>());
            controls.Player.Acceleration.canceled += ctx => SetAcceleration(0);
            controls.Player.BrakeReverse.performed += ctx => SetBrakeReverse(controls.Player.BrakeReverse.ReadValue<float>());
            controls.Player.BrakeReverse.canceled += ctx => SetBrakeReverse(0);
            controls.Player.HandBrake.performed += ctx => SetHandBrake(controls.Player.HandBrake.ReadValue<float>() > 0);
            controls.Player.HandBrake.canceled += ctx => SetHandBrake(false);

            // gear and clutch
            controls.Player.Clutch.performed += ctx => SetClutch(controls.Player.Clutch.ReadValue<float>());
            controls.Player.Clutch.canceled += ctx => SetClutch(0);
            controls.Player.Gear1st.performed += ctx => SetGear(1);
            controls.Player.Gear1st.canceled += ctx => SetGear(0);
            controls.Player.Gear2nd.performed += ctx => SetGear(2);
            controls.Player.Gear2nd.canceled += ctx => SetGear(0);
            controls.Player.Gear3rd.performed += ctx => SetGear(3);
            controls.Player.Gear3rd.canceled += ctx => SetGear(0);
            controls.Player.Gear4th.performed += ctx => SetGear(4);
            controls.Player.Gear4th.canceled += ctx => SetGear(0);
            controls.Player.Gear5th.performed += ctx => SetGear(5);
            controls.Player.Gear5th.canceled += ctx => SetGear(0);
            controls.Player.Gear6th.performed += ctx => SetGear(6);
            controls.Player.Gear6th.canceled += ctx => SetGear(0);
            controls.Player.GearReverse.performed += ctx => SetGear(-1);
            controls.Player.GearReverse.canceled += ctx => SetGear(0);
            controls.Player.NextGear.performed += ctx => NextGear();
            controls.Player.PrevGear.performed += ctx => PrevGear();

            // car lights
            controls.Player.SwitchLights.performed += ctx => SwitchLights();
            controls.Player.SwitchLeftTurnLights.performed += ctx => SwitchLeftTurnSignal();
            controls.Player.SwitchRightTurnLights.performed += ctx => SwitchRightTurnSignal();
            controls.Player.SwitchAlarm.performed += ctx => SwitchAlarm();

            // car actions
            controls.Player.ConnectTrailer.performed += ctx => ConnectTrailer();
            controls.Player.ResetCar.performed += ctx => ResetCar();
        }

        private void Update()
        {
            Horizontal = Mathf.MoveTowards (Horizontal, TargetHorizontal, Time.deltaTime * HorizontalChangeSpeed);
        }

        void UpdateCamera(Vector2 value)
        {
            TargetCamera.UpdateOffset(value.x);
            Debug.Log("Camera: " + value);
        }

        private void OnDestroy ()
        {
            OnDestroyAction.SafeInvoke ();
        }

        public override bool Initialize (VehicleController car)
        {
            base.Initialize (car);

            IsFirstPlayer = true;

            if (Car)
            {
                CarLighting = Car.GetComponent<CarLighting> ();
                var aiControl = Car.GetComponent<ICarControl>();
                if (aiControl == null || !(aiControl is PositioningAIControl))
                {
                    Car.CarControl = this;
                }
            }

            return IsInitialized;
        }

        public override void Uninitialize ()
        {
            if (Car != null && Car.CarControl == this as ICarControl)
            {
                Car.CarControl = null;
            }

            CarLighting = null;
            base.Uninitialize ();
        }


        #region Set input
        // Allow to Move is checked in each method
        // consider 

        public void SetAcceleration (float value)
        {
            Acceleration = value;
        }

        public void SetBrakeReverse (float value)
        {
            BrakeReverse = value;
        }

        public void SetSteer (float value)
        {
            TargetHorizontal = value;
        }

        public void SetPitch (float value)
        {
            Pitch = value;
        }

        public void NextGear ()
        {
            if (Car)
            {
                Car.NextGear();
            }
        }

        public void PrevGear ()
        {
            if (Car)
            {
                Car.PrevGear();
            }
        }

        public void SetClutch(float value)
        {
            if (Car)
            {
                // no suitable processor from input system
                // transform from [-1, 1] to [0, 1]
                Clutch = (value + 1) / 2;
            }
        }

        public void SetGear(int value)
        {
            if (Car)
            {
                if (Clutch > 0.8)
                {
                    Car.SetGear(value);
                }
            }
        }

        public void SwitchLights ()
        {
            CarLighting.SwitchMainLights();
        }

        public void SwitchLeftTurnSignal ()
        {
            CarLighting.TurnsEnable(TurnsStates.Left);
        }

        public void SwitchRightTurnSignal ()
        {
            CarLighting.TurnsEnable(TurnsStates.Right);
        }

        public void SwitchAlarm ()
        {
            CarLighting.TurnsEnable(TurnsStates.Alarm);
        }

        public void ConnectTrailer ()
        {
            Car.TryConnectDisconnectTrailer();
        }

        public void ResetCar ()
        {
            Vehicle.ResetVehicle();
        }

        public void RestoreCar ()
        {
            Vehicle.RestoreVehicle();
        }

        public void ChangeView ()
        {
            OnChangeViewAction.Invoke();
        }

        public void TryExitFromCar ()
        {
            var playerController = GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                playerController.ExitFromCar ();
            }
        }

        // unused
        // public void SetViewDelta (Vector2 value)
        // {
        //     ViewDelta = value;
        // }

        public void SetHandBrake (bool value)
        {
            HandBrake = value;
        }

        public void SetBoost (bool value)
        {
            Boost = value;
        }

#endregion //Set input

    }
}
