﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class AbilityConfig : ScriptableObject
{
    public enum AbilityType
    {
        EnemyCity,
        EnemyPlayerCity,
        FriendlyCity,
        City,
        EnemyAgent,
        AllyAgent,
        EnemyUnit,
        FriendlyUnit,
        EnemyAndFriendlyUnit,
        EnemyOperationCentre,
        NeutralOperationCentre,
        Misc
    }
    [Header("Special Ability General")]
    [SerializeField] int energyCost = 10;
    [SerializeField] int range = 1;
    [SerializeField] GameObject particlePrefab = null;
    [SerializeField] AnimationClip abilityAnimation;
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] Sprite defaultIcon;
    [SerializeField] GameObject textEffect;
    [SerializeField] string abilityName;
    [SerializeField] AbilityType abilityType = AbilityType.Misc;
    [SerializeField] string displayName;
    [SerializeField] string toolTipText;


    protected AbilityBehaviour behaviour;

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

    public Sprite DefaultIcon
    {
        get
        {
            return defaultIcon;
        }

        set
        {
            defaultIcon = value;
        }
    }

    public GameObject TextEffect
    {
        get
        {
            return textEffect;
        }

        set
        {
            textEffect = value;
        }
    }

    public string AbilityName
    {
        get
        {
            return abilityName;
        }

        set
        {
            abilityName = value;
        }
    }

    public AbilityType Type
    {
        get
        {
            return abilityType;
        }

        set
        {
            abilityType = value;
        }
    }

    public string DisplayName
    {
        get
        {
            return displayName;
        }

        set
        {
            displayName = value;
        }
    }

    public string ToolTipText
    {
        get
        {
            return toolTipText;
        }

        set
        {
            toolTipText = value;
        }
    }

    abstract public AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo);

    public void AddComponent(GameObject gameObjectToAttachTo)
    {
        behaviour = GetBehaviourComponent(gameObjectToAttachTo);
        behaviour.SetConfig(this);
    }

    public void Use(HexCell target = null)
    {
        behaviour.Use(target);
    }

    public void Show(HexCell target = null)
    {
        behaviour.ShowAbility(target);
    }

    public void Finish(HexCell target = null)
    {
        behaviour.FinishAbility(target);
    }

    public void RunAll(HexCell target = null)
    {
        behaviour.RunAll(target);
    }
    public bool IsValidTarget(HexCell target)
    {
        return behaviour.IsValidTarget(target);
    }

    public bool IsGoodTarget(HexCell target)
    {
        return behaviour.IsGoodTarget(target);
    }

    public List<HexCell> GetValidTargets(HexCell location)
    {
        return behaviour.GetValidTargets(location);
    }

    public int GetEnergyCost()
    {
        return energyCost;
    }

    public GameObject GetParticlePrefab()
    {
        return particlePrefab;
    }

    public AudioClip GetRandomAudioClip()
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }

    public AnimationClip GetAbilityAnimation()
    {
        return abilityAnimation;
    }

    public bool Merge()
    {
        return behaviour.Merge();
    }

}

