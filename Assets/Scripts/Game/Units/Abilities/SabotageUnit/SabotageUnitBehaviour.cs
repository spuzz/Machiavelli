
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SabotageUnitBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        HexUnit targetUnit = target.hexUnits.Find(C => C.HexUnitType == HexUnit.UnitType.COMBAT);
        if (targetUnit)
        {
            targetUnit.GetComponent<Unit>().SetMovementLeft(0);
        }
    }

    public override void FinishAbility(HexCell target = null)
    {

    }
    public override bool IsValidTarget(HexCell target)
    {
        if(target.hexUnits.FindAll(d => d.HexUnitType == HexUnit.UnitType.COMBAT).Count != 0)
        {
            return true;
        }

        return false;
    }


}


