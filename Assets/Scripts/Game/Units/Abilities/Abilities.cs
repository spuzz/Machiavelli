using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Abilities : MonoBehaviour
{
    [SerializeField] List<AbilityConfig> abilities;

    AudioSource audioSource;
    Agent unit;
    HexGameUI hexGameUI;
    HexUnitActionController hexUnitActionController;
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
        hexUnitActionController = FindObjectOfType<HexUnitActionController>();
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        unit = GetComponent<Agent>();
        AttachInitialAbilities();

    }

    public bool IsAbilityValid(int index, HexCell hexCell)
    {
        if (unit.GetMovementLeft() <= 0)
        {
            return false;
        }
        if(!AbilitiesList[index].IsValidTarget(hexCell))
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

        return AbilitiesList[index].GetValidTargets(hexCell);
    }


    public void AttemptAbility(int index, HexCell hexCell)
    {
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
            RunAbility(index, targets[0], true);
        }
        else
        {
            hexGameUI.DoAbilitySelection(targets, index);
        }
        
    }

    private bool UseAbility(int index, HexCell hexCell)
    {

        if (unit.Energy < AbilitiesList[index].GetEnergyCost())
        {
            return false;
        }
        else
        {
            unit.Energy -= AbilitiesList[index].GetEnergyCost();
        }

        //unit.SetMovementLeft(0);
        AbilitiesList[index].Use(hexCell);
        return true;
    }

    private void ShowAbility(int index, HexCell hexCell)
    {
        AbilitiesList[index].Show(AbilitiesList[index].GetEnergyCost(), hexCell);
    }

    private void FinishAbility(int index, HexCell hexCell)
    {
        AbilitiesList[index].Finish(hexCell);
    }

    public bool RunAbility(int index, HexCell hexCell, bool immediateMode = false)
    {
        if(UseAbility(index, hexCell))
        {
            HexAction action = hexUnitActionController.CreateAction();
            action.ActionsUnit = unit.HexUnit;
            action.AddAction(hexCell, AbilitiesList[index]);
            action.EnergyCost = AbilitiesList[index].GetEnergyCost();
            unit.AddAction(action);
            if(immediateMode)
            {
                unit.DoActions();
            }
            return true;
        }
        return false;
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


