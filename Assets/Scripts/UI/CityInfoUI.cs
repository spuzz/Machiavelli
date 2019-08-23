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
    [SerializeField] Text population;
    [SerializeField] Text foodStored;
    [SerializeField] Text turns;
    [SerializeField] Image currentBuildingImage;
    public override void UpdateUI(City cityUpdated)
    {
        cityStateName.text = city.GetCityState().CityStateName;
        health.text = city.HitPoints.ToString() + "/" + city.BaseHitPoints.ToString();
        strength.text = city.Strength.ToString();
        gold.text = city.CityResouceController.GetGold().ToString();
        food.text = city.CityResouceController.GetFood().ToString();
        production.text = city.CityResouceController.GetProduction().ToString();
        science.text = city.CityResouceController.GetScience().ToString();

        population.text = city.Population.ToString();
        foodStored.text = city.Food + "/" + GameConsts.populationFoodReqirements[city.Population].ToString();
        int foodRequired = (GameConsts.populationFoodReqirements[city.Population] - city.Food);
        int turnsNeeded = (foodRequired + city.CityResouceController.GetFood() - 1) / city.CityResouceController.GetFood();
        turns.text = turnsNeeded.ToString() + " Turns";
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
