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
    [SerializeField] GameObject cityStatesObject;
    [SerializeField] GameObject citiesObject;
    [SerializeField] GameObject playersObject;
    [SerializeField] AIPlayer aiPlayerPrefab;

    [SerializeField] List<Color> possibleCityStateColors;
    [SerializeField] List<Color> possiblePlayerColors;
    [SerializeField] HexMapCamera hexMapCamera;
    [SerializeField] HUD hud;
    [SerializeField] HumanPlayer humanPlayer;
    [SerializeField] VisionSystem visionSystem;
    public CityState cityStatePrefab;
    int turn = 1;

    List<CityState> cityStates = new List<CityState>();
    List<City> cities = new List<City>();
    List<OperationCentre> opCentres = new List<OperationCentre>();
    List<CityState> cityStatesTakingturns = new List<CityState>();
    List<AIPlayer> playersTakingturns = new List<AIPlayer>();

    public OperationCentre opCentrePrefab;
    public City cityPrefab;
    public Agent agentPrefab;
    public CombatUnit combatUnitPrefab;
    public CityStateBuilding cityStateBuildingPrefab;

    public Dictionary<string, AgentConfig> agentConfigs = new Dictionary<string, AgentConfig>();
    public Dictionary<string, CombatUnitConfig> combatUnitConfigs = new Dictionary<string, CombatUnitConfig>();
    public Dictionary<string, BuildConfig> buildConfigs = new Dictionary<string, BuildConfig>();
    HexGrid hexGrid;
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

    public int GetTurn()
    {
        return turn;
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
        humanPlayer.Color = GetNewPlayerColor();
    }
    public void EndPlayerTurn()
    {
        humanPlayer.EndTurn();
        StartCoroutine(NewTurn());

    }

    IEnumerator NewTurn()
    {
        playersTakingturns.Clear();
        foreach (AIPlayer aiPlayer in players)
        {
            if (aiPlayer)
            {
                playersTakingturns.Add(aiPlayer);
                yield return StartCoroutine(aiPlayer.TakeTurn());
            }

        }
        while (playersTakingturns.Count > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        cityStatesTakingturns.Clear();
        foreach (CityState cityState in cityStates)
        {
            if(cityState)
            {
                cityStatesTakingturns.Add(cityState);
                yield return StartCoroutine(cityState.TakeTurn());
            }
            
        }
        while(cityStatesTakingturns.Count > 0)
        {
            yield return new WaitForEndOfFrame();
        }

        foreach (CityState cityState in cityStates)
        {
            cityState.StartTurn();
        }
        foreach (AIPlayer aiPlayer in players)
        {
            aiPlayer.StartTurn();
        }

        if (humanPlayer.Alive == false)
        {
            EndGame();
        }
        humanPlayer.StartTurn();

        hud.StartTurn();
        turn += 1;
    }

    public void CityStateTurnFinished(CityState cityState)
    {
        cityStatesTakingturns.Remove(cityState);
    }

    public void PlayerTurnFinished(AIPlayer player)
    {
        playersTakingturns.Remove(player);
    }

    public void EndGame()
    {
        SceneManager.LoadScene(0);
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


    public OperationCentre CreateOperationCentre(HexCell cell, Player player)
    {
        if(!cell.OpCentre)
        {
            OperationCentre instance = Instantiate(opCentrePrefab);
            instance.transform.localPosition = HexMetrics.Perturb(cell.Position);
            instance.Location = cell;
            instance.Player = player;
            instance.Player.AddOperationCentre(instance);
            instance.HexVision.AddVisibleObject(instance.OpCentreUI.gameObject);
            cell.SpecialIndex = 3;
            hexGrid.AddOperationCentre(instance);
            instance.BuildCommandCentre();
            return instance;
        }
        return null;

    }

    public void DestroyOperationCentre(OperationCentre opCentre)
    {
        hexGrid.RemoveOperationCentre(opCentre);
        opCentre.Player.RemoveOperationCentre(opCentre);
        opCentre.DestroyOperationCentre();
    }
    public CityState CreateCityState()
    {
        CityState instance = Instantiate(cityStatePrefab);
        instance.transform.SetParent(cityStatesObject.transform);
        instance.PickColor();
        cityStates.Add(instance);
        return instance;
    }

    public CityState CreateCityState(Color color)
    {
        CityState instance = Instantiate(cityStatePrefab);
        instance.transform.SetParent(cityStatesObject.transform);
        RemoveCityStateColor(color);
        instance.Color = color;
        cityStates.Add(instance);
        return instance;
    }

    public void DestroyCityState(CityState cityState)
    {
        possibleCityStateColors.Add(cityState.Color);
        if(cityState.Player)
        {
            cityState.Player.RemoveCityState(cityState);
        }
        
        cityStates.Remove(cityState);
        cityState.DestroyCityState();
    }

    public void DestroyCity(City city)
    {
        if(city.GetCityState())
        {
            city.GetCityState().RemoveCity(city);
        }
        city.DestroyCity();
    }

    public City CreateCity(HexCell cell, CityState cityState)
    {
        City city = Instantiate(cityPrefab);
        city.transform.localPosition = HexMetrics.Perturb(cell.Position);
        city.SetHexCell(cell);
        city.transform.SetParent(citiesObject.transform);
        city.SetCityState(cityState);
        city.HexVision.AddVisibleObject(city.CityUI.gameObject);
        city.UpdateUI();
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
        hexUnit.HexUnitType = HexUnit.UnitType.AGENT;

        if (player.IsHuman)
        {
            hexUnit.Controllable = true;
        }

        hexUnit.HexUnitType = HexUnit.UnitType.AGENT;
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
        hexUnit.HexUnitType = HexUnit.UnitType.AGENT;
       
        if (player.IsHuman)
        {
            hexUnit.Controllable = true;
        }

        hexUnit.HexUnitType = HexUnit.UnitType.AGENT;
        player.AddAgent(agent);
        return hexUnit;
    }

    public HexUnit CreateCityStateUnit(CombatUnitConfig combatUnitConfig, HexCell cell, int cityStateID)
    {
        CityState cityState = cityStates.Find(c => c.CityStateID == cityStateID);
        HexUnit hexUnit = Instantiate(combatUnitPrefab).GetComponent<HexUnit>();
        CombatUnit combatUnit = hexUnit.GetComponent<CombatUnit>();
        combatUnit.SetCombatUnitConfig(combatUnitConfig);
        hexUnit.Grid = hexGrid;
        hexUnit.Location = cell;
        hexUnit.Orientation = Random.Range(0f, 360f);
        hexUnit.HexUnitType = HexUnit.UnitType.COMBAT;
        hexGrid.AddUnit(hexUnit);
        cityState.AddUnit(hexUnit.GetComponent<CombatUnit>());
        return hexUnit;
    }
    public HexUnit CreateCityStateUnit(string combatUnitConfig, HexCell cell, int cityStateID)
    {
        
        HexUnit hexUnit = Instantiate(combatUnitPrefab).GetComponent<HexUnit>();
        CombatUnit combatUnit = hexUnit.GetComponent<CombatUnit>();
        combatUnit.SetCombatUnitConfig(GetCombatUnitConfig(combatUnitConfig));
        hexUnit.UnitPrefabName = name;
        hexUnit.Grid = hexGrid;
        hexUnit.Location = cell;
        hexUnit.Orientation = Random.Range(0f, 360f);
        hexUnit.HexUnitType = HexUnit.UnitType.COMBAT;
        hexGrid.AddUnit(hexUnit);
        CityState cityState = cityStates.Find(c => c.CityStateID == cityStateID);
        cityState.AddUnit(hexUnit.GetComponent<CombatUnit>());
        return hexUnit;
    }

    public HexUnit CreateMercenary(CombatUnitConfig combatUnitConfig, HexCell cell, Player player, int cityStateID = -1)
    {

        HexUnit hexUnit = Instantiate(combatUnitPrefab).GetComponent<HexUnit>();
        CombatUnit combatUnit = hexUnit.GetComponent<CombatUnit>();
        combatUnit.SetCombatUnitConfig(combatUnitConfig);
        hexUnit.UnitPrefabName = name;
        hexUnit.Grid = hexGrid;
        hexUnit.Location = cell;
        hexUnit.Orientation = Random.Range(0f, 360f);
        hexUnit.HexUnitType = HexUnit.UnitType.COMBAT;
        hexGrid.AddUnit(hexUnit);
        CityState cityState = cityStates.Find(c => c.CityStateID == cityStateID);
        if(cityState)
        {
            combatUnit.SetCityState(cityState);
        }
        if (player.IsHuman)
        {
            hexUnit.Controllable = true;
        }
        combatUnit.Mercenary = true;
        player.AddMercenary(combatUnit);
        return hexUnit;
    }

    public CityStateBuilding CreateCityStateBuilding(CityStateBuildConfig cityStateBuildConfig)
    {
        CityStateBuilding cityStateBuilding = Instantiate(cityStateBuildConfig.BuildPrefab, transform).GetComponent<CityStateBuilding>();
        cityStateBuilding.BuildConfig = cityStateBuildConfig;
        return cityStateBuilding;
    }
    public CityPlayerBuilding CreateCityPlayerBuilding(CityPlayerBuildConfig cityPlayerBuildConfig)
    {
        CityPlayerBuilding cityPlayerBuilding = Instantiate(cityPlayerBuildConfig.BuildPrefab, transform).GetComponent<CityPlayerBuilding>();
        cityPlayerBuilding.BuildConfig = cityPlayerBuildConfig;
        return cityPlayerBuilding;
    }

    public OpCentreBuilding CreateOpCentreBuilding(OpCentreBuildConfig opCentreBuildConfig)
    {
        OpCentreBuilding opCentreBuilding = Instantiate(opCentreBuildConfig.BuildPrefab, transform).GetComponent<OpCentreBuilding>();
        opCentreBuilding.BuildConfig = opCentreBuildConfig;
        return opCentreBuilding;
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
        unit.KillUnit();
        if(unit.HexUnit.HexUnitType == HexUnit.UnitType.AGENT)
        {
            unit.GetComponent<Agent>().GetPlayer().RemoveAgent(unit.GetComponent<Agent>());
        }

        if (unit.HexUnit.HexUnitType == HexUnit.UnitType.COMBAT && unit.CityState)
        {
            unit.CityState.RemoveUnit(unit.GetComponent<CombatUnit>());
        }
    }

    public void AnimateonlyDestroyUnit(Unit unit)
    {
        unit.HexUnit.DieAnimationAndRemove();
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


    public void SetCityStatePlayer(Player player, int cityStateID)
    {
        CityState cityState = cityStates.Find(c => c.CityStateID == cityStateID);
        if (cityState && cityState.Player != player)
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

    public void ReturnPlayerColor(Color color)
    {
        possiblePlayerColors.Add(color);
    }

    public void RemovePlayerColor(Color color)
    {
        possiblePlayerColors.Remove(color);
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
        CityState.cityStateIDCounter = 1;
    }

    public void ClearPlayers()
    {
        foreach (OperationCentre opCentre in opCentres)
        {
            opCentre.DestroyOperationCentre();
        }
        opCentres.Clear();

        foreach (AIPlayer player in players)
        {
            player.DestroyPlayer();
        }
        players.Clear();
        Player.nextPlayerNumber = 1;
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
        writer.Write(cityStates.Count);
        foreach (CityState cityState in cityStates)
        {
            cityState.Save(writer);
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

        int cityStateCount = reader.ReadInt32();
        for (int i = 0; i < cityStateCount; i++)
        {
            CityState.Load(reader, this, hexGrid, header);
        }

    }
}
