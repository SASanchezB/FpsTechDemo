using UnityEngine;

public class MachineDamageAndRate : BaseMachine
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
                gs.damage *= 2; // Aplica la mejora
                gs.timeBetweenShooting /= 2; // Aplica la mejora
                gs.timeBetweenShots /= 2; // Aplica la mejora
                weaponsUpgraded++;
            }
        }

        Debug.Log($"Mejora 2 adquirida en {weaponsUpgraded} armas");

        return true;
    }
}
