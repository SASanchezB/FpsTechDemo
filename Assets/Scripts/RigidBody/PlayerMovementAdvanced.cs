using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAdvanced : MonoBehaviour, IPausable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    public float ceilingCheckDistance = 0.5f;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        sliding,
        air
    }

    public bool sliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    //PAUSE
    private bool isPaused = false;

    public void OnPause()
    {
        isPaused = true;
    }

    public void OnResume()
    {
        isPaused = false;
    }
    //UNPAUSE

    private void Update()
    {
        if (isPaused) return;

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        IsCeilingBlocked();

        // handle drag
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private float crouchLerpSpeed = 10f;

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // cuando saltas
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // empezar a agacharse
        if (Input.GetKeyDown(crouchKey))
        {
            StartCoroutine(LerpToCrouchScale(crouchYScale)); 
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // salir de agachado
        if (Input.GetKeyUp(crouchKey) && !IsCeilingBlocked())
        {
            // Verifica si está muy cerca del suelo antes de permitir que se levante
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, Vector3.up, out hit, ceilingCheckDistance, whatIsGround))
            {
                // Si no hay techo bloqueando, levanta al personaje un poco para evitar hundirse
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z); // Desplazamiento hacia arriba
            }
        }
    }

    private IEnumerator LerpToCrouchScale(float targetScaleY)
    {
        float time = 0f;
        float startScaleY = transform.localScale.y;

        while (time < 1f)
        {
            time += Time.deltaTime * crouchLerpSpeed; // Incrementa el tiempo
            float newScaleY = Mathf.Lerp(startScaleY, targetScaleY, time); // Lerp entre el valor inicial y el objetivo

            transform.localScale = new Vector3(transform.localScale.x, newScaleY, transform.localScale.z); // Aplica la nueva escala
            yield return null; // Espera al siguiente frame
        }

        // Asegúrate de que la escala final esté exactamente en el objetivo para evitar posibles errores de precisión
        transform.localScale = new Vector3(transform.localScale.x, targetScaleY, transform.localScale.z);
    }

    public bool IsCeilingBlocked()
    {
        if( Physics.Raycast(transform.position, Vector3.up, ceilingCheckDistance, whatIsGround))
        {
            return true;
        }
        else
        {
            if(state != MovementState.crouching)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x, startYScale, transform.localScale.z), Time.deltaTime * 10f);
            }
            return false;
        }
    }


    private void StateHandler()
    {
        // Mode - Sliding
        if (sliding && state != MovementState.crouching)
        {
            state = MovementState.sliding;

            // Asegúrate de que la velocidad se ajusta a slideSpeed durante el deslizamiento
            desiredMoveSpeed = slideSpeed;

            // Mantén la escala reducida durante el deslizamiento
            if (transform.localScale.y != crouchYScale)
            {
                StartCoroutine(LerpToCrouchScale(crouchYScale));  // Aplica la reducción de escala al inicio del deslizamiento
            }

            if (OnSlope() && rb.linearVelocity.y < 0.1f)
            {
                // Si estamos en una pendiente, mantenemos la velocidad de deslizamiento
                rb.AddForce(GetSlopeMoveDirection(moveDirection) * slideSpeed * 20f, ForceMode.Force);

                // Si estamos en una pendiente, ajustar la gravedad
                if (rb.linearVelocity.y > 0)
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            else
            {
                // Si no estamos en una pendiente, simplemente usamos el slideSpeed
                rb.AddForce(moveDirection.normalized * slideSpeed * 10f, ForceMode.Force);
            }
        }

        // Mode - Crouching
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;

            if (!sliding && transform.localScale.y != crouchYScale)
            {
                StartCoroutine(LerpToCrouchScale(crouchYScale));
            }
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey) && !IsCeilingBlocked())
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;

            if (!sliding && transform.localScale.y != startYScale)
            {
                StartCoroutine(LerpToCrouchScale(startYScale));
            }
        }

        // Mode - Walking
        else if (grounded && !IsCeilingBlocked())
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;

            if (!sliding && transform.localScale.y != startYScale)
            {
                StartCoroutine(LerpToCrouchScale(startYScale));
            }
        }

        // Mode - Air
        else if (!IsCeilingBlocked())
        {
            state = MovementState.air;
        }

        // Actualiza moveSpeed si ha cambiado de desiredMoveSpeed
        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) > 0.1f)
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }




    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}