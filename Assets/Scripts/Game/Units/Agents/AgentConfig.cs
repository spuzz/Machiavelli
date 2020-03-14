using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgentClass
{
    ASSASSIN,
    COUNTERSPY,
    DIPLOMAT,
    INFILTRATOR,
    PRIEST,
    ROGUE,
    SABOTEUR
}

[CreateAssetMenu(menuName = ("Units/Agent"))]
public class AgentConfig : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] int baseMovement = 2;
    [SerializeField] int baseStrength = 25;
    [SerializeField] int visionRange = 0;
    [SerializeField] Texture symbol;
    [SerializeField] Sprite portrait;
    [SerializeField] GameObject meshChild;
    [SerializeField] List<AbilityConfig> abilityConfigs;
    [SerializeField] AgentClass agentClass;
    [SerializeField] GameObject gameEffect;

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

    public int VisionRange
    {
        get
        {
            return visionRange;
        }

        set
        {
            visionRange = value;
        }
    }

    public AgentClass AgentClass
    {
        get
        {
            return agentClass;
        }

        set
        {
            agentClass = value;
        }
    }

    public GameObject GameEffect
    {
        get
        {
            return gameEffect;
        }

        set
        {
            gameEffect = value;
        }
    }
}


