
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BribeBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        if(target.City)
        {
            target.City.AdjustInfluence(gameObject.GetComponent<Unit>().GetPlayer(), (config as BribeConfig).GetInfluence());
            target.City.CheckInfluence();
        }

    }

    public override void ShowAbility(HexCell target = null)
    {
        if (target.IsVisible)
        {
            PlayParticleEffect();
            PlayAbilitySound();
            PlayAnimation(target);
            if (gameObject.GetComponent<Unit>().GetPlayer().IsHuman)
            {
                PlayTextEffect((config as BribeConfig).GetInfluence().ToString(), target, Color.yellow);
            }
            else
            {
                PlayTextEffect((config as BribeConfig).GetInfluence().ToString(), target, Color.blue);
            }

        }
    }
    public override void FinishAbility(HexCell target = null)
    {

    }
    public override bool IsValidTarget(HexCell target)
    {
        if(target.City)
        {
            Player player = target.City.Player;
            if (!player)
            {
                return true;
            }
        }

        return false;
    }


}


