using PG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCVehicleController : MonoBehaviour
{
    public void ResetNPCVehicle()
    {
        if (gameObject.activeSelf) {
            /* Debug.Log($"NPC Resetted\n" +
                $"Init pos = {initPos}, init rot = {initRot}\n" +
                $"Curr pos = {this.transform.position}, curr rot = {this.transform.rotation}"); */

            // tryget SimAIControl component
            if (TryGetComponent<SimAIControl>(out var simAIControl))
            {
                simAIControl.ResetAIVehicleControlState();
                simAIControl.ResetPosRotProgress();
            }
            var car = GetComponent<CarController>();
            car.ResetVehicle();
            // stop all coroutines
            car.StopAllCoroutines();

            /* Debug.Log($"After Reset\n" +
                $"Init pos = {initPos}, init rot = {initRot}\n" +
                $"Curr pos = {this.transform.position}, curr rot = {this.transform.rotation}"); */
            gameObject.SetActive(false);
        }
    }

    public void EnableNPCVehicles()
    {
        gameObject.SetActive(true);
        GetComponent<SimAIControl>().ResetAIVehicleControlState();
    }
}
