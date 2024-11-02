using PG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCVehicleController : MonoBehaviour
{
    private Vector3 initPos;
    private Quaternion initRot;

    // Start is called before the first frame update
    void Start()
    {
        initPos = this.transform.position;
        initRot = this.transform.rotation;
        ResetNPCVehicle();
    }

    public void ResetNPCVehicle()
    {
        /* Debug.Log($"NPC Resetted\n" +
            $"Init pos = {initPos}, init rot = {initRot}\n" +
            $"Curr pos = {this.transform.position}, curr rot = {this.transform.rotation}"); */
        this.GetComponent<CarController>().ResetVehicle();
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<Rigidbody>().MovePosition(initPos + Vector3.up * 1f);
        this.GetComponent<Rigidbody>().MoveRotation(initRot);
        this.GetComponent<Rigidbody>().isKinematic = false;

        /* Debug.Log($"After Reset\n" +
            $"Init pos = {initPos}, init rot = {initRot}\n" +
            $"Curr pos = {this.transform.position}, curr rot = {this.transform.rotation}"); */
        this.gameObject.SetActive(false);
    }

    public void EnableNPCVehicles()
    {
        this.gameObject.SetActive(true);
    }
}
