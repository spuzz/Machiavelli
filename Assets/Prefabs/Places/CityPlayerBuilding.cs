using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityPlayerBuilding : MonoBehaviour {

    [SerializeField] ResourceBenefit resourceBenefit;
    City cityBuildIn;
    CityPlayerBuildConfig buildConfig;

    public City CityBuildIn
    {
        get
        {
            return cityBuildIn;
        }

        set
        {
            cityBuildIn = value;
        }
    }

    public ResourceBenefit ResourceBenefit
    {
        get
        {
            return resourceBenefit;
        }

        set
        {
            resourceBenefit = value;
        }
    }

    public CityPlayerBuildConfig BuildConfig
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
}
