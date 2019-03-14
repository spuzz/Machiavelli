
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildOpCentreBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {

        if (IsValidTarget(target))
        {
            PlayParticleEffect();
            PlayAbilitySound();
            PlayAnimation();
            gameObject.GetComponent<Unit>().GetPlayer().CreateOperationCentre(target);
            gameObject.GetComponent<Unit>().GameController.DestroyUnit(gameObject.GetComponent<Unit>());
            gameObject.GetComponent<Unit>().HUDUI.OpCentre = target.OpCentre;
        }


    }

    public override bool IsValidTarget(HexCell target)
    {
        if(!target.City && !target.OpCentre)
        {
            List<HexCell> cells = PathFindingUtilities.GetCellsInRange(target, 3).FindAll(c => c.OpCentre);
            if (cells.Count == 0)
            {
                return true;
            }
        }

        return false;
    }



}


