using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEffect : MonoBehaviour
{
    [System.Serializable]
    public struct FocusBonus
    {
        public FocusType type;
        public Vector2 prodBonus;
        public int percCostReduction;
        public int maintenanceReduction;
    }

    [SerializeField] string effectName;

    [Header("General Effects")]
    [SerializeField] int visionRange;

    [Header("City Effects")]
    [SerializeField] Vector2 production;
    [SerializeField] Vector2 politicalCapital;
    [SerializeField] Vector2 gold;
    [SerializeField] Vector2 science;
    [SerializeField] Vector2 food;
    [SerializeField] Vector2 defence;
    [SerializeField] List<FocusBonus> focusProductionBonus;
    [SerializeField] int happiness;
    [SerializeField] int loyalty;

    [Header("Agent Effects")]
    [SerializeField] int successChance;
    [SerializeField] int successChanceOnCity;
    [SerializeField] int successChanceOnUnit;
    [SerializeField] int woundedOnFail;
    [SerializeField] int xpGainPerc;
    [SerializeField] int xpLevelUpPerc;
    [SerializeField] List<AbilityEffect> abilityEffects;

    [Header("Agent Influence Effects")]
    [SerializeField] int influenceRange;
    [SerializeField] int influenceEffectPerc;

    [Header("Combat Unit Effects")]
    [SerializeField] int combatStrength;
    [SerializeField] int ambushChance;
    [SerializeField] int ambushBonus;

    [Header("General Unit Effects")]
    [SerializeField] int movementSpeed;

    public Vector2 Production
    {
        get
        {
            return production;
        }

        set
        {
            production = value;
        }
    }

    public Vector2 Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
        }
    }

    public Vector2 Science
    {
        get
        {
            return science;
        }

        set
        {
            science = value;
        }
    }

    public Vector2 Food
    {
        get
        {
            return food;
        }

        set
        {
            food = value;
        }
    }

    public Vector2 Defence
    {
        get
        {
            return defence;
        }

        set
        {
            defence = value;
        }
    }

    public Vector2 PoliticalCapital
    {
        get
        {
            return politicalCapital;
        }

        set
        {
            politicalCapital = value;
        }
    }

    public List<FocusBonus> FocusProductionBonus
    {
        get
        {
            return focusProductionBonus;
        }

        set
        {
            focusProductionBonus = value;
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

    public int Happiness
    {
        get
        {
            return happiness;
        }

        set
        {
            happiness = value;
        }
    }

    public int Loyalty
    {
        get
        {
            return loyalty;
        }

        set
        {
            loyalty = value;
        }
    }

    public string EffectName
    {
        get
        {
            return effectName;
        }

        set
        {
            effectName = value;
        }
    }

    public int InfluenceRange
    {
        get
        {
            return influenceRange;
        }

        set
        {
            influenceRange = value;
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

    public int SuccessChanceOnCity
    {
        get
        {
            return successChanceOnCity;
        }

        set
        {
            successChanceOnCity = value;
        }
    }

    public int SuccessChanceOnUnit
    {
        get
        {
            return successChanceOnUnit;
        }

        set
        {
            successChanceOnUnit = value;
        }
    }

    public int WoundedOnFail
    {
        get
        {
            return woundedOnFail;
        }

        set
        {
            woundedOnFail = value;
        }
    }

    public void AddEffect(GameEffect effect)
    {
        Gold += effect.Gold;
        Food += effect.Food;
        Production += effect.Production;
        Science += effect.Science;
        Defence += effect.Defence;
        PoliticalCapital += effect.PoliticalCapital;
        VisionRange += effect.VisionRange;
        Happiness += effect.Happiness;
        Loyalty += effect.Loyalty;

        SuccessChance += effect.SuccessChance;
        SuccessChanceOnCity += effect.SuccessChanceOnCity;
        SuccessChanceOnUnit += effect.SuccessChanceOnUnit;
        WoundedOnFail += effect.WoundedOnFail;
        xpGainPerc += effect.xpGainPerc;
        xpLevelUpPerc += effect.xpLevelUpPerc;
        InfluenceRange += effect.InfluenceRange;
        influenceEffectPerc += effect.influenceEffectPerc;
        combatStrength += effect.combatStrength;
        ambushChance += effect.ambushChance;
        ambushBonus += effect.ambushBonus;
        movementSpeed += effect.movementSpeed;

        foreach (AbilityEffect eff in effect.abilityEffects)
        {
            AddAbilityEffect(eff);
        }

        foreach (FocusBonus bonus in effect.FocusProductionBonus)
        {
            IEnumerable<FocusBonus> bonuses = focusProductionBonus.Where(c => c.type == bonus.type);
            if (bonuses.Count() > 0)
            {
                FocusBonus updateBonus = bonuses.First();
                updateBonus.prodBonus += bonus.prodBonus;
                updateBonus.maintenanceReduction += bonus.maintenanceReduction;
                updateBonus.percCostReduction += bonus.percCostReduction;
            }
            else
            {
                focusProductionBonus.Add(bonus);
            }
        }  
    }

    public void RemoveEffect(GameEffect effect)
    {
        Gold -= effect.Gold;
        Food -= effect.Food;
        Production -= effect.Production;
        Science -= effect.Science;
        Defence -= effect.Defence;
        PoliticalCapital -= effect.PoliticalCapital;
        VisionRange -= effect.VisionRange;
        Happiness -= effect.happiness;
        Loyalty -= effect.Loyalty;

        SuccessChance -= effect.SuccessChance;
        SuccessChanceOnCity -= effect.SuccessChanceOnCity;
        SuccessChanceOnUnit -= effect.SuccessChanceOnUnit;
        WoundedOnFail -= effect.WoundedOnFail;
        xpGainPerc -= effect.xpGainPerc;
        xpLevelUpPerc -= effect.xpLevelUpPerc;
        InfluenceRange -= effect.InfluenceRange;
        influenceEffectPerc -= effect.influenceEffectPerc;
        combatStrength -= effect.combatStrength;
        ambushChance -= effect.ambushChance;
        ambushBonus -= effect.ambushBonus;

        movementSpeed += effect.movementSpeed;

        foreach (AbilityEffect eff in effect.abilityEffects)
        {
            RemoveAbilityEffect(eff);
        }

        foreach (FocusBonus bonus in effect.FocusProductionBonus)
        {
            IEnumerable<FocusBonus> bonuses = focusProductionBonus.Where(c => c.type == bonus.type);
            if (bonuses.Count() > 0)
            {
                FocusBonus updateBonus = bonuses.First();
                updateBonus.prodBonus -= bonus.prodBonus;
                updateBonus.maintenanceReduction -= bonus.maintenanceReduction;
                updateBonus.percCostReduction -= bonus.percCostReduction;
            }
        }
    }

    public void CombineEffect(GameEffect effect)
    {
        Gold = GetMaxVector2(Gold, effect.Gold);
        Food = GetMaxVector2(Food, effect.Food);
        Production = GetMaxVector2(Production, effect.Production);
        Science = GetMaxVector2(Science, effect.Science);
        Defence = GetMaxVector2(Defence, effect.Defence);
        PoliticalCapital = GetMaxVector2(PoliticalCapital, effect.PoliticalCapital);
        VisionRange = GetMax(VisionRange, effect.VisionRange);
        Happiness = GetMax(Happiness, effect.Happiness);
        Loyalty = GetMax(Happiness, effect.Loyalty);

        

        SuccessChance = GetMax(SuccessChance, effect.SuccessChance);
        SuccessChanceOnCity = GetMax(SuccessChanceOnCity, effect.SuccessChanceOnCity);
        SuccessChanceOnUnit = GetMax(SuccessChanceOnUnit, effect.SuccessChanceOnUnit);
        WoundedOnFail = GetMax(WoundedOnFail, effect.WoundedOnFail);
        xpGainPerc = GetMax(xpGainPerc, effect.xpGainPerc);
        xpLevelUpPerc = GetMax(xpLevelUpPerc, effect.xpLevelUpPerc);
        InfluenceRange = GetMax(InfluenceRange, effect.InfluenceRange);
        influenceEffectPerc = GetMax(influenceEffectPerc, effect.influenceEffectPerc);
        combatStrength = GetMax(combatStrength, effect.combatStrength);
        ambushChance = GetMax(ambushChance, effect.ambushChance);
        ambushBonus = GetMax(ambushBonus, effect.ambushBonus);

        foreach(AbilityEffect eff in effect.abilityEffects)
        {
            CombineAbilityEffect(eff);
        }
        //foreach (FocusBonus bonus in effect.FocusProductionBonus)
        //{
        //    IEnumerable<FocusBonus> bonuses = focusProductionBonus.Where(c => c.type == bonus.type);
        //    if (bonuses.Count() > 0)
        //    {
        //        FocusBonus updateBonus = bonuses.First();
        //        updateBonus.prodBonus += bonus.prodBonus;
        //        updateBonus.maintenanceReduction += bonus.maintenanceReduction;
        //        updateBonus.percCostReduction += bonus.percCostReduction;
        //    }
        //    else
        //    {
        //        focusProductionBonus.Add(bonus);
        //    }
        //}
    }

    public void AddAbilityEffect(AbilityEffect effect)
    {
        int index = abilityEffects.FindIndex(c => c.AbilityName == effect.AbilityName);
        if (index == -1)
        {
            abilityEffects.Add(effect);
        }
        else
        {
            abilityEffects[index].AddEffect(effect);
        }
    }

    public void RemoveAbilityEffect(AbilityEffect effect)
    {
        int index = abilityEffects.FindIndex(c => c.AbilityName == effect.AbilityName);
        if (index != -1)
        {
            abilityEffects[index].RemoveEffect(effect);
        }

    }
   
    public void CombineAbilityEffect(AbilityEffect effect)
    {
        int index = abilityEffects.FindIndex(c => c.AbilityName == effect.AbilityName);
        if (index != -1)
        {
            abilityEffects[index].CombineAbilityEffect(effect);
        }
        else
        {
            abilityEffects.Add(effect);
        }
    }

    public void ResetEffect()
    {
        Gold = new Vector2(0, 0);
        Food = new Vector2(0, 0);
        Production = new Vector2(0, 0);
        Science = new Vector2(0, 0);
        Defence = new Vector2(0, 0);
        PoliticalCapital = new Vector2(0, 0);
        VisionRange = 0;
        Happiness = 0;
        Loyalty = 0;

        SuccessChance = 0;
        SuccessChanceOnCity = 0;
        SuccessChanceOnUnit = 0;
        WoundedOnFail = 0;
        xpGainPerc = 0;
        xpLevelUpPerc = 0;
        InfluenceRange = 0;
        influenceEffectPerc = 0;
        combatStrength = 0;
        ambushChance = 0;
        ambushBonus = 0;

        abilityEffects = new List<AbilityEffect>();
    }

    public static int GetMax(int first, int second)
    {
        return first > second ? first : second;
    }

    public static float GetMax(float first, float second)
    {
        return first > second ? first : second;
    }

    public static Vector2 GetMaxVector2(Vector2 first, Vector2 second)
    {
        return new Vector2(GetMax(first.x, second.x), GetMax(first.y, second.y));
    }
}
