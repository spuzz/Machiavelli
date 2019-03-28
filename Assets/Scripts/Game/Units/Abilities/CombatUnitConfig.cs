using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Units/CombatUnit"))]
public class CombatUnitConfig : ScriptableObject {


    [SerializeField] string name;
    [SerializeField] int baseMovement = 2;
    [SerializeField] int baseStrength = 25;
    [SerializeField] int baseRangeStrength = 0;
    [SerializeField] int range = 0;
    [SerializeField] Texture symbol;
    [SerializeField] Sprite portrait;
    [SerializeField] GameObject meshChild;
    [SerializeField] List<AbilityConfig> abilityConfigs;

    public IEnumerable<AbilityConfig> GetAbilityConfigs()
    {
        return abilityConfigs;
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

    public int BaseMovement
    {
        get
        {
            return baseMovement;
        }

        set
        {
            baseMovement = value;
        }
    }

    public int BaseStrength
    {
        get
        {
            return baseStrength;
        }

        set
        {
            baseStrength = value;
        }
    }

    public Texture Symbol
    {
        get
        {
            return symbol;
        }

        set
        {
            symbol = value;
        }
    }

    public Sprite Portrait
    {
        get
        {
            return portrait;
        }

        set
        {
            portrait = value;
        }
    }

    public GameObject MeshChild
    {
        get
        {
            return meshChild;
        }

        set
        {
            meshChild = value;
        }
    }

    public int Range
    {
        get
        {
            return range;
        }

        set
        {
            range = value;
        }
    }

    public int BaseRangeStrength
    {
        get
        {
            return baseRangeStrength;
        }

        set
        {
            baseRangeStrength = value;
        }
    }
}
