
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnvoyBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        if(target.City)
        {
            EnvoyEffect envoy = new EnvoyEffect
            {
                Player = gameObject.GetComponent<Unit>().GetPlayer()
            };
            target.City.AddEffect(envoy, 5);
        }

    }

    public override void ShowAbility(int energyCost, HexCell target = null)
    {
        abilityText = "Envoy(3) - 5 Turns";
        base.ShowAbility(energyCost, target);
    }
    public override void FinishAbility(HexCell target = null)
    {
    }
    public override bool IsValidTarget(HexCell target)
    {
        if (GetComponent<Agent>().Energy < config.GetEnergyCost())
        {
            return false;
        }
        if (target.City)
        {
            Player player = target.City.Player;
            if (!player)
            {
                return true;
            }
        }

        return false;
    }

    public override bool IsGoodTarget(HexCell target)
    {
        if(target.City.HasEffect(GetComponent<Unit>().GetPlayer(),"Envoy"))
        {
            return false;
        }
        return true;
    }

}


