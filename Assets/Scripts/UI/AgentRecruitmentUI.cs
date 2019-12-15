//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

// TODO
//public class AgentRecruitmentUI : OperationCentreInfoPanel {

//    [SerializeField] List<Button> buttons;
//    [SerializeField] List<Button> buildQueueButtons;
//    List<AgentBuildConfig> agentBuildConfigs = new List<AgentBuildConfig>();
//    public override void UpdateUI(OperationCentre opCentre)
//    {
//        agentBuildConfigs.Clear();
//        foreach (AgentBuildConfig config in opCentre.GetAgentBuildConfigs())
//        {
//            agentBuildConfigs.Add(config);
//        }
//        for (int a = 0; a < buttons.Count; a++)
//        {
//            if (a < agentBuildConfigs.Count)
//            {
//                AgentBuildConfig config = agentBuildConfigs[a];
//                buttons[a].gameObject.SetActive(true);
//                buttons[a].image.sprite = config.AgentConfig.Portrait;
//                if (opCentre.Player.Gold >= opCentre.GetAgentCost(config) && opCentre.Player.CanHireAgent(config.AgentConfig))
//                {
//                    buttons[a].interactable = true;
//                }
//                else
//                {
//                    buttons[a].interactable = false;
//                }
                

//                ToolTip tooltip = buttons[a].GetComponent<ToolTip>();
//                if (tooltip)
//                {
//                    tooltip.Clear();
//                    tooltip.SetHeader(config.DisplayName);
//                    tooltip.AddText(config.ToolTipText);
//                    tooltip.AddText("");
//                    tooltip.AddText("Cost");
//                    tooltip.AddSymbolWithText(1, opCentre.GetAgentCost(config).ToString());
//                    tooltip.AddText("");
//                    tooltip.AddText("BuildTime");
//                    tooltip.AddSymbolWithText(1, config.BaseBuildTime.ToString());
//                    int currentCap = opCentre.Player.PlayerAgentTracker.CurrentCap(config.AgentConfig);
//                    if (currentCap != -1)
//                    {
//                        tooltip.AddText("");
//                        tooltip.AddText("Limit");
//                        tooltip.AddSymbolWithText(1, opCentre.Player.PlayerAgentTracker.CurrentUsage(config.AgentConfig) + "/" + currentCap);
//                    }
//                }
//            }
//            else
//            {
//                buttons[a].gameObject.SetActive(false);
//            }
//        }

//        for (int a = 0; a < buildQueueButtons.Count; a++)
//        {
//            if (a < opCentre.BuildingManagerForAgents.buildsInQueue())
//            {
//                AgentBuildConfig config = (opCentre.BuildingManagerForAgents.GetConfigInQueue(a) as AgentBuildConfig);
//                buildQueueButtons[a].gameObject.SetActive(true);
//                buildQueueButtons[a].image.sprite = config.AgentConfig.Portrait;
//                if(a == 0)
//                {
//                    buildQueueButtons[a].GetComponentInChildren<Text>().text = opCentre.BuildingManagerForAgents.TimeLeftOnBuild(1).ToString();
//                }
//                else
//                {
//                    buildQueueButtons[a].GetComponentInChildren<Text>().text = config.BaseBuildTime.ToString();
//                }
                
//            }
//            else
//            {
//                buildQueueButtons[a].gameObject.SetActive(false);
//            }
//        }
//    }

//    public void HireAgent(int id)
//    {
//        opCentre.HireAgent(agentBuildConfigs[id]);
//    }

//}
