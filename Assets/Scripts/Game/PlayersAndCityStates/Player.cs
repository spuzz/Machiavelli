using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public abstract class Player : MonoBehaviour {
    public static int nextPlayerNumber = 1;

    // External Components
    protected GameController gameController;
    // Internal Components
    [SerializeField] ScienceController scienceController;
    // Attributes
    [SerializeField] int gold = 100;
    [SerializeField] int politicalCapital = 0;
    [SerializeField] List<CityPlayerBuildConfig> cityPlayerBuildConfigs;
    [SerializeField] List<AgentBuildConfig> agentBuildConfigs;
    [SerializeField] List<CombatUnitBuildConfig> combatUnitBuildConfigs;
    private int colorID;
    int playerNumber = 0;
    bool isHuman = false;
    bool alive = true;

    public List<Agent> agents = new List<Agent>();
    public List<City> cities = new List<City>();
    public Dictionary<HexCell, int> visibleCells = new Dictionary<HexCell, int>();
    public List<HexCell> exploredCells = new List<HexCell>();
    List<City> visibleCities = new List<City>();
    protected List<CityState> cityStatesMet = new List<CityState>();


    public delegate void OnInfoChange(Player player);
    public event OnInfoChange onInfoChange;

    public bool IsHuman
    {
        get { return isHuman; }
        set { isHuman = value; }
    }
    public int PlayerNumber
    {
        get { return playerNumber;  }
        set { playerNumber = value;  }
    }

    public PlayerColour GetColour()
    {
        return gameController.GetPlayerColor(ColorID);
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

    public int GoldPerTurn
    {
        get
        {
            return CalculateGoldPerTurn();
        }

    }

    public int PCPerTurn
    {
        get
        {
            return CalculatePCPerTurn();
        }

    }

    public int ColorID
    {
        get
        {
            return colorID;
        }

        set
        {
            colorID = value;
        }
    }

    public ScienceController ScienceController
    {
        get
        {
            return scienceController;
        }

        set
        {
            scienceController = value;
        }
    }

    public int PoliticalCapital
    {
        get
        {
            return politicalCapital;
        }

        set
        {
            politicalCapital = value;
        }
    }

    private int CalculateGoldPerTurn()
    {
        int goldPerTurn = GameConsts.HQGold;
        foreach(City city in cities)
        {
            goldPerTurn += city.GetIncomePerTurn();
        }
        return goldPerTurn;
    }

    private int CalculatePCPerTurn()
    {
        int pcPerTurn = GameConsts.HQPC;
        foreach (City city in cities)
        {
            pcPerTurn += city.GetPoliticalCapitalPerTurn();
        }
        return pcPerTurn;
    }


    public virtual void AddVisibleCell(HexCell cell)
    {
        if(!exploredCells.Contains(cell))
        {
            exploredCells.Add(cell);
            if (cell.City)
            {
                visibleCities.Add(cell.City);
                if(!cityStatesMet.Contains(cell.City.GetCityState()))
                {
                    cityStatesMet.Add(cell.City.GetCityState());
                }
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
    public IEnumerable<City> GetEnemyCities()
    {
        return visibleCities.FindAll(c => c.GetCityState() != this);
    }
    public List<City> GetEnemyCitiesOrderByDistance(HexCoordinates unitCoordinates)
    {
        return visibleCities.FindAll(c => !c.GetCityState()).OrderBy(c => c.GetHexCell().coordinates.DistanceTo(unitCoordinates)).ToList();
    }

    public List<City> GetFriendlyCitiesOrderByDistance(HexCoordinates unitCoordinates)
    {
        return visibleCities.FindAll(c => c.GetCityState()).OrderBy(c => c.GetHexCell().coordinates.DistanceTo(unitCoordinates)).ToList();
    }

    public void RemoveVisibleCell(HexCell cell)
    {
        if (visibleCells.ContainsKey(cell))
        {
            visibleCells[cell] -= 1;
            if(visibleCells[cell] <= 0)
            {
                visibleCells.Remove(cell);
            }
        }

    }

    public void AddCity(City city)
    {
        cities.Add(city);
        NotifyInfoChange();
    }

    public virtual bool IsFriend(CityState city)
    {
        return false;
    }

    public void RemoveCity(City city)
    {
        cities.Remove(city);
        NotifyInfoChange();
    }

    public IEnumerable<City> GetCities()
    {
        return cities;
    }

    public IEnumerable<Agent> GetAgents()
    {
        return agents;
    }


    public virtual void AddAgent(Agent agent)
    {
        if(isHuman)
        {
            agent.HexVision.HasVision = true;
        }

        agent.SetPlayer(this);
        agents.Add(agent);
        NotifyInfoChange();
    }

    public void RemoveAgent(Agent agent)
    {
        agents.Remove(agent);
        NotifyInfoChange();
    }

    public void ClearAgents()
    {
        agents.Clear();
        NotifyInfoChange();
    }


    public void ClearExploredCells()
    {
        exploredCells.Clear();
    }
    
    public IEnumerable<CityPlayerBuildConfig> GetCityPlayerBuildConfigs()
    {
        return cityPlayerBuildConfigs;
    }

    public CityPlayerBuildConfig GetCityPlayerBuildConfig(int id)
    {
        return cityPlayerBuildConfigs[id];
    }

    public CityPlayerBuildConfig GetCityPlayerBuildConfig(string name)
    {
        return cityPlayerBuildConfigs.Find(c => c.Name.CompareTo(name) ==0);
    }

    public IEnumerable<AgentBuildConfig> GetAgentBuildConfigs()
    {
        return agentBuildConfigs;
    }

    public AgentBuildConfig GetAgentBuildConfig(int id)
    {
        return agentBuildConfigs[id];
    }

    public AgentBuildConfig GetAgentBuildConfig(string name)
    {
        return agentBuildConfigs.Find(c => c.Name.CompareTo(name) == 0);
    }

    public IEnumerable<CombatUnitBuildConfig> GetCombatUnitBuildConfigs()
    {
        return combatUnitBuildConfigs;
    }

    public CombatUnitBuildConfig GetCombatUnitBuildConfig(int id)
    {
        return combatUnitBuildConfigs[id];
    }

    public CombatUnitBuildConfig GetCombatUnitBuildConfig(string name)
    {
        return combatUnitBuildConfigs.Find(c => c.Name.CompareTo(name) == 0);
    }

    private void SortBuildOptions()
    {
        cityPlayerBuildConfigs.Sort(delegate (CityPlayerBuildConfig x, CityPlayerBuildConfig y)
        {
            int a = x.Name.CompareTo(y.Name);
            return a;
        });

        combatUnitBuildConfigs.Sort(delegate (CombatUnitBuildConfig x, CombatUnitBuildConfig y)
        {
            int a = x.Name.CompareTo(y.Name);
            return a;
        });

        agentBuildConfigs.Sort(delegate (AgentBuildConfig x, AgentBuildConfig y)
        {
            int a = x.Name.CompareTo(y.Name);
            return a;
        });
    }

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        playerNumber = nextPlayerNumber;
        nextPlayerNumber++;
    }

    private void Start()
    {
        foreach(CityPlayerBuildConfig config in gameController.DefaultBuildings)
        {
            cityPlayerBuildConfigs.Add(config);
        }

        foreach (AgentBuildConfig config in gameController.DefaultAgents)
        {
            agentBuildConfigs.Add(config);
        }

        foreach (CombatUnitBuildConfig config in gameController.DefaultCombatUnits)
        {
            combatUnitBuildConfigs.Add(config);
        }
    }

    public void StartTurn()
    {

        agents.RemoveAll(c => c.Alive == false);
        foreach (Agent agent in agents)
        {
            agent.StartTurn();
        }

        foreach (City city in cities)
        {
            city.StartTurn();
        }
        gold += GoldPerTurn;
        politicalCapital += PCPerTurn;
        ScienceController.StartTurn();
        NotifyInfoChange();
    }

    public abstract void PlayerDefeated();

    public void EndTurn()
    {
        foreach (Agent agent in agents)
        {
            if(agent.CheckPath())
            {
                agent.MoveUnit();
            }
            
        }
        agents.RemoveAll(c => c.Alive == false);

    }

    public int GetScience()
    {
        int science = 0;
        foreach(City city in cities)
        {
            science += city.CityResouceController.GetScience();
        }
        return science;
    }

    public void AddResearch(Research research)
    {
        foreach(CityPlayerBuildConfig config in research.GetBuildingConfigs())
        {
            cityPlayerBuildConfigs.Add(config);
        }

        foreach (AgentBuildConfig config in research.GetAgentConfigs())
        {
            agentBuildConfigs.Add(config);
        }

        foreach (CombatUnitBuildConfig config in research.GetCombatUnitConfigs())
        {
            combatUnitBuildConfigs.Add(config);
        }
        NotifyInfoChange();
    }
    public void SavePlayer(BinaryWriter writer)
    {

        writer.Write(gold);
        writer.Write(politicalCapital);
    }

    public void LoadPlayer(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {

        int exploredCellCount = reader.ReadInt32();
        for (int i = 0; i < exploredCellCount; i++)
        {
            HexCell cell = hexGrid.GetCell(reader.ReadInt32());
            if (!exploredCells.Contains(cell))
            {
                exploredCells.Add(cell);
            }

        }

        Gold = reader.ReadInt32();
        if(header >= 2)
        {
            politicalCapital = reader.ReadInt32();
        }

    }
    public abstract void Save(BinaryWriter writer);

    public void DestroyPlayer()
    {
        foreach (Agent agent in agents)
        {
            Destroy(agent.gameObject);
        }

        Destroy(gameObject);

    }

    public void NotifyInfoChange()
    {
        if(onInfoChange != null)
        {
            onInfoChange(this);
        }
    }

}
