using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class City : MonoBehaviour {

    [SerializeField] int baseHitPoints = 200;
    [SerializeField] int baseStrength = 25;
    [SerializeField] int baseProduction = 0;
    [SerializeField] int baseFood = 0;
    [SerializeField] int baseScience = 0;
    [SerializeField] int baseIncome = 0;
    [SerializeField] CityUI cityUI;
    
    [SerializeField] HexGrid hexGrid;
    [SerializeField] List<CityStateBuilding> buildings;
    [SerializeField] List<BuildConfig> availableBuildings;
    [SerializeField] List<BuildConfig> availableUnits;
    [SerializeField] int food = 0;
    [SerializeField] int population = 1;
    [SerializeField] int visionRange = 2;
    [SerializeField] int unitCap = 3;
    [SerializeField] PlayerBuildingControl playerBuildingControl;

    BuildingManager buildingManager = new BuildingManager();
    private bool capital = false;
    public int currentStrength = 0;
    public int currentProduction = 0;
    public int currentFood = 0;
    public int currentScience = 0;
    public int currentIncome = 0;
    public int currentPlayerIncome = 0;
    public int foodConsumption = 0;
    public int foodForNextPop = 0;
    CityState cityStateOwner;
    GameController gameController;
    HexCell hexCell;
    List<HexCell> ownedCells = new List<HexCell>();
    HexVision hexVision;

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
    public int Strength
    {
        get { return currentStrength; }
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

    public int Population
    {
        get
        {
            return population;
        }

        set
        {
            population = value;
            CityUI.SetPopulation(population.ToString());
            NotifyInfoChange();
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

    public PlayerBuildingControl PlayerBuildingControl
    {
        get
        {
            return playerBuildingControl;
        }

        set
        {
            playerBuildingControl = value;
        }
    }

    public bool Capital
    {
        get
        {
            return capital;
        }

        set
        {
            capital = value;
            RefreshYields();
        }
    }

    public bool HasUnitSpace()
    {
        return cityUnits.Count < unitCap;

    }

    public void RemoveUnit(CombatUnit unit)
    {
        cityUnits.Remove(unit);
        NotifyInfoChange();
    }

    public void AddUnit(CombatUnit unit)
    {
        cityUnits.Add(unit);
        unit.CityOwner = this;
        NotifyInfoChange();
    }

    public void SetCityState(CityState cityState)
    {
        if (cityStateOwner)
        {
            cityStateOwner.RemoveCity(this);
            UpdateOwnerVisiblity(hexCell, false);
        }
        cityStateOwner = cityState;
        cityStateOwner.AddCity(this);
        UpdateOwnerVisiblity(hexCell, true);
        NotifyInfoChange();

    }
    public virtual void UpdateOwnerVisiblity(HexCell hexCell, bool increase)
    {
        if (GetCityState())
        {
            List<HexCell> cells = PathFindingUtilities.GetCellsInRange(hexCell, VisionRange);
            for (int i = 0; i < cells.Count; i++)
            {
                if (increase)
                {
                    GetCityState().AddVisibleCell(cells[i]);
                }
                else
                {
                    GetCityState().RemoveVisibleCell(cells[i]);
                }
            }
            ListPool<HexCell>.Add(cells);

        }

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
        AddCellYield(hexCell);
        HexVision.SetCells(hexGrid.GetVisibleCells(hexCell, VisionRange));
        NotifyInfoChange();
    }


    public HexCell GetHexCell()
    {
        return hexCell;
    }

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        hexGrid = FindObjectOfType<HexGrid>();
        hexVision = gameObject.AddComponent<HexVision>();
        hitPoints = BaseHitPoints;
        CalculateFoodForNextPop();
        gameController.VisionSystem.AddHexVision(hexVision);

    }

    private void CalculateFoodForNextPop()
    {
        foodForNextPop = 10;
        for (int i = 1; i < population; i++)
        {
            foodForNextPop = (int)(foodForNextPop * 2f);
        }
    }

    public void StartTurn()
    {
       // cityStateOwner.Gold += currentIncome;
        //Food += currentFood;
        //if(Food >= foodForNextPop)
        //{
        //    Food -= foodForNextPop;
        //    Population += 1;
        //    foodConsumption += 1;
        //    AddCellYield(hexCell);
        //    CalculateFoodForNextPop();
        //}
        //else if(Food <= 0 && population != 1)
        //{
        //    Population -= 1;
        //    foodConsumption -= 1;
        //    RemoveCellYield(hexCell);
        //    CalculateFoodForNextPop();
        //}

        PlayerBuildingControl.StartTurn();

        RefreshYields();
    }


    public void TakeTurn()
    {
        BuildingManager.DayPassed(currentProduction);
        BuildConfig buildConfig = BuildingManager.GetCompletedBuild();
        while (buildConfig)
        {
            if (buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.COMBAT_UNIT)
            {
                CreateUnit((buildConfig as CombatUnitBuildConfig).CombatUnitConfig);
            }
            else if (buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.CITY_STATE_BUILDING)
            {
                AddBuilding(buildConfig as CityStateBuildConfig);
            }
            buildConfig = BuildingManager.GetCompletedBuild();
        }
    }
    public void UpdateHealthBar()
    {

        CityUI.UpdateHealthBar();


    }

    public void UpdateCityBar()
    {
        if (cityStateOwner.Player)
        {
            //TowerColor = cityStateOwner.Player.Color;
            CityUI.SetPlayerColour(cityStateOwner.Player.GetColour().Colour);
            hexCell.PlayerColour = cityStateOwner.Player.GetColour();
        }
        else
        {
            //TowerColor = Color.gray;
            CityUI.SetPlayerColour(Color.black);
            hexCell.PlayerColour = null;
        }

        cityUI.CityStateSymbol.sprite = gameController.GetCityStateSymbol(cityStateOwner.SymbolID);

    }


    private void AddCellYield(HexCell hexCell)
    {
        baseProduction += hexCell.Production;
        baseIncome += hexCell.Income;
        baseFood += hexCell.Food;
        RefreshYields();
    }

    private void RemoveCellYield(HexCell hexCell)
    {
        baseProduction -= hexCell.Production;
        baseIncome -= hexCell.Income;
        baseFood -= hexCell.Food;
        RefreshYields();
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

    public int Plunder()
    {
        int gold = GetCityState().Gold;
        gold = gold / 10;
        GetCityState().Gold -= gold;
        return gold;
    }
    public bool CreateUnit(CombatUnitConfig combatUnitConfig)
    {
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(hexCell, 2);
        foreach(HexCell cell in cells)
        {
            if(!cell.IsUnderwater && cell.CanUnitMoveToCell(HexUnit.UnitType.COMBAT))
            {
                HexUnit hexUnit = gameController.CreateCityStateUnit(combatUnitConfig, cell, cityStateOwner.CityStateID);
                AddUnit(hexUnit.GetComponent<CombatUnit>());
                hexUnit.Location.UpdateVision();
                cityStateOwner.UpdateVision();
                return true;
            }
        }
        return false;
    }

    public void KillCityUnits()
    {
        List<CombatUnit> unitsToKill = new List<CombatUnit>();
        foreach(CombatUnit unit in cityUnits)
        {
            unitsToKill.Add(unit);
        }
        foreach (CombatUnit unit in unitsToKill)
        {
            gameController.KillUnit(unit);
        }
        cityUnits.Clear();
    }
    public void BuildBuilding()
    {

        BuildConfig config = availableBuildings[UnityEngine.Random.Range(0, availableBuildings.Count)];
        buildingManager.AddBuild(config);
    }

    public void BuildUnit()
    {

        BuildConfig config = availableUnits[UnityEngine.Random.Range(0, availableUnits.Count)];
        buildingManager.AddBuild(config);
    }

    public void AddBuilding(CityStateBuildConfig buildConfig)
    {
        CityStateBuilding building = gameController.CreateCityStateBuilding(buildConfig);
        building.CityBuildIn = this;
        buildings.Add(building);
        RefreshYields();
    }

    public void RemoveBuilding(CityStateBuilding cityStateBuilding)
    {
        buildings.Remove(cityStateBuilding);
        RefreshYields();
    }

    public void RefreshYields()
    {
        currentStrength = baseStrength;
        currentProduction = baseProduction;
        currentFood = baseFood;
        currentFood -= foodConsumption;
        currentScience = baseScience;
        currentIncome = baseIncome;

        foreach(CityStateBuilding building in buildings)
        {
            currentStrength += (int)building.ResourceBenefit.Defence.y;
            currentStrength += (int)((float)baseStrength * (building.ResourceBenefit.Defence.x /100.0f));

            currentIncome += (int)building.ResourceBenefit.Gold.y;
            currentIncome += (int)((float)baseIncome * (building.ResourceBenefit.Gold.x / 100.0f));

            currentFood += (int)building.ResourceBenefit.Food.y;
            currentFood += (int)((float)baseFood * (building.ResourceBenefit.Food.x / 100.0f));

            currentScience += (int)building.ResourceBenefit.Science.y;
            currentScience += (int)((float)baseScience * (building.ResourceBenefit.Science.x / 100.0f));

            currentProduction += (int)building.ResourceBenefit.Production.y;
            currentProduction += (int)((float)baseProduction * (building.ResourceBenefit.Production.x / 100.0f));
        }

        ResourceBenefit playerBenefits = PlayerBuildingControl.GetTotalEffects();
        currentStrength += (int)playerBenefits.Defence.y;
        currentStrength += (int)((float)baseStrength * (playerBenefits.Defence.x / 100.0f));

        currentIncome += (int)playerBenefits.Gold.y;
        currentIncome += (int)((float)baseIncome * (playerBenefits.Gold.x / 100.0f));

        currentFood += (int)playerBenefits.Food.y;
        currentFood += (int)((float)baseFood * (playerBenefits.Food.x / 100.0f));

        currentScience += (int)playerBenefits.Science.y;
        currentScience += (int)((float)baseScience * (playerBenefits.Science.x / 100.0f));

        currentProduction += (int)playerBenefits.Production.y;
        currentProduction += (int)((float)baseProduction * (playerBenefits.Production.x / 100.0f));

        currentPlayerIncome = (int)(((float)currentIncome / 100.0f) * (30.0f + playerBenefits.PlayerGold.x) + playerBenefits.PlayerGold.y);

        unitCap = GameConsts.baseUnitCap + playerBenefits.UnitCap;
        if(capital)
        {
            unitCap += 2;
        }
        NotifyInfoChange();
    }

    public int GetIncome()
    {
        return currentIncome;
    }

    public int GetPlayerIncome()
    {
        return currentPlayerIncome;
    }

    public void Save(BinaryWriter writer)
    {
        GetHexCell().coordinates.Save(writer);
        writer.Write(cityStateOwner.CityStateID);
        buildingManager.Save(writer);
        playerBuildingControl.Save(writer);
        writer.Write(food);
        writer.Write(population);
        writer.Write(cityUnits.Count);
        foreach(CombatUnit unit in cityUnits)
        {
            unit.Save(writer);
        }
    }

    public static void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        int cityStateID = reader.ReadInt32();
        CityState cityState = gameController.GetCityState(cityStateID);
        HexCell cell = hexGrid.GetCell(coordinates);
        City city = gameController.CreateCity(cell, cityState);
        city.buildingManager.Load(reader,gameController,header);
        city.PlayerBuildingControl.Load(reader,gameController,header);
        if(header >=5)
        {
            city.food = reader.ReadInt32();
            city.Population = reader.ReadInt32();
            city.foodConsumption = city.population;
            city.CalculateFoodForNextPop();
            for (int a = 1; a < city.population; a++)
            {
                city.AddCellYield(city.GetHexCell());
            }
        }
        if (header >= 7)
        {
            int unitCount = reader.ReadInt32();
            for (int i = 0; i < unitCount; i++)
            {
                CombatUnit combatUnit = CombatUnit.Load(reader, gameController, hexGrid, header, city.GetCityState().CityStateID);
                city.AddUnit(combatUnit);
            }
        }

    }
    public void NotifyInfoChange()
    {
        if (onInfoChange != null)
        {
            onInfoChange(this);
        }
    }


}
