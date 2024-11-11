using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayers : MonoBehaviour
{
    [SerializeField] private ParticleSystem _vfx;
    [SerializeField] private AudioSource _sfx;
    [SerializeField] bool _isVfxLoop = true;
    [SerializeField] float _vfxLoopDuration = 3;
    [SerializeField] bool _isSfxLoop = true;
    [SerializeField] float _sfxLoopDuration = 3;
    float sfxCountdown;
    float vfxCountdown;

    // Start is called before the first frame update
    void Start()
    {
        sfxCountdown = _sfxLoopDuration;
        vfxCountdown = _vfxLoopDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isSfxLoop)
        {
            LoopSFX();
        }
        if (_isVfxLoop)
        {
            LoopVFX();
        }
    }

    void LoopSFX()
    {
        if (sfxCountdown > 0)
        {
            sfxCountdown -= 1 * Time.deltaTime;
        }
        else
        {
            _sfx.Play();
            sfxCountdown = _sfxLoopDuration;
        }
    }

    void LoopVFX()
    {
        if (vfxCountdown > 0)
        {
            vfxCountdown -= 1 * Time.deltaTime;
        }
        else
        {
            _vfx.Play();
            vfxCountdown = _vfxLoopDuration;
        }
    }
}
