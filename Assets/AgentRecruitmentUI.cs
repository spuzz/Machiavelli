using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentRecruitmentUI : OperationCentreInfoPanel {

    [SerializeField] List<Button> buttons;
    List<AgentConfig> agentConfigs = new List<AgentConfig>();
    OperationCentre opCentre;
    public override void UpdateUI(OperationCentre opCentre)
    {
        this.opCentre = opCentre;
        if (isActiveAndEnabled)
        {

            if (opCentre)
            {
                agentConfigs.Clear();
                foreach (AgentConfig config in opCentre.GetAgentConfigs())
                {
                    agentConfigs.Add(config);
                }
                for (int a = 0; a < buttons.Count; a++)
                {
                    if (a < agentConfigs.Count)
                    {
                        buttons[a].gameObject.SetActive(true);
                        buttons[a].interactable = true;
                        buttons[a].image.sprite = agentConfigs[a].Portrait;
                    }
                    else
                    {
                        buttons[a].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void HireAgent(int id)
    {
        opCentre.HireAgent(agentConfigs[id]);
    }

}
