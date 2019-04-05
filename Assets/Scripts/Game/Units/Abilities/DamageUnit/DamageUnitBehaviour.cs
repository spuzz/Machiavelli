
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageUnitBehaviour : AbilityBehaviour
{
    HexUnit targetUnit;
    public override void Use(HexCell target = null)
    {
        targetUnit = target.hexUnits.Find(C => C.HexUnitType == HexUnit.UnitType.COMBAT);
        if (targetUnit)
        {
            int damage = -(config as DamageUnitConfig).GetDamage();
            targetUnit.GetComponent<Unit>().HitPoints += damage;
            if (targetUnit.GetComponent<Unit>().HitPoints <= 0)
            {
                GameController.Instance.KillUnit(targetUnit.unit);
            }
        }
    }

    public override void FinishAbility(HexCell target = null)
    {
        int damage = -(config as DamageUnitConfig).GetDamage();
        targetUnit.GetComponent<Unit>().UpdateUI(damage);
        if (targetUnit.GetComponent<Unit>().HitPoints <= 0)
        {
            if(target.IsVisible)
            {
                targetUnit.DieAnimationAndRemove();
            }
            else
            {
                targetUnit.DestroyHexUnit();
            }
        }
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


