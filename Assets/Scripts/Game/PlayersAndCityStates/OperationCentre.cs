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
    List<OpCentreAgentBuildMod> buildMods = new List<OpCentreAgentBuildMod>();
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

    public IEnumerable<OpCentreBuilding> GetBuildings()
    {
        return buildings;
    }


    public bool buildingSpaceAvailable()
    {
        foreach(OpCentreBuilding building in buildings)
        {
            if(!building)
            {
                return true;
            }
        }
        return false;
    }

    private int GetFreeBuildSlot()
    {
        int count = 0;
        foreach(OpCentreBuilding building in buildings)
        {
            if(!building)
            {
                return count;
            }
            count++;
        }
        return -1;
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
            Location.PlayerColour = player.GetColour();
            OpCentreUI.SetPlayerColour(player.GetColour().Colour);
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

    public BuildingManager BuildingManagerForAgents
    {
        get
        {
            return buildingManagerForAgents;
        }

        set
        {
            buildingManagerForAgents = value;
        }
    }

    public BuildingManager BuildingManagerForBuildings
    {
        get
        {
            return buildingManagerForBuildings;
        }

        set
        {
            buildingManagerForBuildings = value;
        }
    }

    public void UpdateLocation(HexCell loc)
    {
        location = loc;
        location.OpCentre = this;
    }

    public void UpdateVision()
    {
        HexVision.SetCells(PathFindingUtilities.GetCellsInRange(Location, VisionRange));
    }

    public IEnumerable<AgentBuildConfig> GetAgentBuildConfigs(List<string> excluded = null)
    {
        if(excluded == null)
        {
            excluded = new List<string>();
        }
        return agentBuildConfigs.FindAll(c => !excluded.Contains(c.Name));
    }

    public AgentBuildConfig GetAgentBuildConfigs(string name)
    {
        return agentBuildConfigs.Find(c => c.Name == name);
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
            availableBuilds.Add(opCentreBuildConfig);
        }
        NotifyInfoChange();
    }

    public void AddBuildMod(OpCentreAgentBuildModConfig buildModConfig)
    {
        OpCentreAgentBuildMod currentMod = buildMods.Find(c => c.AgentBuildConfig == buildModConfig.AgentBuildConfig);
        if (currentMod != null)
        {
            currentMod.AddMod(buildModConfig);
        }
        else
        {
            buildMods.Add(new OpCentreAgentBuildMod(buildModConfig));
        }
        if(buildModConfig.CapIncrease > 0)
        {
            player.PlayerAgentTracker.IncreaseCap(buildModConfig.AgentBuildConfig.AgentConfig,buildModConfig.CapIncrease);
        }
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
        BuildingManagerForBuildings.DayPassed(1);
        BuildConfig buildConfig = BuildingManagerForBuildings.GetCompletedBuild();
        while (buildConfig)
        {
            if (buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.OPCENTRE_BUILDING)
            {
                AddBuilding(buildConfig as OpCentreBuildConfig,BuildingManagerForBuildings.GetIDofLastCompletedBuild());
            }
            buildConfig = BuildingManagerForBuildings.GetCompletedBuild();
        }
        BuildingManagerForAgents.DayPassed(1);
        buildConfig = BuildingManagerForAgents.GetCompletedBuild();
        while (buildConfig)
        {
            if (buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.AGENT)
            {
                CreateAgent((buildConfig as AgentBuildConfig).AgentConfig);
            }
            buildConfig = BuildingManagerForAgents.GetCompletedBuild();
        }

        NotifyInfoChange();
    }

    private void AddBuilding(OpCentreBuildConfig opCentreBuildConfig, int slotID)
    {
        buildings[slotID] = gameController.CreateOpCentreBuilding(opCentreBuildConfig);
        buildings[slotID].OpCentreBuiltIn = this;
        buildings[slotID].AddConfigs();
    }

    public bool HireAgent(AgentBuildConfig agentBuildConfig)
    {
        int cost = GetAgentCost(agentBuildConfig);
        if (cost <= player.Gold && player.CanHireAgent(agentBuildConfig.AgentConfig))
        {
            player.Gold -= cost;
            BuildingManagerForAgents.AddBuild(agentBuildConfig);
            player.UseAgentSpace(agentBuildConfig.AgentConfig);
            NotifyInfoChange();
            return true;
        }
        return false;
    }

    public int GetAgentCost(AgentBuildConfig agentBuildConfig)
    {
        OpCentreAgentBuildMod mod = buildMods.Find(c => c.AgentBuildConfig == agentBuildConfig);
        if(mod != null)
        {
            return agentBuildConfig.BasePurchaseCost - (int)((float)agentBuildConfig.BasePurchaseCost / 100.0f * (float)mod.CostPerc);
        }
        else
        {
            return agentBuildConfig.BasePurchaseCost;
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
    public bool HireMercenary(CombatUnitBuildConfig mercBuildConfig)
    {
        if (mercBuildConfig.BasePurchaseCost < player.Gold)
        {
            player.Gold -= mercBuildConfig.BasePurchaseCost;
            CreateMercenary(mercBuildConfig.CombatUnitConfig);
            return true;
        }
        return false;
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
    }

    public void BuildAvailableBuilding(int listID, int slotID)
    {  
        if(Player.Gold >= availableBuilds[listID].BasePurchaseCost)
        {
            Player.Gold -= availableBuilds[listID].BasePurchaseCost;
            BuildingManagerForBuildings.AddBuild(availableBuilds[listID],slotID);
            availableBuilds.Remove(availableBuilds[listID]);
            NotifyInfoChange();
        }

    }

    public bool BuildUsingBuildConfig(BuildConfig buildConfig)
    {
        bool result = false;
        if (Player.Gold >= buildConfig.BasePurchaseCost)
        {
            if (availableBuilds.FindAll(c => c.Name == buildConfig.Name).Count > 0)
            {
                int slot = GetFreeBuildSlot();
                if(slot != -1)
                {
                    Player.Gold -= buildConfig.BasePurchaseCost;
                    BuildingManagerForBuildings.AddBuild(buildConfig, slot);
                    result = true;
                }

            }

            if(agentBuildConfigs.FindAll(c => c.Name == buildConfig.Name).Count > 0)
            {
                result = HireAgent((buildConfig as AgentBuildConfig));
            }

            if (mercBuildConfigs.FindAll(c => c.Name == buildConfig.Name).Count > 0)
            {
                result = HireMercenary((buildConfig as CombatUnitBuildConfig));
            }
            
            if(result == true)
            {
                NotifyInfoChange();
            }

        }
        return result;
    }



    public bool IsConstructingBuilding(int slot)
    {
        if (BuildingManagerForBuildings.GetConfigInQueueByID(slot))
        {
            return true;
        }
        return false;
    }

    public bool IsConstructingBuilding()
    {
        if (BuildingManagerForBuildings.buildsInQueue() != 0)
        {
            return true;
        }
        return false;
    }

    public bool IsTraining()
    {
        if (BuildingManagerForAgents.buildsInQueue() != 0)
        {
            return true;
        }
        return false;
    }


    public int TimeLeftOnConstruction(int slot)
    {
        if (BuildingManagerForBuildings.IDInConstruction() == slot)
        {
            return BuildingManagerForBuildings.TimeLeftOnBuild(1);
        }
        if (BuildingManagerForBuildings.IsIDInQueue(slot))
        {
            return BuildingManagerForBuildings.GetConfigInQueueByID(slot).BaseBuildTime;
        }
        return -1;
    }

    public void DestroyOperationCentre()
    {
        Destroy(opCentreUI.gameObject);
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
        BuildingManagerForBuildings.Save(writer);
        BuildingManagerForAgents.Save(writer);
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

        opCentre.BuildingManagerForBuildings.Load(reader,gameController,header);
        opCentre.BuildingManagerForAgents.Load(reader, gameController, header);

    }

}
