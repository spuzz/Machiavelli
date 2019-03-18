using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GovernmentAdvisor : CityPlayerBuilding {

    [SerializeField] int influencePerTurn = 2;
    public override void StartTurn()
    {
        cityBuildIn.GetCityState().AdjustInfluence(PlayersBuilding, influencePerTurn);
        cityBuildIn.GetCityState().CheckInfluence();
    }

}
