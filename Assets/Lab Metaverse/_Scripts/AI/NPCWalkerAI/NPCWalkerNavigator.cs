using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCWalkerNavigator : MonoBehaviour
{
    private Vector3 initPos;
    private Quaternion initRot;
    [SerializeField] private Transform _navTarget;
    private Transform currentNavTarget;
    [SerializeField] private NavMeshAgent _agent;

    // Start is called before the first frame update
    void Start()
    {
        initPos = this.transform.position;
        initRot = this.transform.rotation;
        ResetPedestrian();
    }

    private void Update()
    {
        if (currentNavTarget != null)
        {
            _agent.destination = currentNavTarget.position;
        }
    }

    public void ResetPedestrian()
    {
        currentNavTarget = _navTarget;
        this.transform.SetPositionAndRotation(initPos, initRot);
        this.gameObject.SetActive(false);
    }

    public void ActivatePedestrian()
    {
        this.gameObject.SetActive(true);
    }

    public void AssignDestination(Transform target)
    {
        currentNavTarget = target; 
    }
}
