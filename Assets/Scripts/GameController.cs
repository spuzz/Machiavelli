using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{

    [SerializeField] List<Player> players = new List<Player>();
    [SerializeField] GameObject cityStatesObject;
    [SerializeField] GameObject citiesObject;
    [SerializeField] GameObject playersObject;
    [SerializeField] AIPlayer aiPlayerPrefab;

    [SerializeField] List<Color> possibleCityStateColors;
    [SerializeField] List<Color> possiblePlayerColors;
    [SerializeField] HexMapCamera hexMapCamera;
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
        humanPlayer.Color = GetNewPlayerColor();
    }
    public void EndPlayerTurn()
    {
        humanPlayer.EndTurn();
        StartCoroutine(NewTurn());


    }

    IEnumerator NewTurn()
    {
        foreach (CityState cityState in cityStates)
        {
            if(cityState)
            {
                yield return StartCoroutine(cityState.TakeTurn());
            }
            
        }

        turn += 1;
        foreach (CityState cityState in cityStates)
        {
            cityState.StartTurn();
        }
        humanPlayer.StartTurn();
    }

    public int GetTurn()
    {
        return turn;
    }

    public void EndGame()
    {

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

    public City GetCity(HexCell hexCell)
    {
        return cities.Find(c => c.GetHexCell() == hexCell);
    }

    public void AddCity(City city)
    {
        city.transform.SetParent(citiesObject.transform);
        if (!city.GetCityState())
        {
            city.SetCityState(CreateCityState());
            city.UpdateUI();
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
                DestroyCityState(cityState);
            }
        }
    }

    private void DestroyCityState(CityState cityState)
    {
        cityStates.Remove(cityState);
        cityState.DestroyCityState();
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
        instance.PickColor();
        cityStates.Add(instance); 
        return instance;
    }

    public void AddCityState(CityState cityState)
    {
        cityStates.Add(cityState);
    }
    public void RemoveCityState(CityState cityState)
    {
        possibleCityStateColors.Add(cityState.Color);
        cityStates.Remove(cityState);
    }

    public void CreateAgent(HexUnit hexUnit, Player player)
    {
        Agent agent = hexUnit.GetComponent<Agent>();
        if (player.IsHuman)
        {
            agent.HexUnit.Visible = true;
            hexUnit.Controllable = true;
        }

        hexUnit.HexUnitType = HexUnit.UnitType.AGENT;
        player.AddAgent(agent);
    }

    public void CreateCityStateUnit(HexUnit hexUnit, int cityStateID)
    {
        CityState cityState = cityStates.Find(c => c.CityStateID == cityStateID);
        cityState.AddUnit(hexUnit.GetComponent<CombatUnit>());
    }

    public void SetCityStatePlayer(Player player, int cityStateID)
    {
        CityState cityState = cityStates.Find(c => c.CityStateID == cityStateID);
        if(cityState && cityState.Player != player)
        {
            cityState.Player = player;
        }
    }

    public AIPlayer CreateAIPlayer()
    {
        AIPlayer instance = Instantiate(aiPlayerPrefab);
        instance.transform.SetParent(playersObject.transform);
        instance.Color = GetNewPlayerColor();
        players.Add(instance);
        
        return instance;
    }

    public Player GetPlayer(int playerNumber)
    {
        if(playerNumber == 0)
        {
            return humanPlayer;
        }
        Player player = players.Find(p => p.PlayerNumber == playerNumber);
        if(!player)
        {
            throw new ArgumentException("No Player With That ID");
        }
        return player;
    }



    public Color GetNewCityStateColor()
    {
        int colorIndex = Random.Range(0, possibleCityStateColors.Count - 1);
        Color color = possibleCityStateColors[colorIndex];
        possibleCityStateColors.Remove(color);
        return color;
    }

    public Color GetNewPlayerColor()
    {
        int colorIndex = Random.Range(0, possiblePlayerColors.Count - 1);
        Color color = possiblePlayerColors[colorIndex];
        possiblePlayerColors.Remove(color);
        return color;
    }

    public void ReturnCityStateColor(Color color)
    {
        possibleCityStateColors.Add(color);
    }

    public void RemoveCityStateColor(Color color)
    {
        possibleCityStateColors.Remove(color);
    }
    public void Save(BinaryWriter writer)
    {
        HumanPlayer.Save(writer);
        writer.Write(players.Count);
        foreach (AIPlayer aiPlayer in players)
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

    public void CentreMap()
    {
        CityState cityState = humanPlayer.GetCityStates().FirstOrDefault();
        if(cityState)
        {
            hexMapCamera.MoveCamera(cityState.GetCity().GetHexCell());
        }
        else
        {
            Agent agent = humanPlayer.GetAgents().FirstOrDefault();
            if(agent)
            {
                hexMapCamera.MoveCamera(agent.HexUnit.Location);
            }
            else
            {
                HexMapCamera.ValidatePosition();
            }
        }
    }

    public void CentreMap(HexCell hexCell)
    {

    }

    public void CentreMap(City city)
    {
        if(city)
        {
            CentreMap(city.GetHexCell());
        }
    }

    public void Load(BinaryReader reader, int header, HexGrid hexGrid)
    {
        HumanPlayer.Load(reader, this, hexGrid, header);
        if(header >= 2)
        {
            int playerCount = reader.ReadInt32();
            for (int i = 0; i < playerCount; i++)
            {
                AIPlayer.Load(reader, this, hexGrid, header);
            }
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
