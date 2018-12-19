using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CityState : MonoBehaviour
{

    List<City> cities = new List<City>();

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

    public void Save(BinaryWriter writer)
    {
        writer.Write(CityStateID);
    }

    public static void Load(BinaryReader reader, GameController gameController, int header)
    {
        CityState instance = gameController.CreateCityState();
        instance.CityStateID = reader.ReadInt32();
    }
}
