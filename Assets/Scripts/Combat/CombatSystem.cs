using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class CombatSystem
{
    static int defaultDamage = 35;

    public static KeyValuePair<int, int> Fight(HexCell attackCell, HexCell targetCell)
    {
        KeyValuePair<int, int> result = new KeyValuePair<int, int>();

        List<HexUnit> attackCellUnits = attackCell.hexUnits.FindAll(c => c.unit.HexUnitType == Unit.UnitType.COMBAT);
        List<HexUnit> defenceCellUnits = targetCell.hexUnits.FindAll(c => c.unit.HexUnitType == Unit.UnitType.COMBAT);
        int attackCombatStrength = 0;
        int defenceCombatStrength = 0;
        HexUnit frontLineAttackUnit = attackCellUnits[0];
        HexUnit frontLineDefenceUnit = defenceCellUnits[0];
        foreach (HexUnit unit in attackCellUnits)
        {
            attackCombatStrength += CalculateStrength(unit, frontLineDefenceUnit, targetCell, false);
        }

        foreach (HexUnit unit in defenceCellUnits)
        {
            defenceCombatStrength += CalculateStrength(unit, frontLineAttackUnit, targetCell, false);
        }
        int attackDamage = GetDamage(attackCombatStrength, defenceCombatStrength);
        int defenceDamage = GetDamage(defenceCombatStrength, attackCombatStrength);

        foreach (HexUnit unit in attackCellUnits)
        {
            unit.unit.DamageUnit(defenceDamage);
        }

        foreach (HexUnit unit in defenceCellUnits)
        {
            unit.unit.DamageUnit(attackDamage);
        }

        result = new KeyValuePair<int, int>(attackDamage,defenceDamage);
        return result;
    }

    private static int CalculateStrength(HexUnit unit, HexUnit frontLineUnit, HexCell targetCell, bool defending)
    {
        CombatUnit combatUnit = unit.unit as CombatUnit;
        CombatUnitConfig config = combatUnit.GetCombatUnitConfig();
        int strength = config.BaseStrength;
        int positiveModifier = 0;
        int negativeModifer = 0;
        if (defending)
        {
            AddModifier(config.DefenceModifier, config, ref positiveModifier, ref negativeModifer);
            if(targetCell.City)
            {
                AddModifier(config.DifficultTerrainModifier, config, ref positiveModifier, ref negativeModifer);
            }
        }
        else
        {
            AddModifier(config.OffenceModifier, config, ref positiveModifier, ref negativeModifer);
        }

        List<CombatClassModifier> modifiers = config.ClassModifers.FindAll(c => c.Classification == config.Classification);
        foreach(CombatClassModifier modifier in modifiers)
        {
            if((frontLineUnit.unit as CombatUnit).GetCombatUnitConfig().Classification == modifier.Classification)
            {
                AddModifier(modifier.Modifier, config, ref positiveModifier, ref negativeModifer);
            }

        }
        if(negativeModifer > 100)
        {
            negativeModifer = 100;
        }
        strength = (int) ((float)strength * ((float)positiveModifier / 100.0f));
        strength = (int)((float)strength * ((100.0f - (float)negativeModifer) / 100.0f));
        return strength;
    }

    private static void AddModifier(int modifier, CombatUnitConfig config, ref int positiveModifier, ref int negativeModifer)
    {
        if (modifier >= 0)
        {
            positiveModifier += modifier;
        }
        else
        {
            negativeModifer += (-modifier);
        }
    }


    //public static KeyValuePair<int, int> UnitFight(Unit fighter, Unit target)
    //{
    //    KeyValuePair<int, int> result;
    //    int targetDamage;
    //        int fighterDamage;
    //    if (fighter.Range == 0)
    //    {
    //        targetDamage = GetMeleeDamage(fighter.Strength, target.Strength);
    //        fighterDamage = GetMeleeDamage(target.Strength, fighter.Strength);
    //    }
    //    else
    //    {
    //        targetDamage = GetRangeDamage(fighter.RangeStrength, target.Strength);
    //        fighterDamage = 0;
    //    }

    //    int currentTargetHealth = target.HitPoints;
    //    int currentFighterHealth = fighter.HitPoints;
    //    int targetHitpoints = target.HitPoints - targetDamage;
    //    int fighterHitpoints = fighter.HitPoints - fighterDamage;

    //    if(targetHitpoints <= 0 && fighterHitpoints <= 0)
    //    {
    //        if(targetHitpoints > fighterHitpoints)
    //        {
    //            targetHitpoints = 1;
    //        }
    //        else
    //        {
    //            fighterHitpoints = 1;
    //        }
    //    }

    //    target.HitPoints = targetHitpoints;
    //    fighter.HitPoints = fighterHitpoints;
    //    if (target.HitPoints < 0)
    //    {
    //        target.HitPoints = 0;
    //    }

    //    if (fighter.HitPoints < 0)
    //    {
    //        fighter.HitPoints = 0;
    //    }
    //    result = new KeyValuePair<int, int>(currentTargetHealth - target.HitPoints, currentFighterHealth - fighter.HitPoints);
    //    return result;
    //}

    //public static KeyValuePair<int, int> CityFight(Unit fighter, City target)
    //{
    //    KeyValuePair<int, int> result;
    //    int targetDamage = GetMeleeDamage(fighter.Strength, target.Strength);
    //    int fighterDamage = GetMeleeDamage(target.Strength, fighter.Strength);
    //    int currentTargetHealth = target.HitPoints;
    //    int currentFighterHealth = fighter.HitPoints;
    //    int targetHitpoints = target.HitPoints - targetDamage;
    //    int fighterHitpoints = fighter.HitPoints - fighterDamage;

    //    if (targetHitpoints <= 0 && fighterHitpoints <= 0)
    //    {
    //        if (targetHitpoints > fighterHitpoints)
    //        {
    //            targetHitpoints = 1;
    //        }
    //        else
    //        {
    //            fighterHitpoints = 1;
    //        }
    //    }

    //    target.HitPoints = targetHitpoints;
    //    fighter.HitPoints = fighterHitpoints;
    //    if (target.HitPoints < 0)
    //    {
    //        target.HitPoints = 0;
    //    }

    //    if (fighter.HitPoints < 0)
    //    {
    //        fighter.HitPoints = 0;
    //    }
    //    result = new KeyValuePair<int, int>(currentTargetHealth - target.HitPoints, currentFighterHealth - fighter.HitPoints);
    //    return result;
    //}


    public static int GetDamage(int strength, int targetStrength)
    {
        int difference = strength - targetStrength;
        int damageChange = difference * 2;
        int damage = LimitToRange(defaultDamage + damageChange, 5, 105);
        damage = UnityEngine.Random.Range(damage - 5, damage + 6);
        damage = LimitToRange(damage, 1, 100);
        return damage;

    }

    public static int LimitToRange(
        this int value, int inclusiveMinimum, int inclusiveMaximum)
    {
        if (value < inclusiveMinimum) { return inclusiveMinimum; }
        if (value > inclusiveMaximum) { return inclusiveMaximum; }
        return value;
    }
}
