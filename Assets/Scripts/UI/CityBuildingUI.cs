using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityBuildingUI : CityInfoPanel {
    [SerializeField] HumanPlayer humanPlayer;
    [SerializeField] GameObject optionsContent;
    [SerializeField] BuildingOption buildingOptionPrefab;
    List<BuildingOption> buildingOptions = new List<BuildingOption>();
    [SerializeField] GameObject existingContent;
    [SerializeField] ExistingBuildingPanel existingBuildingPrefab;
    List<ExistingBuildingPanel> existingBuildings = new List<ExistingBuildingPanel>();
    [SerializeField] Image currentBuild;
    [SerializeField] Sprite defaultBuildImage;
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
        foreach (ExistingBuildingPanel panel in existingBuildings)
        {
            Destroy(panel.gameObject);
        }

        existingBuildings.Clear();
        buildingOptions.Clear();

        foreach (BuildConfig config in city.GetBuildingOptions())
        {
            AddOption(config);
        }
        foreach (BuildConfig config in city.GetTrainingOptions())
        {
            AddOption(config);
        }

        foreach (CityBuilding building in city.GetCityBuildings())
        {
            AddExisting(building);
        }

        if (city.BuildingManager.buildsInQueue() > 0)
        {
            currentBuild.sprite = city.BuildingManager.currentBuilding().BuildingImage;
        }
        else
        {
            currentBuild.sprite = defaultBuildImage;
        }

    }

    public void AddOption(BuildConfig config)
    {
        BuildingOption option = Instantiate(buildingOptionPrefab, optionsContent.transform);
        option.City = city;
        option.BuildConfig = config;
        option.OptionImage.sprite = config.BuildingImage;
        option.OptionName.text = config.Name;
        buildingOptions.Add(option);
    }

    public void AddExisting(CityBuilding building)
    {
        ExistingBuildingPanel panel = Instantiate(existingBuildingPrefab, existingContent.transform);
        panel.City = city;
        panel.CityBuilding = building;
        panel.OptionImage.sprite = building.BuildConfig.BuildingImage;
        panel.OptionName.text = building.BuildConfig.Name;
        existingBuildings.Add(panel);
    }


}
