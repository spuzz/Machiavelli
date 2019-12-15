using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public struct FightResult
{
    public HexUnit unit;
    public int damageReceived;
    public bool isKilled;
    public bool ambush;
}

public static class CombatSystem
{
    static int defaultDamage = 35;

    public static List<FightResult> Fight(HexCell attackCell, HexCell targetCell)
    {
        if (targetCell.City)
        {
            return FightCity(attackCell, targetCell.City);
        }
        else
        {
            return FightUnits(attackCell, targetCell);
        }
    }

    private static List<FightResult> FightCity(HexCell attackCell, City city)
    {
        List<FightResult> results = new List<FightResult>();
        List<HexUnit> attackCellUnits = attackCell.hexUnits.FindAll(c => c.unit.HexUnitType == Unit.UnitType.COMBAT);
        int attackCombatStrength = 0;
        int defenceCombatStrength = 0;
        foreach (HexUnit unit in attackCellUnits)
        {
            attackCombatStrength += CalculateStrength(unit, city);
        }

        defenceCombatStrength = CalculateCityStrength(city);

        int attackDamage = GetDamage(attackCombatStrength, defenceCombatStrength);
        int defenceDamage = GetDamage(defenceCombatStrength, attackCombatStrength);

        foreach (HexUnit unit in attackCellUnits)
        {
            FightResult fight;
            fight.damageReceived = defenceDamage;
            fight.unit = unit;

            unit.unit.DamageUnit(defenceDamage);
            if (unit.unit.HitPoints <= 0)
            {
                fight.isKilled = true;
            }
            else
            {
                fight.isKilled = false;
            }
            fight.ambush = false;
            results.Add(fight);
        }

        city.DamageCity(attackDamage);

        FightResult result;
        result.unit = null;
        result.ambush = false;
        result.damageReceived = attackDamage;
        result.isKilled = (city.HitPoints <= 0);
        results.Add(result);

        if (city.HitPoints <= 0)
        {
            city.GetCityState().SetPlayerOnly(attackCellUnits[0].unit.GetPlayer());
            city.KillAllUnits();
            city.HitPoints = 100;
        }

        return results;
    }

    private static List<FightResult> FightUnits(HexCell attackCell, HexCell targetCell)
    {
        List<FightResult> results = new List<FightResult>();

        List<HexUnit> attackCellUnits = attackCell.hexUnits.FindAll(c => c.unit.HexUnitType == Unit.UnitType.COMBAT);
        List<HexUnit> defenceCellUnits = targetCell.hexUnits.FindAll(c => c.unit.HexUnitType == Unit.UnitType.COMBAT);
        int attackCombatStrength = 0;
        int defenceCombatStrength = 0;
        bool attackAmbush = false;
        bool defenceAmbush = false;
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

        bool attackerVision = attackCellUnits[0].unit.GetPlayer().visibleCells.Keys.Contains(targetCell);
        bool defenderVision = defenceCellUnits[0].unit.GetPlayer().visibleCells.Keys.Contains(targetCell);
        if (attackerVision && !defenderVision)
        {
            attackCombatStrength = (int)((float)attackCombatStrength * GameConsts.ambushBonus);
            attackAmbush = true;
        }
        else if (!attackerVision && defenderVision)
        {
            defenceCombatStrength = (int)((float)defenceCombatStrength * GameConsts.ambushBonus);
            defenceAmbush = true;
        }
        int attackDamage = GetDamage(attackCombatStrength, defenceCombatStrength);
        int defenceDamage = GetDamage(defenceCombatStrength, attackCombatStrength);

        foreach (HexUnit unit in attackCellUnits)
        {
            FightResult fight;
            fight.damageReceived = defenceDamage;
            fight.unit = unit;

            unit.unit.DamageUnit(defenceDamage);
            if (unit.unit.HitPoints <= 0)
            {
                fight.isKilled = true;
            }
            else
            {
                fight.isKilled = false;
            }
            fight.ambush = attackAmbush;
            results.Add(fight);
        }

        foreach (HexUnit unit in defenceCellUnits)
        {
            FightResult fight;
            fight.damageReceived = attackDamage;
            fight.unit = unit;

            unit.unit.DamageUnit(attackDamage);
            if (unit.unit.HitPoints <= 0)
            {
                fight.isKilled = true;
            }
            else
            {
                fight.isKilled = false;
            }
            fight.ambush = defenceAmbush;
            results.Add(fight);

        }
        return results;
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
        strength = (int) ((float)strength * ((100.0f + (float)positiveModifier) / 100.0f));
        strength = (int)((float)strength * ((100.0f - (float)negativeModifer) / 100.0f));
        return strength;
    }

    private static int CalculateStrength(HexUnit unit, City city)
    {
        CombatUnit combatUnit = unit.unit as CombatUnit;
        CombatUnitConfig config = combatUnit.GetCombatUnitConfig();
        int strength = config.BaseStrength;
        int positiveModifier = 0;
        int negativeModifer = 0;

        AddModifier(config.OffenceModifier, config, ref positiveModifier, ref negativeModifer);

        if (negativeModifer > 100)
        {
            negativeModifer = 100;
        }
        strength = (int)((float)strength * ((100.0f + (float)positiveModifier) / 100.0f));
        strength = (int)((float)strength * ((100.0f - (float)negativeModifer) / 100.0f));
        return strength;
    }
    private static int CalculateCityStrength(City city)
    {
        int strength = city.Strength;
        foreach(HexUnit unit in city.GetHexCell().hexUnits.FindAll( c=> c.unit.HexUnitType != Unit.UnitType.AGENT))
        {
            CombatUnit combatUnit = unit.unit as CombatUnit;
            CombatUnitConfig config = combatUnit.GetCombatUnitConfig();
            strength += config.BaseStrength;
        }
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
