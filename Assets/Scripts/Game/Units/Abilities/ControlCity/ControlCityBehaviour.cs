
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControlCityBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {

        if (IsValidTarget(target))
        {
            gameObject.GetComponent<Unit>().GetPlayer().PoliticalCapital -= target.City.GetPoliticalCost();
            target.City.GetCityState().SetAllPoliticians(gameObject.GetComponent<Unit>().GetPlayer());
            gameObject.GetComponent<Unit>().GameController.KillUnit(gameObject.GetComponent<Unit>());
        }
    }

    public override void FinishAbility(HexCell target = null)
    {
        target.City.GetCityState().UpdateCityState();
        gameObject.GetComponent<Unit>().GameController.DestroyUnit(gameObject.GetComponent<Unit>());
    }

    public override bool IsValidTarget(HexCell target)
    {
        if (target.City && !target.City.GetCityState().Player && target.City.GetPoliticalCost() < gameObject.GetComponent<Unit>().GetPlayer().PoliticalCapital)
        {

            return true;
        }

        return false;
    }


}


