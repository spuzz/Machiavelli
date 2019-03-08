using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpCentreBuildingsUI : OperationCentreInfoPanel
{
    [SerializeField] Button commandCentre;
    [SerializeField] Sprite defaultImage;
    [SerializeField] List<Button> buildingButtons;


    int slotID;
    private void Awake()
    {
        commandCentre.GetComponentInChildren<Text>().gameObject.SetActive(false);
    }
    public override void UpdateUI(OperationCentre opCentre)
    {
        if(opCentre.CommandCentre)
        {
            commandCentre.image.sprite = opCentre.CommandCentre.BuildConfig.BuildingImage;
            commandCentre.interactable = false;
        }
        else
        {
            commandCentre.image.sprite = defaultImage;
            commandCentre.interactable = true;
        }

        for(int a=0;a < 5; a++)
        {
            if (opCentre.GetBuilding(a))
            {
                buildingButtons[a].image.sprite = opCentre.GetBuilding(a).BuildConfig.BuildingImage;
                buildingButtons[a].interactable = false;
                buildingButtons[a].GetComponentInChildren<Text>().text = "";
            }
            else if (opCentre.IsConstructingBuilding(a))
            {
                buildingButtons[a].image.sprite = defaultImage;
                buildingButtons[a].interactable = false;
                buildingButtons[a].GetComponentInChildren<Text>().text = opCentre.TimeLeftOnConstruction(a).ToString();
            }
            else
            {
                buildingButtons[a].image.sprite = defaultImage;
                buildingButtons[a].interactable = true;
                buildingButtons[a].GetComponentInChildren<Text>().text = "";
            }
            

        }
    }

    public void BuildingClicked(int id)
    {
        slotID = id;
        BuildingChoicePanel.SetActive(opCentre);
    }

    public void Build(int buildingID)
    {
        opCentre.BuildAvailableBuilding(buildingID, slotID);
    }
}
