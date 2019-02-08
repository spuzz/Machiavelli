using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class AbilityConfig : ScriptableObject
{

    [Header("Special Ability General")]
    [SerializeField] int cost = 10;
    [SerializeField] GameObject particlePrefab = null;
    [SerializeField] AnimationClip abilityAnimation;
    [SerializeField] AudioClip[] audioClips;

    protected AbilityBehaviour behaviour;

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
    public bool IsValidTarget(HexCell target = null)
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

