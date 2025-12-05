using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float gravity = -9.81f;

    [Header("Look Settings")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;
    public float maxLookAngle = 80f;


    private CharacterController characterController;
    private float verticalVelocity;
    private float cameraVerticalRotation = 0f;

    void Start()
    {
        // Get the CharacterController component
        characterController = GetComponent<CharacterController>();

        // Lock and hide the cursor for FPS controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // If camera wasn't assigned, try to find it
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>().transform;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();

        // Press ESC to unlock cursor (for testing)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleMovement()
    {
        // Get input from WASD or Arrow Keys
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

        // Calculate movement direction relative to where player is facing
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Check if sprinting (Left Shift)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        // Apply movement
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Apply gravity
        if (characterController.isGrounded)
        {
            verticalVelocity = -2f; // Small downward force to keep grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Apply vertical velocity (gravity/falling)
        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate player body left/right (horizontal rotation)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera up/down (vertical rotation)
        cameraVerticalRotation -= mouseY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -maxLookAngle, maxLookAngle);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);
        }
    }
}