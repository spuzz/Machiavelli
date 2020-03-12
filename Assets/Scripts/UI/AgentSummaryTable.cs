using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSummaryTable : SearchableTable
{


    private Player player;
    List<Agent> agents = new List<Agent>();

    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
        }
    }

    public override void SortBy(int column = 0, bool ascending = false)
    {
        switch (column)
        {
            case 0:
                agents.Sort((x, y) => x.GetAgentConfig().AgentClass.CompareTo(y.GetAgentConfig().AgentClass));
                break;
            case 1:
                agents.Sort((x, y) => string.Compare(x.AgentName, y.AgentName));
                break;
            case 2:
                agents.Sort((x, y) => x.Level.CompareTo(y.Level));
                break;
            case 3:
                agents.Sort((x, y) => x.UnspentPoints.CompareTo(y.UnspentPoints));
                break;
            case 4:
                agents.Sort((x, y) => x.Maintenance.CompareTo(y.Maintenance));
                break;
            case 5:
                agents.Sort((x, y) => x.Strength.CompareTo(y.Strength));
                break;
            default:
                agents.Sort((x, y) => x.GetAgentConfig().AgentClass.CompareTo(y.GetAgentConfig().AgentClass));
                break;
        }
        if (ascending == false)
        {
            agents.Reverse();
        }
        FillTable();
    }

    public void FillList(Player playerToList)
    {
        agents.Clear();
        player = playerToList;
        foreach (Agent agent in player.agents)
        {
            agents.Add(agent);
        }
    }

    public override void UpdateTableList()
    {
        Player player = FindObjectOfType<HumanPlayer>();
        FillList(player);
    }

    public override void FillTable(int column = 0, bool ascending = true)
    {

        ClearObjects();

        foreach (Agent agent in agents)
        {
            AgentSummaryTableObject objectAdded = Instantiate(TableObjectPrefab, transform).GetComponent<AgentSummaryTableObject>();
            objectAdded.SetAgent(agent);
            SearchableTableObjects.Add(objectAdded);
        }
    }

}
