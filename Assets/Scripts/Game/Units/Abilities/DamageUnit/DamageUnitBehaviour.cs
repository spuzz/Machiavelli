
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageUnitBehaviour : AbilityBehaviour
{
    HexUnit targetUnit;
    List<int> damageList = new List<int>();
    public override void Use(HexCell target = null)
    {
        targetUnit = target.hexUnits.Find(C => C.HexUnitType == HexUnit.UnitType.COMBAT);
        if (targetUnit)
        {
            int damage = -(config as DamageUnitConfig).GetDamage();
            damageList.Add(damage);
            targetUnit.GetComponent<Unit>().HitPoints += damage;
            abilityText = damageList[0].ToString();
        }
    }
    public override bool Merge()
    {
        if (damageList.Count < 2)
        {
            throw new InvalidOperationException("No Previous action to merge with");
        }
        damageList[1] += damageList[0];
        damageList.Remove(damageList[0]);
        abilityText = damageList[0].ToString();
        return true;

    }

    public override void ShowAbility(int energyCost, HexCell target = null)
    {
        base.ShowAbility(energyCost, target);
        int damage = damageList[0];
        targetUnit.GetComponent<Unit>().UpdateUI(damage);
    }
    public override void FinishAbility(HexCell target = null)
    {
        damageList.Remove(damageList[0]);
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


