using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Units/Agent/Talent"))]
public class Talent : ScriptableObject {

    [SerializeField] string talentName;
    [SerializeField] Sprite sprite;
    [SerializeField] GameEffect gameEffect;
    [SerializeField] string description;
    [SerializeField] AbilityConfig abilityConfig;

    public string TalentName
    {
        get
        {
            return talentName;
        }

        set
        {
            talentName = value;
        }
    }

    public GameEffect GameEffect
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

    public string Description
    {
        get
        {
            return description;
        }

        set
        {
            description = value;
        }
    }

    public AbilityConfig AbilityConfig
    {
        get
        {
            return abilityConfig;
        }

        set
        {
            abilityConfig = value;
        }
    }

    public Sprite Sprite
    {
        get
        {
            return sprite;
        }

        set
        {
            sprite = value;
        }
    }
}
