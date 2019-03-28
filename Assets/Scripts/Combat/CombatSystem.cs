using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class CombatSystem
{
    static int defaultDamage = 35;

    public static KeyValuePair<int, int> UnitFight(Unit fighter, Unit target)
    {
        KeyValuePair<int, int> result;
        int targetDamage;
            int fighterDamage;
        if (fighter.Range == 0)
        {
            targetDamage = GetMeleeDamage(fighter.Strength, target.Strength);
            fighterDamage = GetMeleeDamage(target.Strength, fighter.Strength);
        }
        else
        {
            targetDamage = GetRangeDamage(fighter.RangeStrength, target.Strength);
            fighterDamage = 0;
        }
        
        int currentTargetHealth = target.HitPoints;
        int currentFighterHealth = fighter.HitPoints;
        int targetHitpoints = target.HitPoints - targetDamage;
        int fighterHitpoints = fighter.HitPoints - fighterDamage;

        if(targetHitpoints <= 0 && fighterHitpoints <= 0)
        {
            if(targetHitpoints > fighterHitpoints)
            {
                targetHitpoints = 1;
            }
            else
            {
                fighterHitpoints = 1;
            }
        }

        target.HitPoints = targetHitpoints;
        fighter.HitPoints = fighterHitpoints;
        if (target.HitPoints < 0)
        {
            target.HitPoints = 0;
        }

        if (fighter.HitPoints < 0)
        {
            fighter.HitPoints = 0;
        }
        result = new KeyValuePair<int, int>(currentTargetHealth - target.HitPoints, currentFighterHealth - fighter.HitPoints);
        return result;
    }

    public static KeyValuePair<int, int> CityFight(Unit fighter, City target)
    {
        KeyValuePair<int, int> result;
        int targetDamage = GetMeleeDamage(fighter.Strength, target.Strength);
        int fighterDamage = GetMeleeDamage(target.Strength, fighter.Strength);
        int currentTargetHealth = target.HitPoints;
        int currentFighterHealth = fighter.HitPoints;
        int targetHitpoints = target.HitPoints - targetDamage;
        int fighterHitpoints = fighter.HitPoints - fighterDamage;

        if (targetHitpoints <= 0 && fighterHitpoints <= 0)
        {
            if (targetHitpoints > fighterHitpoints)
            {
                targetHitpoints = 1;
            }
            else
            {
                fighterHitpoints = 1;
            }
        }

        target.HitPoints = targetHitpoints;
        fighter.HitPoints = fighterHitpoints;
        if (target.HitPoints < 0)
        {
            target.HitPoints = 0;
        }

        if (fighter.HitPoints < 0)
        {
            fighter.HitPoints = 0;
        }
        result = new KeyValuePair<int, int>(currentTargetHealth - target.HitPoints, currentFighterHealth - fighter.HitPoints);
        return result;
    }


    public static int GetMeleeDamage(int strength, int targetStrength)
    {
        int difference = strength - targetStrength;
        int damageChange = difference * 2;
        int damage = LimitToRange(defaultDamage + damageChange, 5, 105);
        damage = UnityEngine.Random.Range(damage - 5, damage + 6);
        damage = LimitToRange(damage,1, 100);
        return damage;

    }

    public static int GetRangeDamage(int strength, int targetStrength)
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
