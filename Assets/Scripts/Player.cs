using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class Player : MonoBehaviour {

    public static int nextPlayerNumber = 1;

    int playerNumber = 0;
    bool isHuman = false;
    bool alive = true;
    public List<Agent> agents = new List<Agent>();
    public List<CityState> cityStates = new List<CityState>();
    public List<OperationCentre> opCentres = new List<OperationCentre>();

    public Dictionary<HexCell, int> visibleCells = new Dictionary<HexCell, int>();
    public List<HexCell> exploredCells = new List<HexCell>();


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

    Color color;


    public GameObject operationCenterTransformParent;

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
    }

    public void RemoveCityState(CityState cityState)
    {
        cityStates.Remove(cityState);
    }

    public IEnumerable<CityState> GetCityStates()
    {
        return cityStates;
    }

    public IEnumerable<Agent> GetAgents()
    {
        return agents;
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

    public IEnumerable<OperationCentre> GetOperationCentres()
    {
        return opCentres;
    }

    public void AddOperationCentre(OperationCentre operationCentre)
    {
        opCentres.Add(operationCentre);
    }

    public void RemoveOperationCentre(OperationCentre operationCentre)
    {
        operationCentre.DestroyOperationCentre();
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


    private void Awake()
    {
        playerNumber = nextPlayerNumber;
        nextPlayerNumber++;
    }


    public void StartTurn()
    {
        if(cityStates.Count == 0)
        {
            PlayerDefeated();
        }
        agents.RemoveAll(c => c.Alive == false);
        foreach (Agent agent in agents)
        {
            agent.StartTurn();
        }
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


    public abstract void Save(BinaryWriter writer);

}
