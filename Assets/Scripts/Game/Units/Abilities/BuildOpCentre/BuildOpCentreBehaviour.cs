﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildOpCentreBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        if(IsValidTarget(target).Count > 0)
        {
            gameObject.GetComponent<Unit>().GetPlayer().CreateOperationCentre(target);
        }
        gameObject.GetComponent<Unit>().GameController.DestroyUnit(gameObject.GetComponent<Unit>());
        gameObject.GetComponent<Unit>().HUDUI.OpCentre = target.OpCentre;
        PlayParticleEffect();
        PlayAbilitySound();
        PlayAnimation();
        
    }

    public override List<HexCell> IsValidTarget(HexCell target)
    {
        List<HexCell> possibleLocations = new List<HexCell>();
        
        if(!target.City && !target.OpCentre)
        {
            List<HexCell> cells = PathFindingUtilities.GetCellsInRange(target, 3).FindAll(c => c.OpCentre);
            if (cells.Count == 0)
            {
                possibleLocations.Add(target);
            }
        }


        return possibleLocations;
    }



}


