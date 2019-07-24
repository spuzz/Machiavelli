using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOption : MonoBehaviour {

    [SerializeField] Image optionImage;
    [SerializeField] TextMeshProUGUI optionName;
    City city;
    BuildConfig buildConfig;

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

    public BuildConfig BuildConfig
    {
        get
        {
            return buildConfig;
        }

        set
        {
            buildConfig = value;
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

    public void OnBuild()
    {
        city.AddBuild(BuildConfig);
    }
}
