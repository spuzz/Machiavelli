using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class City : MonoBehaviour {

    // External Components
    [SerializeField] HexGrid hexGrid;
    GameController gameController;
    Player player;
    CityState cityStateOwner;
    HexCell hexCell;

    // Internal Components
    [SerializeField] CityUI cityUI;
    [SerializeField] CityResouceController cityResouceController;
    HexVision hexVision;

    // Attributes
    [SerializeField] int visionRange = 2;
    [SerializeField] int baseHitPoints = 200;
    [SerializeField] int baseStrength = 25;
    public static int cityIDCounter = 1;
    int cityID;

    [SerializeField] List<CityBuilding> buildings;
    //[SerializeField] List<BuildConfig> buildingOptions;
    //[SerializeField] List<BuildConfig> trainingOptions;

    int maintenance = 0;
    int population = 1;
    int food = 0;
    bool alive;

    BuildingManager buildingManager = new BuildingManager();
    List<HexCell> ownedCells = new List<HexCell>();
    List<HexCell> workedCells = new List<HexCell>();
    List<CombatUnit> cityUnits = new List<CombatUnit>();

    public delegate void OnInfoChange(City city);
    public event OnInfoChange onInfoChange;

    public float HealthAsPercentage
    {
        get { return (float)hitPoints / (float)BaseHitPoints; }
    }

    int hitPoints;
    public int HitPoints
    {
        get
        {
            return hitPoints;
        }

        set
        {
            hitPoints = value;
            NotifyInfoChange();
        }
    }

    public BuildingManager BuildingManager
    {
        get
        {
            return buildingManager;
        }

        set
        {
            buildingManager = value;
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

    public CityUI CityUI
    {
        get
        {
            return cityUI;
        }

        set
        {
            cityUI = value;
        }
    }

    public int BaseHitPoints
    {
        get
        {
            return baseHitPoints;
        }

        set
        {
            baseHitPoints = value;
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
                player.onInfoChange -= PlayerUpdated;
            }
            player = value;

            if(player)
            {
                player.onInfoChange += PlayerUpdated;
            }
            UpdateCityBar();
            foreach (CombatUnit unit in cityUnits)
            {
                unit.UpdateUI(0);
            }
            cityStateOwner.Player = player;
            UpdateVision();
            NotifyInfoChange();
        }
    }

    private void PlayerUpdated(Player player)
    {
        NotifyInfoChange();
    }

    public void UpdateVision()
    {
        if (!Player)
        {
            SetVision(false);
            return;
        }

        if (player.IsHuman)
        {
            SetVision(true);
        }
        else
        {
            SetVision(false);
        }
    }

    private void SetVision(bool vision)
    {

        HexVision.HasVision = vision;
        foreach (CombatUnit unit in cityUnits)
        {
            unit.HexVision.HasVision = vision;
        }

    }

    public int CityID
    {
        get
        {
            return cityID;
        }

        set
        {
            cityID = value;
        }
    }


    public int Strength
    {

        get
        {
            return baseStrength;
        }

    }

    public bool Alive
    {
        get
        {
            return alive;
        }

        set
        {
            alive = value;
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

    public CityResouceController CityResouceController
    {
        get
        {
            return cityResouceController;
        }

        set
        {
            cityResouceController = value;
        }
    }

    public int Population
    {
        get
        {
            return population;
        }

        set
        {
            population = value;
        }
    }

    public int Food
    {
        get
        {
            return food;
        }

        set
        {
            food = value;
        }
    }

    //public IEnumerable<BuildConfig> GetBuildingOptions()
    //{
    //    return buildingOptions;
    //}
    //public IEnumerable<BuildConfig> GetTrainingOptions()
    //{
    //    return trainingOptions;
    //}

    public void RemoveUnit(CombatUnit unit)
    {
        cityUnits.Remove(unit);
        NotifyInfoChange();
    }

    public void AddUnit(CombatUnit unit)
    {
        if (player && player.IsHuman)
        {
            unit.HexVision.HasVision = true;
        }
        cityUnits.Add(unit);
        if(Player)
        {
            unit.SetPlayer(Player);
        }

        unit.UnitUI.SetCityStateSymbol(gameController.GetCityStateSymbol(cityStateOwner.SymbolID));
        NotifyInfoChange();
    }

    public IEnumerable<CombatUnit> GetUnits()
    {
        return cityUnits;
    }

    public void SetCityState(CityState cityState)
    {
        cityStateOwner = cityState;
        UpdateCityBar();
        NotifyInfoChange();

    }

    public CityState GetCityState()
    {
        return cityStateOwner;
    }

    public void SetHexCell(HexCell cell)
    {
        hexCell = cell;
        hexCell.Walled = false;
        hexCell.City = this;
        ownedCells.Clear();
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbour = hexCell.GetNeighbor(d);
            if (neighbour)
            {
                ownedCells.Add(neighbour);
            }

        }
        UpdateWalls();
        HexVision.SetCells(PathFindingUtilities.GetCellsInRange(hexCell, VisionRange));
        NotifyInfoChange();
    }


    public HexCell GetHexCell()
    {
        return hexCell;
    }


    public IEnumerable<HexCell> GetOwnedCells()
    {
        return ownedCells;
    }

    public int UnassignedPopulation()
    {
        return Population - (workedCells.Count - 1);
    }
    public IEnumerable<HexCell> GetWorkedCells()
    {
        return workedCells;
    }

    public void RemoveWorkedCell(HexCell cell)
    {
        if(workedCells.Contains(cell))
        {
            workedCells.Remove(cell);
        }
        NotifyInfoChange();
    }

    public void AddWorkedCell(HexCell cell)
    {
        if (!workedCells.Contains(cell))
        {
            workedCells.Add(cell);
        }
        NotifyInfoChange();
    }

    public bool CanWorkAnotherCell()
    {
        if(workedCells.Count() <= Population)
        {
            return true;
        }
        return false;
    }

    private int GetMaintenance()
    {
        int adjustedMaintenance = maintenance;
        return adjustedMaintenance;
    }

    private int GetIncome()
    {
        int adjustedGold = CityResouceController.GetGold();
        return adjustedGold;
    }

    public void IncreasePopulation()
    {
        Population += 1;
        AdjustWorkedCells();
    }

    public void DecreasePopulation()
    {
        if(Population < 1)
        {
            throw new InvalidOperationException("Population already at 1!");
        }
        Population -= 1;
        AdjustWorkedCells();
    }

    private void AdjustWorkedCells()
    {
        foreach(HexCell cell in workedCells)
        {
            cell.GetComponent<HexCellGameData>().IsWorked = false;
        }
        workedCells.Clear();
        hexCell.HexCellGameData.IsWorked = true;
        workedCells.Add(hexCell);
        for(int pop = 0; pop < Population; pop++)
        {
            if(ownedCells.Count - workedCells.Count > 0)
            {
                HexCell cellToWork = PickCellToWork();
                workedCells.Add(cellToWork);
                cellToWork.GetComponent<HexCellGameData>().IsWorked = true;
            }

        }
        NotifyInfoChange();
    }

    private HexCell PickCellToWork()
    {
        List<HexCell> cellsInOrder = ownedCells.Where(c => !workedCells.Contains(c)).ToList();
        cellsInOrder = cellsInOrder.OrderByDescending(c => c.GetComponent<HexCellGameData>().GetTotalYield()).ToList();
        return cellsInOrder[0];
        
    }

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        cityStateOwner = GetComponent<CityState>();
        hexGrid = FindObjectOfType<HexGrid>();
        hexVision = gameObject.AddComponent<HexVision>();
        hitPoints = BaseHitPoints;
        gameController.VisionSystem.AddHexVision(hexVision);
        cityID = cityIDCounter;
        cityIDCounter++;

    }

    public void Start()
    {
        AdjustWorkedCells();
    }

    public void StartTurn()
    {
        BuildingManager.DayPassed(CityResouceController.GetProduction());
        BuildConfig buildConfig = BuildingManager.GetCompletedBuild();
        while (buildConfig)
        {
            if (buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.COMBAT_UNIT)
            {
                CreateUnit((buildConfig as CombatUnitBuildConfig).CombatUnitConfig);
            }
            else if (buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.BUILDING)
            {
                CreateBuilding(buildConfig as CityPlayerBuildConfig);
            }
            else if(buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.AGENT)
            {
                CreateAgent((buildConfig as AgentBuildConfig).AgentConfig);
            }
            buildConfig = BuildingManager.GetCompletedBuild();
        }
        foreach(Unit unit in cityUnits)
        {
            unit.StartTurn();
        }
        Player.Gold += GetIncome();
        Player.Gold -= GetMaintenance();
        Food += CityResouceController.GetFood();
        if(Food >= GameConsts.populationFoodReqirements[Population] && Population < GameConsts.populationFoodReqirements.Count())
        {
            Food -= GameConsts.populationFoodReqirements[Population];
            IncreasePopulation();
        }

        if (Food <= 0 && Population > 1)
        {
            DecreasePopulation();
            Food = GameConsts.populationFoodReqirements[Population] / 2;
            
        }
        NotifyInfoChange();
    }

    public void TakeTurn()
    {

    }

    public void UpdateHealthBar()
    {

        CityUI.UpdateHealthBar();
    }

    public void UpdateCityBar()
    {
        if (Player)
        {
            CityUI.SetPlayerColour(Player.GetColour().Colour);
            hexCell.PlayerColour = Player.GetColour();
        }
        else
        {
            CityUI.SetPlayerColour(Color.black);
            hexCell.PlayerColour = null;
        }

        cityUI.CityStateSymbol.sprite = gameController.GetCityStateSymbol(cityStateOwner.SymbolID);

    }


    private void UpdateWalls()
    {
        foreach(HexCell cell in ownedCells)
        {
            cell.Walled = false;
        }
    }

    public void DestroyCity()
    {
        foreach (HexCell cell in ownedCells)
        {
            cell.Walled = false;
        }
        hexCell.Walled = false;
        Destroy(gameObject);
        
    }


    public void AddBuild(BuildConfig config)
    {
        buildingManager.AddBuild(config);
        NotifyInfoChange();
    }

    public void RemoveBuild(int queueNumber)
    {

        BuildConfig config = BuildingManager.RemoveFromQueue(queueNumber);
        NotifyInfoChange();
    }

    public IEnumerable<BuildConfig> GetBuildingOptions()
    {
        List<BuildConfig> configs = new List<BuildConfig>();
        if (Player)
        {
            foreach (BuildConfig config in Player.GetCityPlayerBuildConfigs())
            {
                if (!buildings.Find(C => C.BuildConfig == config))
                {
                    configs.Add(config);
                }
            }
        }
        return configs;
    }

    public IEnumerable<BuildConfig> GetCombatUnitTrainingOptions()
    {
        List<BuildConfig> configs = new List<BuildConfig>();
        if(Player)
        {
            foreach (BuildConfig config in Player.GetCombatUnitBuildConfigs())
            {
                configs.Add(config);
            }
        }

        return configs;
    }
    public IEnumerable<BuildConfig> GetAgentTrainingOptions()
    {
        List<BuildConfig> configs = new List<BuildConfig>();
        if (Player)
        {
            foreach (BuildConfig config in Player.GetAgentBuildConfigs())
            {
                configs.Add(config);
            }
        }
        return configs;
    }

    public IEnumerable<CityBuilding> GetCityBuildings()
    {
        return buildings;
    }

    public bool CreateUnit(CombatUnitConfig combatUnitConfig)
    {
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(hexCell, 2);
        foreach (HexCell cell in cells)
        {
            if (!cell.IsUnderwater && cell.CanUnitMoveToCell(Unit.UnitType.COMBAT))
            {
                HexUnit hexUnit = gameController.CreateCityStateUnit(combatUnitConfig, cell, this);
                hexUnit.Location.UpdateVision();
                UpdateVision();
                NotifyInfoChange();
                return true;
            }
        }
        return false;
    }

    public bool CreateAgent(AgentConfig agentConfig)
    {
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(hexCell, 2);
        foreach (HexCell cell in cells)
        {
            if (cell.CanUnitMoveToCell(Unit.UnitType.AGENT))
            {
                gameController.CreateAgent(agentConfig, cell, Player);
                NotifyInfoChange();
                return true;
            }
        }
        NotifyInfoChange();
        return false;
    }


    public bool CreateBuilding(CityPlayerBuildConfig config)
    {

        CityBuilding building = gameController.CreateCityPlayerBuilding(config);
        buildings.Add(building);
        cityResouceController.AddBuilding(building);
        NotifyInfoChange();
        return true;
    }


    public void Save(BinaryWriter writer)
    {
        GetHexCell().coordinates.Save(writer);
        if(player)
        {
            writer.Write(player.PlayerNumber);
        }
        else
        {
            writer.Write(-1);
        }
        buildingManager.Save(writer);
        writer.Write(cityUnits.Count);
        foreach(CombatUnit unit in cityUnits)
        {
            unit.Save(writer);
        }
        cityStateOwner.Save(writer);
    }

    public static void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        
        int playerNumber = reader.ReadInt32();
        HexCell cell = hexGrid.GetCell(coordinates);
        City city = gameController.CreateCity(cell);
        if (playerNumber != -1)
        {
            gameController.GetPlayer(playerNumber).AddCity(city);
        }
        city.buildingManager.Load(reader,gameController,header);
        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            CombatUnit combatUnit = CombatUnit.Load(reader, gameController, hexGrid, header, city);
        }
        city.GetComponent<CityState>().Load(reader, gameController, hexGrid, header);
        for (int i = 0; i < city.cityUnits.Count; i++)
        {
            city.cityUnits[i].UnitUI.SetCityStateSymbol(gameController.GetCityStateSymbol(city.cityStateOwner.SymbolID));
            city.cityUnits[i].CityStateOwner = city.GetCityState();
        }
        city.UpdateCityBar();

    }
    public void NotifyInfoChange()
    {
        if (onInfoChange != null)
        {
            onInfoChange(this);
        }
    }


}
