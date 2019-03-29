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
    public void UpdateUnits()
    {
        AssignStance();

        List<CombatUnit> unitsAtStart = new List<CombatUnit>();

        foreach (CombatUnit unit in cityState.GetUnits())
        {
            unitsAtStart.Add(unit);
        }

        foreach (CombatUnit unit in unitsAtStart)
        {
            int currentMovement = -1;
            List<HexCell> path = new List<HexCell>();
            while (unit && unit.Alive && unit.GetMovementLeft() > 0 && currentMovement != unit.GetMovementLeft())
            {
                CheckAttackTarget();
                currentMovement = unit.GetMovementLeft();
                UpdateUnit(unit);
 
            }
        }
    }

    public void UpdateCities()
    {

        foreach (City city in cityState.GetCities())
        {
            city.TakeTurn();
            if (city.BuildingManager.buildsInQueue() == 0)
            {


                if (UnityEngine.Random.value < 1)
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
        attackTarget = null;
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
            unit.CurrentStance = GetUnitStancePriority(unit);
        }
    }

    private void AssignStance(CombatUnit unit)
    {
        unit.CurrentStance = GetUnitStancePriority(unit);
    }

    private CombatUnit.Stance GetUnitStancePriority(CombatUnit unit)
    {
        // if(!cityState.GetUnits().ToList().Find(c => c.CurrentStance == CombatUnit.Stance.EXPLORE))
        if (unit.GetCombatUnitConfig().Name.CompareTo("Scout") == 0)
        {
            return CombatUnit.Stance.EXPLORE;
        }

        List<CombatUnit> defenceUnits = cityState.GetUnits().ToList().FindAll(c => c.CurrentStance == CombatUnit.Stance.DEFENCE);
        if (defenceUnits.Count < cityState.GetCityCount())
        {
            return CombatUnit.Stance.DEFENCE;
        }
        return CombatUnit.Stance.OFFENCE;
    }

    public void UpdateUnit(CombatUnit unit)
    {
        if(unit.CurrentStance == CombatUnit.Stance.UNASSIGNED)
        {
            AssignStance(unit);
        }

        NextMove(unit);

    }

    private void NextMove(CombatUnit unit)
    {
        switch (unit.CurrentStance)
        {
            case CombatUnit.Stance.DEFENCE:
                DefendCity(unit);
                break;
            case CombatUnit.Stance.OFFENCE:
                DetermineTarget(unit);
                break;
            case CombatUnit.Stance.EXPLORE:
                unit.Behaviour.ExploreArea(unit.HexUnit.Location, cityState.GetCity().GetHexCell(), 6, cityState.GetExploredCells());
                break;
        }
    }

    private void DetermineTarget(CombatUnit unit)
    {
        List<HexCell> nearbyEnemyUnits = CheckNearby(unit);
        foreach(HexCell cell in nearbyEnemyUnits)
        {
            if(unit.Behaviour.Attack(cell))
            {
                return;
            }
        }
        if (attackTarget)
        {
            unit.Behaviour.Attack(attackTarget);
        }
        //unit.Behaviour.Patrol(cityState.GetCity().GetHexCell(),2);
    }

    private void DefendCity(CombatUnit unit)
    {
        IEnumerable<City> cities = cityState.GetCities().OrderBy(c => c.GetHexCell().coordinates.DistanceTo(unit.HexUnit.Location.coordinates));
        foreach (City city in cities)
        {
            unit.Behaviour.Defend(city);

        }
    }

    private List<HexCell> CheckNearby(Unit unit)
    {
        List<HexCell> nearbyCells = PathFindingUtilities.GetCellsInRange(unit.HexUnit.Location,3);
        List<HexCell> enemyCells = new List<HexCell>();
        foreach(HexCell hexCell in nearbyCells)
        {
            if(cityState.exploredCells.Contains(hexCell) && unit.HexUnit.IsValidAttackDestination(hexCell) && !hexCell.City)
            {
                enemyCells.Add(hexCell);
            }
        }
        if(enemyCells.Count == 0)
        {
            return new List<HexCell>();
        }
        enemyCells = enemyCells.OrderBy(c => c.coordinates.DistanceTo(unit.HexUnit.Location.coordinates)).ToList();
        return enemyCells;
    }
}
