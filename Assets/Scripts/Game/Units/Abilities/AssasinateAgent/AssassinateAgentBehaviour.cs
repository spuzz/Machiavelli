
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class AssassinateAgentBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        if(!IsValidTarget(target))
        {
            return;
        }
        Dictionary<CityPlayerBuilding,Player> buildings = target.City.PlayerBuildingControl.GetAllBuildings();
        IEnumerable<CityPlayerBuilding> building = IListExtensions.RandomKeys(buildings);
        target.City.PlayerBuildingControl.DestroyBuilding(buildings[building.First()], building.First());
        abilityText = "Assasinate - " + building.First().BuildConfig.DisplayName + " (" + buildings[building.First()].PlayerNumber + ")";
    }

    public override bool IsValidTarget(HexCell target)
    {
        if (target.City && target.City.PlayerBuildingControl.HasEnemyBuildings(GetComponent<Unit>().GetPlayer()))
        {
            return true;
        }
        return false;
    }

    public override void FinishAbility(HexCell target = null)
    {
    }
}


