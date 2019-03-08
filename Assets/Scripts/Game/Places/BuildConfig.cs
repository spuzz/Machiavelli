using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BuildConfig : ScriptableObject {

    public enum BUILDTYPE
    {
        AGENT,
        COMBAT_UNIT,
        CITY_STATE_BUILDING,
        CITY_PLAYER_BUILDING,
        OPCENTRE_BUILDING,
        MISC
    }
    [Header("Build Config General")]
    [SerializeField] int baseProductionCost = 1;
    [SerializeField] int basePurchaseCost = 1;
    [SerializeField] List<BuildConfig> prerequisites;
    [SerializeField] FocusType focusType;
    [SerializeField] Sprite buildingImage;
    [SerializeField] string name;

    public int BaseBuildTime
    {
        get
        {
            return baseProductionCost;
        }

        set
        {
            baseProductionCost = value;
        }
    }

    public int BasePurchaseCost
    {
        get
        {
            return basePurchaseCost;
        }

        set
        {
            basePurchaseCost = value;
        }
    }


    public FocusType FocusType
    {
        get
        {
            return focusType;
        }

        set
        {
            focusType = value;
        }
    }


    public Sprite BuildingImage
    {
        get
        {
            return buildingImage;
        }

        set
        {
            buildingImage = value;
        }
    }

    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public abstract BUILDTYPE GetBuildType();

}
