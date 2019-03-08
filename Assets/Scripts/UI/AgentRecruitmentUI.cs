using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentRecruitmentUI : OperationCentreInfoPanel {

    [SerializeField] List<Button> buttons;
    List<AgentBuildConfig> agentBuildConfigs = new List<AgentBuildConfig>();
    public override void UpdateUI(OperationCentre opCentre)
    {
        agentBuildConfigs.Clear();
        foreach (AgentBuildConfig config in opCentre.GetAgentBuildConfigs())
        {
            agentBuildConfigs.Add(config);
        }
        for (int a = 0; a < buttons.Count; a++)
        {
            if (a < agentBuildConfigs.Count)
            {
                buttons[a].gameObject.SetActive(true);
                buttons[a].image.sprite = agentBuildConfigs[a].AgentConfig.Portrait;
                if (opCentre.Player.Gold >= agentBuildConfigs[a].BasePurchaseCost)
                {
                    buttons[a].interactable = true;
                }
                else
                {
                    buttons[a].interactable = false;
                }
            }
            else
            {
                buttons[a].gameObject.SetActive(false);
            }
        }
    }

    public void HireAgent(int id)
    {
        opCentre.HireAgent(agentBuildConfigs[id]);
    }

}
