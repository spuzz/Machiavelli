
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InciteRiotBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        if (target.City)
        {
            target.City.Happiness += (config as InciteRiotConfig).GetHappinessChange();
        }
    }
    public override void ShowAbility(HexCell target = null)
    {
        if (target.IsVisible)
        {
            PlayParticleEffect();
            PlayAbilitySound();
            PlayAnimation(target);
            target.TextEffectHandler.AddTextEffect((config as InciteRiotConfig).GetHappinessChange().ToString() + " Happiness", target.transform, Color.magenta);

        }
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


