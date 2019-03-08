using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = ("Build/OpCentreBuildConfig"))]
public class OpCentreBuildConfig : BuildConfig
{
    [Header("Op Centre Build Config Specific")]
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
        return BUILDTYPE.OPCENTRE_BUILDING;
    }
}

