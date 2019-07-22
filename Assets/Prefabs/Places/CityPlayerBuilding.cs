using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityPlayerBuilding : MonoBehaviour {

    [SerializeField] ResourceBenefit resourceBenefit;
    protected City cityBuildIn;
    private Player playersBuilding;
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

    public Player PlayersBuilding
    {
        get
        {
            return playersBuilding;
        }

        set
        {
            playersBuilding = value;
        }
    }

    public virtual void Init()
    {

    }
    public virtual void StartTurn()
    {

    }

    public virtual void Destroy()
    {

    }

}
