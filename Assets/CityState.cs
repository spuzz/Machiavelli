using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CityState : MonoBehaviour
{

    List<City> cities = new List<City>();
    [SerializeField] List<CombatUnit> units = new List<CombatUnit>();
    Color color;
    int cityStateID;

    GameController gameController;

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
    }
    public Color Color
    {
        get { return color; }
        set { color = value; }
    }

    public int CityStateID
    {
        get { return cityStateID; }
        set { cityStateID = value; }
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
        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            CombatUnit combatUnit = CombatUnit.Load(reader, hexGrid, header);
            combatUnit.GetComponent<HexUnit>().Visible = false;
            instance.AddUnit(combatUnit);
        }
    }
}
