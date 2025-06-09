using UnityEngine;

public class MachineHealth : BaseMachine
{

    public HealthControllerPlayer Hp;

    public override bool AplicarMejora()
    {
        Hp.maxHealth = Hp.maxHealth + (Hp.maxHealth / 2); 
        Debug.Log("Mejora 2 adquirida");

        return true;
    }
}
