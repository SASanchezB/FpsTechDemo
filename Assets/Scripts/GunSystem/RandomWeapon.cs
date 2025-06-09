using UnityEngine;
using System.Collections.Generic;
using TMPro; // Para TextMeshPro

public class RandomWeapon : MonoBehaviour
{
    public WeaponSceneManager weaponManager;
    public LayerMask jugadorLayer;
    public List<int> indicesProhibidos;

    [Header("Configuración de UI y Costo")]
    [SerializeField] private int costo = 100;
    [SerializeField] private TextMeshProUGUI textoDispenser;
    [SerializeField] private string fraseBase = "Random weapon: ";

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
            DarArmaAleatoria();
        }
    }

    private void DarArmaAleatoria()
    {
        if (playerWeaponManager == null || weaponManager == null)
        {
            Debug.LogWarning("Faltan referencias.");
            return;
        }

        if (PointsManager.Instance.GetPoints() < costo)
        {
            Debug.Log("No tienes puntos suficientes para una arma aleatoria.");
            return;
        }

        List<GameObject> armasValidas = new List<GameObject>();

        for (int i = 0; i < weaponManager.armasEnEscena.Count; i++)
        {
            if (indicesProhibidos.Contains(i)) continue;

            GameObject arma = weaponManager.armasEnEscena[i];

            if (!playerWeaponManager.YaTieneArma(arma))
            {
                armasValidas.Add(arma);
            }
        }

        if (armasValidas.Count == 0)
        {
            Debug.Log("No hay armas válidas para entregar.");
            return;
        }

        GameObject armaElegida = armasValidas[Random.Range(0, armasValidas.Count)];

        GunSystem gunSystem = armaElegida.GetComponent<GunSystem>();
        if (gunSystem != null)
        {
            gunSystem.ForzarCargadorLleno();
        }

        playerWeaponManager.RecogerArma(armaElegida);

        // Restar puntos
        PointsManager.Instance.RemovePoints(costo);

        Debug.Log($"Jugador recibió arma aleatoria: {armaElegida.name}");
    }
}
