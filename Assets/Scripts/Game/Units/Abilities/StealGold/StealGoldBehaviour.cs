
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StealGoldBehaviour : AbilityBehaviour
{

    List<int> stealGoldChanges = new List<int>();
    public override void Use(HexCell target = null)
    {

        if (target.City)
        {
            int goldChange = target.City.TakeGold(10.0f);
            GetComponent<Unit>().GetPlayer().Gold += goldChange;
            stealGoldChanges.Add(goldChange);
            abilityText = "Steal Gold - " + goldChange;
        }
    }
    public override bool Merge()
    {
        if (stealGoldChanges.Count < 2)
        {
            throw new InvalidOperationException("No Previous action to merge with");
        }
        stealGoldChanges[1] += stealGoldChanges[0];
        stealGoldChanges.Remove(stealGoldChanges[0]);
        abilityText = "Steal Gold - " + stealGoldChanges[0];
        return true;

    }

    public override void ShowAbility(int energyCost, HexCell target = null)
    {
        base.ShowAbility(energyCost, target);

    }
    public override void FinishAbility(HexCell target = null)
    {
        stealGoldChanges.Remove(stealGoldChanges[0]);
    }
    public override bool IsValidTarget(HexCell target)
    {
        if(target.City)
        {
            return true;
        }

        return false;
    }


}


