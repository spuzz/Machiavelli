using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OperationCentre : MonoBehaviour
{
    
    [SerializeField] HexGrid hexGrid;

    [SerializeField] List<AgentBuildConfig> agentBuildConfigs = new List<AgentBuildConfig>();
    [SerializeField] List<CombatUnitBuildConfig> mercBuildConfigs = new List<CombatUnitBuildConfig>();
    [SerializeField] List<OpCentreBuildConfig> opCentreBuildConfigs = new List<OpCentreBuildConfig>();
    [SerializeField] int visionRange = 2;
    [SerializeField] OpCentreUI opCentreUI;
    [SerializeField] OpCentreBuildConfig commandCentreBuildConfig;

    GameController gameController;
    Player player;
    HexCell location;
    int influence = 2;
    int influenceRange = 2;
    HexVision hexVision;
    BuildingManager buildingManagerForBuildings = new BuildingManager();
    BuildingManager buildingManagerForAgents = new BuildingManager();

    public delegate void OnInfoChange(OperationCentre opCentre);
    public event OnInfoChange onInfoChange;

    OpCentreBuilding commandCentre;
    OpCentreBuilding[] buildings = new OpCentreBuilding[5];

    public List<OpCentreBuildConfig> availableBuilds = new List<OpCentreBuildConfig>();
    public OpCentreBuilding GetBuilding(int id)
    {
        if (id > 4 || id < 0)
        {
            return null;
        }
        else
        {
            return buildings[id];
        }
    }
    public HexCell Location
    {
        get
        {
            return location;
        }

        set
        {
            location = value;
            location.OpCentre = this;
            HexVision.SetCells(hexGrid.GetVisibleCells(Location, VisionRange));
        }
    }

    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            if(player)
            {
                player.onInfoChange -= OnPlayerInfoChange;
            }
            player = value;
            transform.parent = player.operationCenterTransformParent.transform;
            Location.CellSecondColor = player.Color;
            OpCentreUI.SetPlayerColour(player.Color);
            player.onInfoChange += OnPlayerInfoChange;

        }
    }

    public HexVision HexVision
    {
        get
        {
            return hexVision;
        }

        set
        {
            hexVision = value;
        }
    }

    public int VisionRange
    {
        get
        {
            return visionRange;
        }

        set
        {
            visionRange = value;
        }
    }

    public OpCentreBuilding CommandCentre
    {
        get
        {
            return commandCentre;
        }

        set
        {
            commandCentre = value;
        }
    }

    public OpCentreUI OpCentreUI
    {
        get
        {
            return opCentreUI;
        }

        set
        {
            opCentreUI = value;
        }
    }

    public IEnumerable<AgentBuildConfig> GetAgentBuildConfigs()
    {
        return agentBuildConfigs;
    }

    public IEnumerable<CombatUnitBuildConfig> GetCombatUnitBuildConfigs()
    {
        return mercBuildConfigs;
    }
    public IEnumerable<OpCentreBuildConfig> GetOpCentreBuildConfigs()
    {
        return opCentreBuildConfigs;
    }


    public void AddAgentBuildConfigs(AgentBuildConfig agentBuildConfig)
    {
        if(!agentBuildConfigs.Contains(agentBuildConfig))
        {
            agentBuildConfigs.Add(agentBuildConfig);
        }
        NotifyInfoChange();
    }

    public void AddCombatUnitBuildConfigs(CombatUnitBuildConfig combatUnitBuildConfig)
    {
        if (!mercBuildConfigs.Contains(combatUnitBuildConfig))
        {
            mercBuildConfigs.Add(combatUnitBuildConfig);
        }
        NotifyInfoChange();
    }
    public void AddOpCentreBuildConfigs(OpCentreBuildConfig opCentreBuildConfig)
    {
        if (!opCentreBuildConfigs.Contains(opCentreBuildConfig))
        {
            opCentreBuildConfigs.Add(opCentreBuildConfig);
        }
        NotifyInfoChange();
    }

    private void Awake()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        gameController = FindObjectOfType<GameController>();
        hexVision = gameObject.AddComponent<HexVision>();
        gameController.VisionSystem.AddHexVision(hexVision);
        foreach(OpCentreBuildConfig config in opCentreBuildConfigs)
        {
            availableBuilds.Add(config);
        }

    }

    public void StartTurn()
    {
        buildingManagerForBuildings.DayPassed(1);
        BuildConfig buildConfig = buildingManagerForBuildings.GetCompletedBuild();
        while (buildConfig)
        {
            if (buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.OPCENTRE_BUILDING)
            {
                AddBuilding(buildConfig as OpCentreBuildConfig,buildingManagerForBuildings.GetIDofLastCompletedBuild());
            }
            buildConfig = buildingManagerForBuildings.GetCompletedBuild();
        }
        buildingManagerForAgents.DayPassed(1);
        buildConfig = buildingManagerForAgents.GetCompletedBuild();
        while (buildConfig)
        {
            if (buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.AGENT)
            {
                HireAgent((buildConfig as AgentBuildConfig));
            }
            buildConfig = buildingManagerForAgents.GetCompletedBuild();
        }

        foreach (HexCell cityCell in PathFindingUtilities.GetCellsInRange(Location, influenceRange).FindAll(c => c.City))
        {
            cityCell.City.GetCityState().AdjustInfluence(Player, influence);
        }
        NotifyInfoChange();
    }

    private void AddBuilding(OpCentreBuildConfig opCentreBuildConfig, int slotID)
    {
        buildings[slotID] = gameController.CreateOpCentreBuilding(opCentreBuildConfig);
        buildings[slotID].OpCentreBuiltIn = this;
        buildings[slotID].AddConfigs();
    }

    public void HireAgent(AgentBuildConfig agentBuildConfig)
    {
        if(agentBuildConfig.BasePurchaseCost <= player.Gold)
        {
            player.Gold -= agentBuildConfig.BasePurchaseCost;
            CreateAgent(agentBuildConfig.AgentConfig);
        }
    }


    public bool CreateAgent(AgentConfig agentConfig)
    {
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(Location, 2);
        foreach (HexCell cell in cells)
        {
            if (cell.CanUnitMoveToCell(HexUnit.UnitType.AGENT))
            {
                gameController.CreateAgent(agentConfig,cell, Player);
                return true;
            }
        }
        return false;
    }
    public void HireMercenary(CombatUnitBuildConfig mercBuildConfig)
    {
        if (mercBuildConfig.BasePurchaseCost < player.Gold)
        {
            player.Gold -= mercBuildConfig.BasePurchaseCost;
            CreateMercenary(mercBuildConfig.CombatUnitConfig);
        }
    }

    public bool CreateMercenary(CombatUnitConfig mercConfig)
    {
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(Location, 2);
        foreach (HexCell cell in cells)
        {
            if (cell.CanUnitMoveToCell(HexUnit.UnitType.AGENT))
            {
                gameController.CreateMercenary(mercConfig, cell, Player);
                return true;
            }
        }
        return false;
    }

    public void BuildCommandCentre()
    {
        commandCentre = gameController.CreateOpCentreBuilding(commandCentreBuildConfig);
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(Location, 2);
        foreach(HexCell cell in cells)
        {
            if(cell.City)
            {
                cell.City.PlayerBuildingControl.AddOutpost(player);
            }
        }
    }

    public void BuildAvailableBuilding(int listID, int slotID)
    {  
        if(Player.Gold >= availableBuilds[listID].BasePurchaseCost)
        {
            Player.Gold -= availableBuilds[listID].BasePurchaseCost;
            buildingManagerForBuildings.AddBuild(availableBuilds[listID],slotID);
            NotifyInfoChange();
        }

    }

    public bool IsConstructingBuilding(int slot)
    {
        if (buildingManagerForBuildings.GetConfigInQueueByID(slot))
        {
            return true;
        }
        return false;
    }

    public int TimeLeftOnConstruction(int slot)
    {
        if (buildingManagerForBuildings.IDInConstruction() == slot)
        {
            return buildingManagerForBuildings.TimeLeftOnBuild(1);
        }
        if (buildingManagerForBuildings.IsIDInQueue(slot))
        {
            return buildingManagerForBuildings.GetConfigInQueueByID(slot).BaseBuildTime;
        }
        return -1;
    }

    public void DestroyOperationCentre()
    {
        Destroy(gameObject);
    }

    public void OnPlayerInfoChange(Player player)
    {
        NotifyInfoChange();
    }

    public void NotifyInfoChange()
    {
        if (onInfoChange != null)
        {
            onInfoChange(this);
        }
    }

    public void Save(BinaryWriter writer)
    {
        Location.coordinates.Save(writer);

        foreach(OpCentreBuilding building in buildings)
        {
            if(building)
            {
                writer.Write(building.BuildConfig.Name);
            }
            else
            {
                writer.Write("No Building");
            }

        }
        buildingManagerForBuildings.Save(writer);
        buildingManagerForAgents.Save(writer);
    }

    public static void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, Player player, int header)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        HexCell cell = hexGrid.GetCell(coordinates);
        OperationCentre opCentre = gameController.CreateOperationCentre(cell, player);
        for(int a=0;a<5; a++)
        {
            BuildConfig config = gameController.GetBuildConfig(reader.ReadString());
            if(config)
            {
                opCentre.AddBuilding(config as OpCentreBuildConfig,a);
            }
            
        }

        opCentre.buildingManagerForBuildings.Load(reader,gameController,header);
        opCentre.buildingManagerForAgents.Load(reader, gameController, header);

    }

}
