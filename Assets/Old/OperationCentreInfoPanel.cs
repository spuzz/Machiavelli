//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public abstract class OperationCentreInfoPanel : MonoBehaviour {

//    protected OperationCentre opCentre;
//    [SerializeField] BuildingChoicePanel buildingChoicePanel;
//    bool active = false;

//    public BuildingChoicePanel BuildingChoicePanel
//    {
//        get
//        {
//            return buildingChoicePanel;
//        }

//        set
//        {
//            buildingChoicePanel = value;
//        }
//    }

//    public void SetActive(OperationCentre opCentre)
//    {

//        if (active == false)
//        {
//            gameObject.SetActive(true);
//            this.opCentre = opCentre;
//            this.opCentre.onInfoChange += UpdateUI;
//            active = true;
//            UpdateUI(this.opCentre);
//        }
//        else if (this.opCentre != opCentre)
//        {
//            SetInactive();
//            SetActive(opCentre);
//        }

//    }

//    public abstract void UpdateUI(OperationCentre opCentre);

//    public void SetInactive()
//    {
//        if (active == true)
//        {
//            if(BuildingChoicePanel)
//            {
//                BuildingChoicePanel.SetInactive();
//            }
            
//            gameObject.SetActive(false);
//            if(opCentre)
//            {
//                opCentre.onInfoChange -= UpdateUI;
//            }
            
//            active = false;
//        }
//    }
//}
