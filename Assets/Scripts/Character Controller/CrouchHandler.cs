using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CrouchHandler : MonoBehaviour
{
    public float crouchSpeed = 3.5f;
    public float crouchHeight = 0.5f;
    private float originalHeight;
    private bool isCrouching = false;

    private CharacterController characterController;
    private SlideHandler slideHandler;

    public bool IsCrouching => isCrouching;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        slideHandler = GetComponent<SlideHandler>();  // Referencia al SlideHandler
        originalHeight = characterController.height;
    }

    void Update()
    {
        // Si el jugador está deslizándose, NO cambiar la altura
        if (slideHandler.IsSliding) return;

        bool crouchKey = Input.GetKey(KeyCode.C);

        if (crouchKey)
        {
            isCrouching = true;
        }
        else if (isCrouching && !Physics.Raycast(transform.position, Vector3.up, originalHeight - crouchHeight))
        {
            isCrouching = false;
        }

        characterController.height = isCrouching ? crouchHeight : originalHeight;
    }

    public float GetCrouchSpeed(float normalSpeed)
    {
        return isCrouching ? crouchSpeed : normalSpeed;
    }
}
