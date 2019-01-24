using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Build/BuildConfig"))]
public class BuildConfig : ScriptableObject {

    [SerializeField] int baseBuildTime = 1;
    [SerializeField] int basePurchaseCost = 1;
    [SerializeField] GameObject gameObjectPrefab;
    [SerializeField] string unitPreFabName;
    public int BaseBuildTime
    {
        get
        {
            return baseBuildTime;
        }

        set
        {
            baseBuildTime = value;
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

    public GameObject GameObjectPrefab
    {
        get
        {
            return gameObjectPrefab;
        }

        set
        {
            gameObjectPrefab = value;
        }
    }

    public string UnitPreFabName
    {
        get
        {
            return unitPreFabName;
        }

        set
        {
            unitPreFabName = value;
        }
    }
}
