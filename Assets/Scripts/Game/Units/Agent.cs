using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Agent : Unit {

    AgentConfig agentConfig;
    Player player;
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


    public AgentConfig GetAgentConfig()
    {
        return agentConfig;
    }

    private void Setup()
    {
        if (player)
        {
            unitUI.SetColour(player.Color);
        }
    }

    public void SetPlayer(Player player)
    {
        if (player)
        {
            UpdateOwnerVisiblity(HexUnit.Location, false);
        }
        this.player = player;

        UpdateOwnerVisiblity(HexUnit.Location, true);
        if (unitUI)
        {
            unitUI.SetColour(player.Color);
        }

    }
    public override Player GetPlayer()
    {
        return player;
    }
    public override void UpdateOwnerVisiblity(HexCell hexCell, bool increase)
    {
        if (player)
        {
            List<HexCell> cells = hexGrid.GetVisibleCells(hexCell, HexUnit.VisionRange);
            for (int i = 0; i < cells.Count; i++)
            {
                if (increase)
                {
                    player.AddVisibleCell(cells[i]);
                }
                else
                {
                    player.RemoveVisibleCell(cells[i]);
                }
            }
            ListPool<HexCell>.Add(cells);

        }
    }

    public override bool CanAttack(Unit unit)
    {
        if (unit.HexUnit.HexUnitType == HexUnit.UnitType.AGENT && unit.GetComponent<Agent>().GetPlayer() != GetPlayer())
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
