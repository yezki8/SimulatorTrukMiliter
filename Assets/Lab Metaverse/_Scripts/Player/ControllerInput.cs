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

        //Added by user to stop player input midgame
        public bool AllowToMove = true;

        // InputAction
        public SimulatorInputActions controls;

        public float HorizontalChangeSpeed = 10;            //To simulate the use of a keyboard trigger.
        public bool RotateCameraWithMousePressed;


        #region String Binding
        [Header("Key binding settings")]
        public string SteerAxis = "Steer";
        public string AccelerationAxis = "Acceleration";
        public string BrakeReverseAxis = "BrakeReverse";
        public string PitchAxis = "Pitch";
        public string NextGearButton = "NextGear";
        public string PrevGearButton = "PrevGear";
        public string SwitchLightsButton = "SwitchLights";
        public string SwitchLeftTurnLightsButton=  "SwitchLeftTurnLights";
        public string SwitchRightTurnLightsButton = "SwitchRightTurnLights";
        public string SwitchAlarmButton = "SwitchAlarm";
        public string ConnectTrailerButton = "ConnectTrailer";
        public string ResetCarButton = "ResetCar";
        public string RestoreCarButton = "RestoreCar";
        public string ChangeViewButton = "ChangeView";
        public string HandBrakeButton = "HandBrake";
        public string BoostButton = "Boost";
        public string EnterExitButton = "EnterExit";
        public string Dpad = "Dpad";
        public string Look = "Look";

        public string MouseX = "Mouse X";
        public string MouseY = "Mouse Y";
        #endregion

        public float Horizontal { get; private set; }
        public float Acceleration { get; private set; }
        public float BrakeReverse { get; private set; }
        public float Pitch { get; private set; }
        public bool HandBrake { get; private set; }
        public bool Boost { get; private set; }
        public Vector2 ViewDelta { get; private set; }
        public bool ManualCameraRotation { get; private set; }

        public UnityEvent OnChangeViewAction;

        float TargetHorizontal;

        CarLighting CarLighting;

        event System.Action OnDestroyAction;

        public bool IsFirstPlayer { get; private set; }

        public static int GamepadP1no;
        public static int GamepadP2no;

        Vector2 PrevDpadValue;
        bool DpadUpDown;
        bool DpadDownDown;
        bool DpadLeftDown;
        bool DpadRightDown;

        // enabling and disabling input
        public void OnEnable()
        {
            controls.Enable();
        }

        public void OnDisable()
        {
            controls.Disable();
        }

        // create instance and hook up controls
        private void Awake()
        {
            controls = new SimulatorInputActions();
            controls.Player.Dpad.performed += ctx => UpdateDpad(controls.Player.Dpad.ReadValue<Vector2>());
            controls.Player.Dpad.canceled += ctx => UpdateDpad(Vector2.zero);

            controls.Player.ChangeView.performed += ctx => ChangeView();

            // call UpdateOffset() on camera gameobject
            controls.Player.Look.performed += ctx => UpdateCamera(controls.Player.Look.ReadValue<Vector2>());
            controls.Player.Look.canceled += ctx => UpdateCamera(Vector2.zero);

            // convert vector2 x to steer value
            controls.Player.Steer.performed += ctx => SetSteer(controls.Player.Steer.ReadValue<Vector2>().x);
            controls.Player.Steer.canceled += ctx => SetSteer(0);

            controls.Player.Acceleration.performed += ctx => SetAcceleration(controls.Player.Acceleration.ReadValue<float>());
            controls.Player.Acceleration.canceled += ctx => SetAcceleration(0);
            controls.Player.BrakeReverse.performed += ctx => SetBrakeReverse(controls.Player.BrakeReverse.ReadValue<float>());
            controls.Player.BrakeReverse.canceled += ctx => SetBrakeReverse(0);
            controls.Player.HandBrake.performed += ctx => SetHandBrake(controls.Player.HandBrake.ReadValue<float>() > 0);
            controls.Player.HandBrake.canceled += ctx => SetHandBrake(false);
        }

        private void Update()
        {
            Horizontal = Mathf.MoveTowards (Horizontal, TargetHorizontal, Time.deltaTime * HorizontalChangeSpeed);

            // ManualCameraRotation = RotateCameraWithMousePressed ? Input.GetMouseButton(0) : ViewDelta.sqrMagnitude > 0.05f;
            // ViewDelta = new Vector2(Input.GetAxis(MouseX), Input.GetAxis(MouseY));

            //Added by player to stop midway
            if (AllowToMove)
            {
                UpdateKeys();
            }
            else
            {
                SetAcceleration(0);
                SetBrakeReverse(-1);
                SetBoost(false);
                SetHandBrake(true);
                SetPitch(0);
            }
        }

        void UpdateKeys ()
        {
            if (GameController.Instance)
            {
                if (Input.GetKeyDown (KeyCode.F3))
                {
                    GameController.Instance.RestartScene ();
                }

                if (!GameController.SplitScreen && Input.GetKeyDown (KeyCode.N))
                {
                    GameController.Instance.SetNextCar ();
                }

                if (Input.GetKeyDown (KeyCode.Equals))
                {
                    GameController.Instance.ChangeTimeScale (0.1f);
                }

                if (Input.GetKeyDown (KeyCode.Minus))
                {
                    GameController.Instance.ChangeTimeScale (-0.1f);
                }
            }

            // SetSteer(Input.GetAxis(SteerAxis));
            // SetAcceleration(Input.GetAxis(AccelerationAxis));
            // SetBrakeReverse(Input.GetAxis(BrakeReverseAxis));
            // SetBoost(Input.GetButton(BoostButton));
            // SetHandBrake(Input.GetButton(HandBrakeButton));
            // SetPitch(Input.GetAxis(PitchAxis));
            // 
            // if (Input.GetButtonDown(NextGearButton))
            // {
            //     NextGear();
            // }
            // if (Input.GetButtonDown(PrevGearButton))
            // {
            //     PrevGear();
            // }
            // if (Input.GetButtonDown(SwitchLightsButton))
            // {
            //     SwitchLights();
            // }
            // if (Input.GetButtonDown(SwitchLeftTurnLightsButton) || DpadLeftDown)
            // {
            //     SwitchLeftTurnSignal();
            // }
            // if (Input.GetButtonDown(SwitchRightTurnLightsButton) || DpadRightDown)
            // {
            //     SwitchRightTurnSignal();
            // }
            // if (Input.GetButtonDown(SwitchAlarmButton) || DpadDownDown)
            // {
            //     SwitchAlarm();
            // }
            // if (Input.GetButtonDown(ConnectTrailerButton) || DpadUpDown)
            // {
            //     ConnectTrailer();
            // }
            // if (Input.GetButtonDown(ResetCarButton))
            // {
            //     ResetCar();
            // }
            // if (Input.GetKeyDown(RestoreCarButton))
            // {
            //     RestoreCar();
            // }
            // if (Input.GetButtonDown(ChangeViewButton))
            // {
            //     ChangeView();
            // }
            // if (Input.GetButtonDown(EnterExitButton))
            // {
            //     TryExitFromCar();
            // }
        }

        void UpdateCamera(Vector2 value)
        {
            if (AllowToMove)
            {
                TargetCamera.UpdateOffset(value.x);
                Debug.Log("Camera: " + value);
            }
        }

        // read from ReadValue
        void UpdateDpad(Vector2 currentDpad)
        {
            if (AllowToMove)
            {
                DpadUpDown = currentDpad.y > 0 && PrevDpadValue.y == 0;
                DpadDownDown = currentDpad.y < 0 && PrevDpadValue.y == 0;
                DpadRightDown = currentDpad.x > 0 && PrevDpadValue.x == 0;
                DpadLeftDown = currentDpad.x < 0 && PrevDpadValue.x == 0;

                PrevDpadValue = currentDpad;
                Debug.Log("Dpad: " + currentDpad);
            }
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

        public void SetAcceleration (float value)
        {
            if (AllowToMove) 
            { 
                Acceleration = value;
                Debug.Log("Acceleration: " + value);
            }
        }

        public void SetBrakeReverse (float value)
        {
            if (AllowToMove) 
            { 
                BrakeReverse = value;
                Debug.Log("BrakeReverse: " + value);
            }
        }

        public void SetSteer (float value)
        {
            TargetHorizontal = value;
            Debug.Log("Steer: " + value);
        }

        public void SetPitch (float value)
        {
            Pitch = value;
        }

        public void NextGear ()
        {
            if (Car)
            {
                Car.NextGear ();
            }
        }

        public void PrevGear ()
        {
            if (Car)
            {
                Car.PrevGear ();
            }
        }

        public void SwitchLights ()
        {
            CarLighting.SwitchMainLights ();
        }

        public void SwitchLeftTurnSignal ()
        {
            CarLighting.TurnsEnable (TurnsStates.Left);
        }

        public void SwitchRightTurnSignal ()
        {
            CarLighting.TurnsEnable (TurnsStates.Right);
        }

        public void SwitchAlarm ()
        {
            CarLighting.TurnsEnable (TurnsStates.Alarm);
        }

        public void ConnectTrailer ()
        {
            if (Car)
            {
                Car.TryConnectDisconnectTrailer ();
            }
        }

        public void ResetCar ()
        {
            Vehicle.ResetVehicle ();
        }

        public void RestoreCar ()
        {
            Vehicle.RestoreVehicle ();
        }

        public void ChangeView ()
        {
            if (AllowToMove)
            {
                OnChangeViewAction.Invoke();
            }
            Debug.Log("ChangeView");
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
            if (AllowToMove) 
            { 
                HandBrake = value;
                Debug.Log("HandBrake: " + value);
            }
        }

        public void SetBoost (bool value)
        {
            Boost = value;
        }

#endregion //Set input

    }
}
