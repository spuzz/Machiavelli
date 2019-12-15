using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityLeftPanel : CityInfoPanel
{
    [SerializeField] ExistingBuildingPanel existingBuildingPrefab;
    List<ExistingBuildingPanel> existingBuildings = new List<ExistingBuildingPanel>();
    [SerializeField] GameObject existingContent;

    [SerializeField] ExistingUnit existingUnitPrefab;
    List<ExistingUnit> existingUnits = new List<ExistingUnit>();
    [SerializeField] GameObject existingUnitsContent;

    public override void UpdateUI(City cityUpdated)
    {
        foreach (ExistingBuildingPanel panel in existingBuildings)
        {
            Destroy(panel.gameObject);
        }
        existingBuildings.Clear();

        foreach (ExistingUnit panel in existingUnits)
        {
            Destroy(panel.gameObject);
        }
        existingUnits.Clear();

        foreach (CityBuilding building in city.GetCityBuildings())
        {
            AddExisting(building);
        }

        foreach (CombatUnit unit in city.GetUnits())
        {
            AddExisting(unit);
        }

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

    public void AddExisting(CombatUnit unit)
    {
        ExistingUnit panel = Instantiate(existingUnitPrefab, existingUnitsContent.transform);
        panel.Unit = unit;
        panel.OptionImage.sprite = unit.GetCombatUnitConfig().Portrait;
        panel.OptionName.text = unit.GetCombatUnitConfig().Name;
        existingUnits.Add(panel);
    }

}
