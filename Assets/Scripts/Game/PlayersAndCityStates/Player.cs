using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class Player : MonoBehaviour {
    public static int nextPlayerNumber = 1;

    [SerializeField] int gold = 100;
    [SerializeField] List<CityPlayerBuildConfig> cityPlayerBuildConfigs;
    [SerializeField] PlayerAgentTracker playerAgentTracker;
    [SerializeField] GameObject textEffect;
    private int colorID;
    public GameObject operationCenterTransformParent;
    int playerNumber = 0;
    bool isHuman = false;
    bool alive = true;
    public List<Agent> agents = new List<Agent>();
    public List<CombatUnit> mercenaries = new List<CombatUnit>();
    public List<CityState> cityStates = new List<CityState>();
    public List<City> cities = new List<City>();
    public List<OperationCentre> opCentres = new List<OperationCentre>();
    public Dictionary<HexCell, int> visibleCells = new Dictionary<HexCell, int>();
    public List<HexCell> exploredCells = new List<HexCell>();

    protected GameController gameController;
    private int goldPerTurn = 5;

    public delegate void OnInfoChange(Player player);
    public event OnInfoChange onInfoChange;

    protected Dictionary<City,CityBonus> cityBonuses = new Dictionary<City, CityBonus>();
    protected Dictionary<City, AgentConfig> randomisedBonus = new Dictionary<City, AgentConfig>();
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
            return goldPerTurn;
        }

        set
        {
            goldPerTurn = value;
            NotifyInfoChange();
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

    public PlayerAgentTracker PlayerAgentTracker
    {
        get
        {
            return playerAgentTracker;
        }

        set
        {
            playerAgentTracker = value;
        }
    }

    public void AddVisibleCell(HexCell cell)
    {
        if(!exploredCells.Contains(cell))
        {
            exploredCells.Add(cell);
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
            if(visibleCells[cell] <= 0)
            {
                visibleCells.Remove(cell);
            }
        }

    }

    public void AddCityState(CityState cityState)
    {
        cityStates.Add(cityState);
        UpdateResources();
        cityState.onInfoChange += cityStateChange;
    }

    private void cityStateChange(CityState cityState)
    {
        UpdateResources();
    }

    public void AddCity(City city)
    {
        cities.Add(city);
        cityBonuses.Add(city,city.PlayerCityBonus);
        AgentConfig bonusConfig;
        if(city.PlayerCityBonus.AgentCapIncrease == null)
        {
            bonusConfig = PlayerAgentTracker.GetRandomCappedAgent();
            randomisedBonus.Add(city, bonusConfig);
            PlayerAgentTracker.IncreaseCap(bonusConfig);
        }
        else
        {
            bonusConfig = city.PlayerCityBonus.AgentCapIncrease;
            PlayerAgentTracker.IncreaseCap(bonusConfig);
        }
        if (isHuman)
        {
            ShowBonusText(city.PlayerCityBonus, bonusConfig, city.GetHexCell());
        }
        UpdateResources();
    }
    public void RemoveCity(City city)
    {
        cities.Remove(city);
        cityBonuses.Remove(city);
        AgentConfig bonusConfig;
        if (city.PlayerCityBonus.AgentCapIncrease == null)
        {
            bonusConfig = randomisedBonus[city];
            randomisedBonus.Remove(city);
            PlayerAgentTracker.DecreaseCap(bonusConfig);
        }
        else
        {
            bonusConfig = city.PlayerCityBonus.AgentCapIncrease;
            PlayerAgentTracker.DecreaseCap(bonusConfig);
        }
        if(isHuman)
        {
            ShowBonusText(city.PlayerCityBonus, bonusConfig, city.GetHexCell());
        }

        UpdateResources();
    }

    private void ShowBonusText(CityBonus bonus, AgentConfig bonusConfig, HexCell cell)
    {

        cell.TextEffectHandler.AddTextEffect("+1 " + bonusConfig.Name, cell.transform, Color.yellow);
        cell.TextEffectHandler.AddTextEffect("+" + bonus.GoldBonus + " Gold ", cell.transform, Color.yellow);
    }

    private void ShowRemoveBonusText(CityBonus bonus, AgentConfig bonusConfig, HexCell cell)
    {
        cell.TextEffectHandler.AddTextEffect("-1 " + bonusConfig.Name, cell.transform, Color.yellow);
        cell.TextEffectHandler.AddTextEffect("-" + bonus.GoldBonus + " Gold ", cell.transform, Color.yellow);
    }

    public void RemoveCityState(CityState cityState)
    {
        cityStates.Remove(cityState);
        cityState.onInfoChange -= cityStateChange;
        UpdateResources();
    }

    public IEnumerable<CityState> GetCityStates()
    {
        return cityStates;
    }

    public IEnumerable<Agent> GetAgents()
    {
        return agents;
    }

    public void UseAgentSpace(AgentConfig config)
    {
        PlayerAgentTracker.AddAgent(config);
    }

    public void RemoveAgentSpace(AgentConfig config)
    {
        PlayerAgentTracker.AddAgent(config);
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
        PlayerAgentTracker.RemoveAgent(agent.GetAgentConfig());
        agents.Remove(agent);
        NotifyInfoChange();
    }

    public void ClearAgents()
    {
        agents.Clear();
        NotifyInfoChange();
    }

    public IEnumerable<CombatUnit> GetMercenaries()
    {
        return mercenaries;
    }

    public void AddMercenary(CombatUnit mercenary)
    {
        if (isHuman)
        {
            mercenary.HexVision.HasVision = true;
        }

        mercenary.SetPlayer(this);
        mercenaries.Add(mercenary);
        NotifyInfoChange();
    }

    public void RemoveMercenary(CombatUnit mercenary)
    {
        mercenary.HexVision.HasVision = false;
        mercenaries.Remove(mercenary);
        NotifyInfoChange();
    }

    public void ClearMercenaries()
    {
        mercenaries.Clear();
        NotifyInfoChange();
    }

    public IEnumerable<OperationCentre> GetOperationCentres()
    {
        return opCentres;
    }

    public void CreateOperationCentre(HexCell cell)
    {
        gameController.CreateOperationCentre(cell, this);
    }
    public void AddOperationCentre(OperationCentre operationCentre)
    {
        opCentres.Add(operationCentre);
        if (isHuman)
        {
            operationCentre.HexVision.HasVision = true;
        }
    }

    public void RemoveOperationCentre(OperationCentre operationCentre)
    {
        opCentres.Remove(operationCentre);
    }

    public void ClearOperationCentres()
    {
        foreach (OperationCentre opCentre in opCentres)
        {
            opCentre.DestroyOperationCentre();
        }
        opCentres.Clear();
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
    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        playerNumber = nextPlayerNumber;
        nextPlayerNumber++;
    }


    public void StartTurn()
    {
        if(opCentres.Count == 0)
        {
            PlayerDefeated();
        }
        agents.RemoveAll(c => c.Alive == false);
        foreach (Agent agent in agents)
        {
            agent.StartTurn();
        }

        foreach (CombatUnit merc in mercenaries)
        {
            merc.StartTurn();
        }

        foreach (OperationCentre opCentre in opCentres)
        {
            opCentre.StartTurn();
        }

        UpdateResources();
        gold += goldPerTurn;
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

        foreach (CombatUnit merc in mercenaries)
        {
            if (merc.CheckPath())
            {
                merc.MoveUnit();
            }

        }
        mercenaries.RemoveAll(c => c.Alive == false);
    }

    private void UpdateResources()
    {
        goldPerTurn = 5;
        foreach (City city in cities)
        {
            goldPerTurn += city.GetPlayerIncome();
        }

        foreach (CityBonus bonus in cityBonuses.Values)
        {
            goldPerTurn += bonus.GoldBonus;
        }
        foreach(CityState cityState in cityStates)
        {
            cityState.GetPlayerIncome();
        }
        NotifyInfoChange();
    }

    public bool CanHireAgent(AgentConfig agentConfig)
    {
        return PlayerAgentTracker.CanRecruit(agentConfig);
    }

    public IEnumerable<AgentConfig> GetCappedAgents()
    {
        return PlayerAgentTracker.GetCappedAgents();
    }

    public void SavePlayer(BinaryWriter writer)
    {
        writer.Write(opCentres.Count);
        foreach (OperationCentre opCentre in opCentres)
        {
            opCentre.Save(writer);
        }

        writer.Write(gold);
    }

    public void LoadPlayer(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {
        if (header >= 3)
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
            if (header < 6)
            {
                Color playerColor = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1.0f);
            }

        }

        if (header >= 5)
        {
            int opCentreCount = reader.ReadInt32();
            for (int i = 0; i < opCentreCount; i++)
            {
                OperationCentre.Load(reader,gameController,hexGrid,this,header);
            }

            gold = reader.ReadInt32();

        }

    }
    public abstract void Save(BinaryWriter writer);

    public void DestroyPlayer()
    {
        foreach (Agent agent in agents)
        {
            Destroy(agent.gameObject);
        }

        foreach (CombatUnit merc in mercenaries)
        {
            Destroy(merc.gameObject);
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
