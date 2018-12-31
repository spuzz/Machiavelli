using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CityState : MonoBehaviour
{
    static int cityStateIDCounter = 1;
    List<City> cities = new List<City>();
    [SerializeField] List<CombatUnit> units = new List<CombatUnit>();
    
    int cityStateID;

    GameController gameController;

    

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        cityStateID = cityStateIDCounter;
        cityStateIDCounter++;
    }

    Color color = Color.black;
    public Color Color
    {
        get { return color; }
        set { color = value; }
    }

    Color towerColor = Color.black;
    public Color TowerColor
    {
        get { return towerColor; }
        set { towerColor = value; }
    }

    Player player;
    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
            towerColor = player.Color;
        }
    }

    public int CityStateID
    {
        get { return cityStateID; }
        set { cityStateID = value; }
    }


    public void PickColor()
    {

        Color = gameController.GetNewCityStateColor();
    }
    public void AddCity(City city)
    {
        cities.Add(city);
    }

    public void RemoveCity(City city)
    {
        cities.Remove(city);
    }

    public int GetCityCount()
    {
        return cities.Count;
    }
    public void DestroyCityState()
    {
        gameController.ReturnCityStateColor(color);
        Destroy(gameObject);
    }

    public void AddUnit(CombatUnit unit)
    {
        unit.CityState = this;
        units.Add(unit);
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(CityStateID);
        writer.Write(Color.r);
        writer.Write(Color.g);
        writer.Write(Color.b);
        writer.Write(units.Count);
        for (int i = 0; i < units.Count; i++)
        {
            units[i].Save(writer);
        }
    }

    public static void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {
        CityState instance = gameController.CreateCityState();
        instance.CityStateID = reader.ReadInt32();
        Color color = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1.0f);
        gameController.RemoveCityStateColor(color);
        instance.Color = color;
        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            CombatUnit combatUnit = CombatUnit.Load(reader, hexGrid, header);
            combatUnit.GetComponent<HexUnit>().Visible = false;
            instance.AddUnit(combatUnit);
        }
    }
}
