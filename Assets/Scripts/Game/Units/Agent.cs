using NPBehave;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Agent : Unit {

    public enum Stance
    {
        UNASSIGNED,
        EXPLORE,
        USEABILITY
    }
    Stance currentStance = Stance.UNASSIGNED;

    AgentConfig agentConfig;
    Player player;

    int energy = 100;
    [SerializeField] int energyRegen = 5;
    public Stance CurrentStance
    {
        get
        {
            return currentStance;
        }

        set
        {
            currentStance = value;
        }
    }

    public int Energy
    {
        get
        {
            return energy;
        }

        set
        {
            energy = value;
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();
        energy += energyRegen;
        if(energy > 100)
        {
            energy = 100;
        }
        unitUI.SetEnergy(energy);
    }
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
            unitUI.SetColour(player.GetColour().Colour);
            HexUnit.MaterialColourChanger.ChangeMaterial(player.GetColour());
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
            unitUI.SetColour(player.GetColour().Colour);
        }
        MaterialColourChanger changer = HexUnit.MaterialColourChanger;
        changer.ChangeMaterial(player.GetColour());


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

    public void UpdateEnergy(int energyChange)
    {
        unitUI.UpdateEnergyBar(energyChange);
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
        int hitPoints = 100;
        int movementLeft = 2;
        string unitName = reader.ReadString();
        if (header >= 3)
        {
            hitPoints = reader.ReadInt32();
            movementLeft = reader.ReadInt32();
        }
        string agentConfig = "Builder";
        if (header >= 4)
        {
            agentConfig = reader.ReadString();
        }
        HexUnit unit = gameController.CreateAgent(agentConfig,grid.GetCell(coordinates), player);

        Agent agent = unit.GetComponent<Agent>();

        if (header >= 3)
        {
            agent.HitPoints = hitPoints;
            agent.SetMovementLeft(movementLeft);
        }

        return agent;
    }

}
