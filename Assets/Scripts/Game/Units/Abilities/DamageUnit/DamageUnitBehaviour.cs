﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageUnitBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        HexUnit targetUnit = target.hexUnits.Find(C => C.HexUnitType == HexUnit.UnitType.COMBAT);
        if (targetUnit)
        {
            targetUnit.GetComponent<Unit>().HitPoints -= (config as DamageUnitConfig).GetDamage();
            targetUnit.GetComponent<Unit>().UpdateUI();
            if (targetUnit.GetComponent<Unit>().HitPoints <= 0)
            {
                targetUnit.DieAnimationAndRemove();
            }
        }
        PlayParticleEffect();
        PlayAbilitySound();
        PlayAnimation();
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


