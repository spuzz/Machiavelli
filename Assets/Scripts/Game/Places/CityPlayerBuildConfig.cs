using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = ("Build/CityPlayerBuildConfig"))]
public class CityPlayerBuildConfig : BuildConfig
{
    [Header("City Player Build Config Specific")]
    [SerializeField] GameObject buildPrefab;


    public GameObject BuildPrefab
    {
        get
        {
            return buildPrefab;
        }

        set
        {
            buildPrefab = value;
        }
    }

    public override BUILDTYPE GetBuildType()
    {
        return BUILDTYPE.CITY_PLAYER_BUILDING;
    }
}

