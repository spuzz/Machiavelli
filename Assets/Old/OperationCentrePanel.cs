//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class OperationCentrePanel : MonoBehaviour {


//    [SerializeField] List<OperationCentreInfoPanel> infoPanels;
//    OperationCentre opCentre;


//    public OperationCentre OpCentre
//    {
//        get
//        {
//            return opCentre;
//        }

//        set
//        {
//            opCentre = value;
//        }
//    }


//    public void SetActive(OperationCentre opCentreToWatch)
//    {
//        opCentre = opCentreToWatch;
//        gameObject.SetActive(true);
//        SetActiveInfoPanel(0);
//    }

//    public void SetInactive()
//    {
//        SetActiveInfoPanel(-1);
//        gameObject.SetActive(false);
//    }

//    public void SetActiveInfoPanel(int panelNumber)
//    {
//        for(int a = 0; a < infoPanels.Count; a++)
//        {
//            if(a == panelNumber)
//            {
//                infoPanels[a].SetActive(OpCentre);
//            }
//            else
//            {
//                infoPanels[a].SetInactive();
//            }
//        }
//    }

//    public void UpdateUI()
//    {
//        if (isActiveAndEnabled)
//        {

//            foreach(OperationCentreInfoPanel panel in infoPanels)
//            {
//                if(panel.isActiveAndEnabled)
//                {
//                    panel.UpdateUI(opCentre);
//                }
//            }
//        }
//    }
//}
