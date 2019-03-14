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
            buttons[count].gameObject.SetActive(true);
            buttons[count].image.sprite = config.BuildingImage;
            buttons[count].interactable = true;
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
