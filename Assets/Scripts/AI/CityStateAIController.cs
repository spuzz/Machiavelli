using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityStateAIController : MonoBehaviour
{

    [SerializeField] CityState cityState;
    [SerializeField] HexCell attackTarget = null;
    [SerializeField] List<BuildConfig> buildConfigs;

    bool areaLeftToExplore = true;
    public IEnumerator UpdateUnits()
    {
        AssignStance();
        CheckAttackTarget();
        foreach (CombatUnit unit in cityState.GetUnits())
        {
            int currentMovement = -1;
            List<HexCell> path = new List<HexCell>();
            while (unit.Alive && unit.GetMovementLeft() > 0 && currentMovement != unit.GetMovementLeft())
            {

                if (unit.HexUnit.pathToTravel == null || unit.HexUnit.pathToTravel.Count == 0)
                {
                    currentMovement = unit.GetMovementLeft();
                    yield return StartCoroutine(UpdateUnit(unit));
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }

    }

    public void UpdateCities()
    {
        foreach (City city in cityState.GetCities())
        {
            if (city.BuildingManager.buildsInQueue() == 0)
            {


                if (UnityEngine.Random.value < .5)
                {
                    city.BuildUnit();
                }
                else
                    city.BuildBuilding();


            }

        }
    }
    private void CheckAttackTarget()
    {
        if (cityState.GetUnits().ToList().FindAll(c => c.CurrentStance == CombatUnit.Stance.OFFENCE).Count >= 3)
        {
            List<City> enemyCities = cityState.GetEnemyCitiesOrderByDistance(cityState.GetCity().GetHexCell().coordinates);
            if (enemyCities.Count > 0)
            {
                attackTarget = enemyCities[0].GetHexCell();
            }
        }
    }

    private void AssignStance()
    {
        List<CombatUnit> unassignedUnits = cityState.GetUnits().ToList().FindAll(c => c.CurrentStance == CombatUnit.Stance.UNASSIGNED);
        foreach (CombatUnit unit in unassignedUnits)
        {
            unit.CurrentStance = GetUnitStancePriority();
        }
    }

    private void AssignStance(CombatUnit unit)
    {
        unit.CurrentStance = GetUnitStancePriority();
    }

    private CombatUnit.Stance GetUnitStancePriority()
    {
        if(areaLeftToExplore && !cityState.GetUnits().ToList().Find(c => c.CurrentStance == CombatUnit.Stance.EXPLORE))
        {
            return CombatUnit.Stance.EXPLORE;
        }

        List<CombatUnit> defenceUnits = cityState.GetUnits().ToList().FindAll(c => c.CurrentStance == CombatUnit.Stance.DEFENCE);
        if(defenceUnits.Count < cityState.GetCityCount())
        {
            return CombatUnit.Stance.DEFENCE;
        }
        return CombatUnit.Stance.OFFENCE;
    }

    public IEnumerator UpdateUnit(CombatUnit unit)
    {
        HexCell nextMove = null;
        if(unit.CurrentStance == CombatUnit.Stance.UNASSIGNED)
        {
            AssignStance(unit);
        }

        nextMove = GetNextMove(unit);

        if (!nextMove)
        {
            nextMove = unit.Behaviour.Patrol(cityState.GetCity().GetHexCell(), 2);
        }

        if (!nextMove || nextMove == unit.HexUnit.Location)
        {
            unit.EndTurn();
        }
        else
        {
            unit.SetPath(nextMove);
            while (unit.AttackUnit && unit.HexUnit.pathToTravel != null && unit.HexUnit.pathToTravel.Count != 0)
            {
                yield return new WaitForFixedUpdate();
            }

        }
    }

    private HexCell GetNextMove(CombatUnit unit)
    {
        HexCell nextMove = unit.HexUnit.Location;
        switch (unit.CurrentStance)
        {
            case CombatUnit.Stance.DEFENCE:
                nextMove = DefendCity(unit);
                break;
            case CombatUnit.Stance.OFFENCE:
                nextMove = DetermineTarget(unit);
                break;
            case CombatUnit.Stance.EXPLORE:
                nextMove = unit.Behaviour.ExploreArea(unit.HexUnit.Location, cityState.GetCity().GetHexCell(), 6, cityState.GetExploredCells());
                if (!nextMove)
                {
                    areaLeftToExplore = false;
                    unit.CurrentStance = CombatUnit.Stance.UNASSIGNED;
                    AssignStance(unit);
                    nextMove = GetNextMove(unit);
                }
                break;
        }
        return nextMove;
    }

    private HexCell DetermineTarget(CombatUnit unit)
    {
        HexCell nearbyEnemyUnit = CheckNearby(unit);
        if (nearbyEnemyUnit)
        {
            return unit.Behaviour.Attack(nearbyEnemyUnit);
        }
        if (attackTarget)
        {
            return unit.Behaviour.Attack(attackTarget);
        }
        return unit.Behaviour.Patrol(cityState.GetCity().GetHexCell(),2);
    }

    private HexCell DefendCity(CombatUnit unit)
    {
        IEnumerable<City> cities = cityState.GetCities().OrderBy(c => c.GetHexCell().coordinates.DistanceTo(unit.HexUnit.Location.coordinates));
        foreach (City city in cities)
        {
            HexCell nextMove = unit.Behaviour.Defend(city);
            if(nextMove)
            {
                return nextMove;
            }
        }

        return unit.HexUnit.Location;
    }

    private HexCell CheckNearby(Unit unit)
    {
        List<HexCell> nearbyCells = PathFindingUtilities.GetMovableCells(unit);
        List<HexCell> enemyCells = new List<HexCell>();
        foreach(HexCell hexCell in nearbyCells)
        {
            if(cityState.visibleCells.Keys.Contains(hexCell) && unit.HexUnit.IsValidAttackDestination(hexCell))
            {
                enemyCells.Add(hexCell);
            }
        }
        if(enemyCells.Count == 0)
        {
            return null;
        }
        enemyCells = enemyCells.OrderBy(c => c.coordinates.DistanceTo(unit.HexUnit.Location.coordinates)).ToList();
        return enemyCells[0];
    }
}
