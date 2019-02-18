using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Agent : Unit {

    AgentConfig agentConfig;

    public void SetAgentConfig(AgentConfig config)
    {
        agentConfig = config;
        HexUnit.SetMeshChild(Instantiate(config.MeshChild, gameObject.transform).transform);
        HexVision.AddVisibleObject(HexUnit.GetMesh());
        BaseMovement = config.BaseMovement;
        BaseStrength = config.BaseStrength;
        Symbol = config.Symbol;
        foreach(AbilityConfig abilityConfig in config.GetAbilityConfigs())
        {
            abilities.AbilitiesList.Add(abilityConfig);
        }
        
    }
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
        writer.Write(agentConfig.Name);
    }


    public static Agent Load(BinaryReader reader, GameController gameController, HexGrid grid, int header, Player player)
    {
        
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        float orientation = reader.ReadSingle();
        string unitName = reader.ReadString();
        string agentConfig = "Builder";
        if (header >= 4)
        {
            agentConfig = reader.ReadString();
        }
        HexUnit unit = gameController.CreateAgent(agentConfig,grid.GetCell(coordinates), player);

        Agent agent = unit.GetComponent<Agent>();

        if (header >= 3)
        {
            agent.HitPoints = reader.ReadInt32();
            agent.SetMovementLeft(reader.ReadInt32());
        }

        return agent;
    }

}
