﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBehaviour : MonoBehaviour
{
    const string DEFAULT_ATTACK = "Default Attack";
    protected AbilityConfig config;
    protected string abilityText;
    const float PARTICLE_CLEAN_UP_DELAY = 20;
    public abstract bool Use(HexCell target = null);
    protected bool failed = false;

    public virtual void ShowAbility(HexCell target = null)
    {

        PlayParticleEffect();
        PlayAbilitySound();
        PlayAnimation(target);
        string text = abilityText;
        if(failed)
        {
            target.TextEffectHandler.AddTextEffect(abilityText + " FAILED", target.transform, Color.red);
        }
        else
        {
            target.TextEffectHandler.AddTextEffect(abilityText + " SUCCEDED", target.transform, Color.green);
        }
        
    }


    public abstract void FinishAbility(HexCell target = null);

    public abstract bool IsValidTarget(HexCell target);

    public abstract int GetSuccessChance(HexCell target);

    public virtual bool IsGoodTarget(HexCell target) { return true; }

    public void RunAll(HexCell target = null)
    {
        Use(target);
        ShowAbility(target);
        FinishAbility(target);
    }
    public List<HexCell> GetValidTargets(HexCell location)
    {
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(location, config.Range);
        List<HexCell> agentCells = cells.FindAll(c => IsValidTarget(c));

        return agentCells;
    }

    public void SetConfig(AbilityConfig configToSet)
    {
        config = configToSet;
    }
    protected void PlayParticleEffect()
    {
        GameObject particles = config.GetParticlePrefab();

        if (particles != null)
        {
            var particleObject = Instantiate(particles, gameObject.transform.position, particles.transform.rotation);
            particleObject.transform.parent = transform;
            particleObject.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DestroyParticleWhenFinished(particleObject));
        }
    }

    IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
    {
        while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
        {
            yield return new WaitForSeconds(PARTICLE_CLEAN_UP_DELAY);
        }
        Destroy(particlePrefab);
        yield return new WaitForEndOfFrame();
    }

    protected void PlayAbilitySound()
    {
        var abilitySound = config.GetRandomAudioClip();
        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = abilitySound;
        audioSource.Play();
    }

    protected void PlayAnimation(HexCell target)
    {
        StartCoroutine(gameObject.GetComponent<Unit>().HexUnit.LookAt(target.transform.position));
        var abilityAnimation = config.GetAbilityAnimation();
        var overrideController = GetComponent<HexUnit>().AnimatorOverrideController;
        var animator = GetComponentInChildren<Animator>();

        overrideController["Ability"] = abilityAnimation;
        animator.SetTrigger("UseAbility");

    }

    public virtual bool Merge() { return false; }
}



