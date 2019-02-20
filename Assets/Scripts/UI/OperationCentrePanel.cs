using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationCentrePanel : MonoBehaviour {


    [SerializeField] List<OperationCentreInfoPanel> infoPanels;
    OperationCentre opCentre;


    public OperationCentre OpCentre
    {
        get
        {
            return opCentre;
        }

        set
        {
            opCentre = value;
        }
    }


    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        SetActiveInfoPanel(0);

    }

    public void SetActiveInfoPanel(int panelNumber)
    {
        for(int a = 0; a < infoPanels.Count; a++)
        {
            if(a == panelNumber)
            {
                infoPanels[a].gameObject.SetActive(true);
                infoPanels[a].UpdateUI(OpCentre);
            }
            else
            {
                infoPanels[a].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateUI()
    {
        if (isActiveAndEnabled)
        {

            foreach(OperationCentreInfoPanel panel in infoPanels)
            {
                if(panel.isActiveAndEnabled)
                {
                    panel.UpdateUI(opCentre);
                }
            }
        }
    }
}
