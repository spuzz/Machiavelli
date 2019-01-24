using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class Player : MonoBehaviour {

    public static int nextPlayerNumber = 1;

    int playerNumber = 0;
    bool isHuman = false;
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

    public Color Color
    {
        get
        {
            return color;
        }

        set
        {
            color = value;
        }
    }

    Color color;

    public List<Agent> agents = new List<Agent>();
    public List<CityState> cityStates = new List<CityState>();

    public Dictionary<HexCell, int> visibleCells = new Dictionary<HexCell, int>();
    public List<HexCell> exploredCells = new List<HexCell>();

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

    public IEnumerable<Agent> GetAgents()
    {
        return agents;
    }

    public void AddCityState(CityState cityState)
    {
        cityStates.Add(cityState);
    }

    public void RemoveCityState(CityState cityState)
    {
        cityStates.Remove(cityState);
    }

    public IEnumerable<CityState> GetCityStates()
    {
        return cityStates;
    }

    private void Awake()
    {
        playerNumber = nextPlayerNumber;
        nextPlayerNumber++;
    }


    public void StartTurn()
    {
        agents.RemoveAll(c => c.Alive == false);
        foreach (Agent agent in agents)
        {
            agent.StartTurn();
        }
    }

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

    public void AddAgent(Agent agent)
    {
        agent.SetPlayer(this);
        agents.Add(agent);
    }

    public void RemoveAgent(Agent agent)
    {
        agent.GetComponent<HexUnit>().KillUnit();
        agents.Remove(agent);
    }

    public void ClearAgents()
    {
        foreach (Agent agent in agents)
        {
            agent.GetComponent<HexUnit>().KillUnit();
        }
        agents.Clear();
    }
    public abstract void Save(BinaryWriter writer);

}
