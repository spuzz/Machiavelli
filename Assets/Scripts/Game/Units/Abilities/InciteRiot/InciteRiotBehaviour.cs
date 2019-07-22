
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InciteRiotBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        if (target.City)
        {
            InciteRiotEffect inciteRiotEffect = new InciteRiotEffect();
            target.City.AddEffect(inciteRiotEffect, 5);
        }
    }
    public override void ShowAbility(int energyCost, HexCell target = null)
    {
        abilityText = "Incite Riots(5) - 5 Turns";
        base.ShowAbility(energyCost, target);
    }

    public override bool IsValidTarget(HexCell target)
    {
        if(target.City)
        {
            return true;
        }

        return false;
    }

    public override void FinishAbility(HexCell target = null)
    {

    }
}


