using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BigMapCamera : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float dragSpeed = 1f;
    [SerializeField] private float edgePanBorderThickness = 1f;
    [SerializeField] private float edgePanSpeed = 1f;
    [SerializeField] private float zoomSpeed = 50f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 150f;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject mapsSwitcher;
    [SerializeField] private Button edgePanSwitch;
    [SerializeField] private Button followPlayerSwitch;
    [SerializeField] private GameObject informationText;
    private Vector3 dragOrigin;
    private bool isDragging = false;
    private bool isPanning = false;
    private bool isFollowing;
    private SimulatorInputActions _controls;


    void Awake()
    {
        _controls = new SimulatorInputActions();

        _controls.Maps.Click.started += ctx => StartDrag();
        _controls.Maps.Click.canceled += ctx => EndDrag();
        _controls.Maps.Zoom.performed += ctx => Zoom(ctx.ReadValue<float>());
        _controls.Maps.GoToPlayer.performed += ctx => FollowPlayer();
    }
    void Start()
    {
        GetComponent<Camera>().enabled = false;
        SwitchFollowingTrue();
        UpdateEdgePanSwitchColor();
        UpdateFollowingSwitchColor();
    }

    void OnEnable()
    {
        _controls.Maps.Enable();
    }

    void OnDisable()
    {
        _controls.Maps.Disable();
    }

    void Update()
    {

        if (GetComponent<Camera>().enabled)
        {
            if (isFollowing)
            {
                FollowPlayer();
            }
            mapsSwitcher.SetActive(true);
            informationText.SetActive(true);
            MoveCamera();                                                                                                                        
        }
        else
        {
            mapsSwitcher.SetActive(false);
            informationText.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (!GetComponent<Camera>().enabled)
        {
            FollowPlayer();
        }
    }

    void MoveCamera()
    {
        Vector2 moveInput = _controls.Maps.Move.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * movementSpeed;
        transform.position += move;
        if (moveInput != new Vector2(0f, 0f))
        {
            SwitchFollowingFalse();
        }

        if (isDragging)
        {
            Vector2 mousePosition = _controls.Maps.Drag.ReadValue<Vector2>();
            Vector3 mouseWorldPos = GetComponent<Camera>().ScreenToWorldPoint(mousePosition);
            Vector3 originWorldPos = GetComponent<Camera>().ScreenToWorldPoint(dragOrigin);
            Vector3 moveDrag = new Vector3(mouseWorldPos.x - originWorldPos.x, 0, mouseWorldPos.z - originWorldPos.z);
            transform.position -= dragSpeed * moveDrag;

            dragOrigin = mousePosition;
            SwitchFollowingFalse();
        }

        if (isPanning)
        {
            EdgePanMove();
            SwitchFollowingFalse();
        }
    }

    void StartDrag()
    {
        isDragging = true;
        dragOrigin = _controls.Maps.Drag.ReadValue<Vector2>();
    }

    void EndDrag()
    {
        isDragging = false;
    }

    void EdgePanMove()
    {
        Vector3 screenMousePosition = _controls.Maps.EdgePan.ReadValue<Vector2>();
        if (screenMousePosition.x <= edgePanBorderThickness)
        {
            transform.position -= Vector3.right * edgePanSpeed;
        }
        else if (screenMousePosition.x >= Screen.width - edgePanBorderThickness)
        {
            transform.position += Vector3.right * edgePanSpeed;
        }

        if (screenMousePosition.y <= edgePanBorderThickness)
        {
            transform.position -= Vector3.forward * edgePanSpeed;
        }
        else if (screenMousePosition.y >= Screen.height - edgePanBorderThickness)
        {
            transform.position += Vector3.forward * edgePanSpeed;
        }
    }

    void Zoom(float scrollInput)
    {
        if (scrollInput != 0)
        {
            GetComponent<Camera>().orthographicSize -= scrollInput * zoomSpeed;
            GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize, minZoom, maxZoom);
        }
    }

    private void FollowPlayer()
    {
        Vector3 followPlayer = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.position = followPlayer;
    }

    public void SwitchFollowingFalse()
    {
        isFollowing = false;
        UpdateFollowingSwitchColor();
        followPlayerSwitch.gameObject.SetActive(true);
    }

    public void SwitchFollowingTrue()
    {
        isFollowing = true;
        followPlayerSwitch.gameObject.SetActive(false);
        
    }

    public void SwitchPanning()
    {
        isPanning = !isPanning;
        UpdateEdgePanSwitchColor();
    }

    void UpdateEdgePanSwitchColor()
    {
        ColorBlock colorBlock = edgePanSwitch.colors;

        colorBlock.normalColor = isPanning ? Color.green : Color.red;

        colorBlock.highlightedColor = colorBlock.normalColor * 1.1f;
        colorBlock.pressedColor = colorBlock.normalColor * 0.9f;
        colorBlock.selectedColor = colorBlock.normalColor * 1.05f;
        colorBlock.disabledColor = colorBlock.normalColor * 0.5f;

        edgePanSwitch.colors = colorBlock;
    }

    void UpdateFollowingSwitchColor()
    {
        ColorBlock colorBlock = followPlayerSwitch.colors;
        colorBlock.normalColor = isFollowing ? Color.cyan : Color.white;

        colorBlock.highlightedColor = colorBlock.normalColor * 1.1f;
        colorBlock.pressedColor = colorBlock.normalColor * 0.9f;
        colorBlock.selectedColor = colorBlock.normalColor * 1.05f;
        colorBlock.disabledColor = colorBlock.normalColor * 0.5f;

        followPlayerSwitch.colors = colorBlock;
    }
}
