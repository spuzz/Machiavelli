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

    [SerializeField] string agentName;
    [SerializeField] int level = 1;
    [SerializeField] int unspentPoints = 0;
    [SerializeField] int maintenance = 0;
    [SerializeField] int experience = 0;

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

    public int Level
    {
        get
        {
            return level;
        }

        set
        {
            level = value;
        }
    }

    public int UnspentPoints
    {
        get
        {
            return unspentPoints;
        }

        set
        {
            unspentPoints = value;
        }
    }

    public int Maintenance
    {
        get
        {
            return maintenance;
        }

        set
        {
            maintenance = value;
        }
    }

    public string AgentName
    {
        get
        {
            return agentName;
        }

        set
        {
            agentName = value;
        }
    }

    public int Experience
    {
        get
        {
            return experience;
        }

        set
        {
            experience = value;
        }
    }

    public override void StartTurn()
    {
        if(gameController.GetTurn() != 1)
        {
            AddExperience(100);
        }

        base.StartTurn();
    }

    public void SetAgentConfig(AgentConfig config)
    {
        agentConfig = config;
        HexUnit.SetMeshChild(Instantiate(config.MeshChild, gameObject.transform).transform);
        HexVision.AddVisibleObject(HexUnit.GetMesh());
        BaseMovement = config.BaseMovement;
        BaseStrength = config.BaseStrength;
        UnitUI.SetUnitSymbol(config.Symbol);
        HexUnit.VisionRange = config.VisionRange;
        
        foreach (AbilityConfig abilityConfig in config.GetAbilityConfigs())
        {
            abilities.AbilitiesList.Add(abilityConfig);
        }
        //HexUnit.OffSet = new Vector3(0, 0, -4);
    }

    public AgentConfig GetAgentConfig()
    {
        return agentConfig;
    }

    private void Setup()
    {
        if (player)
        {
            UnitUI.SetColour(player.GetColour().Colour);
            HexUnit.MaterialColourChanger.ChangeMaterial(player.GetColour());
        }

    }

    public void SetLevel(int level)
    {
        this.Level = level;
        experience = 0;
    }
    
    public void AddExperience(int exp)
    {
        experience += exp;
        if(experience >= GameConsts.agentLevelXP)
        {
            AddLevel(1);
            experience = experience - GameConsts.agentLevelXP;
        }
    }

    public void AddLevel(int levels)
    {
        Level += levels;
        UnspentPoints += GameConsts.pointsPerLevel *levels;
    }

    public void SetPlayer(Player player)
    {
        if (player)
        {
            UpdateOwnerVisiblity(HexUnit.Location, false);
        }
        this.player = player;

        UpdateOwnerVisiblity(HexUnit.Location, true);
        if (UnitUI)
        {
            UnitUI.SetColour(player.GetColour().Colour);
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
        if (unit.HexUnitType == Unit.UnitType.AGENT && unit.GetComponent<Agent>().GetPlayer() != GetPlayer())
        {
            return true;
        }

        return false;

    }


    public override Vector3 GetPositionInCell(HexCell cell)
    {
        HexUnit combatUnit = cell.combatUnit;
        if (combatUnit)
        {
            transform.rotation = combatUnit.transform.rotation;
            HexUnit.OffSet = combatUnit.transform.rotation * GameConsts.agentBaseOffset;
            return combatUnit.transform.localPosition + HexUnit.OffSet;
        }
        else
        {
            return cell.Position;
        }
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
        string agentConfig = "Builder";

        hitPoints = reader.ReadInt32();
        movementLeft = reader.ReadInt32();
        agentConfig = reader.ReadString();

        HexUnit unit = gameController.CreateAgent(agentConfig,grid.GetCell(coordinates), player);
        Agent agent = unit.GetComponent<Agent>();
        agent.HitPoints = hitPoints;
        agent.SetMovementLeft(movementLeft);

        return agent;
    }

}
