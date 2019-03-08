using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Abilities : MonoBehaviour
{
    [SerializeField] List<AbilityConfig> abilities;

    AudioSource audioSource;
    Unit unit;
    HexGameUI hexGameUI;
    public List<AbilityConfig> AbilitiesList
    {
        get
        {
            return abilities;
        }

        set
        {
            abilities = value;
        }
    }

    void Awake()
    {
        hexGameUI = FindObjectOfType<HexGameUI>();
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        unit = GetComponent<Unit>();
        AttachInitialAbilities();

    }

    public bool IsAbilityValid(int index, HexCell hexCell)
    {
        if (unit.GetMovementLeft() <= 0)
        {
            return false;
        }
        if(AbilitiesList[index].IsValidTarget(hexCell).Count == 0)
        {
            return false;
        }
        return true;
    }

    public List<HexCell> ValidTargets(int index, HexCell hexCell)
    {
        if (unit.GetMovementLeft() <= 0)
        {
            return new List<HexCell>();
        }

        return AbilitiesList[index].IsValidTarget(hexCell);
    }


    public void AttemptAbility(int index, HexCell hexCell)
    {

        int goldCost = AbilitiesList[index].GetCost();
        if(unit.GetMovementLeft() <= 0)
        {
            if (!audioSource.isPlaying)
            {
                //audioSource.PlayOneShot(outOfEnergy);
            }
            return;
        }
        List<HexCell> targets = ValidTargets(index, hexCell);
        if(targets.Count == 1 && AbilitiesList[index].Range == 0)
        {
            UseAbility(index, targets[0]);
        }
        else
        {
            hexGameUI.DoAbilitySelection(targets, index);
        }
        
    }

    public void UseAbility(int index, HexCell hexCell)
    {
        unit.SetMovementLeft(0);
        AbilitiesList[index].Use(hexCell);
    }

    public int GetNumberOfAbilities()
    {
        return AbilitiesList.Count;
    }


    private void AttachInitialAbilities()
    {
        List<AbilityConfig> abilityListClone = new List<AbilityConfig>();
        foreach (AbilityConfig ability in AbilitiesList)
        {
            var abilityClone = Instantiate(ability);
            abilityClone.AddComponent(gameObject);
            abilityListClone.Add(abilityClone);
        }
        AbilitiesList.Clear();
        foreach (AbilityConfig ability in abilityListClone)
        {
            AbilitiesList.Add(ability);
        }
    }

}


