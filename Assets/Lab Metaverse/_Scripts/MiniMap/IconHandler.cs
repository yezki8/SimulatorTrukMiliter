using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconHandler : MonoBehaviour
{
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private RectTransform miniMapRectTransform;
    [SerializeField] private Camera miniMapCamera;

    private List<(IconPosition iconPosition, RectTransform iconRectTransform)> icons;

    void Awake()
    {
        icons = new List<(IconPosition iconPosition, RectTransform iconRectTransform)>();
    }

    void Update()
    {
        for (int i = 0; i < icons.Count; i++)
        {
            (IconPosition iconPosition, RectTransform iconRectTransform) = icons[i];
            Vector3 offset = Vector3.ClampMagnitude(iconPosition.transform.position - playerObject.transform.position, miniMapCamera.orthographicSize);
            
            offset = offset / miniMapCamera.orthographicSize * (miniMapRectTransform.rect.width / 2f);

            iconRectTransform.anchoredPosition = new Vector2(offset.x, offset.z);
        }
    }

    public void AddIcons(IconPosition sender)
    {
        RectTransform rectTransform = Instantiate(iconPrefab, miniMapRectTransform).GetComponent<RectTransform>();
        icons.Add((sender, rectTransform));
    }

    public void RemoveIcons(IconPosition sender)
    {
        if (!icons.Exists(icon => icon.iconPosition == sender))
        {
            return;
        }
        (IconPosition pos, RectTransform rectTransform) foundObject = icons.Find(icon => icon.iconPosition == sender);
        Destroy(foundObject.rectTransform.gameObject);
        icons.Remove(foundObject);
    }
}
