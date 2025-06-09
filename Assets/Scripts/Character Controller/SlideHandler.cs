using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SlideHandler : MonoBehaviour
{
    [Header("Sliding Settings")]
    public float slideSpeed = 10f;
    public float slideDuration = 0.8f;
    public float slideHeight = 0.5f;
    private float originalHeight;
    private float yOffset;  // Diferencia de altura para bajar el transform

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;

    private bool isSliding = false;
    private float slideTimer;
    private CharacterController characterController;
    private Vector3 slideDirection;

    public bool IsSliding => isSliding;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;
    }

    void Update()
    {
        if (Input.GetKeyDown(slideKey) && characterController.isGrounded)
        {
            StartSlide();
        }

        if (isSliding)
        {
            SlideMovement();
            slideTimer -= Time.deltaTime;

            if (slideTimer <= 0 || Input.GetKeyUp(slideKey))
            {
                StopSlide();
            }
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        // Calcular cuánto bajar el transform (misma lógica que en el CrouchHandler)
        yOffset = (originalHeight - slideHeight) / 2;

        // Reducir la altura
        characterController.height = slideHeight;

        // Ajustar la posición del personaje hacia abajo
        transform.position -= new Vector3(0, yOffset, 0);

        // Capturar la dirección del movimiento
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        slideDirection = inputDirection.magnitude == 0 ? transform.forward : transform.TransformDirection(inputDirection.normalized);
    }

    private void SlideMovement()
    {
        characterController.Move(slideDirection * slideSpeed * Time.deltaTime);
    }

    private void StopSlide()
    {
        isSliding = false;

        // Restaurar la altura
        characterController.height = originalHeight;

        // Devolver la posición del personaje hacia arriba
        transform.position += new Vector3(0, yOffset, 0);
    }

    public Vector3 GetSlideMovement()
    {
        return isSliding ? slideDirection * slideSpeed : Vector3.zero;
    }
}
