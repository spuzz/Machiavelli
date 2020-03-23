
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class PropagandaBehaviour : AbilityBehaviour
//{
//    List<int> influenceChanges = new List<int>();
//    public override void Use(HexCell target = null)
//    {
//        if(target.City)
//        {
//            target.City.AdjustInfluenceForAllExcluding(gameObject.GetComponent<Unit>().GetPlayer(), (config as PropagandaConfig).GetInfluence());
//            target.City.CheckInfluence();
//            influenceChanges.Add((config as PropagandaConfig).GetInfluence());
//        }
//    }
//    public override bool Merge()
//    {
//        if (influenceChanges.Count < 2)
//        {
//            throw new InvalidOperationException("No Previous action to merge with");
//        }
//        influenceChanges[1] += influenceChanges[0];
//        influenceChanges.Remove(influenceChanges[0]);
//        return true;

//    }

//    public override void ShowAbility(int energyCost, HexCell target = null)
//    {
//        abilityText = "Propaganda - " + influenceChanges[0];
//        base.ShowAbility(energyCost, target);

//    }
//    public override void FinishAbility(HexCell target = null)
//    {
//        influenceChanges.Remove(influenceChanges[0]);
//    }
//    public override bool IsValidTarget(HexCell target)
//    {

//        if(target.City)
//        {
//            if (target.City.Player && target.City.Player != gameObject.GetComponent<Unit>().GetPlayer())
//            {
//                return true;
//            }
//        }

//        return false;
//    }


//}


