using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CombatUnit : Unit
{

    public enum Stance
    {
        UNASSIGNED,
        OFFENCE,
        DEFENCE,
        EXPLORE
    }

    Stance currentStance = Stance.UNASSIGNED;

    public Stance CurrentStance
    {
        get { return currentStance; }
        set { currentStance = value; }
    }

    public override bool CanAttack(Unit unit)
    {
        if(unit.CityState && unit.CityState != CityState)
        {
            return true;
        }
        return false;
    }

    public void Save(BinaryWriter writer)
    {
        GetComponent<HexUnit>().Save(writer);
        writer.Write(HitPoints);
        writer.Write(GetMovementLeft());
    }

    public static CombatUnit Load(BinaryReader reader, HexGrid grid, int header)
    {
        HexUnit unit = HexUnit.Load(reader, grid, header);
        CombatUnit combatUnit = unit.GetComponent<CombatUnit>();
        if (header >= 3)
        {
            combatUnit.HitPoints = reader.ReadInt32();
            combatUnit.SetMovementLeft(reader.ReadInt32());
        }
        return combatUnit;
    }
}