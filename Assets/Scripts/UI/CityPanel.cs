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
        ActivatePanels();
    }

    public void SetInactive()
    {
        DeactivePanels();
        gameObject.SetActive(false); 
    }

    public void ActivatePanels()
    {
        for (int a = 0; a < infoPanels.Count; a++)
        {
            infoPanels[a].SetActive(city);
        }
    }

    public void DeactivePanels()
    {
        for (int a = 0; a < infoPanels.Count; a++)
        {
            infoPanels[a].SetInactive();
        }
    }
}
