using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoTopBarUI : MonoBehaviour {

    [SerializeField] Text goldText;
    [SerializeField] List<Button> agentTypeButtons;
    Player player;
    GameController gameController;

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        player = gameController.GetPlayer(0);
        player.onInfoChange += playerChanged;
        ShowTypes();
    }

    private void ShowTypes()
    {
        IEnumerable<AgentConfig> configs = player.PlayerAgentTracker.GetAllAgents();
        int count = 0;
        foreach(AgentConfig config in configs)
        {
            agentTypeButtons[count].gameObject.SetActive(true);
            agentTypeButtons[count].GetComponentInChildren<Image>().sprite = config.Portrait;
            count++;
        }
        while(count < agentTypeButtons.Count)
        {
            agentTypeButtons[count].gameObject.SetActive(false);
            count++;
        }
    }

    private void playerChanged(Player player)
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        IEnumerable<AgentConfig> configs = player.PlayerAgentTracker.GetAllAgents();
        goldText.text = player.Gold.ToString() + "(+" + player.GoldPerTurn.ToString() + ")";
        int count = 0;
        foreach (AgentConfig config in configs)
        {
            string currentCap = player.PlayerAgentTracker.CurrentCap(config).ToString();
            if (currentCap.CompareTo("-1") == 0)
            {
                currentCap = "-";
            }
            agentTypeButtons[count].GetComponentInChildren<Text>().text = player.PlayerAgentTracker.CurrentUsage(config) + "/" + currentCap;
            count++;
        }
    }
}
