using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingChoicePanel : MonoBehaviour {

    City city;
    [SerializeField] List<Button> buttons;
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


    public void SetInactive()
    {

        if(city)
        {
            city.onInfoChange -= OnCityInfoChange;
            city = null;
        }
        
        
        gameObject.SetActive(false);
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


    public void BuildSelected(int id)
    {
        // TODO
        SetInactive();
    }
}
