using UnityEngine;

public class GhostAnimRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f; // grados por segundo, configurable en el editor

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
