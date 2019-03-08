using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentPanel : MonoBehaviour {

    [SerializeField] List<Button> abilityButtons;
    [SerializeField] Text typeText;
    [SerializeField] Text movementText;
    [SerializeField] Text healthText;
    [SerializeField] Text strengthText;
    [SerializeField] Text visibilityText;
    [SerializeField] Image portrait;
    Unit unit;

    public Unit Unit
    {
        get
        {
            return unit;
        }

        set
        {
            unit = value;
        }
    }

    public void SetActive(Unit unitToWatch)
    {
        if(unitToWatch != unit)
        {
            SetInactive();
            
        }
        unit = unitToWatch;
        gameObject.SetActive(true);
        unit.onInfoChange += UpdateUI;
        UpdateUI(unit);


    }

    public void SetInactive()
    {
        gameObject.SetActive(false);
        if(unit)
        {
            unit.onInfoChange -= UpdateUI;
        }
    }

    public void UpdateUI(Unit unit)
    {
        if (isActiveAndEnabled)
        {
            
            if(Unit)
            {
                for (int count = 0; count < abilityButtons.Count; count++)
                {
                    if(count >= unit.GetNumberOfAbilities())
                    {
                        abilityButtons[count].gameObject.SetActive(false);
                    }
                    else
                    {
                        abilityButtons[count].gameObject.SetActive(true);
                        abilityButtons[count].interactable = Unit.IsAbilityUsable(count);
                        abilityButtons[count].image.sprite = Unit.GetAbility(count).DefaultIcon;
                        
                    }
                }
                Agent agent = Unit.GetComponent<Agent>();
                portrait.sprite = agent.GetAgentConfig().Portrait;
                typeText.text = agent.GetAgentConfig().Name;
                movementText.text = agent.GetMovementLeft()/agent.BaseMovementFactor + "/" + agent.BaseMovement;
                healthText.text = agent.HitPoints + "/" + agent.GetBaseHitpoints();
                strengthText.text = agent.GetAgentConfig().BaseStrength.ToString();
                visibilityText.text = agent.HexUnit.VisionRange.ToString();
                    
            }
        }
    }
}
