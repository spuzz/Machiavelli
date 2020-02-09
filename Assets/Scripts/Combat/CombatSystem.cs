using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Combat
{
    public enum BATTLE_LIKELY_OUTCOME
    {
        CERTAIN_DEFEAT,
        CLOSE_DEFEAT,
        CLOSE_VICTORY,
        CERTAIN_VICTORY
    };

    public int attackerStrength;
    public int defenderStrength;

    public bool defenderAmbushed = false;
    public bool attackerAmbushed = false;

    public BATTLE_LIKELY_OUTCOME likely_outcome;

    public List<HexUnit> defendSupport = new List<HexUnit>();
    public List<HexUnit> attackSupport = new List<HexUnit>();

    public HexCell attackerCell;
    public HexCell defenderCell;

    public HexUnit attackerUnit;
    public HexUnit defenderUnit;

    public bool includeUnseen;

    public Combat(HexCell attackerCell, HexCell defenderCell, bool includeUnseen = true)
    {
        this.includeUnseen = includeUnseen;
        this.attackerCell = attackerCell;
        this.defenderCell = defenderCell;
        attackerUnit = attackerCell.combatUnit;
        defenderUnit = defenderCell.combatUnit;
        CombatStrength();
    }

    public void CombatStrength()
    {
        ClearData();


        attackSupport = CombatSystem.GetSupportUnits(attackerCell, defenderCell, includeUnseen);
        attackerStrength += CombatSystem.CalculateUnitsStrength(attackerCell.combatUnit, defenderCell.combatUnit, defenderCell, false);
        foreach (HexUnit hexUnit in attackSupport)
        {
            attackerStrength += CombatSystem.CalculateUnitsStrength(hexUnit, defenderCell.combatUnit, defenderCell, false);
        }

        if (attackerCell.City && !defenderUnit)
        {
            defenderStrength = 0;
        }
        else
        {
            defendSupport = CombatSystem.GetSupportUnits(defenderCell, attackerCell, includeUnseen);
            defenderStrength += CombatSystem.CalculateUnitsStrength(defenderCell.combatUnit, attackerCell.combatUnit, attackerCell, false);
            foreach (HexUnit hexUnit in defendSupport)
            {
                defenderStrength += CombatSystem.CalculateUnitsStrength(hexUnit, attackerCell.combatUnit, attackerCell, false);
            }

            bool attackerVision = attackerCell.combatUnit.unit.GetPlayer().visibleCells.Keys.Contains(defenderCell);
            bool defenderVision = defenderCell.combatUnit.unit.GetPlayer().visibleCells.Keys.Contains(attackerCell);
            if (attackerVision && !defenderVision)
            {
                attackerStrength = (int)((float)attackerStrength * GameConsts.ambushBonus);
                defenderAmbushed = true;
            }
            else if (!attackerVision && defenderVision)
            {
                defenderStrength = (int)((float)defenderStrength * GameConsts.ambushBonus);
                attackerAmbushed = true;
            }
        }



        if (attackerStrength > defenderStrength * 1.2)
        {
            likely_outcome = BATTLE_LIKELY_OUTCOME.CERTAIN_VICTORY;
        }
        else if (attackerStrength >= defenderStrength)
        {
            likely_outcome = BATTLE_LIKELY_OUTCOME.CLOSE_VICTORY;
        }
        else if (defenderStrength > attackerStrength * 1.2)
        {
            likely_outcome = BATTLE_LIKELY_OUTCOME.CERTAIN_DEFEAT;
        }
        else if (defenderStrength > attackerStrength)
        {
            likely_outcome = BATTLE_LIKELY_OUTCOME.CLOSE_DEFEAT;
        }

    }

    private void ClearData()
    {
        attackerStrength = 0;
        defenderStrength = 0;
        defenderAmbushed = false;
        attackerAmbushed = false;
        defendSupport = new List<HexUnit>();
        attackSupport = new List<HexUnit>();
    }

    public List<FightResult> Fight()
    {
        List<FightResult> results = new List<FightResult>();
        int defenceDamage = CombatSystem.GetDamage(attackerStrength, defenderStrength);
        int attackDamage = CombatSystem.GetDamage(defenderStrength, attackerStrength);

        attackerUnit.unit.DamageUnit(attackDamage);
        defenderUnit.unit.DamageUnit(defenceDamage);

        FightResult attackerResult = GetFightResult(attackDamage, attackerUnit, defenderUnit, attackerCell, defenderCell, defenderAmbushed);
        if (defenderCell.City && defenderUnit.unit.HitPoints <= 0)
        {
            attackerResult.cityTaken = true;
        }
        results.Add(attackerResult);

        foreach (HexUnit unit in attackSupport)
        {
            results.Add(GetFightResult(0, unit, defenderUnit, unit.Location, defenderCell, false));
        }

        results.Add(GetFightResult(defenceDamage, defenderUnit, attackerUnit, defenderCell, attackerCell, attackerAmbushed));
        
        foreach (HexUnit unit in defendSupport)
        {
            results.Add(GetFightResult(0, unit, attackerUnit, unit.Location, attackerCell, false));
        }

        return results;
    }

    public FightResult GetFightResult(int damage, HexUnit attackerUnit, HexUnit defenderUnit, HexCell attackerCell, HexCell defenderCell, bool ambush)
    {
        FightResult fight;
        fight.damageReceived = damage;
        fight.unit = attackerUnit;

        if (attackerUnit.unit.HitPoints <= 0)
        {
            fight.isKilled = true;
        }
        else
        {
            fight.isKilled = false;
        }
        fight.ambush = ambush;
        fight.targetUnit = defenderUnit;
        fight.targetCell = defenderCell;
        fight.cityTaken = false;
        return fight;
    }
}

public struct FightResult
{
    public HexUnit unit;
    public HexCell targetCell;
    public HexUnit targetUnit;
    public int damageReceived;
    public bool isKilled;
    public bool ambush;
    public bool cityTaken;
}

public static class CombatSystem
{
    static int defaultDamage = 35;

    public static Combat Fight(HexCell attackCell, HexCell targetCell, bool includeUnseen = true)
    {
        Combat combat = new Combat(attackCell, targetCell, includeUnseen);
        return combat;
    }


    public static int CalculateUnitsStrength(HexUnit unit, HexUnit frontLineUnit, HexCell targetCell, bool defending)
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
        if(city.GetHexCell().combatUnit)
        {
            CombatUnit combatUnit = city.GetHexCell().combatUnit.unit as CombatUnit;
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

    public static List<HexUnit> GetSupportUnits(HexCell ownerCell, HexCell targetCell, bool includeUnseen)
    {
        List<HexUnit> combatUnits = new List<HexUnit>();

        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbour = ownerCell.GetNeighbor(d);
            HexUnit combatUnit = neighbour.combatUnit;
            if ((includeUnseen || neighbour.IsVisible) && combatUnit && combatUnit.unit.GetPlayer() == ownerCell.combatUnit.unit.GetPlayer())
            {
                if((combatUnit.unit as CombatUnit).CombatType == CombatUnit.CombatUnitType.SUPPORT)
                {
                    combatUnits.Add(combatUnit);
                }
                else if ((combatUnit.unit as CombatUnit).CombatType == CombatUnit.CombatUnitType.MELEE)
                {
                    if(neighbour.coordinates.DistanceTo(targetCell.coordinates) <= 1)
                    {
                        combatUnits.Add(combatUnit);
                    }
                }
                    
            }
        }
        return combatUnits;
    }

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
