using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Agent : Unit {

    public override bool CanAttack(Unit unit)
    {
        if (unit.GetPlayer() && unit.GetPlayer() != GetPlayer())
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

    public static Agent Load(BinaryReader reader, HexGrid grid, int header)
    {
        HexUnit unit = HexUnit.Load(reader, grid, header);
        unit.HexUnitType = HexUnit.UnitType.AGENT;
        Agent agent = unit.GetComponent<Agent>();
        if (header >= 3)
        {
            agent.HitPoints = reader.ReadInt32();
            agent.SetMovementLeft(reader.ReadInt32());
        }

        return agent;
    }

}
