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
    HexVision hexVision;
    // Attributes
    public static int cityIDCounter = 1;
    int cityID;
    [SerializeField] int baseHitPoints = 200;
    [SerializeField] int baseStrength = 25;
    [SerializeField] int baseProduction = 0;
    [SerializeField] int baseFood = 0;
    [SerializeField] int baseScience = 0;
    [SerializeField] int baseIncome = 0;
    [SerializeField] int visionRange = 2;
    [SerializeField] int gold = 100;

    [SerializeField] List<CityPlayerBuilding> buildings;
    [SerializeField] List<BuildConfig> availableBuildings;
    [SerializeField] List<BuildConfig> availableUnits;

    bool alive;

    BuildingManager buildingManager = new BuildingManager();
    List<HexCell> ownedCells = new List<HexCell>();
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
            player = value;

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

    public int Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
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
        AddCellYield(hexCell);
        HexVision.SetCells(PathFindingUtilities.GetCellsInRange(hexCell, VisionRange));
        NotifyInfoChange();
    }


    public HexCell GetHexCell()
    {
        return hexCell;
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


    public void StartTurn()
    {

    }


    public void TakeTurn()
    {
        BuildingManager.DayPassed(baseProduction);
        BuildConfig buildConfig = BuildingManager.GetCompletedBuild();
        while (buildConfig)
        {
            if (buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.COMBAT_UNIT)
            {
                CreateUnit((buildConfig as CombatUnitBuildConfig).CombatUnitConfig);
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

    private void AddCellYield(HexCell hexCell)
    {
        baseProduction += hexCell.Production;
        baseIncome += hexCell.Income;
        baseFood += hexCell.Food;
    }

    private void RemoveCellYield(HexCell hexCell)
    {
        baseProduction -= hexCell.Production;
        baseIncome -= hexCell.Income;
        baseFood -= hexCell.Food;
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

    public void BuildUnit()
    {

        BuildConfig config = availableUnits[UnityEngine.Random.Range(0, availableUnits.Count)];
        buildingManager.AddBuild(config);
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
                return true;
            }
        }
        return false;
    }

    public void Save(BinaryWriter writer)
    {
        GetHexCell().coordinates.Save(writer);
        cityStateOwner.Save(writer);
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
    }

    public static void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        int playerNumber = reader.ReadInt32();
        HexCell cell = hexGrid.GetCell(coordinates);
        City city = gameController.CreateCity(cell);
        city.GetComponent<CityState>().Load(reader ,gameController, hexGrid, header);
        if (playerNumber != -1)
        {
            city.Player = gameController.GetPlayer(playerNumber);
        }
        city.buildingManager.Load(reader,gameController,header);
        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            CombatUnit combatUnit = CombatUnit.Load(reader, gameController, hexGrid, header, city);
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
