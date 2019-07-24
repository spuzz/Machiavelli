using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityPanel : MonoBehaviour {

    [SerializeField] List<CityInfoPanel> infoPanels;
    City city;

    public void SetActive(City cityToWatch)
    {
        city = cityToWatch;
        gameObject.SetActive(true);
        SetActiveInfoPanel(0);
    }

    public void SetInactive()
    {
        SetActiveInfoPanel(-1);
        gameObject.SetActive(false); 
    }

    public void SetActiveInfoPanel(int panelNumber)
    {
        for (int a = 0; a < infoPanels.Count; a++)
        {
            if (a == panelNumber)
            {
                infoPanels[a].SetActive(city);
            }
        }
    }
}
