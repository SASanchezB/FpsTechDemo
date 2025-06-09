using TMPro;
using UnityEngine;

public class GrenadeDispenser : MonoBehaviour
{
    public LayerMask jugadorLayer;

    [Header("Configuración de UI y Costo")]
    [SerializeField] private int costo = 200;
    [SerializeField] private TextMeshProUGUI textoDispenser;
    [SerializeField] private string fraseBase = "Buy weapon: ";
    [SerializeField] private ThrowingMechanic playerGrenades;

    [SerializeField] private bool jugadorDentro = false;

    [SerializeField] private int maxGrenades;

    [SerializeField] GameObject Throwable;

    [Header("Variables de la granada")]
    [SerializeField] private float cooldown;
    [SerializeField] private int force;
    [SerializeField] private int upForce;


    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & jugadorLayer) != 0)
        {
            jugadorDentro = true;
            playerGrenades = other.GetComponentInParent<ThrowingMechanic>();

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
            playerGrenades = null;

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
            FillGrenades();
        }
    }

    private void FillGrenades()
    {
        if (PointsManager.Instance.GetPoints() < costo)
        {
            Debug.Log("Not enough point to buy knifes");
            return;
        }

        /*if (playerGrenades.totalThrows >= 4)
        {
            textoDispenser.text = "Max grenade slot";
            return;
        }*/

        playerGrenades.totalThrows = maxGrenades;
        PointsManager.Instance.RemovePoints(costo); // ✅ Descuenta puntos

        playerGrenades.throwCooldown = cooldown;
        playerGrenades.throwForce = force;
        playerGrenades.throwUpwardForce = upForce;
        playerGrenades.objectToThrow = Throwable;

        playerGrenades.UpdateUI();
    }
}
