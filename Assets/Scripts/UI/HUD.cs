using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    [SerializeField] GameController gameController;
    [SerializeField] Text turn;

    [SerializeField] HexGameUI HexGameUI;
    [SerializeField] Button endTurnButton;
    [SerializeField] AgentPanel agentPanel;
    [SerializeField] CityPanel cityPanel;
    [SerializeField] TextFadeOut newTurnText;
    [SerializeField] GameObject toolTip;
    [SerializeField] TextMeshProUGUI toolTipText;
    [SerializeField] CombatPanel combatPanel;

    Unit unit;
    City city;
    HexCell targetCell;
    public Unit Unit
    {
        get
        {
            return unit;
        }

        set
        {
            unit = value;
            if (unit != null)
            {
                city = null;
                TargetCell = unit.HexUnit.Location;
                
            }
            UpdateUI();
        }
    }

    public City City
    {
        get
        {
            return city;
        }

        set
        {
            city = value;
            if (city != null)
            {
                unit = null;
                TargetCell = city.GetHexCell();
                
            }
            UpdateUI();
        }
    }

    public HexCell TargetCell
    {
        get
        {
            return targetCell;
        }

        set
        {
            targetCell = value;
        }
    }

    // Update is called once per frame
    void Update () {
        turn.text = "Turn : " + gameController.GetTurn().ToString();
        if (Input.GetKeyUp(KeyCode.L))
        {
            HexGameUI.ToggleEditMode();
        }
    }

    public bool IsInEditMode()
    {
        return HexGameUI.GetEditMode();
    }
    public void ClearUI()
    {
        unit = null;
        city = null;
        UpdateUI();
    }

    public void EndTurn()
    {
        endTurnButton.interactable = false;
        gameController.EndPlayerTurn();
    }

    public void StartTurn()
    {
        endTurnButton.interactable = true;
        //newTurnText.FadeOut();
        //UpdateUI();
    }

    public void UpdateUI()
    {

        if (city)
        {
            cityPanel.SetActive(city);
        }
        else
        {
            cityPanel.SetInactive();
        }
        if (unit)
        {
            //if(unit.HexUnitType == Unit.UnitType.AGENT)
            //{
                agentPanel.SetActive(unit.HexUnit.Location);
            //}
            //else
            //{
            //    agentPanel.SetInactive();
            //}
            
        }
        else
        {
            agentPanel.SetInactive();
        }
    }

    public void UseAbility(int abilityNumber)
    {
        TargetCell = Unit.HexUnit.Location;
        Unit.GetComponent<Abilities>().AttemptAbility(abilityNumber, TargetCell);
    }

    public void ShowCombatPanel(HexCell owner, HexCell target)
    {
        combatPanel.Activate(owner,target);
    }

    public void HideCombatPanel(List<HexCell> path)
    {
        combatPanel.Deactivate(path);
    }


    public void ShowToolTip(string text)
    {
        toolTip.SetActive(true);
        toolTip.GetComponent<MoveToMouseCursor>().UpdatePosition();
        toolTipText.SetText(text);

    }

    public void HideToolTip()
    {
        if(toolTip)
        {
            toolTip.SetActive(false);
        }


    }
}
