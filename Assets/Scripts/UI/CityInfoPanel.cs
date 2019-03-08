using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CityInfoPanel : MonoBehaviour {
    protected City city;
    [SerializeField] BuildingChoicePanel buildingChoicePanel;
    bool active = false;

    public BuildingChoicePanel BuildingChoicePanel
    {
        get
        {
            return buildingChoicePanel;
        }

        set
        {
            buildingChoicePanel = value;
        }
    }

    public void SetActive(City cityToWatch)
    {
        
        if (active == false)
        {
            gameObject.SetActive(true);
            city = cityToWatch;
            city.onInfoChange += UpdateUI;
            active = true;
            UpdateUI(city);
        }
        else if(city != cityToWatch)
        {
            SetInactive();
            SetActive(cityToWatch);
        }

    }

    public abstract void UpdateUI(City cityUpdated);

    public void SetInactive()
    {
        if (active == true)
        {
            gameObject.SetActive(false);
            if(city)
            {
                city.onInfoChange -= UpdateUI;
            }
            
            active = false;
        }
    }
}
