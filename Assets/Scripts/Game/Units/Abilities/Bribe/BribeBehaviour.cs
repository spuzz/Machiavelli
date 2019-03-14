
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
        //if(target.IsVisible)
        //{
            PlayParticleEffect();
            PlayAbilitySound();
            PlayAnimation();
            if(gameObject.GetComponent<Unit>().GetPlayer().IsHuman)
            {
                PlayTextEffect((config as BribeConfig).GetInfluence().ToString(), target, Color.yellow);
            }
            else
            {
                PlayTextEffect((config as BribeConfig).GetInfluence().ToString(), target, Color.blue);
            }
            
        //}

    }
    public override bool IsValidTarget(HexCell target)
    {
        if(target.City)
        {
            CityState cityState = target.City.GetCityState();
            if (!cityState)
            {
                return false;
            }

            Player player = cityState.Player;
            if (!player)
            {
                return true;
            }
        }

        return false;
    }



}


