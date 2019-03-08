using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = ("Build/CombatUnitBuildConfig"))]
public class CombatUnitBuildConfig : BuildConfig
{
    [Header("Combat Unit Build Config Specific")]
    [SerializeField] CombatUnitConfig combatUnitConfig;


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

    public override BUILDTYPE GetBuildType()
    {
        return BUILDTYPE.COMBAT_UNIT;
    }
}

