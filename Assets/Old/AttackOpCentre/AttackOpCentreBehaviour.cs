
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class AttackOpCentreBehaviour : AbilityBehaviour
//{
//    OperationCentre opCentreTarget;
//    public override void Use(HexCell target = null)
//    {
//        if(IsValidTarget(target))
//        {
//            opCentreTarget = target.OpCentre;
//            Player player = opCentreTarget.Player;
            
//            gameObject.GetComponent<Unit>().GameController.KillOperationCentre(opCentreTarget);

//        }
//    }
//    public override bool IsValidTarget(HexCell target)
//    {
//        if(target.OpCentre && target.OpCentre.Player != gameObject.GetComponent<Unit>().GetPlayer())
//        {
//            return true;
//        }
//        return false;
//    }

//    public override void FinishAbility(HexCell target = null)
//    {
//        gameObject.GetComponent<Unit>().GameController.DestroyOperationCentre(opCentreTarget);
//    }
//}


