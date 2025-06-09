using UnityEngine;
using System.Collections.Generic;

public class MachineRandomUpgrade : BaseMachine
{
    [Header("Referencias a otras máquinas")]
    public BaseMachine[] otrasMaquinas;

    public override bool AplicarMejora()
    {
        List<BaseMachine> disponibles = new List<BaseMachine>();

        foreach (BaseMachine maquina in otrasMaquinas)
        {
            if (!maquina.yaComprado)
            {
                disponibles.Add(maquina);
            }
        }

        if (disponibles.Count == 0)
        {
            Debug.Log("No hay mejoras disponibles para aplicar.");
            textoUI.text = "All perks sold";
            return false; // ⛔ Nada que aplicar
        }

        int index = Random.Range(0, disponibles.Count);
        BaseMachine seleccionada = disponibles[index];

        seleccionada.AplicarMejora();
        seleccionada.yaComprado = true;

        if (seleccionada.imagenCanvas != null)
        {
            if (!posicionInicialSet)
            {
                posicionInicial = seleccionada.imagenCanvas.transform.position;
                posicionInicialSet = true;
            }

            seleccionada.imagenCanvas.SetActive(true);
            seleccionada.imagenCanvas.transform.position = new Vector3(
                posicionInicial.x + 60f,
                posicionInicial.y,
                posicionInicial.z
            );

            posicionInicial = seleccionada.imagenCanvas.transform.position;
        }

        Debug.Log("Mejora aleatoria aplicada desde: " + seleccionada.name);
        return true; // ✅ Mejora aplicada con éxito
    }

}
