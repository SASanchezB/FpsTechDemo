using UnityEngine;
using TMPro;


public class Door : MonoBehaviour
{

    public LayerMask jugadorLayer;

    [Header("Configuración de UI y Costo")]
    [SerializeField] private int costo = 150;
    [SerializeField] private TextMeshProUGUI textoDispenser;
    [SerializeField] private string fraseBase = "Abrir Puerta: ";

    [SerializeField] private PlayerWeaponManager playerWeaponManager;
    [SerializeField] private bool jugadorDentro = false;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & jugadorLayer) != 0)
        {
            jugadorDentro = true;
            playerWeaponManager = other.GetComponentInParent<PlayerWeaponManager>();

            if (textoDispenser != null)
            {
                textoDispenser.text = $"{fraseBase}${costo}";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & jugadorLayer) != 0)
        {
            jugadorDentro = false;
            playerWeaponManager = null;

            if (textoDispenser != null)
            {
                textoDispenser.text = "";
            }
        }
    }

    private void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            AbrirPuerta();
        }
    }

    private void AbrirPuerta()
    {
        if (playerWeaponManager == null)
        {
            Debug.LogWarning("No se encontró el PlayerWeaponManager en el jugador.");
            return;
        }

        if (PointsManager.Instance.GetPoints() < costo)
        {
            Debug.Log("No tienes puntos suficientes para esta arma.");
            return;
        }

        if (PointsManager.Instance.GetPoints() >= costo)
        {
            Destroy(gameObject);
            return;
        }
    }

}
