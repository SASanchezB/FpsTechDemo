using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FpsController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    public Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    public float currentSpeed;

    public bool canMove = true;

    CharacterController characterController;
    private CrouchHandler crouchHandler;
    private SlideHandler slideHandler;

    [SerializeField] private MovementState state = MovementState.walking;

    private enum MovementState
    {
        walking,
        sprinting,
        air,
        frozen
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        crouchHandler = GetComponent<CrouchHandler>();
        slideHandler = GetComponent<SlideHandler>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        if (state != MovementState.frozen)
        {
            HandleInput();
            HandleMovement();
            HandleRotation();
        }
        HandleState();
    }

    private void HandleInput()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = crouchHandler.GetCrouchSpeed(isRunning ? runSpeed : walkSpeed);

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        Vector3 desiredMove = (forward * inputY) + (right * inputX);
        if (desiredMove.magnitude > 1)
            desiredMove.Normalize();

        float movementDirectionY = moveDirection.y;
        moveDirection = desiredMove * currentSpeed;
        moveDirection.y = movementDirectionY;

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded && !crouchHandler.IsCrouching)
        {
            moveDirection.y = jumpPower;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

    }

    private void HandleMovement()
    {
        // Si el jugador está deslizándose, aplicamos el movimiento del slide
        if (slideHandler.IsSliding)
        {
            moveDirection = slideHandler.GetSlideMovement();
        }
        else
        {
            // Movimiento normal en tierra
            if (state == MovementState.walking || state == MovementState.sprinting)
            {
                characterController.Move(moveDirection * Time.deltaTime);
            }

            // Movimiento en el aire (aplicando gravedad)
            if (state == MovementState.air)
            {
                moveDirection.y -= gravity * Time.deltaTime;
                characterController.Move(moveDirection * Time.deltaTime);
            }
        }
    }

    private void HandleRotation()
    {
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void HandleState()
    {
        if (characterController.isGrounded)
        {
            state = Input.GetKey(KeyCode.LeftShift) ? MovementState.sprinting : MovementState.walking;
        }
        else
        {
            state = MovementState.air;
        }

        switch (state)
        {
            case MovementState.walking:
                currentSpeed = walkSpeed;
                break;
            case MovementState.sprinting:
                currentSpeed = runSpeed;
                break;
            case MovementState.air:
                currentSpeed = walkSpeed;
                break;
            case MovementState.frozen:
                currentSpeed = 0;
                moveDirection = Vector3.zero;
                break;
        }
    }
}
