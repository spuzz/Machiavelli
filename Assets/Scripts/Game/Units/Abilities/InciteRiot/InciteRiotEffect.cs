using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InciteRiotEffect : Effect
{
    int riotStrength = 5;
    public InciteRiotEffect()
    {
        name = "InciteRiot";
    }

    public int RiotStrength
    {
        get
        {
            return riotStrength;
        }

        set
        {
            riotStrength = value;
        }
    }

    public override bool Compare(Effect effect)
    {
        if(effect.Name.CompareTo(Name) == 0)
        {
            return true;
        }
        return false;
    }

    public override void UseEffect(City city)
    {
        city.Happiness -= RiotStrength;
        if(city.GetHexCell().IsVisible)
        {
            city.GetHexCell().TextEffectHandler.AddTextEffect("-3 Happiness", city.GetHexCell().transform, Color.cyan);
        }

    }
}
