using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PG;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    public void StopTruck()
    {
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _carController.StopEngine();
        _carController.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            CheckpointManager.Instance.ActivateCheckpoint(other.gameObject);
        }
        else if (other.tag == "Finish")
        {
            GameStateController.Instance.ChangeGameState((int)StateOfGame.End);
        }
    }
}
