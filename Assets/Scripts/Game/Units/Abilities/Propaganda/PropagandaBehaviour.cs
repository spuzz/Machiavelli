
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PropagandaBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        if(target.City)
        {
            CityState cityState = target.City.GetCityState();
            cityState.AdjustInfluenceForAllExcluding(gameObject.GetComponent<Unit>().GetPlayer(), (config as PropagandaConfig).GetInfluence());
            cityState.CheckInfluence();
        }
    }

    public override void ShowAbility(HexCell target = null)
    {
        if (target.IsVisible)
        {
            PlayParticleEffect();
            PlayAbilitySound();
            PlayAnimation();
            if (gameObject.GetComponent<Unit>().GetPlayer().IsHuman)
            {
                PlayTextEffect((config as PropagandaConfig).GetInfluence().ToString(), target, Color.yellow);
            }
            else
            {
                PlayTextEffect((config as PropagandaConfig).GetInfluence().ToString(), target, Color.blue);
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
            CityState cityState = target.City.GetCityState();
            if (cityState && cityState.Player && cityState.Player != gameObject.GetComponent<Unit>().GetPlayer())
            {
                return true;
            }
        }

        return false;
    }


}


