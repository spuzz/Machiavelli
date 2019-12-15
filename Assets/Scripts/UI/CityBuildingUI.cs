using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityBuildingUI : CityInfoPanel {
    [SerializeField] HumanPlayer humanPlayer;
    [SerializeField] GameObject buildingsContent;
    [SerializeField] GameObject unitsContent;
    [SerializeField] BuildingOption buildingOptionPrefab;
    List<BuildingOption> buildingOptions = new List<BuildingOption>();
    [SerializeField] Image currentBuild;
    [SerializeField] List<Image> buildQueue;
    [SerializeField] Sprite defaultBuildImage;
    [SerializeField] TextMeshProUGUI daysLeft;

    public HumanPlayer HumanPlayer
    {
        get
        {
            return humanPlayer;
        }

        set
        {
            humanPlayer = value;
        }
    }

    private void Awake()
    {
    }
    public override void UpdateUI(City cityToWatch)
    {
        foreach(BuildingOption option in buildingOptions)
        {
            Destroy(option.gameObject);
        }


        buildingOptions.Clear();

        foreach (BuildConfig config in city.GetBuildingOptions())
        {
            AddOption(config,buildingsContent);
        }
        foreach (BuildConfig config in city.GetCombatUnitTrainingOptions())
        {
            AddOption(config, unitsContent);
        }
        foreach (BuildConfig config in city.GetAgentTrainingOptions())
        {
            AddOption(config, unitsContent);
        }

        if (city.BuildingManager.BuildsInQueue() > 0)
        {
            currentBuild.sprite = city.BuildingManager.currentBuilding().BuildingImage;
            daysLeft.text = city.BuildingManager.TimeLeftOnBuild(city.CityResouceController.GetProduction()).ToString();
            currentBuild.GetComponent<ToolTip>().enabled = true;
        }
        else
        {
            currentBuild.sprite = defaultBuildImage;
            daysLeft.text = "";
            currentBuild.GetComponent<ToolTip>().enabled = false;
        }

        int queueNumber = 1;
        foreach(Image queueImage in buildQueue)
        {
            BuildConfig config = city.BuildingManager.GetConfigInQueue(queueNumber);
            if (config)
            {
                queueImage.sprite = config.BuildingImage;
                queueImage.GetComponent<ToolTip>().enabled = true;
            }
            else
            {
                queueImage.sprite = defaultBuildImage;
                queueImage.GetComponent<ToolTip>().enabled = false;
            }
            queueNumber++;
        }


    }

    public void AddOption(BuildConfig config, GameObject content)
    {
        BuildingOption option = Instantiate(buildingOptionPrefab, content.transform);
        option.City = city;
        option.BuildConfig = config;
        option.OptionImage.sprite = config.BuildingImage;
        option.OptionName.text = config.Name;
        option.ConstructionTime.text = CalculateConstructionTime(option.BuildConfig);
        buildingOptions.Add(option);
    }

    private string CalculateConstructionTime(BuildConfig config)
    {
        int ConstructionTime = config.BaseBuildTime;
        int production = city.CityResouceController.GetProduction(config.FocusType);
        int days = 999;
        if (production != 0)
        {
            days = (ConstructionTime + production - 1) / production;
        }
        return days.ToString();
    }


    public void RemoveFromQueue(int queueNumber)
    {
        city.RemoveBuild(queueNumber - 1);
    }
}
