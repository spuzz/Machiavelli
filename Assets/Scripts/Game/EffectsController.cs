using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectsController : MonoBehaviour {

    [SerializeField] GameEffect prefab;
    [SerializeField] GameEffect totalEffects;
    Dictionary<string, GameEffect> combinedEffects = new Dictionary<string, GameEffect>();
    Dictionary<GameObject, GameEffect> individualEffects = new Dictionary<GameObject, GameEffect>();


    public delegate void OnInfoChange();
    public event OnInfoChange onInfoChange;

    public GameEffect TotalEffects
    {
        get
        {
            return totalEffects;
        }

        set
        {
            totalEffects = value;
        }
    }

    public void AddEffect(GameObject obj, GameEffect effect)
    {
        individualEffects[obj] = effect;
        UpdateCombinedEffect(effect.EffectName);
    }

    public void RemoveEffect(GameObject obj, string effectName)
    {
        individualEffects.Remove(obj);
        UpdateCombinedEffect(effectName);

    }
    private void UpdateCombinedEffect(string effectName)
    {
        if(combinedEffects.ContainsKey(effectName))
        {
            TotalEffects.RemoveEffect(combinedEffects[effectName]);
            combinedEffects[effectName].ResetEffect();

        }
        else
        {
            combinedEffects[effectName] = Instantiate<GameEffect>(prefab, gameObject.transform.Find("CombinedEffects").transform);
            combinedEffects[effectName].EffectName = effectName;
        }

        AddCombinedEffects(effectName);
    }

    private void AddCombinedEffects(string effectName)
    {
        List<GameEffect> effects = individualEffects.Values.ToList().FindAll(C => C.EffectName == effectName);
        if (effects.Count > 0)
        {
            CombineEffects(effects, effectName);
            TotalEffects.AddEffect(combinedEffects[effectName]);
        }
    }

    private void CombineEffects(List<GameEffect> list, string effectName)
    {
        bool init = true;
        foreach(GameEffect effect in list)
        {
            if(init == true)
            {
                combinedEffects[effectName].AddEffect(effect);
            }
            else
            {
                combinedEffects[effectName].CombineEffect(effect);
            }

        }
    }

    public void NotifyInfoChange()
    {
        if (onInfoChange != null)
        {
            onInfoChange();
        }
    }
}
