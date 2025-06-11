using UnityEngine;

public class GhostLevitate : MonoBehaviour
{
    [SerializeField] private float levitationOffset = 1f;   // Cu�nto sube y baja desde el centro
    [SerializeField] private float levitationSpeed = 1f;    // Velocidad de la oscilaci�n

    private float centerY;

    void Start()
    {
        // Tomamos la posici�n actual como el centro de oscilaci�n
        centerY = transform.position.y;
    }

    void Update()
    {
        // Movimiento suave arriba y abajo usando seno
        float offset = Mathf.Sin(Time.time * levitationSpeed) * levitationOffset;
        transform.position = new Vector3(transform.position.x, centerY + offset, transform.position.z);
    }
}
