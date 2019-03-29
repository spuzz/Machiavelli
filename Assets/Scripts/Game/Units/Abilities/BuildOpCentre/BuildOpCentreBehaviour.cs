
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildOpCentreBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        
        if (IsValidTarget(target))
        {
            gameObject.GetComponent<Unit>().GameController.AddOperationCentre(target, gameObject.GetComponent<Unit>().GetPlayer());
            gameObject.GetComponent<Unit>().GameController.KillUnit(gameObject.GetComponent<Unit>());
        }
    }

    public override void FinishAbility(HexCell target = null)
    {
        if (gameObject.GetComponent<Unit>().GetPlayer() && gameObject.GetComponent<Unit>().GetPlayer().IsHuman)
        {
            gameObject.GetComponent<Unit>().HUDUI.OpCentre = target.OpCentre;
        }
        gameObject.GetComponent<Unit>().GameController.ShowOperationCentre(target.OpCentre);
        gameObject.GetComponent<Unit>().GameController.DestroyUnit(gameObject.GetComponent<Unit>());
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


