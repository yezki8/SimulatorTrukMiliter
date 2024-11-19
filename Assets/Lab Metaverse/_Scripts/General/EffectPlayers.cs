using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayers : MonoBehaviour
{
    [SerializeField] private ParticleSystem _vfx;
    [SerializeField] private AudioSource _sfx;
    [SerializeField] bool _isEffectLoop = true;
    [SerializeField] float _effectLoopDuration = 3;
    float vfxCountdown;

    [SerializeField] float _startDelayDuration = 0;
    bool hasInitiatedOnEnable = false;

    private void OnEnable()
    {
        hasInitiatedOnEnable = false;
        StartCoroutine(InitiateEffect());
    }

    private void OnDisable()
    {
        hasInitiatedOnEnable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isEffectLoop && hasInitiatedOnEnable)
        {
            LoopEffect();
        }
    }

    IEnumerator InitiateEffect()
    {
        vfxCountdown = _effectLoopDuration;
        yield return new WaitForSeconds(_startDelayDuration);
        hasInitiatedOnEnable = true;
        PlayEffect();
    }

    void LoopEffect()
    {
        if (vfxCountdown > 0)
        {
            vfxCountdown -= 1 * Time.deltaTime;
        }
        else
        {
            PlayEffect();
        }
    }

    public void PlayEffect()
    {
        _vfx.Play();
        _sfx.Play();
        vfxCountdown = _effectLoopDuration;
    }
}
