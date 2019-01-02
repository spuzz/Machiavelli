using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Agent : Unit {

    public void Save(BinaryWriter writer)
    {
        GetComponent<HexUnit>().Save(writer);
    }

    public static Agent Load(BinaryReader reader, HexGrid grid, int header)
    {
        HexUnit unit = HexUnit.Load(reader, grid, header);
        Agent agent = unit.GetComponent<Agent>();
        return agent;
    }

    public override bool CanAttack(Unit unit)
    {
        if (unit.GetPlayer() && unit.GetPlayer() != GetPlayer())
        {
            return true;
        }
        return false;
    }
}
