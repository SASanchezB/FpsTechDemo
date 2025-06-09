using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class KnifeDispenser : MonoBehaviour
{
    public LayerMask jugadorLayer;

    [Header("Configuración de UI y Costo")]
    [SerializeField] private int costo = 200;
    [SerializeField] private TextMeshProUGUI textoDispenser;
    [SerializeField] private string fraseBase = "Buy weapon: ";
    [SerializeField] private ThrowingMechanic playerKnifes;

    [SerializeField] private bool jugadorDentro = false;

    [SerializeField] private int maxKnifes;

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
            playerKnifes = other.GetComponentInParent<ThrowingMechanic>();

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
            playerKnifes = null;

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

        /*if (playerKnifes.totalThrows >= 7)
        {
            textoDispenser.text = "Max Knifes slot";
            return;
        }*/

        playerKnifes.totalThrows = maxKnifes;
        PointsManager.Instance.RemovePoints(costo); // ✅ Descuenta puntos

        playerKnifes.throwCooldown = cooldown;
        playerKnifes.throwForce = force;
        playerKnifes.throwUpwardForce = upForce;
        playerKnifes.objectToThrow = Throwable;

        playerKnifes.UpdateUI();
    }
}
