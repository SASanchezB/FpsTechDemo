using UnityEngine;
using TMPro;

public class ThrowingMechanic : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    [Header("Settings")]
    public int totalThrows;
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.G;
    public float throwForce;
    public float throwUpwardForce;

    private bool readyToThrow;

    [Header("UI")]
    public TextMeshProUGUI throwableText;

    private void Start()
    {
        readyToThrow = true;
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
        {
            Throw();
        }
    }

    private void Throw()
    {
        readyToThrow = false;

        // Obtener el objeto desde el pool
        GameObject projectile = ObjectPool.Instance.GetPooledObject(objectToThrow, attackPoint.position, cam.rotation);

        // Reiniciar velocidad del Rigidbody
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            projectileRb.linearVelocity = Vector3.zero;
            projectileRb.angularVelocity = Vector3.zero;

            // Calcular direcci�n
            Vector3 forceDirection = cam.transform.forward;

            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 500f))
            {
                forceDirection = (hit.point - attackPoint.position).normalized;
            }

            // Aplicar fuerza
            Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;
            projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        }

        totalThrows--;

        Invoke(nameof(ResetThrow), throwCooldown);
        UpdateUI();
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    public void UpdateUI()
    {
        if (throwableText != null)
        {
            throwableText.text = "Throws: " + totalThrows;
        }
    }
}
