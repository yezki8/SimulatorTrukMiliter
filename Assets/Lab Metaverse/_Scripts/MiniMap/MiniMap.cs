using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 15, 0);

    void Start()
    {
        transform.position = player.position + cameraOffset;
    }

    void LateUpdate()
    {
        transform.position = player.position + cameraOffset;
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
