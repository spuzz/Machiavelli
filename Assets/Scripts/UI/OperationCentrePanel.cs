using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationCentrePanel : MonoBehaviour {

    [SerializeField] List<Button> buttons;

    OperationCentre opCentre;
    List<AgentConfig> agentConfigs = new List<AgentConfig>();
    
    public OperationCentre OpCentre
    {
        get
        {
            return opCentre;
        }

        set
        {
            opCentre = value;
        }
    }


    public void SetActive(bool active)
    {
        gameObject.SetActive(active);

    }

    public void HireAgent(int id)
    {
        opCentre.HireAgent(agentConfigs[id]);
    }
    public void UpdateUI()
    {
        if (isActiveAndEnabled)
        {

            if (OpCentre)
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
}
