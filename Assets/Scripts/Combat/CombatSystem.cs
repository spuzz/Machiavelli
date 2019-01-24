using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class CombatSystem
{
    static int defaultDamage = 35;

    public static void UnitFight(Unit fighter, Unit target)
    {
        int targetDamage = GetMeleeDamage(fighter.Strength, target.Strength);
        int fighterDamage = GetMeleeDamage(target.Strength, fighter.Strength);
        target.HitPoints -= targetDamage;
        fighter.HitPoints -= fighterDamage;

        if(target.HitPoints <= 0 && fighter.HitPoints <= 0)
        {
            if(target.HitPoints > fighter.HitPoints)
            {
                target.HitPoints = 1;
            }
            else
            {
                fighter.HitPoints = 1;
            }
        }

        if (target.HitPoints < 0)
        {
            target.HitPoints = 0;
        }

        if (fighter.HitPoints < 0)
        {
            fighter.HitPoints = 0;
        }
    }

    public static void CityFight(Unit fighter, City target)
    {
        int targetDamage = GetMeleeDamage(fighter.Strength, target.Strength);
        int fighterDamage = GetMeleeDamage(target.Strength, fighter.Strength);
        target.HitPoints -= targetDamage;
        fighter.HitPoints -= fighterDamage;

        if (target.HitPoints <= 0 && fighter.HitPoints <= 0)
        {
            if (target.HitPoints > fighter.HitPoints)
            {
                target.HitPoints = 1;
            }
            else
            {
                fighter.HitPoints = 1;
            }
        }

        if (target.HitPoints < 0)
        {
            target.HitPoints = 0;
        }

        if (fighter.HitPoints < 0)
        {
            fighter.HitPoints = 0;
        }
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

    public static int LimitToRange(
        this int value, int inclusiveMinimum, int inclusiveMaximum)
    {
        if (value < inclusiveMinimum) { return inclusiveMinimum; }
        if (value > inclusiveMaximum) { return inclusiveMaximum; }
        return value;
    }
}
