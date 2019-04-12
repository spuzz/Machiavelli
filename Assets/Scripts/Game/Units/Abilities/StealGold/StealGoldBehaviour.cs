
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StealGoldBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {

        if (target.City)
        {
            GetComponent<Unit>().GetPlayer().Gold += target.City.TakeGold(10.0f);
        }
    }

    public override void FinishAbility(HexCell target = null)
    {

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


