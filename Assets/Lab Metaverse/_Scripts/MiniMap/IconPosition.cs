using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<IconHandler>().AddIcons(this);
    }
}
