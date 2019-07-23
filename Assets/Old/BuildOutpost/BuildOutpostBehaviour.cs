
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class BuildOutpostBehaviour : AbilityBehaviour
//{

//    public override void Use(HexCell target = null)
//    {
        
//        if (IsValidTarget(target))
//        {
//            target.City.PlayerBuildingControl.AddOutpost(gameObject.GetComponent<Unit>().GetPlayer());
//            gameObject.GetComponent<Unit>().GameController.KillUnit(gameObject.GetComponent<Unit>());
//        }
//    }

//    public override void FinishAbility(HexCell target = null)
//    {
//        if (gameObject.GetComponent<Unit>().GetPlayer() && gameObject.GetComponent<Unit>().GetPlayer().IsHuman)
//        {
//            gameObject.GetComponent<Unit>().HUDUI.City = target.City;
//        }
//        gameObject.GetComponent<Unit>().GameController.DestroyUnit(gameObject.GetComponent<Unit>());
//    }

//    public override bool IsValidTarget(HexCell target)
//    {
//        if (target.City && !target.City.PlayerBuildingControl.HasOutpost(gameObject.GetComponent<Unit>().GetPlayer()))
//        {

//            return true;
//        }

//        return false;
//    }


//}


