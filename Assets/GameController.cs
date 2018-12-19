using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField] Player humanPlayer;
    [SerializeField] GameObject cityStatesObject;
    [SerializeField] GameObject citiesObject;

    public CityState cityStatePrefab;

    List<CityState> cityStates = new List<CityState>();
    List<City> cities = new List<City>();

    int turn = 1;
    void Start()
    {
        turn = 1;
    }

    public CityState GetCityState(int cityStateID)
    {
        foreach (CityState cityState in cityStates)
        {
            if (cityState.CityStateID == cityStateID)
            {
                return cityState;
            }
        }
        return null;
    }
    public void AddCity(City city)
    {
        city.transform.SetParent(citiesObject.transform);
        if (!city.GetCityState())
        {
            city.SetCityState(CreateCityState());
        }
        cities.Add(city);
    }

    public void RemoveCity(HexCell cell)
    {
        foreach (City city in cities)
        {
            if (city.GetHexCell() == cell)
            {
                RemoveCity(city);
                break;
            }

        }
    }

    public void RemoveCity(City city)
    {
        RemoveCityFromState(city);
        city.DestroyCity();
        cities.Remove(city);
    }

    private void RemoveCityFromState(City city)
    {
        CityState cityState = city.GetCityState();
        if (cityState)
        {
            cityState.RemoveCity(city);
            if(cityState.GetCityCount() == 0)
            {
                cityStates.Remove(cityState);
                cityState.DestroyCityState();
            }
        }
    }

    public CityState CreateCityState()
    {
        CityState instance = Instantiate(cityStatePrefab);
        instance.transform.SetParent(cityStatesObject.transform);
        cityStates.Add(instance);
        return instance;
    }

    public void AddCityState(CityState cityState)
    {
        cityStates.Add(cityState);
    }
    public void RemoveCityState(CityState cityState)
    {
        cityStates.Remove(cityState);
    }

    public Player GetPlayer(int playerNumber)
    {
        return humanPlayer;
    }

    public void EndPlayerTurn()
    {
        humanPlayer.EndTurn();
        // Do AI
        turn += 1;
        humanPlayer.StartTurn();
    }

    public int GetTurn()
    {
        return turn;
    }

    public void EndGame()
    {

    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(cityStates.Count);
        foreach (CityState cityState in cityStates)
        {
            writer.Write(cityState.CityStateID);
        }

        writer.Write(cities.Count);
        foreach (City city in cities)
        {
            city.GetHexCell().coordinates.Save(writer);
            writer.Write(city.GetCityState().CityStateID);
        }
    }

    public void Load(BinaryReader reader, int header)
    {
        if (header >= 7)
        {
            int cityStateCount = reader.ReadInt32();
            for (int i = 0; i < cityStateCount; i++)
            {
                CityState.Load(reader, this, header);
            }

        }
    }

    public void ClearCitiesAndStates()
    {
        foreach (City city in cities)
        {
            city.DestroyCity();
        }
        cities.Clear();
        foreach (CityState cityState in cityStates)
        {
            cityState.DestroyCityState();
        }
        cityStates.Clear();
    }

}
