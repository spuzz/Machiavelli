using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvoyEffect : Effect
{
    int envoyStrength = 2;

    public EnvoyEffect()
    {
        name = "Envoy";
    }

    public int EnvoyStrength
    {
        get
        {
            return envoyStrength;
        }

        set
        {
            envoyStrength = value;
        }
    }

    public override bool Compare(Effect effect)
    {
        if(effect.Name.CompareTo(Name) != 0 || effect.Player != Player)
        {
            return false;
        }
        return true;
    }

    public override void UseEffect(City city)
    {
        city.AdjustInfluence(Player, envoyStrength);
        if(Player.IsHuman && city.GetHexCell().IsVisible)
        {
            city.GetHexCell().TextEffectHandler.AddTextEffect("Envoy(3) - Player(" + Player.PlayerNumber + ")", city.GetHexCell().transform, Color.green);
        }

    }
}
