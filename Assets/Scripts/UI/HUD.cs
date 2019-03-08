using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    [SerializeField] GameController gameController;
    [SerializeField] Text turn;

    [SerializeField] HexGameUI HexGameUI;
    [SerializeField] Button endTurnButton;
    [SerializeField] AgentPanel agentPanel;
    [SerializeField] CityPanel cityPanel;
    [SerializeField] OperationCentrePanel opCentrePanel;
    [SerializeField] TextFadeOut newTurnText;
    Unit unit;
    City city;
    HexCell targetCell;
    OperationCentre opCentre;
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
                opCentre = null;
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
                opCentre = null;
                unit = null;
                TargetCell = city.GetHexCell();
                
            }
            UpdateUI();
        }
    }

    public OperationCentre OpCentre
    {
        get
        {
            return opCentre;
        }

        set
        {
            opCentre = value;
            if(opCentre != null)
            {
                city = null;
                unit = null;
                TargetCell = opCentre.Location;
                
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

    public void ClearUI()
    {
        unit = null;
        city = null;
        opCentre = null;
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
        newTurnText.FadeOut();
        //UpdateUI();
    }

    public void UpdateUI()
    {
        if (opCentre)
        {
            opCentrePanel.SetActive(opCentre);
        }
        else
        {
            opCentrePanel.SetInactive();
        }
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
            if(unit.HexUnit.HexUnitType == HexUnit.UnitType.AGENT)
            {
                agentPanel.SetActive(unit);
            }
            else
            {
                agentPanel.SetInactive();
            }
            
        }
        else
        {
            agentPanel.SetInactive();
        }
    }

    public void UseAbility(int abilityNumber)
    {
        TargetCell = Unit.HexUnit.Location;
        Unit.AttemptAbility(abilityNumber, TargetCell);
    }

    public void UseOpCentreAbility(int abilityNumber)
    {
        TargetCell = OpCentre.Location;
    }
}
