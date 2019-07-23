using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityBuildingUI : CityInfoPanel {
    [SerializeField] HumanPlayer humanPlayer;

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
    }
    public override void UpdateUI(City cityToWatch)
    {
    }

}
