using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityStateBuilding : MonoBehaviour {


    [SerializeField] ResourceBenefit resourceBenefit;
    City cityBuildIn;
    CityStateBuildConfig buildConfig;


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

    public CityStateBuildConfig BuildConfig
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
