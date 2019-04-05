using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CityState : MonoBehaviour
{
    public static int cityStateIDCounter = 1;
    [SerializeField] CityStateAIController cityStateAIController;
    [SerializeField] int gold = 100;
    [SerializeField] string cityStateName = "City State";
    [SerializeField] int symbolID;
    GameController gameController;
    int cityStateID;
    Player player;
    bool alive = true;
    public Dictionary<HexCell, int> visibleCells = new Dictionary<HexCell, int>();
    public List<HexCell> exploredCells = new List<HexCell>();
    List<City> cities = new List<City>();
    List<City> visibleCities = new List<City>();


    public delegate void OnInfoChange(CityState cityState);
    public event OnInfoChange onInfoChange;

    public IEnumerable<HexCell> GetExploredCells()
    {
        return exploredCells;
    }

    public IEnumerable<City> GetEnemyCities()
    {
        return visibleCities.FindAll(c => c.GetCityState() != this);
    }
    public List<City> GetEnemyCitiesOrderByDistance(HexCoordinates unitCoordinates)
    {
        return visibleCities.FindAll(c => c.GetCityState() != this).OrderBy(c => c.GetHexCell().coordinates.DistanceTo(unitCoordinates)).ToList();
    }

    public void AddVisibleCell(HexCell cell)
    {
        if (!exploredCells.Contains(cell))
        {
            exploredCells.Add(cell);
            if(cell.City)
            {
                visibleCities.Add(cell.City);
            }
        }

        if (!visibleCells.ContainsKey(cell))
        {
            visibleCells[cell] = 0;
        }
        else
        {
            visibleCells[cell] += 1;
        }
    }

    public void RemoveVisibleCell(HexCell cell)
    {
        if (visibleCells.ContainsKey(cell))
        {
            visibleCells[cell] -= 1;
            if (visibleCells[cell] <= 0)
            {
                visibleCells.Remove(cell);
            }
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
            NotifyInfoChange();
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
            if (player)
            {
                player.RemoveCityState(this);
            }
            player = value;
            if (player)
            {
                player.AddCityState(this);
            }
        }
    }


    public int CityStateID
    {
        get { return cityStateID; }
        set { cityStateID = value; }
    }

    public string CityStateName
    {
        get
        {
            return cityStateName;
        }

        set
        {
            cityStateName = value;
        }
    }

    public int SymbolID
    {
        get
        {
            return symbolID;
        }

        set
        {
            symbolID = value;
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

    public void AddCity(City city)
    {
        if (player && player.IsHuman)
        {
            city.HexVision.HasVision = true;
        }
        city.onInfoChange += cityChanged;
        city.KillCityUnits();
        if(cities.Count == 0)
        {
            city.Capital = true;
        }
        cities.Add(city);
        NotifyInfoChange();
    }

    public void RemoveCity(City city)
    {
        if(city.Capital)
        {
            city.Capital = false;
            if(cities.Count > 1)
            {
                cities[1].Capital = true;
            }
        }
        city.KillCityUnits();
        city.HexVision.HasVision = false;
        city.onInfoChange -= cityChanged;
        cities.Remove(city);
        NotifyInfoChange();
        if (cities.Count == 0)
        {
            Alive = false;
        }
    }

    private void cityChanged(City city)
    {
        NotifyInfoChange();
    }

    public int GetCityCount()
    {
        return cities.Count;
    }

    public City GetCity()
    {
        return cities[0];
    }
    public IEnumerable<City> GetCities()
    {
        return cities;
    }

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        cityStateID = cityStateIDCounter;
        cityStateIDCounter++;
    }

    public void StartTurn()
    {
        
        foreach (City city in cities)
        {
            city.StartTurn();
            Gold += city.GetIncome();
        }

        NotifyInfoChange();
    }

    public int GetIncome()
    {
        int income = 0;
        foreach(City city in cities)
        {
            income += city.GetIncome();
        }

        return income;
    }
    public int GetPlayerIncome()
    {
        int income = 0;
        int count = 1;
        foreach (City city in cities)
        {
            income += (5 * count);
            count++;
        }


        return income;
    }


    public void TakeTurn()
    {
        cityStateAIController.UpdateUnits();
        cityStateAIController.UpdateCities();
        gameController.CityStateTurnFinished(this);
    }

    public void DestroyCityState()
    {
        if(player)
        {
            player.RemoveCityState(this);
        }
        while(cities.Count > 0)
        {
            gameController.DestroyCity(cities[0]);
        }
        Destroy(gameObject);
    }


    public void Save(BinaryWriter writer)
    {
        writer.Write(CityStateID);
        writer.Write(SymbolID);
        writer.Write(exploredCells.Count);
        for (int i = 0; i < exploredCells.Count; i++)
        {
            writer.Write(exploredCells[i].Index);
        }
    }

    public static void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {

        int cityStateID = reader.ReadInt32();
        int symbolID;
        CityState instance;
        symbolID = reader.ReadInt32();
        instance = gameController.CreateCityState(symbolID);
        instance.CityStateID = cityStateID;
        int exploredCellCount = reader.ReadInt32();
        for (int i = 0; i < exploredCellCount; i++)
        {
            HexCell cell = hexGrid.GetCell(reader.ReadInt32());
            if (!instance.exploredCells.Contains(cell))
            {
                instance.exploredCells.Add(cell);
            }

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
