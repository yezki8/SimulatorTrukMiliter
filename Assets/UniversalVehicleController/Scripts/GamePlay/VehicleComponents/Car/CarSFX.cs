﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PG
{
    /// <summary>
    /// Sound effects, using FMOD.
    /// </summary>
    public class CarSFX : VehicleSFX
    {
        [Header("CarSFX")]

#pragma warning disable 0649

        [SerializeField] AudioSource EngineSourceRef;
        [SerializeField] AudioClip StartEngineClip;
        [SerializeField] AudioClip StopEngineClip;
        [SerializeField] AudioClip LowEngineClip;
        [SerializeField] AudioClip MediumEngineClip;
        [SerializeField] AudioClip HighEngineClip;

        [SerializeField] float MinEnginePitch = 0.5f;
        [SerializeField] float MaxEnginePitch = 1.5f;

        [Header("Additional settings")]
        [SerializeField] AudioSource TurboSource;
        [SerializeField] AudioClip TurboBlowOffClip;
        [SerializeField] float MaxBlowOffVolume = 0.5f;
        [SerializeField] float MinTimeBetweenBlowOffSounds = 1;
        [SerializeField] float MaxTurboVolume = 0.5f;
        [SerializeField] float MinTurboPith = 0.5f;
        [SerializeField] float MaxTurboPith = 1.5f;

        [SerializeField] AudioSource BoostSource;

        [SerializeField] List<AudioClip> BackFireClips;

        [Header("Wind Sound")]
        [SerializeField] AudioSource SpeedWindSource;
        [SerializeField] float WindSoundStartSpeed = 20;
        [SerializeField] float WindSoundMaxSpeed = 100;
        [SerializeField] float WindSoundStartPitch = 0.4f;
        [SerializeField] float WindSoundMaxPitch = 1.5f;

        [Header("Horn")]
        [SerializeField] AudioSource HornSource;
        [SerializeField] AudioSource HornTrailSource;
        [SerializeField] AudioClip HornClip;
        [SerializeField] AudioClip HornTrailClip;
        [SerializeField] float LoopStart = 0.2f;
        [SerializeField] float LoopEnd = 0.8f;

#pragma warning restore 0649


        CarController Car;
        float LastBlowOffTime;
        float[] EngineSourcesRanges = new float[1] { 1f };
        List<AudioSource> EngineSources = new List<AudioSource>();

        Coroutine HornCoroutine;

        protected override void Start()
        {
            base.Start();

            Car = Vehicle as CarController;

            if (Car == null)
            {
                Debug.LogErrorFormat("[{0}] CarSFX without CarController in parent", name);
                enabled = false;
                return;
            }

            if (BoostSource)
            {
                if (Car.Engine.EnableBoost && BoostSource.gameObject.activeInHierarchy)
                {
                    UpdateAction += UpdateBoost;
                }
                else
                {
                    BoostSource.Stop();
                }
            }

            if (TurboSource)
            {
                if (Car.Engine.EnableTurbo && TurboSource.gameObject.activeInHierarchy)
                {
                    UpdateAction += UpdateTurbo;
                }
                else
                {
                    TurboSource.Stop();
                }
            }

            Car.BackFireAction += OnBackFire;
            Car.OnStartEngineAction += StartEngine;
            Car.OnStopEngineAction += StopEngine;

            if (EngineSourceRef && EngineSourceRef.gameObject.activeInHierarchy)
            {

                //Create engine sounds list.
                List<AudioClip> engineClips = new List<AudioClip>();
                if (LowEngineClip != null)
                {
                    engineClips.Add(LowEngineClip);
                }
                if (MediumEngineClip != null)
                {
                    engineClips.Add(MediumEngineClip);
                }
                if (HighEngineClip != null)
                {
                    engineClips.Add(HighEngineClip);
                }

                if (engineClips.Count == 2)
                {
                    //If the engine has 2 sounds, then they will switch at 30% rpm.
                    EngineSourcesRanges = new float[2] { 0.3f, 1f };
                }
                else if (engineClips.Count == 3)
                {
                    //If the engine has 3 sounds, then they will switch at 30% and 60% rpm.
                    EngineSourcesRanges = new float[3] { 0.3f, 0.6f, 1f };
                }

                //Init Engine sounds.
                if (engineClips != null && engineClips.Count > 0)
                {
                    AudioSource engineSource;

                    for (int i = 0; i < engineClips.Count; i++)
                    {
                        if (EngineSourceRef.clip == engineClips[i])
                        {
                            engineSource = EngineSourceRef;
                            EngineSourceRef.transform.SetSiblingIndex(EngineSourceRef.transform.parent.childCount);
                        }
                        else
                        {
                            engineSource = Instantiate(EngineSourceRef, EngineSourceRef.transform.parent);
                            engineSource.clip = engineClips[i];
                            engineSource.Play();
                        }

                        engineSource.name = string.Format("Engine source ({0})", i);
                        EngineSources.Add(engineSource);
                    }

                    if (!EngineSources.Contains(EngineSourceRef))
                    {
                        Destroy(EngineSourceRef);
                    }
                }

                if (!Car.EngineIsOn)
                {
                    if (EngineSources != null && EngineSources.Count > 0)
                    {
                        EngineSources.ForEach(s => s.pitch = 0);
                    }
                    else
                    {
                        EngineSourceRef.pitch = 0;
                    }
                }

                UpdateAction += UpdateEngine;
            }

            if (SpeedWindSource && SpeedWindSource.gameObject.activeInHierarchy)
            {
                UpdateAction += UpdateWindEffect;
            }

            if (HornSource && HornClip)
            {
                HornSource.clip = HornClip;
            }

            if (HornTrailSource && HornTrailClip)
            {
                HornTrailSource.clip = HornTrailClip;
            }

        }

        void StartEngine(float startDellay)
        {
            if (StartEngineClip != null)
            {
                OtherEffectsSource.PlayOneShot(StartEngineClip);
            }
        }

        void StopEngine()
        {
            if (StopEngineClip != null)
            {
                OtherEffectsSource.PlayOneShot(StartEngineClip);
            }
        }

        //Base engine sounds
        void UpdateEngine()
        {
            if (Car.EngineIsOn)
            {
                if (EngineSources.Count == 0 && EngineSourceRef && EngineSourceRef.gameObject.activeInHierarchy && !float.IsNaN(Car.EngineRPM))
                {
                    EngineSourceRef.pitch = Mathf.Lerp(MinEnginePitch, MaxEnginePitch, (Car.EngineRPM - Car.MinRPM) / (Car.MaxRPM - Car.MinRPM));
                }

                // NaN Checks
                else if (EngineSources.Count > 1 && !float.IsNaN(Car.EngineRPM))
                {
                    float rpmNorm = ((Car.EngineRPM - Car.MinRPM) / (Car.MaxRPM - Car.MinRPM)).Clamp();
                    float pith = Mathf.Lerp(MinEnginePitch, MaxEnginePitch, rpmNorm);

                    for (int i = 0; i < EngineSources.Count; i++)
                    {
                        EngineSources[i].pitch = pith;

                        if (i > 0 && rpmNorm < EngineSourcesRanges[i - 1])
                        {
                            EngineSources[i].volume = Mathf.InverseLerp(0.2f, 0, EngineSourcesRanges[i - 1] - rpmNorm);
                        }
                        else if (rpmNorm > EngineSourcesRanges[i])
                        {
                            EngineSources[i].volume = Mathf.InverseLerp(0.3f, 0, rpmNorm - EngineSourcesRanges[i]);
                        }
                        else
                        {
                            EngineSources[i].volume = 1;
                        }

                        if (Mathf.Approximately(EngineSources[i].volume, 0) && EngineSources[i].isPlaying)
                        {
                            EngineSources[i].Stop();
                        }

                        if (EngineSources[i].volume > 0 && !EngineSources[i].isPlaying)
                        {
                            EngineSources[i].Play();
                        }
                    }
                }
            }
            else //if (!EngineIsON)
            {
                float pith = Mathf.Lerp(0, MinEnginePitch, (Car.EngineRPM / Car.MinRPM).Clamp());
                if (EngineSources.Count == 0 && EngineSourceRef && EngineSourceRef.gameObject.activeInHierarchy)
                {
                    EngineSourceRef.pitch = Mathf.MoveTowards(EngineSourceRef.pitch, pith, Time.deltaTime);
                }
                else if (EngineSources.Count > 1)
                {
                    EngineSources[0].pitch = Mathf.MoveTowards(EngineSources[0].pitch, pith, Time.deltaTime);
                    for (int i = 1; i < EngineSources.Count; i++)
                    {
                        EngineSources[i].pitch = pith;
                        if (EngineSources[i].isPlaying)
                        {
                            EngineSources[i].Stop();
                        }
                    }
                }
            }
        }

        //Additional turbo sound
        void UpdateTurbo()
        {
            if (Car.Engine.EnableTurbo && TurboSource && TurboSource.gameObject.activeInHierarchy)
            {
                // prevent pitch to infinite
                float targetTurbo = Car.CurrentTurbo;
                if (targetTurbo > 0 && float.IsNaN(targetTurbo))
                {
                    targetTurbo = 0;
                }
                TurboSource.volume = Mathf.Lerp(0, MaxTurboVolume, targetTurbo);
                TurboSource.pitch = Mathf.Lerp(MinTurboPith, MaxTurboPith, targetTurbo);
                if (targetTurbo > 0.2f && (Car.CurrentAcceleration < 0.2f || Car.InChangeGear) && ((Time.realtimeSinceStartup - LastBlowOffTime) > MinTimeBetweenBlowOffSounds))
                {
                    OtherEffectsSource.PlayOneShot(TurboBlowOffClip, targetTurbo * MaxBlowOffVolume);
                    LastBlowOffTime = Time.realtimeSinceStartup;
                }
            }
        }

        //Additional boost sound
        void UpdateBoost()
        {
            if (Car.Engine.EnableBoost && BoostSource && BoostSource.gameObject.activeInHierarchy)
            {
                if (Car.InBoost && !BoostSource.isPlaying)
                {
                    BoostSource.Play();
                }
                if (!Car.InBoost && BoostSource.isPlaying)
                {
                    BoostSource.Stop();
                }
            }
        }

        void UpdateWindEffect()
        {
            if (Car.IsPlayerVehicle)
            {
                var curentSpeedNorm = Mathf.InverseLerp(WindSoundStartSpeed, WindSoundMaxSpeed, Car.CurrentSpeed);
                if (curentSpeedNorm > 0 && !SpeedWindSource.isPlaying)
                {
                    SpeedWindSource.Play();
                }
                SpeedWindSource.volume = curentSpeedNorm;
                SpeedWindSource.pitch = Mathf.Lerp(WindSoundStartPitch, WindSoundMaxPitch, curentSpeedNorm);
            }
            else if (SpeedWindSource.isPlaying)
            {
                SpeedWindSource.Stop();
            }
        }

        void OnBackFire()
        {
            if (BackFireClips != null && BackFireClips.Count > 0)
            {
                OtherEffectsSource.PlayOneShot(BackFireClips[Random.Range(0, BackFireClips.Count - 1)]);
            }
        }

        // create a horn loop mechanism
        // start the horn sound
        // after reaching loop end, loop back to loop start
        // after horn button is released, play sound after the loop end

        public void HornOn()
        {
            if (HornSource && HornClip)
            {
                HornSource.loop = true;
                HornSource.Play();

                // if the horn is not released, loop the horn sound
                HornCoroutine = StartCoroutine(LoopHorn());
            }
        }

        IEnumerator LoopHorn()
        {
            while (HornSource.isPlaying)
            {
                if (HornSource.time >= LoopEnd * HornSource.clip.length)
                {
                    HornSource.time = LoopStart * HornSource.clip.length;
                }
                yield return null;
            }
        }

        public void HornOff()
        {
            // check if coroutine is running and stop it
            StopCoroutine(HornCoroutine);
            HornSource.loop = false;
            if (HornTrailSource && HornTrailClip)
            {
                HornTrailSource.Play();
            }
            HornSource.Stop();
        }
    }
}
