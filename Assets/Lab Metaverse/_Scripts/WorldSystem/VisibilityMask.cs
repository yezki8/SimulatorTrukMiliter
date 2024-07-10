using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityMask : MonoBehaviour
{
    [SerializeField]
    private GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        // set mask to follow the player
        transform.position = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // set mask to follow the player
        transform.position = player.transform.position;
    }
}
