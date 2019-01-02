using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CombatUnit : Unit
{


    public void Save(BinaryWriter writer)
    {
        GetComponent<HexUnit>().Save(writer);
    }

    public static CombatUnit Load(BinaryReader reader, HexGrid grid, int header)
    {
        HexUnit unit = HexUnit.Load(reader, grid, header);
        CombatUnit combatUnit = unit.GetComponent<CombatUnit>();
        return combatUnit;
    }

    public override bool CanAttack(Unit unit)
    {
        if(unit.CityState && unit.CityState != CityState)
        {
            return true;
        }
        return false;
    }
}