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
    [SerializeField] BuildingManager buildingManager;
    [SerializeField] HexGrid hexGrid;
    [SerializeField] List<CityStateBuilding> buildings;
    [SerializeField] List<BuildConfig> availableBuildings;
    [SerializeField] List<BuildConfig> availableUnits;
    [SerializeField] int food = 0;
    [SerializeField] int population = 1;
    [SerializeField] int visionRange = 2;

    int currentStrength = 0;
    int currentProduction = 0;
    int currentFood = 0;
    int currentScience = 0;
    int currentIncome = 0;
    int foodConsumption = 0;
    int foodForNextPop = 0;
    CityState cityStateOwner;
    GameController gameController;
    HexCell hexCell;
    Color towerColor = Color.black;
    List<HexCell> ownedCells = new List<HexCell>();
    HexVision hexVision;
    public float HealthAsPercentage
    {
        get { return (float)hitPoints / (float)baseHitPoints; }
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
        }
    }
    public int Strength
    {
        get { return baseStrength; }
    }

    public Color TowerColor
    {
        get { return towerColor; }
        set
        {
            towerColor = value;

            CityUI.SetPlayerColour(towerColor);
            foreach (HexCell hexCell in ownedCells)
            {
                hexCell.CellSecondColor = towerColor;
                
            }
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

    public void SetCityState(CityState cityState)
    {
        if (cityStateOwner)
        {
            cityStateOwner.RemoveCity(this);
        }
        cityStateOwner = cityState;
        cityStateOwner.AddCity(this);

    }

    public CityState GetCityState()
    {
        return cityStateOwner;
    }

    public void SetHexCell(HexCell cell)
    {
        hexCell = cell;
        hexCell.Walled = true;
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
        hitPoints = baseHitPoints;
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
        cityStateOwner.Gold += currentIncome;
        food += currentFood;
        if(food >= foodForNextPop)
        {
            food -= foodForNextPop;
            Population += 1;
            foodConsumption += 1;
            AddCellYield(hexCell);
            CalculateFoodForNextPop();
        }
        else if(food <= 0 && population != 1)
        {
            Population -= 1;
            foodConsumption -= 1;
            RemoveCellYield(hexCell);
            CalculateFoodForNextPop();
        }

        
        BuildingManager.DayPassed(currentProduction);
        BuildConfig buildConfig = BuildingManager.GetCompletedBuild();
        while(buildConfig)
        {
            GameObject gameObjectPrefab = buildConfig.GameObjectPrefab;
            if(gameObjectPrefab.GetComponent<CombatUnit>())
            {
                CreateUnit(buildConfig);
            }
            if(gameObjectPrefab.GetComponent<CityStateBuilding>())
            {
                AddBuilding(buildConfig);
            }
            buildConfig = BuildingManager.GetCompletedBuild();
        }
    }


    public void UpdateUI()
    {

        CityUI.SetCityStateColour(cityStateOwner.Color);
        CityUI.UpdateHealthBar();

        if (cityStateOwner.Player)
        {
            TowerColor = cityStateOwner.Player.Color;
        }
        else
        {
            TowerColor = Color.black;
        }
        foreach (HexCell hexCell in ownedCells)
        {
            hexCell.CellColor = cityStateOwner.Color;
        }
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

    public bool CreateUnit(BuildConfig buildConfig)
    {
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(hexCell, 2);
        foreach(HexCell cell in cells)
        {
            if(cell.CanUnitMoveToCell(HexUnit.UnitType.COMBAT))
            {
                HexUnit hexUnit = gameController.CreateCityStateUnit(buildConfig.GameObjectPrefab, buildConfig.PreFabName, cell, cityStateOwner.CityStateID);
                hexUnit.Location.UpdateVision();
                cityStateOwner.UpdateVision();
                return true;
            }
        }
        return false;
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

    public void AddBuilding(BuildConfig buildConfig)
    {
        CityStateBuilding cityStateBuilding = Instantiate(buildConfig.GameObjectPrefab,transform).GetComponent<CityStateBuilding>();
        buildings.Add(cityStateBuilding);
        RefreshYields();
    }

    public void RemoveBuilding(CityStateBuilding cityStateBuilding)
    {
        buildings.Remove(cityStateBuilding);
        RefreshYields();
    }

    private void RefreshYields()
    {
        currentStrength = baseStrength;
        currentProduction = baseProduction;
        currentFood = baseFood;
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
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(cityStateOwner.CityStateID);
    }

    public static void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        int cityStateID = reader.ReadInt32();
        CityState cityState = gameController.GetCityState(cityStateID);
        HexCell cell = hexGrid.GetCell(coordinates);
        gameController.CreateCity(cell, cityState);
    }
}
