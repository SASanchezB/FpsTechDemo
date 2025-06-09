using System.Collections;
using UnityEngine;
using static PlayerMovementAdvanced;

public class Sliding : MonoBehaviour, IPausable
{
    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;
    private PlayerMovementAdvanced pm;

    [Header("Sliding")]
    public float maxSlideTime = 1.5f;
    public float slideForce = 10f;
    private float slideTimer;

    [Header("Crouch Settings")]
    public float crouchYScale = 0.5f;
    private float startYScale;
    private float crouchLerpSpeed = 10f;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovementAdvanced>();
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

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0) && pm.state != MovementState.crouching)
            StartSlide();

        if (Input.GetKeyUp(slideKey))
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        pm.sliding = true;
        pm.state = PlayerMovementAdvanced.MovementState.sliding;

        // Mantener la escala reducida durante el deslizamiento
        StartCoroutine(LerpToCrouchScale(crouchYScale));

        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (!pm.OnSlope() || rb.linearVelocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide()
    {
        pm.sliding = false;

        // Verifica si hay un techo bloqueando
        if (pm.IsCeilingBlocked())
        {
            // Si hay un techo bloqueando, pasa al estado de agachado
            pm.state = PlayerMovementAdvanced.MovementState.crouching;

            // Mantén la escala reducida si es necesario
            StartCoroutine(LerpToCrouchScale(crouchYScale));
        }
        else
        {
            // Solo restaurar la escala y el estado si no hay techo bloqueando
            if (!pm.IsCeilingBlocked())
                StartCoroutine(LerpToCrouchScale(startYScale));
        }
    }


    private IEnumerator LerpToCrouchScale(float targetScaleY)
    {
        float time = 0f;
        float startScaleY = transform.localScale.y;

        while (time < 1f)
        {
            time += Time.deltaTime * crouchLerpSpeed;
            float newScaleY = Mathf.Lerp(startScaleY, targetScaleY, time);
            transform.localScale = new Vector3(transform.localScale.x, newScaleY, transform.localScale.z);
            yield return null;
        }

        transform.localScale = new Vector3(transform.localScale.x, targetScaleY, transform.localScale.z);
    }
}
