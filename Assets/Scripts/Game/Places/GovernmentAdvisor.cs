using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GovernmentAdvisor : CityPlayerBuilding {

    [SerializeField] int influencePerTurn = 2;
    public override void Init()
    {
        cityBuildIn.AddInfluencePerTurn(PlayersBuilding, influencePerTurn);
    }

    public override void Destroy()
    {
        cityBuildIn.RemoveInfluencePerTurn(PlayersBuilding, influencePerTurn);
    }

}
