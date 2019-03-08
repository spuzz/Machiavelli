using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityBuildingUI : CityInfoPanel {
    [SerializeField] Button outpost;
    [SerializeField] Sprite defaultImage;
    [SerializeField] List<Button> buildingButtons;
    [SerializeField] HumanPlayer humanPlayer;

    int slotID;

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
        outpost.GetComponentInChildren<Text>().gameObject.SetActive(false);
    }
    public override void UpdateUI(City cityToWatch)
    {
        city = cityToWatch;
        if (city.PlayerBuildingControl.HasOutpost(HumanPlayer))
        {
            outpost.image.sprite = city.PlayerBuildingControl.OutpostConfig.BuildingImage;
            outpost.interactable = true;
        }
        else
        {
            outpost.image.sprite = defaultImage;
            outpost.interactable = false;
        }

        for (int a = 0; a < 5; a++)
        {

            if (city.PlayerBuildingControl.GetPlayerBuilding(a,HumanPlayer))
            {
                buildingButtons[a].image.sprite = city.PlayerBuildingControl.GetPlayerBuilding(a, HumanPlayer).BuildConfig.BuildingImage;
                buildingButtons[a].interactable = false;
                buildingButtons[a].GetComponentInChildren<Text>().text = "";
            }
            else if (city.PlayerBuildingControl.IsConstructingBuilding(a,HumanPlayer))
            {
                buildingButtons[a].image.sprite = defaultImage;
                buildingButtons[a].interactable = false;
                buildingButtons[a].GetComponentInChildren<Text>().text = city.PlayerBuildingControl.TimeLeftOnConstruction(a,HumanPlayer).ToString();
            }
            else
            {
                buildingButtons[a].image.sprite = defaultImage;
                buildingButtons[a].interactable = true;
                buildingButtons[a].GetComponentInChildren<Text>().text = "";
            }

            if (!city.PlayerBuildingControl.HasOutpost(HumanPlayer))
            {
                buildingButtons[a].interactable = false;
            }


        }
    }

    public void BuildingClicked(int id)
    {
        slotID = id;
        BuildingChoicePanel.SetActive(city);
    }

    public void Build(int buildingID)
    {
        city.PlayerBuildingControl.BuildBuilding(buildingID, HumanPlayer, slotID);
    }
}
