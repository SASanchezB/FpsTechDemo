using UnityEngine;

public class MachineSpeed : BaseMachine
{

    public PlayerMovementAdvanced pm;

   public override bool AplicarMejora()
    {
        pm.walkSpeed = pm.walkSpeed * 1.25f;
        pm.sprintSpeed = pm.sprintSpeed * 1.5f;
        //pm.slideSpeed = pm.slideSpeed * 2;
        pm.crouchSpeed = pm.crouchSpeed * 1.25f;
        Debug.Log("Mejora 1 adquirida");

        return true;
    }
}
