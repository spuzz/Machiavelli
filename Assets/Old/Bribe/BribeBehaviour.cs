
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class BribeBehaviour : AbilityBehaviour
//{

//    List<int> influenceChanges = new List<int>();

//    public override void Use(HexCell target = null)
//    {
//        if(target.City)
//        {
//            target.City.AdjustInfluence(gameObject.GetComponent<Unit>().GetPlayer(), (config as BribeConfig).GetInfluence());
//            target.City.CheckInfluence();
//            influenceChanges.Add((config as BribeConfig).GetInfluence());
//        }

//    }

//    public override bool Merge()
//    {
//        if(influenceChanges.Count < 2)
//        {
//            throw new InvalidOperationException("No Previous action to merge with");
//        }
//        influenceChanges[1] += influenceChanges[0];
//        influenceChanges.Remove(influenceChanges[0]);
//        return true;

//    }
//    public override void ShowAbility(int energyCost, HexCell target = null)
//    {
//        abilityText = "Bribe - " + influenceChanges[0];
//        base.ShowAbility(energyCost, target);

//    }
//    public override void FinishAbility(HexCell target = null)
//    {
//        influenceChanges.Remove(influenceChanges[0]);
//    }
//    public override bool IsValidTarget(HexCell target)
//    {
//        if(GetComponent<Agent>().Energy < config.GetEnergyCost())
//        {
//            return false;
//        }
//        if(target.City)
//        {
//            Player player = target.City.Player;
//            if (!player)
//            {
//                return true;
//            }
//        }

//        return false;
//    }


//}


