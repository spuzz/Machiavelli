using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{

    [SerializeField] List<Player> players = new List<Player>();
    [SerializeField] GameObject citiesObject;
    [SerializeField] GameObject playersObject;
    [SerializeField] AIPlayer aiPlayerPrefab;

    [SerializeField] List<Sprite> possibleCityStateSymbols;
    [SerializeField] List<PlayerColour> possiblePlayerColors;
    [SerializeField] HexMapCamera hexMapCamera;
    [SerializeField] HUD hud;
    [SerializeField] HumanPlayer humanPlayer;
    [SerializeField] VisionSystem visionSystem;
    [SerializeField] HexUnitActionController hexUnitActionController;
    int turn = 1;

    List<City> cities = new List<City>();
    List<AIPlayer> playersTakingturns = new List<AIPlayer>();
    List<int> usedSymbols = new List<int>();
    List<int> usedColors = new List<int>();

    public City cityPrefab;
    public Agent agentPrefab;
    public CombatUnit combatUnitPrefab;

    public Dictionary<string, AgentConfig> agentConfigs = new Dictionary<string, AgentConfig>();
    public Dictionary<string, CombatUnitConfig> combatUnitConfigs = new Dictionary<string, CombatUnitConfig>();
    public Dictionary<string, BuildConfig> buildConfigs = new Dictionary<string, BuildConfig>();
    HexGrid hexGrid;

    private bool turnOver = false;
    static GameController instance;
    public HumanPlayer HumanPlayer
    {
        get { return humanPlayer; }
    }

    public VisionSystem VisionSystem
    {
        get
        {
            return visionSystem;
        }

        set
        {
            visionSystem = value;
        }
    }

    public HexUnitActionController HexUnitActionController
    {
        get
        {
            return hexUnitActionController;
        }

        set
        {
            hexUnitActionController = value;
        }
    }

    public bool TurnOver
    {
        get
        {
            return turnOver;
        }

        set
        {
            turnOver = value;
        }
    }

    public static GameController Instance
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType<GameController>();
            }
            return instance;
        }

    }

    public int GetTurn()
    {
        return turn;
    }

    public City GetCity(HexCell hexCell)
    {
        return cities.Find(c => c.GetHexCell() == hexCell);
    }
    public City GetCity(int id)
    {
        return cities.Find(c => c.CityID == id);
    }

    public IEnumerable<City> GetCities()
    {
        return cities;
    }

    public List<string> GetCityNames()
    {
        List<string> names = new List<string>();
        foreach (City city in cities)
        {
            names.Add(city.CityID.ToString());
        }
        return names;
    }

    public int GetCityCount()
    {
        return cities.Count();
    }
    public AgentConfig GetAgentConfig(string name)
    {
        if(!agentConfigs.Keys.Contains(name))
        {
            return null;
        }
        return agentConfigs[name];
    }

    public CombatUnitConfig GetCombatUnitConfig(string name)
    {
        if (!combatUnitConfigs.Keys.Contains(name))
        {
            return null;
        }
        return combatUnitConfigs[name];
    }

    public BuildConfig GetBuildConfig(string name)
    {
        if (!buildConfigs.Keys.Contains(name))
        {
            return null;
        }
        return buildConfigs[name];
    }

    public void Awake()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        AgentConfig[] d = Resources.LoadAll<AgentConfig>("AgentConfigs");
        foreach(AgentConfig agentConfig in d)
        {
            agentConfigs.Add(agentConfig.Name, agentConfig);
        }

        CombatUnitConfig[] c = Resources.LoadAll<CombatUnitConfig>("CombatUnitConfigs");
        foreach (CombatUnitConfig combatUnitConfig in c)
        {
            combatUnitConfigs.Add(combatUnitConfig.Name, combatUnitConfig);
        }

        BuildConfig[] e = Resources.LoadAll<BuildConfig>("BuildConfigs");
        foreach (BuildConfig buildConfig in e)
        {
            buildConfigs.Add(buildConfig.Name, buildConfig);
        }
    }
    void Start()
    {
        turn = 1;
        humanPlayer.ColorID = GetNewPlayerColor();
    }
    public void EndPlayerTurn()
    {
        TurnOver = true;
        humanPlayer.EndTurn();
        StartCoroutine(NewTurn());

    }

    IEnumerator NewTurn()
    {
        foreach (AIPlayer aiPlayer in players)
        {
            aiPlayer.StartTurn();
        }
        playersTakingturns.Clear();
        foreach (AIPlayer aiPlayer in players)
        {
            if (aiPlayer)
            {
                playersTakingturns.Add(aiPlayer);
                yield return StartCoroutine(aiPlayer.TakeTurn());
            }

        }
        while (playersTakingturns.Count > 0 || !hexUnitActionController.FinishedActions())
        {
            yield return new WaitForEndOfFrame();
        }

        if (humanPlayer.Alive == false)
        {
            EndGame();
        }
        CheckWinner();
        TurnOver = false;
        humanPlayer.StartTurn();

        hud.StartTurn();
        turn += 1;
    }


    public void PlayerTurnFinished(AIPlayer player)
    {
        playersTakingturns.Remove(player);
    }

    public void EndGame()
    {
        SceneManager.LoadScene(0);
    }

    public void CheckWinner()
    {
        //if(HumanPlayer.cities.Count == cities.FindAll(c => c.Alive).Count)
        //{
        //    Debug.Log("Winner");
        //    EndGame();
        //}
        //else
        //{
        //    foreach(Player player in players)
        //    {
        //        if (player.cities.Count == cities.FindAll(c => c.Alive).Count)
        //        {
        //            Debug.Log("Loser");
        //            EndGame();
        //        }
        //    }
        //}

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

    public City CreateCity(HexCell cell, bool createCityState = false)
    {
        City city = Instantiate(cityPrefab);
        if (createCityState)
        {
            city.GetComponent<CityState>().SymbolID = PickSymbol();

        }
        city.transform.localPosition = HexMetrics.Perturb(cell.Position);
        city.SetHexCell(cell);
        city.transform.SetParent(citiesObject.transform);
        city.HexVision.AddVisibleObject(city.CityUI.gameObject);
        city.UpdateHealthBar();
        city.UpdateCityBar();
        cities.Add(city);
        hexGrid.AddCity(city);

        return city;
    }

    public HexUnit CreateAgent(AgentConfig agentConfig, HexCell cell, Player player)
    {
        HexUnit hexUnit = Instantiate(agentPrefab).GetComponent<HexUnit>();
        Agent agent = hexUnit.GetComponent<Agent>();
        agent.SetAgentConfig(agentConfig);
        hexGrid.AddUnit(hexUnit);
        hexUnit.Grid = hexGrid;
        hexUnit.Location = cell;
        hexUnit.Orientation = Random.Range(0f, 360f);
        agent.HexUnitType = Unit.UnitType.AGENT;

        if (player.IsHuman)
        {
            hexUnit.Controllable = true;
        }

        player.AddAgent(agent);
        return hexUnit;
    }

    public HexUnit CreateAgent(string agentConfig, HexCell cell, Player player)
    {
        HexUnit hexUnit = Instantiate(agentPrefab).GetComponent<HexUnit>();
        Agent agent = hexUnit.GetComponent<Agent>();
        agent.SetAgentConfig(GetAgentConfig(agentConfig));
        hexGrid.AddUnit(hexUnit);
        hexUnit.Grid = hexGrid;
        hexUnit.Location = cell;
        hexUnit.Orientation = Random.Range(0f, 360f);
        agent.HexUnitType = Unit.UnitType.AGENT;
       
        if (player.IsHuman)
        {
            hexUnit.Controllable = true;
        }

        player.AddAgent(agent);
        return hexUnit;
    }

    public HexUnit CreateCityStateUnit(CombatUnitConfig combatUnitConfig, HexCell cell, City city)
    {
        HexUnit hexUnit = Instantiate(combatUnitPrefab).GetComponent<HexUnit>();
        CombatUnit combatUnit = hexUnit.GetComponent<CombatUnit>();
        combatUnit.SetCombatUnitConfig(combatUnitConfig);
        hexUnit.Grid = hexGrid;
        hexUnit.Location = cell;
        hexUnit.Orientation = Random.Range(0f, 360f);
        combatUnit.HexUnitType = Unit.UnitType.COMBAT;
        hexGrid.AddUnit(hexUnit);
        city.AddUnit(hexUnit.GetComponent<CombatUnit>());
        if(city.Player && city.Player.IsHuman)
        {
            hexUnit.Controllable = true;
        }
        return hexUnit;
    }
    public HexUnit CreateCityStateUnit(string combatUnitConfig, HexCell cell, City city)
    {
        
        HexUnit hexUnit = Instantiate(combatUnitPrefab).GetComponent<HexUnit>();
        CombatUnit combatUnit = hexUnit.GetComponent<CombatUnit>();
        combatUnit.SetCombatUnitConfig(GetCombatUnitConfig(combatUnitConfig));
        hexUnit.Grid = hexGrid;
        hexUnit.Location = cell;
        hexUnit.Orientation = Random.Range(0f, 360f);
        combatUnit.HexUnitType = Unit.UnitType.COMBAT;
        hexGrid.AddUnit(hexUnit);
        city.AddUnit(hexUnit.GetComponent<CombatUnit>());
        return hexUnit;
    }


    public CityBuilding CreateCityPlayerBuilding(CityPlayerBuildConfig cityPlayerBuildConfig)
    {
        CityBuilding cityBuilding = Instantiate(cityPlayerBuildConfig.BuildPrefab, transform).GetComponent<CityBuilding>();
        cityBuilding.BuildConfig = cityPlayerBuildConfig;
        return cityBuilding;
    }


    public void DestroyUnit(Unit unit)
    {
        if(hud.Unit == unit)
        {
            hexGrid.ClearPath();
        }
        KillUnit(unit);
        unit.HexUnit.DestroyHexUnit();
    }

    public void KillUnit(Unit unit)
    {
        hexGrid.RemoveUnit(unit.HexUnit);
        if(unit.Alive)
        {
            unit.KillUnit();
        }

        if(unit.HexUnitType == Unit.UnitType.AGENT)
        {
            unit.GetComponent<Agent>().GetPlayer().RemoveAgent(unit.GetComponent<Agent>());
        }

        if (unit.HexUnitType == Unit.UnitType.COMBAT && unit.GetCityOwner())
        {
            unit.GetCityOwner().RemoveUnit(unit as CombatUnit);
            CombatUnit combatUnit = unit.GetComponent<CombatUnit>();
        }
    }

    public void AnimateAndDestroyUnit(Unit unit)
    {
        unit.HexUnit.DieAnimationAndRemove();
    }

    public int CityStateCount()
    {
        return cities.Count;
    }

    public List<string> CityStateNames()
    {
        List<string> names = new List<string>();
        foreach(City city in cities)
        {
            names.Add(city.GetCityState().CityStateID.ToString());
        }
        return names;
    }

    public AIPlayer CreateAIPlayer()
    {
        AIPlayer instance = Instantiate(aiPlayerPrefab);
        instance.transform.SetParent(playersObject.transform);
        instance.ColorID = GetNewPlayerColor();
        players.Add(instance);
        
        return instance;
    }

    public AIPlayer CreateAIPlayer(int colorID)
    {
        AIPlayer instance = Instantiate(aiPlayerPrefab);
        instance.transform.SetParent(playersObject.transform);
        instance.ColorID = GetNewPlayerColor(colorID);
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
    public List<Player> GetPlayers()
    {
        List<Player> playerList = new List<Player>();
        playerList.Add(humanPlayer);
        foreach(Player player in players)
        {
            playerList.Add(player);
        }
        return playerList;
    }


    public int GetNewPlayerColor()
    {
        if (usedColors.Count >= possiblePlayerColors.Count)
        {
            throw new Exception("Too many players");
        }
        List<int> unusedColors = new List<int>();
        for (int a = 0; a < possiblePlayerColors.Count; a++)
        {
            if (!usedColors.Contains(a))
            {
                unusedColors.Add(a);
            }
        }
        int colorIndex = IListExtensions.RandomElement(unusedColors);
        usedColors.Add(colorIndex);
        return colorIndex;
    }

    public int ResetPlayerColour(int colorID)
    {
        usedColors.Remove(colorID);
        return GetNewPlayerColor();
    }

    public int ResetPlayerColour(int colorID, int newID)
    {
        usedColors.Remove(colorID);
        return GetNewPlayerColor(newID);
    }

    public int GetNewPlayerColor(int colorID)
    {
        if(colorID >= possiblePlayerColors.Count || colorID < 0 || usedColors.Contains(colorID))
        {
            return GetNewPlayerColor();
        }
        else
        {
            usedColors.Add(colorID);
            return colorID;
        }
    }


    public PlayerColour GetPlayerColor(int colorID)
    {
        PlayerColour colour = possiblePlayerColors.Find(c => c.Id == colorID);
        if(!colour)
        {
            throw new ArgumentException("Invalid color");
        }
        return colour;
    }

    public int PickSymbol()
    {
        if(usedSymbols.Count >= possibleCityStateSymbols.Count)
        {
            throw new Exception("Too many city states");
        }
        List<int> unusedSymbols = new List<int>();
        for(int a=0;a<possibleCityStateSymbols.Count;a++)
        {
            if(!usedSymbols.Contains(a))
            {
                unusedSymbols.Add(a);
            }
        }
        int symbolID = IListExtensions.RandomElement(unusedSymbols);
        usedSymbols.Add(symbolID);
        return symbolID;
    }

    public int PickSymbol(int symbolID)
    {
        if (symbolID >= possibleCityStateSymbols.Count || symbolID < 0)
        {
            throw new Exception("Invalid Symbol");
        }

        if (usedSymbols.Contains(symbolID))
        {
            return PickSymbol();
        }
        else
        {
            usedSymbols.Add(symbolID);
            return symbolID;
        }
    }

    public Sprite GetCityStateSymbol(int symbolID)
    {
        return possibleCityStateSymbols[symbolID];
    }
    public void CentreMap()
    {

        Agent agent = humanPlayer.GetAgents().FirstOrDefault();
        if (agent)
        {
            hexMapCamera.MoveCamera(agent.HexUnit.Location);
        }
        else
        {
            HexMapCamera.ValidatePosition();
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


    public void ClearCitiesAndStates()
    {
        
        foreach (City city in cities)
        {
            city.DestroyCity();
        }
        cities.Clear();

        CityState.cityStateIDCounter = 1;
        City.cityIDCounter = 1;
        usedSymbols.Clear();
    }

    public void ClearPlayers()
    {
        foreach (AIPlayer player in players)
        {
            player.DestroyPlayer();
        }
        humanPlayer.ClearExploredCells();
        humanPlayer.ClearAgents();
        humanPlayer.Gold = GameConsts.startingGold;
        players.Clear();
        Player.nextPlayerNumber = 1;
        usedColors.Clear();
    }

    public void ResetHumanPlayer()
    {
        humanPlayer.ColorID = GetNewPlayerColor();
    }
    public void ClearHud()
    {
        hud.ClearUI();
    }
    public void Save(BinaryWriter writer)
    {
        writer.Write(turn);
        HumanPlayer.Save(writer);
        writer.Write(players.Count);
        foreach (AIPlayer aiPlayer in players)
        {
            aiPlayer.Save(writer);
        }

        writer.Write(cities.Count);
        foreach (City city in cities)
        {
            city.Save(writer);
        }


    }

    public void Load(BinaryReader reader, int header, HexGrid hexGrid)
    {
        if (header >= 3)
        {
            turn = reader.ReadInt32();
        }

        HumanPlayer.Load(reader, this, hexGrid, header);
        if (header >= 2)
        {
            int playerCount = reader.ReadInt32();
            for (int i = 0; i < playerCount; i++)
            {
                AIPlayer.Load(reader, this, hexGrid, header);
            }
        }

        int cityCount = reader.ReadInt32();
        for (int i = 0; i < cityCount; i++)
        {
            City.Load(reader, this, hexGrid, header);
        }

    }
}
