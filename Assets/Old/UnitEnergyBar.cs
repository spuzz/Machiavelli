//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class UnitEnergyBar : MonoBehaviour
//{
//    RawImage energyBarRawImage = null;
//    Agent agent = null;
//    int currentEnergy;
//    int maxEnergy;
//    public Agent Agent
//    {
//        get
//        {
//            return agent;
//        }

//        set
//        {
//            agent = value;
//            currentEnergy = agent.Energy;
//            maxEnergy = 100;
//            UpdateEnergy(0);
//        }
//    }

//    // Use this for initialization
//    void Awake()
//    {
//        energyBarRawImage = GetComponent<RawImage>();
//    }

//    // Update is called once per frame
//    public void UpdateEnergy(int energyChange)
//    {
//        float energyAfterChange = (float)currentEnergy + (float)energyChange;
//        if(energyAfterChange < 0) { energyAfterChange = 0;  }
//        float healthAsPerc = energyAfterChange / (float)maxEnergy;
//        float xValue = -(healthAsPerc / 2f) - 0.5f;
//        energyBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
//        currentEnergy = (int)energyAfterChange;
//    }

//    public void SetEnergy(int energy)
//    {
//        float healthAsPerc = energy / (float)maxEnergy;
//        float xValue = -(healthAsPerc / 2f) - 0.5f;
//        energyBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
//        currentEnergy = energy;
//    }

//}
