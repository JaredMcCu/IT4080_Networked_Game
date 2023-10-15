using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpChangeColor : BasePowerUp
{
    protected override bool ApplyToPlayer(Player thePickerUpper)
    {
        thePickerUpper.playerColorNetVar.Value = Color.black;
        return true;
    }
}
