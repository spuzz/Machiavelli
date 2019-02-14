
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackOpCentreBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        if(target.OpCentre)
        {
            Player player = target.OpCentre.Player;
            //target.OpCentre.DestroyOperationCentre();

        }
        PlayParticleEffect();
        PlayAbilitySound();
        PlayAnimation();
    }
    public override List<HexCell> IsValidTarget(HexCell target)
    {
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(target, (config as AttackOpCentreConfig).Range);
        List<HexCell> opCentreCells = cells.FindAll(c => c.OpCentre && c.OpCentre.Player != gameObject.GetComponent<Unit>().GetPlayer());

        return opCentreCells;
    }



}


