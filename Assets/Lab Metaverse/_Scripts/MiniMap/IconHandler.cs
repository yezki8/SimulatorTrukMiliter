using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconHandler : MonoBehaviour
{
    [SerializeField] private GameObject checkpointPrefab;
    [SerializeField] private GameObject finishPrefab;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private RectTransform miniMapRectTransform;
    [SerializeField] private Camera miniMapCamera;
    private List<(GameObject gameObject, RectTransform iconRectTransform)> icons;

    void Awake()
    {
        icons = new List<(GameObject gameObject, RectTransform iconRectTransform)>();
    }

    void Update()
    {
        for (int i = 0; i < icons.Count; i++)
        {
            (GameObject gameObject, RectTransform iconRectTransform) = icons[i];
            
            Vector3 offset = gameObject.transform.position - playerObject.transform.position;
            offset = Quaternion.Inverse(playerObject.transform.rotation) * offset;
            offset = Vector3.ClampMagnitude(offset, miniMapCamera.orthographicSize);
            offset = offset / miniMapCamera.orthographicSize * (miniMapRectTransform.rect.width / 2f);
            iconRectTransform.anchoredPosition = new Vector2(offset.x, offset.z);
        }
    }

    public void RemoveUnusedCheckpoint(){
        for (int i = 0; i < icons.Count; i++)
        {
            (GameObject gameObject, _) = icons[i];
            if (!gameObject.transform.parent.gameObject.activeSelf)
            {
                RemoveIcon(gameObject);
            }
        }
    }

    public void AddCheckpointIcon(GameObject gameObject)
    {
        if (!icons.Exists(icon => icon.gameObject == gameObject))
        {
            RectTransform rectTransform = Instantiate(checkpointPrefab, miniMapRectTransform).GetComponent<RectTransform>();
            icons.Add((gameObject, rectTransform));
        }
    }

    public void AddFinishIcon(GameObject gameObject)
    {
        if (!icons.Exists(icon => icon.gameObject == gameObject))
        {
            RectTransform rectTransform = Instantiate(finishPrefab, miniMapRectTransform).GetComponent<RectTransform>();
            icons.Add((gameObject, rectTransform));
        }
    }

    public void RemoveIcon(GameObject gameObject)
    {
        if (!icons.Exists(icon => icon.gameObject == gameObject))
        {
            return;
        }
        (GameObject gameObject, RectTransform rectTransform) foundObject = icons.Find(icon => icon.gameObject == gameObject);
        Destroy(foundObject.rectTransform.gameObject);
        icons.Remove(foundObject);
    }
}
