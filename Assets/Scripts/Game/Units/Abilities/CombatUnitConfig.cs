using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CombatClassModifier
{
    [SerializeField] CombatUnit.CombatClassification classification;
    [SerializeField] [Range(-100, 1000)] int modifier;

    public CombatUnit.CombatClassification Classification
    {
        get
        {
            return classification;
        }

        set
        {
            classification = value;
        }
    }

    public int Modifier
    {
        get
        {
            return modifier;
        }

        set
        {
            modifier = value;
        }
    }
}

[CreateAssetMenu(menuName = ("Units/CombatUnit"))]
public class CombatUnitConfig : ScriptableObject {


    [SerializeField] string name;
    [SerializeField] int baseMovement = 2;
    [SerializeField] int baseStrength = 25;
    [SerializeField] Texture symbol;
    [SerializeField] Sprite portrait;
    [SerializeField] GameObject meshChild;
    [SerializeField] List<AbilityConfig> abilityConfigs;
    [SerializeField] CombatUnit.CombatUnitType combatUnitType;
    [SerializeField] CombatUnit.CombatClassification classification;
    [SerializeField] [Range(-100, 1000)] int defenceModifier;
    [SerializeField] [Range(-100, 1000)] int offenceModifier;
    [SerializeField] [Range(-100, 1000)] int siegeModifier;
    [SerializeField] [Range(-100, 1000)] int difficultTerrainModifier;
    [SerializeField] List<CombatClassModifier> classModifers;
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


    public CombatUnit.CombatUnitType CombatUnitType
    {
        get
        {
            return combatUnitType;
        }

        set
        {
            combatUnitType = value;
        }
    }

    public int DefenceModifier
    {
        get
        {
            return defenceModifier;
        }

        set
        {
            defenceModifier = value;
        }
    }

    public int OffenceModifier
    {
        get
        {
            return offenceModifier;
        }

        set
        {
            offenceModifier = value;
        }
    }

    public int SiegeModifier
    {
        get
        {
            return siegeModifier;
        }

        set
        {
            siegeModifier = value;
        }
    }

    public int DifficultTerrainModifier
    {
        get
        {
            return difficultTerrainModifier;
        }

        set
        {
            difficultTerrainModifier = value;
        }
    }

    public List<CombatClassModifier> ClassModifers
    {
        get
        {
            return classModifers;
        }

        set
        {
            classModifers = value;
        }
    }

    public CombatUnit.CombatClassification Classification
    {
        get
        {
            return classification;
        }

        set
        {
            classification = value;
        }
    }
}
