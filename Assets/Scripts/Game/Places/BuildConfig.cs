using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Build/BuildConfig"))]
public class BuildConfig : ScriptableObject {

    [SerializeField] int baseProductionCost = 1;
    [SerializeField] int basePurchaseCost = 1;
    [SerializeField] GameObject gameObjectPrefab;
    [SerializeField] string preFabName;
    [SerializeField] FocusType focusType;
    [SerializeField] CombatUnitConfig combatUnitConfig;
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

    public string PreFabName
    {
        get
        {
            return preFabName;
        }

        set
        {
            preFabName = value;
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

    public CombatUnitConfig CombatUnitConfig
    {
        get
        {
            return combatUnitConfig;
        }

        set
        {
            combatUnitConfig = value;
        }
    }
}
