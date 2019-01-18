using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CityState : MonoBehaviour
{
    static int cityStateIDCounter = 1;
    List<City> cities = new List<City>();
    [SerializeField] List<CombatUnit> units = new List<CombatUnit>();
    [SerializeField] CityStateAIController cityStateAIController;
    int cityStateID;

    GameController gameController;
    public Dictionary<HexCell, int> visibleCells = new Dictionary<HexCell, int>();
    public List<HexCell> exploredCells = new List<HexCell>();

    List<City> enemyCities = new List<City>();
    public IEnumerable<City> getEnemyCities()
    {
        return enemyCities;
    }
    public void AddVisibleCell(HexCell cell)
    {
        if (!exploredCells.Contains(cell))
        {
            exploredCells.Add(cell);
            if(cell.City)
            {
                enemyCities.Add(cell.City);
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

    public IEnumerable GetUnits()
    {
        return units;
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
    }

    public IEnumerator TakeTurn()
    {
        yield return StartCoroutine(cityStateAIController.UpdateUnits());
        //foreach (CombatUnit unit in units)
        //{
        //    int currentMovement = -1;
        //    List<HexCell> path = new List<HexCell>();
        //    while (unit.GetMovementLeft() > 0 && currentMovement != unit.GetMovementLeft())
        //    {
        //        currentMovement = unit.GetMovementLeft();
        //        yield return StartCoroutine(MoveUnit(unit));
        //    }
        //}
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

    public IEnumerator MoveUnit(CombatUnit unit)
    {
        HexCell nextMove = null;
        if(cityStateID == 4)
        {
            if(enemyCities.Count > 0)
            {
                var sortedList = enemyCities.OrderBy(c => c.GetHexCell().coordinates.DistanceTo(unit.HexUnit.Location.coordinates));
                City city = sortedList.First();
                nextMove = unit.Behaviour.Attack(city);
            }
            else
            {
                nextMove = unit.Behaviour.ExploreArea(unit.HexUnit.Location, cities[0].GetHexCell(), 6, exploredCells);
            }
            
        }
        
        if(!nextMove)
        {
            nextMove = unit.Behaviour.Patrol(cities[0].GetHexCell(), 4);
        }
        if (!nextMove || nextMove == unit.HexUnit.Location)
        {
            unit.EndTurn();
        }
        else
        {
            unit.SetPath(nextMove);
            while (unit.AttackCell && unit.HexUnit.pathToTravel != null && unit.HexUnit.pathToTravel.Count != 0)
            {
                yield return new WaitForFixedUpdate();
            }

        }
    }

    Color color = Color.black;
    public Color Color
    {
        get { return color; }
        set { color = value; }
    }


    Player player;
    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            if(player)
            {
                player.RemoveCityState(this);
            }
            player = value;
            if (player)
            {
                player.AddCityState(this);
            }
            foreach (City city in cities)
            {
                city.UpdateUI();
            }
            UpdateVision();

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

    public void PickColor()
    {

        Color = gameController.GetNewCityStateColor();
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
        if(cities.Count == 0)
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
        foreach(Unit unit in units)
        {
            unit.HexUnit.DieAndRemove();
        }
        Destroy(gameObject);
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
    }
}
