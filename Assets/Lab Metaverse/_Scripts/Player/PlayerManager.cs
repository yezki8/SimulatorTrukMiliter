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
        _carControllerInput.EnableControls();
    }

    public void DisableTruck()
    {
        _carControllerInput.DisableControls();
        _carController.StopEngine();
        _carControllerInput.ResetCar();
    }

    private void OnTriggerEnter(Collider other)
    {
        //To Ensure these trigger calls only happen during gameplay
        if (GameStateController.Instance.GameState == StateOfGame.Match)
        {
            switch (other.tag)
            {
                case "Checkpoint":
                    CheckpointManager.Instance.ActivateCheckpoint(other.gameObject);
                    break;
                case "Finish":
                    GameStateController.Instance.ChangeGameState((int)StateOfGame.End);
                    other.GetComponent<CheckpointController>().SetCheckpointVisual(false);
                    break;
                case "ConvoyStart":
                    ConvoySystem.Instance.StartConvoyVehicles();
                    break;
                case "ConvoyFinish":
                    ConvoySystem.Instance.StopConvoyVehicles();
                    break;
            }
        }
    }
}
