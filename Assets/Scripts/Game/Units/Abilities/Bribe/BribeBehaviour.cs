
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BribeBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        if(target.City)
        {
            CityState cityState = target.City.GetCityState();
            cityState.AdjustInfluence(gameObject.GetComponent<Unit>().GetPlayer(), (config as BribeConfig).GetInfluence());
            cityState.CheckInfluence();

        }
        PlayParticleEffect();
        PlayAbilitySound();
        PlayAnimation();
    }
    public override bool IsValidTarget(HexCell target)
    {
        if (!target.City)
        {
            return false;
        }
        CityState cityState = target.City.GetCityState();
        if(!cityState)
        {
            return false;
        }

        Player player = cityState.Player;
        if(player)
        {
            if(player != gameObject.GetComponent<Unit>().GetPlayer())
            {
                return false;
            }
            if (cityState.GetInfluence(player) >= 100)
            {
                return false;
            }
        }
        return true;
    }



}


