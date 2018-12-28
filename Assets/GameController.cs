using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField] List<Player> players = new List<Player>();
    [SerializeField] GameObject cityStatesObject;
    [SerializeField] GameObject citiesObject;
    [SerializeField] GameObject playersObject;
    [SerializeField] AIPlayer aiPlayerPrefab;

    public CityState cityStatePrefab;

    List<CityState> cityStates = new List<CityState>();
    List<City> cities = new List<City>();

    [SerializeField] HumanPlayer humanPlayer;

    public HumanPlayer HumanPlayer
    {
        get { return humanPlayer; }
    }
    int turn = 1;
    void Start()
    {
        turn = 1;
    }

    public int PlayerCount()
    {
        return players.Count + 1;
    }

    public List<string> PlayerNames()
    {
        List<string> names = new List<string>();
        names.Add("Human Player");
        foreach (Player player in players)
        {
            names.Add(player.PlayerNumber.ToString());
        }
        return names;
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

    public int CityStateCount()
    {
        return cityStates.Count;
    }

    public List<string> CityStateNames()
    {
        List<string> names = new List<string>();
        foreach(CityState cityState in cityStates)
        {
            names.Add(cityState.CityStateID.ToString());
        }
        return names;
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

    public void CreateAgent(HexUnit hexUnit, Player player)
    {
        if (player.IsHuman)
        {
            hexUnit.Visible = true;
            hexUnit.Controllable = true;
        }
        Agent agent = hexUnit.GetComponent<Agent>();
        player.AddAgent(agent);
    }

    public void CreateCityStateUnit(HexUnit hexUnit, int cityStateID)
    {
        CityState cityState = cityStates.Find(c => c.CityStateID == cityStateID);
        cityState.AddUnit(hexUnit.GetComponent<CombatUnit>());
    }


    public AIPlayer CreateAIPlayer()
    {
        AIPlayer instance = Instantiate(aiPlayerPrefab);
        instance.transform.SetParent(playersObject.transform);
        players.Add(instance);
        return instance;
    }

    public Player GetAIPlayer(int playerNumber)
    {
        Player player = players.Find(p => p.PlayerNumber == playerNumber);
        if(!player)
        {
            throw new ArgumentException("No Player With That ID");
        }
        return player;
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
        HumanPlayer.Save(writer);
        foreach(AIPlayer aiPlayer in players)
        {
            aiPlayer.Save(writer);
        }
        writer.Write(cityStates.Count);
        foreach (CityState cityState in cityStates)
        {
            cityState.Save(writer);
        }

        writer.Write(cities.Count);
        foreach (City city in cities)
        {
            city.GetHexCell().coordinates.Save(writer);
            writer.Write(city.GetCityState().CityStateID);
        }
    }

    public void Load(BinaryReader reader, int header, HexGrid hexGrid)
    {
        HumanPlayer.Load(reader, this, hexGrid, header);
        foreach (AIPlayer aiPlayer in players)
        {
            AIPlayer.Load(reader, this, hexGrid, header);
        }

        int cityStateCount = reader.ReadInt32();
        for (int i = 0; i < cityStateCount; i++)
        {
            CityState.Load(reader, this,hexGrid, header);
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
