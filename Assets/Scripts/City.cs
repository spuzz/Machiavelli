using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class City : MonoBehaviour {

    [SerializeField] int baseHitPoints = 200;
    [SerializeField] CityUI cityUI;
    [SerializeField] int baseStrength = 25;
    [SerializeField] BuildingManager buildingManager;
    [SerializeField] HexGrid hexGrid;
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

    CityState cityStateOwner;
    GameController gameController;
    HexCell hexCell;
    List<HexCell> ownedCells = new List<HexCell>();

    

    bool vision = false;

    public bool Vision
    {
        get
        {
            return vision;
        }

        set
        {
            if(vision != value)
            {
                foreach (HexCell hexCell in ownedCells)
                {
                    if (vision == false && value == true)
                    {
                        hexCell.IncreaseVisibility();
                    }
                    else if (vision == true && value == false)
                    {
                        hexCell.DecreaseVisibility();
                    }

                    for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                    {
                        HexCell neighbour = hexCell.GetNeighbor(d);
                        if(neighbour)
                        {
                            if (vision == false && value == true)
                            {
                                neighbour.IncreaseVisibility();
                            }
                            else if(vision == true && value == false)
                            {
                                neighbour.DecreaseVisibility();
                            }
                            
                        }
                            
                    }

                }
                vision = value;
            }

 
        }
    }

    Color towerColor = Color.black;
    public Color TowerColor
    {
        get { return towerColor; }
        set
        {
            towerColor = value;
            foreach(HexCell hexCell in ownedCells)
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

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        hexGrid = FindObjectOfType<HexGrid>();
        hitPoints = baseHitPoints;
        
    }

    public void StartTurn()
    {
        BuildingManager.DayPassed();
        BuildConfig buildConfig = BuildingManager.GetCompletedBuild();
        while(buildConfig)
        {
            GameObject gameObjectPrefab = buildConfig.GameObjectPrefab;
            if(gameObjectPrefab.GetComponent<CombatUnit>())
            {
                CreateUnit(buildConfig);
            }
            buildConfig = BuildingManager.GetCompletedBuild();
        }
    }


    public void EnableUI(bool vision)
    {
        cityUI.Visible = vision;

    }
    public void UpdateUI()
    {
        cityUI.SetColour(cityStateOwner.Color);
        cityUI.UpdateHealthBar();
        if (cityStateOwner.Player)
        {
            TowerColor = cityStateOwner.Player.Color;
        }
        foreach (HexCell hexCell in ownedCells)
        {
            hexCell.CellColor = cityStateOwner.Color;
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
    }

    public HexCell GetHexCell()
    {
        return hexCell;
    }

    private void UpdateWalls()
    {
        foreach(HexCell cell in ownedCells)
        {
            cell.Walled = true;
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
        HexUnit hexUnit = Instantiate(buildConfig.GameObjectPrefab.GetComponent<HexUnit>());
        hexUnit.UnitPrefabName = buildConfig.UnitPreFabName;
        HexCell cell = FindFreeCell(hexUnit);
        if(!cell)
        {
            return false;
        }
        hexGrid.AddUnit(hexUnit, cell, UnityEngine.Random.Range(0f, 360f));
        gameController.CreateCityStateUnit(hexUnit, cityStateOwner.CityStateID);
        return true;
    }

    private HexCell FindFreeCell(HexUnit hexUnit)
    {
        if(hexCell.hexUnits.Count == 0)
        {
            return hexCell;
        }
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbour = hexCell.GetNeighbor(d);
            if (neighbour && neighbour.CanUnitMoveToCell(hexUnit))
            {
                return neighbour;
            }
        }
        return null;
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
        hexGrid.AddCity(cell, cityState);
    }
}
