using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityStateAIController : MonoBehaviour
{

    [SerializeField] CityState cityState;

    public IEnumerator UpdateUnits()
    {
        foreach (CombatUnit unit in cityState.GetUnits())
        {
            int currentMovement = -1;
            List<HexCell> path = new List<HexCell>();
            while (unit.GetMovementLeft() > 0 && currentMovement != unit.GetMovementLeft())
            {

                if (unit.HexUnit.pathToTravel == null || unit.HexUnit.pathToTravel.Count == 0)
                {
                    currentMovement = unit.GetMovementLeft();
                    yield return StartCoroutine(cityState.MoveUnit(unit));
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }

    }

}
