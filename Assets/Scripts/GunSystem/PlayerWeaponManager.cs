using System.Collections.Generic;
using UnityEngine;
using TMPro; // Asegúrate de tener esto

public class PlayerWeaponManager : MonoBehaviour
{
    public List<GameObject> armasEquipadas = new List<GameObject>(2); // Máximo 2 armas
    [SerializeField] private int armaActivaIndex = 0; // Índice del arma activa

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI armaNombreTexto; // Referencia al TMP

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CambiarArma(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CambiarArma(1);
        }
    }

    public void RecogerArma(GameObject nuevaArma)
    {
        if (YaTieneArma(nuevaArma))
        {
            Debug.Log("Ya tienes esta arma.");
            return;
        }

        if (armasEquipadas.Count < 2)
        {
            armasEquipadas.Add(nuevaArma);
        }
        else
        {
            Debug.Log($"Reemplazando {armasEquipadas[armaActivaIndex].name} con {nuevaArma.name}");
            armasEquipadas[armaActivaIndex].SetActive(false);
            armasEquipadas[armaActivaIndex] = nuevaArma;
        }

        CambiarArma(armasEquipadas.IndexOf(nuevaArma));
        Debug.Log($"Arma equipada: {nuevaArma.name}");
        nuevaArma.SetActive(true);
    }

    public bool YaTieneArma(GameObject arma)
    {
        return armasEquipadas.Contains(arma);
    }

    private void CambiarArma(int index)
    {
        if (index < armasEquipadas.Count)
        {
            foreach (GameObject arma in armasEquipadas)
            {
                arma.SetActive(false);
            }

            armasEquipadas[index].SetActive(true);
            armaActivaIndex = index;
            ActualizarNombreArmaUI();
            Debug.Log($"Cambiado a: {armasEquipadas[index].name}");
        }
    }

    private void ActualizarNombreArmaUI()
    {
        if (armaNombreTexto != null && armaActivaIndex < armasEquipadas.Count)
        {
            armaNombreTexto.text = armasEquipadas[armaActivaIndex].name;
        }
    }
}
