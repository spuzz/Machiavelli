using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AgentSummaryTableObject : SearchableTableObject
{

    [SerializeField] RawImage agentClass;
    [SerializeField] TextMeshProUGUI agentName;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI unspentPoints;
    [SerializeField] TextMeshProUGUI maintenance;
    [SerializeField] TextMeshProUGUI strength;

    private Agent agent;

    public void SetAgent(Agent agent)
    {
        GameController gameController = FindObjectOfType<GameController>();
        agentClass.texture = agent.GetAgentConfig().Symbol;
        level.text = agent.Level.ToString();
        unspentPoints.text = agent.UnspentPoints.ToString();
        maintenance.text = agent.Maintenance.ToString();
        strength.text = agent.Strength.ToString();
        this.agent = agent;

    }

    public void SelectAgent()
    {
        FindObjectOfType<HexGameUI>().SelectUnit(agent.HexUnit);
        FindObjectOfType<InfoButtonMenu>().ClosePanels();
    }
}
