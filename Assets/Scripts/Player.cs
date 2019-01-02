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
    private void Awake()
    {
        playerNumber = nextPlayerNumber;
        nextPlayerNumber++;
    }

    public void StartTurn()
    {
        foreach(Agent agent in agents)
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
