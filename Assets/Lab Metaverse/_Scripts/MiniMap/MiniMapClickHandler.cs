using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiniMapClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera bigmapCamera;
    [SerializeField] private RenderTexture mainCameraRenderTexture;
    [SerializeField] private GameObject miniMapTexture;
    [SerializeField] private GameObject mainMapTexture;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"texture: {mainCamera.targetTexture}");
        if (mainCamera.targetTexture != null)
        {
            mainCamera.targetTexture = null;
            miniMapTexture.SetActive(true);
            mainMapTexture.SetActive(false);
        }
        else {
            mainCamera.targetTexture = mainCameraRenderTexture;
            miniMapTexture.SetActive(false);
            mainMapTexture.SetActive(true);
        }
        Debug.Log($"texture: {mainCamera.targetTexture}");
        bigmapCamera.enabled = !bigmapCamera.enabled;
    }
}
