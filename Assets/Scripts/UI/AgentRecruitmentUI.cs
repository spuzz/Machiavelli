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
                AgentBuildConfig config = agentBuildConfigs[a];
                buttons[a].gameObject.SetActive(true);
                buttons[a].image.sprite = config.AgentConfig.Portrait;
                if (opCentre.Player.Gold >= config.BasePurchaseCost)
                {
                    buttons[a].interactable = true;
                }
                else
                {
                    buttons[a].interactable = false;
                }
                ToolTip tooltip = buttons[a].GetComponent<ToolTip>();
                if (tooltip)
                {
                    tooltip.Clear();
                    tooltip.SetHeader(config.DisplayName);
                    tooltip.AddText(config.ToolTipText);
                    tooltip.AddText("");
                    tooltip.AddText("Cost");
                    tooltip.AddSymbolWithText(1, config.BasePurchaseCost.ToString());
                    tooltip.AddText("");
                    tooltip.AddText("BuildTime");
                    tooltip.AddSymbolWithText(1, config.BaseBuildTime.ToString());
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
