using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExistingBuildingPanel : MonoBehaviour {

    [SerializeField] Image optionImage;
    [SerializeField] TextMeshProUGUI optionName;
    City city;
    CityBuilding cityBuilding;

    public City City
    {
        get
        {
            return city;
        }

        set
        {
            city = value;
        }
    }

    public CityBuilding CityBuilding
    {
        get
        {
            return cityBuilding;
        }

        set
        {
            cityBuilding = value;
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
}
