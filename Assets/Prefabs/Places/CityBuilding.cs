using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilding : MonoBehaviour {

    protected City cityBuildIn;
    CityPlayerBuildConfig buildConfig;
    [SerializeField] GameEffect resourceBenefit;
    [SerializeField] List<CombatUnitBuildConfig> unitConfigs;
    [SerializeField] List<CityPlayerBuildConfig> buildingConfigs;

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

    public GameEffect ResourceBenefit
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

    public IEnumerable<CityPlayerBuildConfig> BuildConfigs()
    {
        return buildingConfigs;
    }

    public IEnumerable<CombatUnitBuildConfig> UnitConfigs()
    {
        return unitConfigs;
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
