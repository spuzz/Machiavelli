using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExistingUnit : MonoBehaviour {

    [SerializeField] Image optionImage;
    [SerializeField] TextMeshProUGUI optionName;
    Unit unit;
    

    public Unit Unit
    {
        get
        {
            return unit;
        }

        set
        {
            unit = value;
        }
    }


    public Image OptionImage
    {
        get
        {
            return optionImage;
        }

        set
        {
            optionImage = value;
        }
    }

    public TextMeshProUGUI OptionName
    {
        get
        {
            return optionName;
        }

        set
        {
            optionName = value;
        }
    }

    public void SelectUnit()
    {
        FindObjectOfType<HexGameUI>().SelectUnit(unit.HexUnit);
    }
}
