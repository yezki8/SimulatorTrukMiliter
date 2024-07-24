using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PG;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    [SerializeField] private ControllerInput _carControllerInput;

    public void EnableTruck()
    {
        _carControllerInput.AllowToMove = true;
    }

    public void DisableTruck()
    {
        _carController.CurrentGear = 0;
        _carController.StopEngine();
        _carControllerInput.AllowToMove = false;
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
                other.GetComponent<CheckpointController>().SetCheckpointVisual(false);
            }
        }
    }
}
