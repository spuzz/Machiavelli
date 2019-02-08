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

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        unit = GetComponent<Unit>();
        AttachInitialAbilities();

    }

    public bool IsAbilityValid(int index, HexCell hexCell, int range)
    {
        if (unit.GetMovementLeft() <= 0)
        {
            return false;
        }
        int goldCost = AbilitiesList[index].GetCost();
        if (unit.GetPlayer().Gold < goldCost)
        {
            return false;
        }
        return AbilitiesList[index].IsValidTarget(hexCell);
    }

    public void AttemptAbility(int index, HexCell hexCell)
    {
        if(unit.GetMovementLeft() <= 0)
        {
            return;
        }
        int goldCost = AbilitiesList[index].GetCost();
        if (unit.GetPlayer().Gold > goldCost)
        {
            unit.GetPlayer().Gold -= goldCost;
            // todo make work
            AbilitiesList[index].Use(hexCell);
            unit.SetMovementLeft(0);
        }
        else
        {
            if (!audioSource.isPlaying)
            {
                //audioSource.PlayOneShot(outOfEnergy);
            }

        }
    }

    public int GetNumberOfAbilities()
    {
        return AbilitiesList.Count;
    }


    private void AttachInitialAbilities()
    {
        foreach (AbilityConfig ability in AbilitiesList)
        {
            ability.AddComponent(gameObject);
        }
    }


}


