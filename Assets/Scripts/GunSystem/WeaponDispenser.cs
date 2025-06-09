using UnityEngine;
using TMPro;

public class WeaponDispenser : MonoBehaviour
{
    public WeaponSceneManager weaponManager;
    public int indiceArma;
    public LayerMask jugadorLayer;

    [Header("Configuración de UI y Costo")]
    [SerializeField] private int costo = 200;
    [SerializeField] private TextMeshProUGUI textoDispenser;
    [SerializeField] private string fraseBase = "Buy weapon: ";

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
            DarArma();
        }
    }

    private void DarArma()
    {
        if (playerWeaponManager == null)
        {
            Debug.LogWarning("No se encontró el PlayerWeaponManager en el jugador.");
            return;
        }

        if (weaponManager == null)
        {
            Debug.LogWarning("WeaponDatabase no asignado.");
            return;
        }

        if (PointsManager.Instance.GetPoints() < costo)
        {
            Debug.Log("No tienes puntos suficientes para esta arma.");
            return;
        }

        if (indiceArma >= 0 && indiceArma < weaponManager.armasEnEscena.Count)
        {
            GameObject arma = weaponManager.armasEnEscena[indiceArma];

            if (playerWeaponManager.YaTieneArma(arma))
            {
                Debug.Log("Ya tienes esta arma.");
                return;
            }

            GunSystem gunSystem = arma.GetComponent<GunSystem>();
            if (gunSystem != null)
            {
                gunSystem.ForzarCargadorLleno();
            }

            playerWeaponManager.RecogerArma(arma);
            PointsManager.Instance.RemovePoints(costo); // ✅ Descuenta puntos
            Debug.Log($"Jugador recogió: {arma.name}");
        }
        else
        {
            Debug.LogWarning("Índice de arma fuera de rango.");
        }
    }
}
