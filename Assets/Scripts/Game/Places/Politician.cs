using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Politician : CityPlayerBuilding {

    [SerializeField] int influencePerTurn = -2;
    public override void StartTurn()
    {
        cityBuildIn.GetCityState().AdjustInfluenceForAllExcluding(PlayersBuilding, influencePerTurn);
        cityBuildIn.GetCityState().CheckInfluence();
    }

}
