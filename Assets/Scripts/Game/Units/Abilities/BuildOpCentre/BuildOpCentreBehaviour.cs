
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
        PlayParticleEffect();
        PlayAbilitySound();
        PlayAnimation();
    }

    public override List<HexCell> IsValidTarget(HexCell target)
    {
        List<HexCell> possibleLocations = new List<HexCell>();
        
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(target, 3).FindAll(c => c.OpCentre || c.OpCentre);
        if (cells.Count == 0)
        {
            possibleLocations.Add(target);
        }

        return possibleLocations;
    }



}


