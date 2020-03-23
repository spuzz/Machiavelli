using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CityState : MonoBehaviour
{

    // External Components
    GameController gameController;
    Player player;


    // Internal Components
    [SerializeField] CityStateAIController cityStateAIController;

    // Attributes
    int cityStateID;
    public static int cityStateIDCounter = 1;
    [SerializeField] string cityStateName = "City State";
    [SerializeField] int symbolID;
    [SerializeField] List<Politician> politicians;
    [SerializeField] Politician politicianPrefab;
    bool alive = true;

    [SerializeField] City city;


    public delegate void OnInfoChange(CityState cityState);
    public event OnInfoChange onInfoChange;

    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;

            foreach (CombatUnit unit in city.GetUnits())
            {
                unit.SetPlayer(player);
                unit.UpdateUI(0);
            }
            city.UpdateCity();
            NotifyInfoChange();
        }
    }

    public void SetPlayerOnly(Player ply)
    {
        if (player)
        {
            player.RemoveCity(city);
        }
        player = ply;
        if (player)
        {
            player.AddCity(city);
        }
        foreach (CombatUnit unit in city.GetUnits())
        {
            unit.SetPlayer(player);
        }

        city.TrainingOptions.Clear();
        city.BuildingOptions.Clear();
        if(player)
        {
            city.ResetBuildOptions(player);
        }
        else
        {

        }

        NotifyInfoChange();
    }

    public void SetAllPoliticians(Player player, int loyalty = 100)
    {
        foreach (Politician politician in politicians)
        {
            politician.ControllingPlayer = player;
            politician.Loyalty = loyalty;
        }
        UpdatePoliticalLandscape();
    }

    public void AddLoyalty(int Loyalty, Player player)
    {
        foreach(Politician pol in politicians.FindAll(c => c.ControllingPlayer == player))
        {
            pol.Loyalty += Loyalty;
            CheckPoliciticianLoyalty(pol);
        }
    }

    private void CheckPoliciticianLoyalty(Politician pol)
    {
        if (pol.Loyalty == 0)
        {
            pol.ControllingPlayer = null;
            UpdatePoliticalLandscape();
        }

    }

    public void UpdateCityState()
    {
        foreach (CombatUnit unit in city.GetUnits())
        {
            unit.UpdateUI(0);
        }
        city.UpdateCity();
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

    private void cityChanged(City city)
    {
        NotifyInfoChange();
    }


    public City GetCity()
    {
        return city;
    }

    public int TotalPoliticians()
    {
        return politicians.Count;
    }

    public int PoliticiansByPlayer(Player player)
    {
        return politicians.FindAll(c => c.ControllingPlayer == player).Count;
    }

    public IEnumerable<Politician> GetPoliticians()
    {
        return politicians;
    }

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        cityStateID = cityStateIDCounter;
        cityStateIDCounter++;
    }

    public void StartTurn()
    {
        NotifyInfoChange();
    }

    public void TakeTurn()
    {

    }

    public void CreatePolitician()
    {
        Politician politician = Instantiate <Politician>(politicianPrefab,transform.Find("Politicians").transform);
        politician.CityState = this;
        politicians.Add(politician);
        if (player)
        {
            politician.ControllingPlayer = player;
            politician.Loyalty = 100;
        }

    }
    public void UpdatePoliticalLandscape()
    {
        Player playerResult = GetOwningPlayer();
        if (playerResult != Player)
        {
            SetPlayerOnly(playerResult);
        }
    }

    private Player GetOwningPlayer()
    {
        Dictionary<Player, int> players = new Dictionary<Player, int>();
        foreach (Politician pol in politicians)
        {
            Player player = pol.ControllingPlayer;
            if (player)
            {
                if (players.ContainsKey(player))
                {
                    players[player]++;
                }
                else
                {
                    players[player] = 1;
                }
            }
        }
        if (players.Count == 0)
        {
            return null;
        }
        Player highestPlayer = players.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        if (players[highestPlayer] > politicians.Count / 2)
        {
            return highestPlayer;
        }

        return null;
    }


    public int GetPoliticianMaintenance()
    {
        int mnt = 0;
        foreach (Politician pol in politicians)
        {
            mnt += GameConsts.maintanencePerPop;
        }
        return mnt;
    }

    public void DestroyCityState()
    {
        Destroy(gameObject);
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(CityStateID);
        writer.Write(SymbolID);
        writer.Write(politicians.Count);
        foreach(Politician politician in politicians)
        {
            politician.Save(writer);
        }
    }

    public void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {

        CityStateID = reader.ReadInt32();
        SymbolID = gameController.PickSymbol(reader.ReadInt32());
        if(header >= 2)
        {

            int polCount = reader.ReadInt32();
            for(int a=0; a<polCount; a++)
            {
                Politician pol = Instantiate<Politician>(politicianPrefab,transform.Find("Politicians").transform);
                politicians.Add(pol);
                pol.CityState = this;
                pol.Load(reader, gameController, header);
            }
            if(polCount == 0)
            {
                for(int a = 0; a < city.Population; a++)
                {
                    Politician pol = Instantiate<Politician>(politicianPrefab, transform.Find("Politicians").transform);
                    politicians.Add(pol);
                    pol.CityState = this;
                }

            }
            UpdatePoliticalLandscape();
            UpdateCityState();
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
