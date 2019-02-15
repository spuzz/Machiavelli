using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class AbilityConfig : ScriptableObject
{

    [Header("Special Ability General")]
    [SerializeField] int cost = 10;
    [SerializeField] int range = 1;
    [SerializeField] GameObject particlePrefab = null;
    [SerializeField] AnimationClip abilityAnimation;
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] Sprite defaultIcon;
    [SerializeField] GameObject textEffect;
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
    public List<HexCell> IsValidTarget(HexCell target = null)
    {
        return behaviour.IsValidTarget(target);
    }

    public int GetCost()
    {
        return cost;
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

}

