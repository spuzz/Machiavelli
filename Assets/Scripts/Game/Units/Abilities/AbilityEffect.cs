using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityEffect {

    [SerializeField] string abilityName;
    [SerializeField] int successChance = 0;
    [SerializeField] int costPerc = 0;
    [SerializeField] int effectBonus = 0;
    [SerializeField] int duration = 0;

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

    public int SuccessChance
    {
        get
        {
            return successChance;
        }

        set
        {
            successChance = value;
        }
    }

    public int CostPerc
    {
        get
        {
            return costPerc;
        }

        set
        {
            costPerc = value;
        }
    }

    public int EffectBonus
    {
        get
        {
            return effectBonus;
        }

        set
        {
            effectBonus = value;
        }
    }

    public int Duration
    {
        get
        {
            return duration;
        }

        set
        {
            duration = value;
        }
    }


    public void AddEffect(AbilityEffect effect)
    {
        successChance += effect.SuccessChance;
        costPerc += effect.CostPerc;
        effectBonus += effect.EffectBonus;
        duration += effect.Duration;
    }

    public void RemoveEffect(AbilityEffect effect)
    {
        successChance -= effect.SuccessChance;
        costPerc -= effect.CostPerc;
        effectBonus -= effect.EffectBonus;
        duration -= effect.Duration;
    }

    public void CombineAbilityEffect(AbilityEffect effect)
    {
        successChance = GameEffect.GetMax(successChance, effect.successChance);
        CostPerc = GameEffect.GetMax(costPerc, effect.CostPerc);
        effectBonus = GameEffect.GetMax(effectBonus, effect.EffectBonus);
        duration = GameEffect.GetMax(duration, effect.Duration);

    }
}
