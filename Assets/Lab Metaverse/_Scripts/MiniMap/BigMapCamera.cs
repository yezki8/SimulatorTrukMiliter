using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject edgePanSwitch;
    [SerializeField] private GameObject informationText;
    private Button _button;
    private Vector3 dragOrigin;
    private bool isDragging = false;
    private bool isPanning = false;
    private Vector3 offsetUp = new Vector3(0, 0, 1);
    private Vector3 offsetLeft = new Vector3(-1, 0, 0);
    private Vector3 offsetDown = new Vector3(0, 0, -1);
    private Vector3 offsetRight = new Vector3(1, 0, 0);

    void Awake()
    {
        _button = edgePanSwitch.GetComponent<Button>();
    }
    void Start()
    {
        GetComponent<Camera>().enabled = false;
    }

    void Update()
    {
        if (GetComponent<Camera>().enabled)
        {
            UpdateSwitchColor();
            edgePanSwitch.SetActive(true);
            informationText.SetActive(true);
            MoveCamera();                                                                                                                          
        }
        else
        {
            edgePanSwitch.SetActive(false);
            informationText.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (!GetComponent<Camera>().enabled)
        {
            Vector3 followPlayer = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.position = followPlayer;
        }
    }

    void MoveCamera()
    {
        KeyboardMove();
        ClickAndDragMove();
        Zoom();
        if (isPanning)
        {
            EdgePanMove();
        }
        if (Input.GetKey(KeyCode.F1))
        {
            Vector3 followPlayer = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.position = followPlayer;
        }
    }

    void KeyboardMove()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            MoveUp();
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            MoveDown();
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }
    }

    void MoveUp()
    {
        transform.position += offsetUp * movementSpeed;
    }

    void MoveLeft()
    {
        transform.position += offsetLeft * movementSpeed;
    }

    void MoveDown()
    {
        transform.position += offsetDown * movementSpeed;
    }

    void MoveRight()
    {
        transform.position += offsetRight * movementSpeed;
    }

    void ClickAndDragMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            return;
        }

        if (isDragging)
        {
            Vector3 mousePosition = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            Vector3 origin = GetComponent<Camera>().ScreenToWorldPoint(dragOrigin);

            Vector3 move = new Vector3(mousePosition.x - origin.x, 0, mousePosition.z - origin.z);
            transform.position += (-1) * dragSpeed * move;

            dragOrigin = Input.mousePosition;
        } 
    }

    void EdgePanMove()
    {
        Vector3 screenMousePosition = Input.mousePosition;
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

    public void switchPanning()
    {
        isPanning = !isPanning;
    }

    void UpdateSwitchColor()
    {
        ColorBlock colorBlock = _button.colors;

        colorBlock.normalColor = isPanning ? Color.green : Color.red;

        colorBlock.highlightedColor = colorBlock.normalColor * 1.1f;
        colorBlock.pressedColor = colorBlock.normalColor * 0.9f;
        colorBlock.selectedColor = colorBlock.normalColor * 1.05f;
        colorBlock.disabledColor = colorBlock.normalColor * 0.5f;

        _button.colors = colorBlock;
    }

    void Zoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            // Adjust the orthographic size based on the scroll input
            GetComponent<Camera>().orthographicSize -= scrollInput * zoomSpeed;

            // Clamp the orthographic size to stay within min and max zoom levels
            GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize, minZoom, maxZoom);
        }
    }
}
