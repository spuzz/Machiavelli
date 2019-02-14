
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AssassinateAgentBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        HexUnit targetAgent = target.hexUnits.Find(C => C.HexUnitType == HexUnit.UnitType.AGENT);
        if (targetAgent)
        {
            targetAgent.GetComponent<Unit>().HitPoints -= (config as AssassinateAgentConfig).GetDamage();

        }
        PlayParticleEffect();
        PlayAbilitySound();
        PlayAnimation();
    }
    public override List<HexCell> IsValidTarget(HexCell target)
    {
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(target, (config as AssassinateAgentConfig).Range);
        List<HexCell> agentCells = cells.FindAll(c => c.hexUnits.FindAll(d => d.HexUnitType == HexUnit.UnitType.AGENT && d.GetComponent<Unit>().GetPlayer() != gameObject.GetComponent<Unit>().GetPlayer()).Count != 0);

        return agentCells;
    }



}


