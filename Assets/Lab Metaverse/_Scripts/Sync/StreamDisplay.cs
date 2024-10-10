using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Helper component for displaying received camera feed
public class StreamDisplay : MonoBehaviour
{
    public Material displayMaterial;
    private bool isSecondaryPC;

    void Start()
    {
        #if SECONDARY_PC
        isSecondaryPC = true;
        #endif

        if (!isSecondaryPC)
        {
            enabled = false;
            return;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (displayMaterial != null)
        {
            Graphics.Blit(displayMaterial.mainTexture, destination);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}