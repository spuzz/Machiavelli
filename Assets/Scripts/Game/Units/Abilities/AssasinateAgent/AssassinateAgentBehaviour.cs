
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AssassinateAgentBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        if(!IsValidTarget(target))
        {
            return;
        }
        HexUnit targetAgent = target.hexUnits.Find(C => C.HexUnitType == HexUnit.UnitType.AGENT);
        if (targetAgent)
        {
            PlayParticleEffect();
            PlayAbilitySound();
            PlayAnimation();
            targetAgent.GetComponent<Unit>().GameController.KillUnit(targetAgent.GetComponent<Unit>());
            targetAgent.GetComponent<Unit>().GameController.AnimateonlyDestroyUnit(targetAgent.GetComponent<Unit>());

        }

    }
    public override bool IsValidTarget(HexCell target)
    {
        if(target.hexUnits.FindAll(d => d.HexUnitType == HexUnit.UnitType.AGENT && d.GetComponent<Agent>().GetPlayer() != gameObject.GetComponent<Unit>().GetPlayer()).Count != 0)
        {
            return true;
        }
        return false;
    }




}


