using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCWalkerNavigator : MonoBehaviour
{
    private Transform initTransform;
    [SerializeField] private Transform _navTarget;
    [SerializeField] private NavMeshAgent _agent;

    // Start is called before the first frame update
    void Start()
    {
        initTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (_navTarget != null )
            _agent.destination = _navTarget.position;
    }

    public void AssignDestination(Transform target)
    {
        _navTarget = target;
    }

    //private void OnTriggerEnter(Collider other)
    //{

    //    Debug.Log("Feed");
    //    if (other.tag.Contains("WalkingTarget"))
    //    {
    //        AssignDestination(other.GetComponent<NPCWalkerDestinationPoint>().NextWalkingTarget);
    //        Debug.Log("Sneed");
    //    }
    //}
}
