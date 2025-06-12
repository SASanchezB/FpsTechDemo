using UnityEngine;

public class MachineReload : BaseMachine
{
    public override bool AplicarMejora()
    {
        // Busca TODAS las armas en la escena (incluso desactivadas)
        GameObject[] allWeapons = Resources.FindObjectsOfTypeAll<GameObject>();

        int weaponsUpgraded = 0;

        foreach (GameObject weapon in allWeapons)
        {
            GunSystem gs = weapon.GetComponent<GunSystem>();

            if (gs != null)
            {
                gs.reloadTime /= 2; // Aplica la mejora
                gs.hasReloadUpgrade = true;
                weaponsUpgraded++;
            }
        }

        Debug.Log($"Mejora 2 adquirida en {weaponsUpgraded} armas");

        return true;
    }
}
