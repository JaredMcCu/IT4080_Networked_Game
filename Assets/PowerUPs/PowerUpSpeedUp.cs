using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpeedUp : BasePowerUp
{

    protected override bool ApplyToPlayer(Player thePickerUpper)
    {
        thePickerUpper.movementSpeed = 100f;
        return true;
    }  
}
