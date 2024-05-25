using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PG;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    public void StopTruck()
    {
        _carController.StopEngine();        //Disable engine mechanically
        _carController.enabled = false;     //Disable engine script

        //TODO: do lerp / gradual slow down
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;     //Stop truck's velocity
    }

    private void OnTriggerEnter(Collider other)
    {
        //To Ensure these trigger calls only happen during gameplay
        if (GameStateController.Instance.GameState == StateOfGame.Match)
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
}
