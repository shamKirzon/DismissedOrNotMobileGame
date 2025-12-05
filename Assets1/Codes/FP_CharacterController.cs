using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))] //automatically adds the character controller component
public class FP_CharacterController : MonoBehaviour
{
    [Header("Base Values")]
    public float walkingSpeed; //this will be the basic walk speed of the character
    public float runningSPeed; //this will serve as the character's maximum movement speed aka run
    public float jumpForce; //this will be the character's jumping power
    public float gravity; //this will serve as the world gravity

    [Header("Camera Reference")]
    public Camera playerCamera; //this will be referenced to the main camera/ the camera that will serve as the player's vision

    [Header("Camera Rotation")]
    public float lookSpeed = 2.0f; //sets the speed sensitivity
    public float lookXLimit = 45.0f; //the angle of the look up and down

    [Header("Controller Properties")]
    CharacterController characterController; //reference to the character controller component
    Vector3 moveDirection = Vector3.zero; //identifies the direction for movement
    float rotationX = 0; //this is the base rotation of the character

    public bool canMove; //this identifies if the character can move depending on the situation

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>(); //this automatically gets the character controller component

        //this will lock and hide the cursor from the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //this is for showing the cursor------------------------------------
        if (Input.GetKey(KeyCode.Z))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        //end cursor conditions---------------------------------------------

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        //press left shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        //conditions for movement
        float curSpeedX = canMove ? (isRunning ? runningSPeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSPeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY); //identifies what direction the character is going to

        //for jumping
        if (Input.GetButton("Jump") && characterController.isGrounded && canMove)
        {
            moveDirection.y = jumpForce;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        //move the controller (acceleration)
        characterController.Move(moveDirection * Time.deltaTime);

        //for the player and camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed; //this is the mouse y look speed
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); //sets the look x limit
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}
