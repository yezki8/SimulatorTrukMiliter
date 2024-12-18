﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PG
{
    /// <summary>
    /// Car light logic.
    /// </summary>
    public class CarLighting :MonoBehaviour
    {
#pragma warning disable 0649

        [SerializeField] float TurnsSwitchHalfRepeatTime = 0.5f;   //Half time light on/off.
        [SerializeField] private DashboardUIHandler _dashboardUIHandler;

#pragma warning restore 0649

        //All light is searched for in child elements, 
        //depending on the set tag, the light gets into the desired list.
        List<LightObject> MainLights = new List<LightObject>();
        List<LightObject> LeftTurnLights = new List<LightObject>();
        List<LightObject> RightTurnLights = new List<LightObject>();
        List<LightObject> BrakeLights = new List<LightObject>();
        List<LightObject> ReverseLights = new List<LightObject>();

        CarController _Car;
        //Used property, to be able to connect the trailer to the vehicle.
        public CarController Car 
        { 
            get 
            { 
                return _Car; 
            }
            set
            {
                if (_Car != null)
                {
                    Car.OnChangeGearAction -= OnChangeGear;
                    OnChangeGear (0);
                }
                _Car = value;

                if (_Car != null)
                {
                    Car.OnChangeGearAction += OnChangeGear;
                }
            }
        }

        bool InBrake;
        public bool MainLightsIsOn;
        public bool FarLightsIsOn;
        Coroutine TurnsCotoutine;
        List<LightObject> ActiveTurns = new List<LightObject>();
        TurnsStates CurrentTurnsState = TurnsStates.Off;

        public event System.Action<CarLightType, bool> OnSetActiveLight;
        
        public CarLighting AdditionalLighting { get; set; }

        void Start ()
        {
            //Searching and distributing all lights.
            var lights = GetComponentsInChildren<LightObject>();
            foreach (var l in lights)
            {
                switch (l.CarLightType)
                {
                    case CarLightType.Main:
                        MainLights.Add (l); 
                        break;
                    case CarLightType.TurnLeft:
                        LeftTurnLights.Add (l);
                        break;
                    case CarLightType.TurnRight:
                        RightTurnLights.Add (l);
                        break;
                    case CarLightType.Brake:
                        BrakeLights.Add (l);
                        break;
                    case CarLightType.Reverse:
                        ReverseLights.Add (l);
                        break;

                }
            }

            Car = GetComponent<CarController> ();

            //Initializing soft light switching.
            InitSoftSwitches (MainLights);
            InitSoftSwitches (ReverseLights);
            InitSoftSwitches (BrakeLights);
            InitSoftSwitches (LeftTurnLights);
            InitSoftSwitches (RightTurnLights);
        }

        private void Update ()
        {
            bool carInBrake = Car != null && Car.CurrentBrake > 0;
            if (InBrake != carInBrake)
            {
                InBrake = carInBrake;
                SetActiveBrake (InBrake);
            }
        }

        /// <summary>
        /// Initiates soft switching of the light as needed.
        /// </summary>
        void InitSoftSwitches (List<LightObject> lights)
        {
            foreach (var light in lights)
            {
                light.TryInitSoftSwitch ();
            }
        }

        /// <summary>
        /// Reverse light switch logic.
        /// </summary>
        public void OnChangeGear (int gear)
        {
            SetActiveReverse (gear < 0);
        }

        public void SwithOffAllLights ()
        {
            SetActiveMainLights (false, HeadlightsType.Main);
            SetActiveBrake (false);
            SetActiveReverse (false);
            TurnsEnable (TurnsStates.Off);
        }

        public void SetLights (CarLightType type, bool value) 
        { 
            if (type == CarLightType.TurnLeft || type == CarLightType.TurnRight || type == CarLightType.Alarm)
            {
                TurnsStates state= TurnsStates.Off;

                if (value)
                {
                    switch (type)
                    {
                        case CarLightType.TurnLeft: state = TurnsStates.Left; break;
                        case CarLightType.TurnRight: state = TurnsStates.Right; break;
                        case CarLightType.Alarm: state = TurnsStates.Alarm; break;
                    }
                }

                TurnsEnable (state);
            }
            else if (type == CarLightType.Main)
            {
                SetActiveMainLights (value, HeadlightsType.Main);
            }
            else if (type == CarLightType.Brake)
            {
                SetActiveBrake (value);
            }
            else if (type == CarLightType.Reverse)
            {
                SetActiveReverse (value);
            }
        }

        /// <summary>
        /// Main light switch.
        /// </summary>
        public void MainLightsOff ()
        {
            if (MainLights.Count > 0)
            {
                MainLightsIsOn = false;
                SetActiveMainLights(false, HeadlightsType.Main);
                _dashboardUIHandler.ChangeMainLampStatus(false);
            }
        }

        public void MainLightsOn ()
        {
            if (MainLights.Count > 0)
            {
                MainLightsIsOn = true;
                SetActiveMainLights(true, HeadlightsType.Main);
                _dashboardUIHandler.ChangeMainLampStatus(true);
            }
        }

        public void SwitchMainLights ()
        {
            if (MainLights.Count > 0)
            {
                MainLightsIsOn = !MainLightsIsOn;
                SetActiveMainLights (MainLightsIsOn, HeadlightsType.Main);
            }
        }

        public void FarLightsOn()
        {
            if (MainLights.Count > 0)
            {
                SetActiveMainLights(false, HeadlightsType.Main);
                SetActiveMainLights(true, HeadlightsType.Far);
                _dashboardUIHandler.ChangeFarLampStatus(true);
            }
        }

        public void FarLightsOff()
        {
            if (MainLights.Count > 0)
            {
                SetActiveMainLights(false, HeadlightsType.Far);
                _dashboardUIHandler.ChangeFarLampStatus(false);
            }
        }

        public void SwitchDimLights()
        {
            if (MainLights.Count > 0)
            {
                SetActiveMainLights(MainLightsIsOn, HeadlightsType.Dim);
            }
        }

        public void SetActiveMainLights (bool value, HeadlightsType type)
        {
            MainLights.ForEach (l => l.Switch (value, false, type));

            OnSetActiveLight.SafeInvoke (CarLightType.Main, value);

            if (AdditionalLighting)
            {
                AdditionalLighting.SetActiveMainLights (value, HeadlightsType.Main);
            }
        }

        public void SetActiveBrake (bool value)
        {
            BrakeLights.ForEach (l => l.Switch (value));

            OnSetActiveLight.SafeInvoke (CarLightType.Brake, value);

            if (AdditionalLighting)
            {
                AdditionalLighting.SetActiveBrake (value);
            }
        }

        public void SetActiveReverse (bool value)
        {
            ReverseLights.ForEach (l => l.Switch (value));

            OnSetActiveLight.SafeInvoke (CarLightType.Reverse, value);

            if (AdditionalLighting)
            {
                AdditionalLighting.SetActiveReverse (value);
            }
        }

        /// <summary>
        /// Turns lights switch logic.
        /// </summary>
        public void TurnsEnable (TurnsStates state)
        {
            TurnsDisable ();

            if (CurrentTurnsState != state)
            {
                CurrentTurnsState = state;
                TurnsCotoutine = StartCoroutine (DoTurnsEnable (CurrentTurnsState));
            }
            else
            {
                switch (CurrentTurnsState)
                {
                    case TurnsStates.Left: OnSetActiveLight.SafeInvoke (CarLightType.TurnLeft, false); break;
                    case TurnsStates.Right: OnSetActiveLight.SafeInvoke (CarLightType.TurnRight, false); break;
                    case TurnsStates.Alarm: OnSetActiveLight.SafeInvoke (CarLightType.Alarm, false); break;
                }

                CurrentTurnsState = TurnsStates.Off;
                _dashboardUIHandler.CallTurnSignal((int)CurrentTurnsState, false);
            }

            if (AdditionalLighting)
            {
                AdditionalLighting.TurnsEnable (state);
            }
        }

        /// <summary>
        /// Turn off blinking of turn signals.
        /// </summary>
        void TurnsDisable ()
        {
            if (TurnsCotoutine != null)
            {
                StopCoroutine (TurnsCotoutine);
            }
            ActiveTurns.ForEach (l => l.Switch (false));
        }

        /// <summary>
        /// Turn signals IEnumerator.
        /// </summary>
        IEnumerator DoTurnsEnable (TurnsStates state)
        {
            ActiveTurns = new List<LightObject> ();
            
            switch (state)
            {
                case TurnsStates.Left:
                ActiveTurns = LeftTurnLights;
                OnSetActiveLight.SafeInvoke (CarLightType.TurnLeft, true);
                break;

                case TurnsStates.Right:
                ActiveTurns = RightTurnLights;
                OnSetActiveLight.SafeInvoke (CarLightType.TurnRight, true);
                break;

                case TurnsStates.Alarm:
                ActiveTurns.AddRange (LeftTurnLights);
                ActiveTurns.AddRange (RightTurnLights);
                OnSetActiveLight.SafeInvoke (CarLightType.Alarm, true);
                break;
            }

            //Infinite cycle of switching on and off.
            while (true)
            {
                ActiveTurns.ForEach (l => l.Switch (true));
                _dashboardUIHandler.CallTurnSignal((int)CurrentTurnsState, true);
                yield return new WaitForSeconds (TurnsSwitchHalfRepeatTime);
                ActiveTurns.ForEach (l => l.Switch (false));
                _dashboardUIHandler.CallTurnSignal((int)CurrentTurnsState, false);
                yield return new WaitForSeconds (TurnsSwitchHalfRepeatTime);
            }
        }
    }

    public enum TurnsStates
    {
        Off,
        Left,
        Right,
        Alarm
    }

    public enum CarLightType
    {
        Main,
        Brake,
        TurnLeft,
        TurnRight,
        Reverse,
        Alarm
    }

    public enum HeadlightsType
    {
        Main,
        Far,
        Dim
    }
}
