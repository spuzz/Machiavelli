
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AssassinateAgentBehaviour : AbilityBehaviour
{
    protected HexUnit targetAgent;
    public override void Use(HexCell target = null)
    {
        if(!IsValidTarget(target))
        {
            return;
        }
        targetAgent = target.hexUnits.Find(C => C.HexUnitType == HexUnit.UnitType.AGENT);
        targetAgent.GetComponent<Unit>().GameController.KillUnit(targetAgent.unit);
    }

    public override bool IsValidTarget(HexCell target)
    {
        if(target.hexUnits.FindAll(d => d.HexUnitType == HexUnit.UnitType.AGENT && d.GetComponent<Agent>().GetPlayer() != gameObject.GetComponent<Unit>().GetPlayer()).Count != 0)
        {
            return true;
        }
        return false;
    }

    public override void FinishAbility(HexCell target = null)
    {
        if(target.IsVisible && targetAgent)
        {
            targetAgent.GetComponent<Unit>().GameController.AnimateAndDestroyUnit(targetAgent.GetComponent<Unit>());
        }
        else
        {
            targetAgent.GetComponent<Unit>().GameController.DestroyUnit(targetAgent.GetComponent<Unit>());
        }
    }
}


