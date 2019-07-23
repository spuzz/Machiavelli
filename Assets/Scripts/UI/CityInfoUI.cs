using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityInfoUI : CityInfoPanel
{
    [SerializeField] Text cityStateName;
    [SerializeField] Text health;
    [SerializeField] Text strength;
    [SerializeField] Text gold;
    [SerializeField] Text food;
    [SerializeField] Text production;
    [SerializeField] Text science;
    [SerializeField] Image currentBuildingImage;
    public override void UpdateUI(City cityUpdated)
    {
        // TODO
        //cityStateName.text = city.GetCityState().CityStateName;
        //health.text = city.HitPoints.ToString() + "/" + city.BaseHitPoints.ToString();
        //strength.text = city.Strength.ToString();
        //gold.text = city.GetIncome().ToString();
        //food.text = city.currentFood.ToString() + " (" + city.Food + "/" + city.foodForNextPop.ToString() + ")";
        //production.text = city.currentProduction.ToString();
        //science.text = city.currentScience.ToString();
        //BuildConfig config = city.BuildingManager.currentBuilding();
        //if (config)
        //{
        //    currentBuildingImage.gameObject.SetActive(true);
        //    currentBuildingImage.sprite = config.BuildingImage;
        //}
        //else
        //{
        //    currentBuildingImage.gameObject.SetActive(false);
        //}

    }
}
