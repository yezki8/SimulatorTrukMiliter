using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCWalkerAnimationController : MonoBehaviour
{
    [SerializeField] private float _currentSpeed;
    [SerializeField] private NavMeshAgent _nav;
    [SerializeField] private Animator _anim;

    // Update is called once per frame
    void Update()
    {
        SetSpeed();
    }

    void SetSpeed()
    {
        _currentSpeed = _nav.velocity.magnitude / _nav.speed;
        _anim.SetFloat("speed", _currentSpeed);
    }
}
