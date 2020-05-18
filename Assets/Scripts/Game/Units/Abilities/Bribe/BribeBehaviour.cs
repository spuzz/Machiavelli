
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BribeBehaviour : AbilityBehaviour
{

    List<int> influenceChanges = new List<int>();

    private void Awake()
    {
        abilityText = "Bribe";
    }
    public override bool Use(HexCell target = null)
    {
        if(UnityEngine.Random.Range(0,100) >= GetSuccessChance(target))
        {
            failed = true;
            return false;
        }
        failed = false;
        List<Politician> pols = (target.City.GetCityState().GetPoliticians() as List<Politician>).FindAll(c => c.ControllingPlayer != GetComponent<Unit>().GetPlayer());
        pols = pols.OrderBy(c => c.Loyalty).ToList();
        pols[0].ControllingPlayer = GetComponent<Agent>().GetPlayer();

        return true;
    }

    public override bool Merge()
    {
        if (influenceChanges.Count < 2)
        {
            throw new InvalidOperationException("No Previous action to merge with");
        }
        influenceChanges[1] += influenceChanges[0];
        influenceChanges.Remove(influenceChanges[0]);
        return true;

    }

    public override void ShowAbility(HexCell target = null)
    {
        //abilityText = "Bribe - " + influenceChanges[0];
        base.ShowAbility(target);

    }

    public override void FinishAbility(HexCell target = null)
    {
        target.City.UpdateCity();
        //influenceChanges.Remove(influenceChanges[0]);
    }

    public override bool IsValidTarget(HexCell target)
    {
        if (target.City)
        {
            if(target.City.GetCityState().PoliticiansByPlayer(GetComponent<Unit>().GetPlayer()) != target.City.GetCityState().TotalPoliticians())
            {
                return true;
            }
        }

        return false;
    }

    public override int GetSuccessChance(HexCell target)
    {

        List<Politician> pols = (target.City.GetCityState().GetPoliticians() as List<Politician>).FindAll(c => c.ControllingPlayer != GetComponent<Unit>().GetPlayer());
        pols = pols.OrderBy(c => c.Loyalty).ToList();
        int baseChance = 30 + (int)((100 - pols[0].Loyalty) * 0.4f);

        int extraChance = GetComponent<GameEffect>().SuccessChance + GetComponent<GameEffect>().SuccessChanceOnCity;

        int successChance = baseChance + extraChance;
        if(successChance > 100)
        {
            successChance = 100;
        }
        else if(successChance < 0)
        {
            successChance = 0;
        }

        return successChance;
    }


}


