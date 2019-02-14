using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBehaviour : MonoBehaviour
{
    const string DEFAULT_ATTACK = "Default Attack";
    protected AbilityConfig config;

    const float PARTICLE_CLEAN_UP_DELAY = 20;
    public abstract void Use(HexCell target = null);
    public abstract List<HexCell> IsValidTarget(HexCell target);

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

    protected void PlayAnimation()
    {
        //var abilityAnimation = config.GetAbilityAnimation();
        //var overrideController = GetComponent<Character>().GetOverrideController();
        //var animator = GetComponent<Animator>();

        //overrideController[DEFAULT_ATTACK] = abilityAnimation;
        //animator.SetTrigger("Attack");
    }

    protected void PlayTextEffect(string text, HexCell cell, Color color, int time = 0)
    {
        HexCellTextEffect effect = Instantiate(config.TextEffect).GetComponent<HexCellTextEffect>();
        effect.Show(text, cell.transform, color, time);
    }
}



