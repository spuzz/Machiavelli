
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
        PlayParticleEffect();
        PlayAbilitySound();
        PlayAnimation();
    }
    public override List<HexCell> IsValidTarget(HexCell target)
    {
        List<HexCell> targetCells = new List<HexCell>();
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(target, (config as PropagandaConfig).Range);
        List<HexCell> cityCells = cells.FindAll(c => c.City);
        foreach(HexCell cityCell in cityCells)
        {
            CityState cityState = cityCell.City.GetCityState();
            if (!cityState || cityState.Player == gameObject.GetComponent<Unit>().GetPlayer())
            {
                continue;
            }
            targetCells.Add(cityCell);
        }

        return targetCells;
    }



}


