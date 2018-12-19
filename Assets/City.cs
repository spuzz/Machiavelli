using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class City : MonoBehaviour {

    CityState cityStateOwner;
    GameController gameController;
    HexCell hexCell;
    List<HexCell> ownedCells = new List<HexCell>();

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
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
        ownedCells.Clear();
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbour = hexCell.GetNeighbor(d);
            if(neighbour)
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
