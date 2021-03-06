﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;

public class City : MonoBehaviour {

    // External Components
    [SerializeField] HexGrid hexGrid;
    GameController gameController;
    CityState cityStateOwner;
    HexCell hexCell;

    // Internal Components
    [SerializeField] CityUI cityUI;
    [SerializeField] CityResouceController cityResouceController;
    [SerializeField] GameObject selectGlow;
    HexVision hexVision;

    // Attributes
    [SerializeField] int visionRange = 2;
    [SerializeField] int baseHitPoints = 200;
    [SerializeField] int baseStrength = 25;
    public static int cityIDCounter = 1;
    int cityID;

    [SerializeField] List<CityBuilding> buildings;
    [SerializeField] List<BuildConfig> buildingOptions;
    [SerializeField] List<BuildConfig> trainingOptions;

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
            HexVision.SetCells(PathFindingUtilities.GetCellsInRange(hexCell, visionRange));
            UpdateVision();
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

    public void KillAllUnits()
    {
        foreach (CombatUnit unit in cityUnits)
        {
            gameController.DestroyUnit(unit);
        }
    }

    public void UpdateCity(bool killAllUnits = true)
    {
        UpdateCityBar();
        UpdateVision();
        NotifyInfoChange();
    }


    public void UpdateVision()
    {
        if (!cityStateOwner.Player)
        {
            SetVision(false);
            return;
        }

        if (cityStateOwner.Player.IsHuman)
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

    public List<BuildConfig> BuildingOptions
    {
        get
        {
            return buildingOptions;
        }

        set
        {
            buildingOptions = value;
        }
    }

    public List<BuildConfig> TrainingOptions
    {
        get
        {
            return trainingOptions;
        }

        set
        {
            trainingOptions = value;
        }
    }

    public void AddBuildingOptions(Player player)
    {
        if(player)
        {
            foreach(CityPlayerBuildConfig config in player.GetCityPlayerBuildConfigs())
            {
                buildingOptions.Add(config);
            }
        }
    }
    public void AddTrainingOptions(Player player)
    {
        if (player)
        {
            foreach (CombatUnitBuildConfig config in player.GetCombatUnitBuildConfigs())
            {
                trainingOptions.Add(config);
            }
        }
    }


    public void RemoveUnit(CombatUnit unit)
    {
        cityUnits.Remove(unit);
        NotifyInfoChange();
    }

    public void AddUnit(CombatUnit unit)
    {
        if (cityStateOwner.Player && cityStateOwner.Player.IsHuman)
        {
            unit.HexVision.HasVision = true;
        }
        cityUnits.Add(unit);
        unit.CityStateOwner = GetCityState();
        if(cityStateOwner.Player)
        {
            unit.SetPlayer(cityStateOwner.Player);
        }

        unit.UnitUI.SetCityStateSymbol(gameController.GetCityStateSymbol(cityStateOwner.SymbolID));
        NotifyInfoChange();
    }

    public IEnumerable<CombatUnit> GetUnits()
    {
        return cityUnits;
    }

    public int GetTotalUnits()
    {
        return cityUnits.Count();
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

    private int GetPoliticianMaintenance()
    {
        int adjustedMaintenance = 0;
        adjustedMaintenance += cityStateOwner.GetPoliticianMaintenance();
        return adjustedMaintenance;
    }

    public int GetIncomePerTurn()
    {
        int adjustedGold = CityResouceController.GetGold();
        adjustedGold -= GetMaintenance();
        return adjustedGold;
    }

    public int GetPoliticalCapitalPerTurn()
    {
        int pcPerTurn = CityResouceController.GetPC();
        pcPerTurn -= GetPoliticianMaintenance();
        return pcPerTurn;
    }


    public void IncreasePopulation()
    {
        GetCityState().CreatePolitician();
        Population += 1;
        AdjustWorkedCells();
        UpdateCity();
    }

    public void DecreasePopulation()
    {
        if(Population < 1)
        {
            throw new InvalidOperationException("Population already at 1!");
        }
        Population -= 1;
        AdjustWorkedCells();
        UpdateCity();
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

    public int GetPoliticalCost()
    {
        return GameConsts.populationPoliticalCost[population - 1];
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
        gameObject.transform.position = hexCell.gameObject.transform.position;
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
        Food += CityResouceController.GetFood();
        if(Food >= GameConsts.populationFoodReqirements[Population] && Population < GameConsts.populationFoodReqirements.Count())
        {
            Food -= GameConsts.populationFoodReqirements[Population];
            IncreasePopulation();
        }
        else if (Food <= 0 && Population > 1)
        {
            DecreasePopulation();
            Food = GameConsts.populationFoodReqirements[Population] / 2;
            
        }

        if(cityStateOwner.Player)
        {
            cityStateOwner.AddLoyalty(CityResouceController.EffectsController.TotalEffects.Loyalty, cityStateOwner.Player);
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
        if (cityStateOwner.Player)
        {
            CityUI.SetPlayerColour(cityStateOwner.Player.GetColour().Colour);
            hexCell.PlayerColour = cityStateOwner.Player.GetColour();
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
        foreach(Unit unit in cityUnits)
        {
            gameController.DestroyUnit(unit);
        }
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
        if (cityStateOwner.Player)
        {
            foreach (BuildConfig config in buildingOptions)
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
        return trainingOptions;
    }
    public IEnumerable<BuildConfig> GetAgentTrainingOptions()
    {
        List<BuildConfig> configs = new List<BuildConfig>();
        if (cityStateOwner.Player)
        {
            foreach (BuildConfig config in cityStateOwner.Player.GetAgentBuildConfigs())
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
                gameController.CreateAgent(agentConfig, cell, cityStateOwner.Player);
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

        foreach(CityPlayerBuildConfig buildConfig in building.BuildConfigs())
        {
            buildingOptions.Add(buildConfig);
        }
        foreach (CombatUnitBuildConfig buildConfig in building.UnitConfigs())
        {
            trainingOptions.Add(buildConfig);

        }

        buildings.Add(building);

        cityResouceController.AddBuilding(building);
        if(building.ResourceBenefit.VisionRange > 0)
        {
            VisionRange = cityResouceController.EffectsController.TotalEffects.VisionRange + 1;
        }
        NotifyInfoChange();
        return true;
    }

    public void ResetBuildOptions(Player player)
    {
        trainingOptions.Clear();
        buildingOptions.Clear();
        AddTrainingOptions(player);
        AddBuildingOptions(player);
        foreach(CityBuilding building in buildings)
        {
            foreach (CityPlayerBuildConfig buildConfig in building.BuildConfigs())
            {
                buildingOptions.Add(buildConfig);
            }
            foreach (CombatUnitBuildConfig buildConfig in building.UnitConfigs())
            {
                trainingOptions.Add(buildConfig);

            }
        }
    }

    public void DamageCity(int defenceDamage)
    {
        int hitpointsLeft = HitPoints - defenceDamage;
        if (hitpointsLeft < 0)
        {
            hitpointsLeft = 0;
        }
        HitPoints = hitpointsLeft;
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            FindObjectOfType<HexGameUI>().SelectCity(this);
        }
    }

    public void Select()
    {
        selectGlow.SetActive(true);
    }

    public void Deselect()
    {
        selectGlow.SetActive(false);
    }

    public void Save(BinaryWriter writer)
    {
        GetHexCell().coordinates.Save(writer);
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
        int playerNumber = -1;
        if (header < 2)
        {
            playerNumber = reader.ReadInt32();
        }

        HexCell cell = hexGrid.GetCell(coordinates);
        City city = gameController.CreateCity(cell);
        city.buildingManager.Load(reader,gameController,header);
        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            CombatUnit combatUnit = CombatUnit.Load(reader, gameController, hexGrid, header, city);
        }
        city.GetComponent<CityState>().Load(reader, gameController, hexGrid, header);
        if (header < 2)
        {
            if (playerNumber != -1)
            {
                gameController.GetPlayer(playerNumber).AddCity(city);
            }

        }
        for (int i = 0; i < city.cityUnits.Count; i++)
        {
            city.cityUnits[i].UnitUI.SetCityStateSymbol(gameController.GetCityStateSymbol(city.cityStateOwner.SymbolID));
            city.cityUnits[i].CityStateOwner = city.GetCityState();
        }

        city.AddTrainingOptions(city.GetCityState().Player);
        city.AddBuildingOptions(city.GetCityState().Player);
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
