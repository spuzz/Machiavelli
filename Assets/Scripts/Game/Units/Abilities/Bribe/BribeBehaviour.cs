
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
        if(target.IsVisible)
        {
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
            
        }

    }
    public override List<HexCell> IsValidTarget(HexCell target)
    {
        List<HexCell> targetCells = new List<HexCell>();
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(target, (config as BribeConfig).Range);
        List<HexCell> cityCells = cells.FindAll(c => c.City);
        foreach(HexCell cityCell in cityCells)
        {
            CityState cityState = cityCell.City.GetCityState();
            if (!cityState)
            {
                continue;
            }

            Player player = cityState.Player;
            if (player)
            {
                if (player != gameObject.GetComponent<Unit>().GetPlayer())
                {
                    continue;
                }
                if (cityState.GetInfluence(player) >= 100)
                {
                    continue;
                }
            }
            targetCells.Add(cityCell);
        }

        return targetCells;
    }



}


