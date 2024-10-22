using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PG
{
    /// <summary>
    /// Component responsible for all light signals.
    /// </summary>
    public class LightObject :GlassDO
    {
        public CarLightType CarLightType;
        public Light LightGO;
        public Material OnLightMaterial;                //Material with glow, used for soft and hard switching.

        [Header("Headlight settings")]
        [SerializeField] private bool EnableLight = true;
        // light direction
        [SerializeField] private Vector3 MainLightDirection = Vector3.zero;
        [SerializeField] private Vector3 FarLightDirection = Vector3.zero;

        [Header("Soft Switch settings")]
        public bool IsSoftSwitch;
        public float OnSwitchSpeed = 10f;
        public float OffSwitchSpeed = 2f;
        public float Intensity = 2f;                    //Maximum glow intensity.

        [Header("Main settings")]
        public bool EnableOnStart;

        MaterialPropertyBlock MaterialBlock;
        Material MaterialForSoftSwitch;
        Animator LightsAnimator;
        Color BaseColor;

        //IDs for accessing properties, so as not to use the string (Optimization).
        int EmissionColorPropertyID;
        int AnimatorLightIsOnID;
        Coroutine SoftSwitchCoroutine;

        public bool LightIsOn { get; private set; }

        /// <summary>
        /// Initialize soft switch if the IsSoftSwitch flag is set.
        /// </summary>
        public void TryInitSoftSwitch ()
        {
            if (!IsSoftSwitch)
            {
                return;
            }

            if (!IsInited)
            {
                InitDamageObject ();
                // set lightGO to current direction if its zero
                if (MainLightDirection == Vector3.zero)
                {
                    MainLightDirection = transform.localEulerAngles;
                }
                // assume same direction as main light if far light direction is zero
                if (FarLightDirection == Vector3.zero)
                {
                    FarLightDirection = transform.localEulerAngles;
                }
            }

            if (Renderer)
            {
                MaterialForSoftSwitch = OnLightMaterial;
                Materials[GlassMaterialIndex] = OnLightMaterial;
                Renderer.materials = Materials;
                // get base color
                BaseColor = OnLightMaterial.GetColor(EmissionColorPropertyID);
            }
        }

        public override void InitDamageObject ()
        {
            base.InitDamageObject ();

            LightsAnimator = GetComponent<Animator> ();

            EmissionColorPropertyID = Shader.PropertyToID ("_EmissiveColor");
            AnimatorLightIsOnID = Animator.StringToHash ("LightIsOn");
            MaterialBlock = new MaterialPropertyBlock ();
        }

        void Start ()
        {
            LightIsOn = !EnableOnStart;
            Switch (EnableOnStart, forceSwitch: true);
        }

        /// <summary>
        /// Switch light LightIsOn =! LightIsOn.
        /// </summary>
        public void Switch ()
        {
            Switch (!LightIsOn, forceSwitch: true);
        }

        /// <summary>
        /// Switch with parameters.
        /// </summary>
        public void Switch (bool value, bool forceSwitch = false, HeadlightsType type = HeadlightsType.Main)
        {
            value &= !IsDead;

            if (LightIsOn == value)
            {
                return;
            }

            LightIsOn = value;

            if (Renderer)
            {
                if (IsSoftSwitch)
                {
                    if (SoftSwitchCoroutine != null)
                    {
                        StopCoroutine (SoftSwitchCoroutine);
                    }

                    if (MaterialForSoftSwitch != null)
                    {
                        SoftSwitchCoroutine = StartCoroutine (SoftSwitch (LightIsOn, forceSwitch, type));
                    }
                }
                else if (!IsDead)
                {
                    HardSwitch (type);
                }
            }

            //The animator is needed to turn on the headlights such as those of the PG86 or turn on the off the gameobject.
            if (LightsAnimator != null && !IsDead)
            {
                LightsAnimator.SetBool (AnimatorLightIsOnID, LightIsOn);
            }
        }

        IEnumerator SoftSwitch (bool value, bool forceSwitch = false, HeadlightsType type = HeadlightsType.Main)
        {
            //Calculation of the start and target Intensity glow.
            Color targetColor = (value? BaseColor * Intensity: BaseColor);
            Color startColor = (value? BaseColor : BaseColor * Intensity);
            var speed = value? OnSwitchSpeed: OffSwitchSpeed;
            float timer = 0;

            if (!value && LightGO)
            {
                LightGO.SetActive (value);
            }

            if (!forceSwitch)
            {
                while (timer < 1)
                {
                    var color = Color.Lerp (startColor, targetColor, timer);
                    MaterialBlock.SetColor (EmissionColorPropertyID, color);
                    Renderer.SetPropertyBlock (MaterialBlock);
                    timer += speed * Time.deltaTime;
                    yield return null;
                }
            }

            if (value && LightGO && type != HeadlightsType.Dim)
            {
                // set direction
                if (type == HeadlightsType.Main)
                {
                    LightGO.transform.localEulerAngles = MainLightDirection;
                }
                else
                {
                    LightGO.transform.localEulerAngles = FarLightDirection;
                }
                LightGO.SetActive (value);
            }

            //Used MaterialBlock since all light objects can use the same material.
            MaterialBlock.SetColor (EmissionColorPropertyID, targetColor);
            Renderer.SetPropertyBlock (MaterialBlock);

            SoftSwitchCoroutine = null;
        }

        /// <summary>
        /// Just material change, switching on/off occurs in one frame.
        /// </summary>
        void HardSwitch (HeadlightsType type = HeadlightsType.Main)
        {
            if (LightIsOn)
            {
                Materials[GlassMaterialIndex] = OnLightMaterial;
            }
            else
            {
                Materials[GlassMaterialIndex] = DefaultGlassMaterial;
            }

            if (LightGO && type != HeadlightsType.Dim)
            {
                // set direction
                if (type == HeadlightsType.Main)
                {
                    LightGO.transform.eulerAngles = MainLightDirection;
                }
                else
                {
                    LightGO.transform.eulerAngles = FarLightDirection;
                }
                LightGO.SetActive (LightIsOn);
            }

            Renderer.materials = Materials;
        }

        public override void DoDeath ()
        {
            base.DoDeath ();

            if (LightsAnimator)
            {
                LightsAnimator.SetBool ("IsBroken", true);
            }

            if (LightGO)
            {
                LightGO.SetActive (false);
            }
        }

        public override void RestoreObject ()
        {
            base.RestoreObject ();

            if (LightsAnimator)
            {
                LightsAnimator.SetBool ("IsBroken", false);
            }

            if (LightGO)
            {
                LightGO.SetActive (LightIsOn);
            }
        }
    }
}
