using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilding : MonoBehaviour {

    protected City cityBuildIn;
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
