using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigMapCamera : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float dragSpeed = 1f;
    
    [SerializeField] private float edgePanBorderThickness = 10f;
    [SerializeField] private float edgePanSpeed = 1f;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject edgePanSwitch;
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
        gameObject.GetComponent<Camera>().enabled = false;
    }

    void Update()
    {
        if (gameObject.GetComponent<Camera>().enabled)
        {
            UpdateSwitchColor();
            edgePanSwitch.SetActive(true);
            MoveCamera();                                                                                                                          
        }
        else
        {
            edgePanSwitch.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (!gameObject.GetComponent<Camera>().enabled)
        {
            Vector3 followPlayer = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.position = followPlayer;
        }
    }

    void MoveCamera()
    {
        KeyboardMove();
        ClickAndDragMove();
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
        if (Input.GetKey(KeyCode.W))
        {
            MoveUp();
        }

        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }

        if (Input.GetKey(KeyCode.S))
        {
            MoveDown();
        }

        if (Input.GetKey(KeyCode.D))
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
            Vector3 mousePosition = gameObject.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            Vector3 origin = gameObject.GetComponent<Camera>().ScreenToWorldPoint(dragOrigin);

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
}
