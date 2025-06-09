using UnityEngine;

public class WeaponPosition : MonoBehaviour
{

    public Transform targetPosition;
    public Transform targetRotation;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    private void Update()
    {
        if (targetPosition != null)
        {
            transform.position = targetPosition.position + positionOffset;
        }

        if (targetRotation != null)
        {
            transform.rotation = targetRotation.rotation * Quaternion.Euler(rotationOffset);
        }
    }
}
