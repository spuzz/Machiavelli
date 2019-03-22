using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingChoicePanel : MonoBehaviour {

    City city;
    OperationCentre opCentre;
    [SerializeField] List<Button> buttons;
    [SerializeField] OpCentreBuildingsUI opCentreBuildingsUI;
    [SerializeField] CityBuildingUI cityBuildingUI;
    private bool foundBenefit;
    public void SetActive(City cityToBuild)
    {
        SetInactive();
        gameObject.SetActive(true);
        city = cityToBuild;
        city.onInfoChange += OnCityInfoChange;
        OnCityInfoChange(city);
    }

    public void SetActive(OperationCentre opCentreToBuild)
    {
        SetInactive();
        gameObject.SetActive(true);
        opCentre = opCentreToBuild;
        opCentre.onInfoChange += OnOpCentreInfoChange;
        OnOpCentreInfoChange(opCentre);
    }

    public void SetInactive()
    {

        if(city)
        {
            city.onInfoChange -= OnCityInfoChange;
            city = null;
        }

        if (opCentre)
        {
            opCentre.onInfoChange -= OnOpCentreInfoChange;
            opCentre = null;
        }
        
        
        gameObject.SetActive(false);
    }

    private void OnOpCentreInfoChange(OperationCentre opCentre)
    {
        int count = 0;
        foreach(OpCentreBuildConfig config in opCentre.availableBuilds)
        {
            ToolTip tooltip = buttons[count].GetComponent<ToolTip>();
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
            buttons[count].gameObject.SetActive(true);
            buttons[count].image.sprite = config.BuildingImage;
            if (cityBuildingUI.HumanPlayer.Gold >= config.BasePurchaseCost)
            {
                buttons[count].interactable = true;
            }
            else
            {
                buttons[count].interactable = false;
            }

            count++;
        }
        while(count < buttons.Count)
        {
            buttons[count].gameObject.SetActive(false);
            buttons[count].interactable = false;
            count++;
        }
    }

    private void OnCityInfoChange(City city)
    {
        int count = 0;
        foreach (CityPlayerBuildConfig config in cityBuildingUI.HumanPlayer.GetCityPlayerBuildConfigs())
        {
            ToolTip tooltip = buttons[count].GetComponent<ToolTip>();
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

                ResourceBenefit benefit = config.BuildPrefab.GetComponentInChildren<ResourceBenefit>();
                foundBenefit = false;
                if (benefit)
                {
                    if (benefit.Gold.y != 0)
                    {
                        AddBenefitToToolTip(tooltip, 1, benefit.Gold.y.ToString(), "City Bonuses");
                    }
                    if (benefit.Gold.x != 0)
                    {
                        AddBenefitToToolTip(tooltip, 1, benefit.Gold.x.ToString() + "%", "City Bonuses");
                    }

                }
                foundBenefit = false;
                if (benefit)
                {
                    if (benefit.PlayerGold.y != 0)
                    {
                        AddBenefitToToolTip(tooltip, 1, benefit.PlayerGold.y.ToString(), "Player Bonuses");
                    }
                    if (benefit.PlayerGold.x != 0)
                    {
                        AddBenefitToToolTip(tooltip, 1, benefit.PlayerGold.x.ToString() + "%", "Player Bonuses");
                    }

                }

            }
            buttons[count].gameObject.SetActive(true);
            buttons[count].image.sprite = config.BuildingImage;
            if(cityBuildingUI.HumanPlayer.Gold >= config.BasePurchaseCost)
            {
                buttons[count].interactable = true;
            }
            else
            {
                buttons[count].interactable = false;
            }

            count++;
        }
        while (count < buttons.Count)
        {
            buttons[count].gameObject.SetActive(false);
            buttons[count].interactable = false;
            count++;
        }
    }

    private void AddBenefitToToolTip(ToolTip tooltip, int symbol, string text, string benefitText)
    {
        if (foundBenefit == false)
        {
            tooltip.AddText("");
            tooltip.AddText(benefitText);
            foundBenefit = true;
        }
        tooltip.AddSymbolWithText(symbol, text);
    }

    public void BuildSelected(int id)
    {

        if(opCentre)
        {
            opCentreBuildingsUI.Build(id);
        }
        else if(city)
        {
            cityBuildingUI.Build(id);
        }
        SetInactive();
    }
}
