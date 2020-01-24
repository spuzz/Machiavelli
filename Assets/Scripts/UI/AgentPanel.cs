using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentPanel : MonoBehaviour {

    [SerializeField] List<Button> abilityButtons;
    //[SerializeField] Text typeText;
    //[SerializeField] Text movementText;
    //[SerializeField] Text healthText;
    //[SerializeField] Text strengthText;
    //[SerializeField] Text visibilityText;
    [SerializeField] Image agentPortrait;
    [SerializeField] Image unitPortrait;
    [SerializeField] GameObject unitObject;
    [SerializeField] GameObject agentObject;

    HexCell currentCell;

    public HexCell CurrentCell
    {
        get
        {
            return currentCell;
        }

        set
        {
            currentCell = value;
        }
    }

    public void SetActive(HexCell cell)
    {
        SetInactive();
        gameObject.SetActive(true);
        currentCell = cell;
        UpdateUI(cell);
        if (currentCell)
        {
            if(currentCell.agent)
            {
                currentCell.agent.unit.onInfoChange += UpdateUI;
            }

            if (currentCell.combatUnit)
            {
                currentCell.combatUnit.unit.onInfoChange += UpdateUI;
            }
        }
    }

    public void SetInactive()
    {
        gameObject.SetActive(false);
    }

    public void UpdateUI(Unit unit)
    {
        UpdateUI(unit.HexUnit.Location);
    }

    public void UpdateUI(HexCell cell)
    {
        if (isActiveAndEnabled)
        {


            if(CurrentCell.agent)
            {
                HexUnit unit = CurrentCell.agent;
                List<AbilityConfig> abilities = unit.GetComponent<Abilities>().AbilitiesList;
                for (int count = 0; count < abilityButtons.Count; count++)
                {

                    if (count >= abilities.Count)
                    {
                        abilityButtons[count].gameObject.SetActive(false);
                    }
                    else
                    {
                        abilityButtons[count].gameObject.SetActive(true);
                        abilityButtons[count].interactable = unit.GetComponent<Abilities>().ValidTargets(count, unit.Location).Count != 0;
                        abilityButtons[count].image.sprite = abilities[count].DefaultIcon;

                        ToolTip tooltip = abilityButtons[count].GetComponent<ToolTip>();
                        if (tooltip)
                        {
                            tooltip.Clear();
                            tooltip.SetHeader(abilities[count].DisplayName);
                            tooltip.AddText(abilities[count].ToolTipText);
                            tooltip.AddText("");
                            tooltip.AddText("Cost");
                            tooltip.AddSymbolWithText(1, abilities[count].GetEnergyCost().ToString());
                        }

                    }

                    agentObject.gameObject.SetActive(true);
                    Agent agent = unit.GetComponent<Agent>();
                    agentPortrait.sprite = agent.GetAgentConfig().Portrait;
                }
  
                //typeText.text = agent.GetAgentConfig().Name;
                //movementText.text = agent.GetMovementLeft()/agent.BaseMovementFactor + "/" + agent.BaseMovement;
                //healthText.text = agent.HitPoints + "/" + agent.GetBaseHitpoints();
                //strengthText.text = agent.GetAgentConfig().BaseStrength.ToString();
                //visibilityText.text = agent.HexUnit.VisionRange.ToString();

            }
            else
            {
                agentObject.gameObject.SetActive(false);
            }

            if (CurrentCell.combatUnit)
            {
                unitObject.gameObject.SetActive(true);
                CombatUnit combatUnit = CurrentCell.combatUnit.GetComponent<CombatUnit>();
                unitPortrait.sprite = combatUnit.GetCombatUnitConfig().Portrait;
            }
            else
            {
                unitObject.gameObject.SetActive(false);
            }
        }
    }
}
