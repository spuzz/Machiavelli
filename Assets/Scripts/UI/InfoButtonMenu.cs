using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButtonMenu : MonoBehaviour {

    [SerializeField] List<GameObject> infoPanels;

    public void OpenPanel(int panelNumber)
    {
        infoPanels[panelNumber].SetActive(true);
    }

    public void ClosePanels()
    {
        foreach(GameObject obj in infoPanels)
        {
            obj.SetActive(false);
        }
    }
}
