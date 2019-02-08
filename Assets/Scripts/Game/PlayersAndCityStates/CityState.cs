using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CityState : MonoBehaviour
{
    static int cityStateIDCounter = 1;

    [SerializeField] List<CombatUnit> units = new List<CombatUnit>();
    [SerializeField] CityStateAIController cityStateAIController;
    [SerializeField] int gold = 100;

    GameController gameController;
    int cityStateID;
    Color color = Color.black;
    Player player;

    public Dictionary<HexCell, int> visibleCells = new Dictionary<HexCell, int>();
    public List<HexCell> exploredCells = new List<HexCell>();
    List<City> cities = new List<City>();
    List<City> visibleCities = new List<City>();
    Dictionary<Player, int> influenceDict = new Dictionary<Player, int>();

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

    public IEnumerable<CombatUnit> GetUnits()
    {
        return units;
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
        }
    }
    
    public Color Color
    {
        get { return color; }
        set { color = value; }
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
                influenceDict.Clear();
                influenceDict[Player] = 100;
            }
            foreach (City city in cities)
            {
                city.UpdateUI();
            }
            UpdateVision();

        }
    }
    private void SetVision(bool vision)
    {
        foreach (City city in cities)
        {
            city.Vision = vision;
        }
        foreach (CombatUnit unit in units)
        {
            unit.HexUnit.Visible = vision;
        }
    }

    public int CityStateID
    {
        get { return cityStateID; }
        set { cityStateID = value; }
    }
    public void RemoveUnit(Unit unit)
    {
        CombatUnit combatUnit = unit.GetComponent<CombatUnit>();
        if (combatUnit)
        {
            units.Remove(combatUnit);
        }

    }

    public void AddCity(City city)
    {
        cities.Add(city);
        if (Player && Player.IsHuman)
        {
            city.Vision = true;
        }
        else
        {
            city.Vision = false;
        }
    }

    public void RemoveCity(City city)
    {
        cities.Remove(city);
        city.Vision = false;
        if (cities.Count == 0)
        {
            DestroyCityState();
        }
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

    public void AddUnit(CombatUnit unit)
    {
        unit.CityState = this;
        if (Player && Player.IsHuman)
        {
            unit.HexUnit.Visible = true;
        }
        else
        {
            unit.HexUnit.Visible = false;
        }
        units.Add(unit);
    }

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        cityStateID = cityStateIDCounter;
        cityStateIDCounter++;
    }

    public void StartTurn()
    {
        
        foreach (CombatUnit unit in units)
        {
            unit.StartTurn();
        }

        foreach (City city in cities)
        {
            city.StartTurn();
        }

        UpdateInfluence();
    }

    public void AdjustInfluence(Player adjustPlayer,int influence)
    {
        if(!Player)
        {
            if(!influenceDict.Keys.Contains(adjustPlayer) && influence > 0)
            {
                influenceDict[adjustPlayer] = influence;
            }
            else
            {
                influenceDict[adjustPlayer] += influence;
            }
            if (influenceDict[adjustPlayer] < 0)
            {
                influenceDict[adjustPlayer] = 0;
            }
            else if(influenceDict[adjustPlayer] > 100)
            {
                influenceDict[adjustPlayer] = 100;
            }
        }
        else if(Player && adjustPlayer == Player)
        {
            influenceDict[adjustPlayer] += influence;
            if (influenceDict[adjustPlayer] < 0)
            {
                influenceDict[adjustPlayer] = 0;
            }
        }
    }

    public void CheckInfluence()
    {
        if(!Player)
        {
            int maxValue = 0;
            if(influenceDict.Count > 0)
            {
                maxValue = influenceDict.Values.Max();
            }
            if(maxValue >= 100)
            {
                Player keyOfMaxValue = influenceDict.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                Player = keyOfMaxValue;

            }
        }
        else
        {
            if (influenceDict[Player] <= 0)
            {
                Player = null;
            }
            else if(influenceDict[player] > 100)
            {
                influenceDict[player] = 100;
            }
        }
    }

    public int GetInfluence(Player player)
    {
        if(!influenceDict.Keys.Contains(player))
        {
            return 0;
        }
        else
        {
            return influenceDict[player];
        }
    }
    public void AdjustInfluenceForAll(int influence)
    {
        List<Player> keys = influenceDict.Keys.ToList();
        foreach(Player player in keys)
        {
            influenceDict[player] += influence;
            if(influenceDict[player] < 0)
            {
                influenceDict[player] = 0;
            }
        }
    }
    public void AdjustInfluenceForAllExcluding(Player excludedPlayer, int influence)
    {
        foreach (Player player in influenceDict.Keys)
        {
            if(player != excludedPlayer)
            {
                influenceDict[player] += influence;
                if (influenceDict[player] < 0)
                {
                    influenceDict[player] = 0;
                }
            }

        }
    }

    private void UpdateInfluence()
    {
        int negativeInfluence = -((int)Math.Pow(2*1, cities.Count - 1));
        AdjustInfluenceForAll(negativeInfluence);
        CheckInfluence();
    }

    public IEnumerator TakeTurn()
    {
        units.RemoveAll(c => c.Alive == false);
        yield return StartCoroutine(cityStateAIController.UpdateUnits());
        cityStateAIController.UpdateCities();
        gameController.CityStateTurnFinished(this);
    }

    public void KillLocalUnits(City city)
    {
        foreach(CombatUnit unit in units.FindAll(c => c.HexUnit.Location == city.GetHexCell()))
        {
            if(unit.HexUnit.Location == city.GetHexCell())
            {
                unit.HexUnit.Die();
                unit.HexUnit.DieAnimationAndRemove();
                break;
            }
        }
    }

    private void UpdateVision()
    {
        if(!Player)
        {
            return;
        }

        if (player.IsHuman)
        {
            SetVision(true);
        }
        else
        {
            SetVision(false);
        }
    }

    public void PickColor()
    {

        Color = gameController.GetNewCityStateColor();
    }


    public void DestroyCityState()
    {
        if(player)
        {
            player.RemoveCityState(this);
        }
        gameController.ReturnCityStateColor(color);
        SetVision(false);
        foreach(City city in cities)
        {
            city.DestroyCity();
        }
        foreach (Unit unit in units)
        {
            unit.HexUnit.DieAndRemove();
        }
        Destroy(gameObject);
    }


    public void Save(BinaryWriter writer)
    {
        writer.Write(CityStateID);
        writer.Write(Color.r);
        writer.Write(Color.g);
        writer.Write(Color.b);
        if (Player)
        {
            writer.Write(Player.PlayerNumber);
        }
        else
        {
            writer.Write(-1);
        }
        
        writer.Write(units.Count);
        for (int i = 0; i < units.Count; i++)
        {
            units[i].Save(writer);
        }


        writer.Write(exploredCells.Count);
        for (int i = 0; i < exploredCells.Count; i++)
        {
            writer.Write(exploredCells[i].Index);
        }
    }

    public static void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {
        CityState instance = gameController.CreateCityState();
        instance.CityStateID = reader.ReadInt32();
        Color color = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1.0f);
        gameController.RemoveCityStateColor(color);
        instance.Color = color;
        if (header >= 2)
        {
            int playerNumber = reader.ReadInt32();
            if(playerNumber != -1)
            {
                instance.Player = gameController.GetPlayer(playerNumber);
            }
            
        }
        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            CombatUnit combatUnit = CombatUnit.Load(reader, hexGrid, header);
            combatUnit.HexUnit.Visible = false;
            instance.AddUnit(combatUnit);
            instance.UpdateVision();
        }
        if (header >= 3)
        {
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
    }
}
