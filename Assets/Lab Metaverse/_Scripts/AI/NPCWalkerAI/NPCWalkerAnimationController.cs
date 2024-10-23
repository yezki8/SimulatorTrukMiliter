using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCWalkerAnimationController : MonoBehaviour
{
    [SerializeField] private float _currentSpeed;
    [SerializeField] private NavMeshAgent _nav;
    [SerializeField] private Animator _anim;
    private Vector3 previous;

    private void Start()
    {
        previous = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        SetSpeed();
    }

    void SetSpeed()
    {

        _currentSpeed = ((transform.position - previous).magnitude) / Time.deltaTime;
        previous = transform.position;
        _anim.SetFloat("speed", _currentSpeed);
    }
}
