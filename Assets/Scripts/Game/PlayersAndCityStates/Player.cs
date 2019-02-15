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
    [SerializeField] int gold = 100;

    Color color;
    public GameObject operationCenterTransformParent;

    public List<Agent> agents = new List<Agent>();
    public List<CityState> cityStates = new List<CityState>();
    public List<OperationCentre> opCentres = new List<OperationCentre>();
    public Dictionary<HexCell, int> visibleCells = new Dictionary<HexCell, int>();
    public List<HexCell> exploredCells = new List<HexCell>();

    protected GameController gameController;



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
        if(isHuman)
        {
            agent.HexVision.HasVision = true;
        }
        
        agent.SetPlayer(this);
        agents.Add(agent);
    }

    public void RemoveAgent(Agent agent)
    {
        agent.HexVision.HasVision = false;
        agents.Remove(agent);
    }

    public void ClearAgents()
    {
        agents.Clear();
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
    protected void ClearExploredCells()
    {
        exploredCells.Clear();
    }

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
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

        foreach (OperationCentre opCentre in opCentres)
        {
            opCentre.StartTurn();
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



    public void SavePlayer(BinaryWriter writer)
    {
        writer.Write(Color.r);
        writer.Write(Color.g);
        writer.Write(Color.b);
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
            Color playerColor = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1.0f);
            gameController.RemovePlayerColor(playerColor);
            Color = playerColor;
        }

    }
    public abstract void Save(BinaryWriter writer);

    public void DestroyPlayer()
    {
        gameController.ReturnPlayerColor(color);
        foreach (Agent agent in agents)
        {
            Destroy(agent.gameObject);
        }
        Destroy(gameObject);

    }
}
