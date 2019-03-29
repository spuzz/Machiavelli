using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitBehaviour : MonoBehaviour
{
    HexCell target;
    Unit unit;

    public Unit ActiveUnit
    {
        get
        {
            return unit;
        }

        set
        {
            unit = value;
        }
    }

    HexGrid hexGrid;
    public HexGrid GameHexGrid
    {
        get
        {
            return hexGrid;
        }

        set
        {
            hexGrid = value;
        }
    }

    public HexCell Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }

    private void Awake()
    {
        GameHexGrid = FindObjectOfType<HexGrid>();
        ActiveUnit = GetComponent<Unit>();
    }

    public void ExploreArea(HexCell startCell, HexCell centreCell, int distanceFromCentre, IEnumerable<HexCell> visibleCells)
    {
        if(Target && !unit.GetCityState().exploredCells.Contains(Target))
        {
            hexGrid.FindPath(unit.HexUnit.Location, Target, unit.HexUnit, true, false);
            HexCell nextCell = GetFirstCellFromPath();
            if(nextCell)
            {
                unit.SetPath(nextCell);
                return;
            };
        }
        List<HexCell> cells = PathFindingUtilities.FindNearestUnexplored(centreCell, unit.HexUnit.Location, unit.GetCityState().exploredCells, distanceFromCentre);
        foreach (HexCell cell in cells)
        {
            if (cell.CanUnitMoveToCell(unit.HexUnit))
            {
                hexGrid.FindPath(unit.HexUnit.Location, cell, unit.HexUnit, true, false);
                
                HexCell nextCell = GetFirstCellFromPath();
                if (nextCell)
                {
                    target = cell;
                    unit.SetPath(nextCell);
                    return;
                }
            }
        }

        cells = PathFindingUtilities.FindNearestUnexplored(unit.HexUnit.Location, unit.HexUnit.Location, unit.GetCityState().exploredCells, distanceFromCentre);
        foreach (HexCell cell in cells)
        {
            if (cell.CanUnitMoveToCell(unit.HexUnit))
            {
                hexGrid.FindPath(unit.HexUnit.Location, cell, unit.HexUnit, true, false);

                HexCell nextCell = GetFirstCellFromPath();
                if (nextCell)
                {
                    target = cell;
                    unit.SetPath(nextCell);
                    return;
                }
            }
        }

    }

    public HexCell Explore()
    {
        return unit.HexUnit.Location;
    }
    public HexCell Idle()
    {
        return unit.HexUnit.Location;
    }

    public bool Attack(Unit target)
    {
        int distance = target.HexUnit.Location.coordinates.DistanceTo(unit.HexUnit.Location.coordinates);
        if ((unit.Range == 0 && distance <= 1) || distance <= unit.Range)
        {
            unit.AttackCell(target.HexUnit.Location);
        }
        else
        {
            hexGrid.FindPath(unit.HexUnit.Location, target.HexUnit.Location, unit.HexUnit, true, false);
            HexCell targetCell = GetFirstCellFromPath();
            if (targetCell == null)
            {
                return false;
            }
            unit.SetPath(targetCell);
        }
        return true;
    }

    public bool Attack(HexCell hexCell)
    {
        int distance = hexCell.coordinates.DistanceTo(unit.HexUnit.Location.coordinates);
        if ((unit.Range == 0 && distance <= 1) || distance <= unit.Range)
        {
            unit.AttackCell(hexCell);
        }
        else
        {
            hexGrid.FindPath(unit.HexUnit.Location, hexCell, unit.HexUnit, true, false);
            HexCell targetCell = GetFirstCellFromPath();
            if(targetCell == null)
            {
                return false;
            }
            unit.SetPath(targetCell);
        }
        return true;
    }


    private HexCell GetFirstCellFromPath()
    {
        List<HexCell> path = hexGrid.GetPath();
        if (path != null && path.Count > 1)
        {
            return path[1];
        }
        else
        {
            return null;
        }
    }

    public void Attack(City city)
    {
        int distance = city.GetHexCell().coordinates.DistanceTo(unit.HexUnit.Location.coordinates);
        if ((unit.Range == 0 && distance <=1) || distance <= unit.Range)
        {
            unit.AttackCell(city.GetHexCell());
        }
        else
        {
            hexGrid.FindPath(unit.HexUnit.Location, city.GetHexCell(), unit.HexUnit, true, false);
            unit.SetPath(GetFirstCellFromPath());
        }
    }

    public void Defend(City city)
    {
        HexCell target = null;
        List<HexUnit> units = hexGrid.GetUnitsInRange(city.GetHexCell(), 5);
        foreach (HexUnit unit in units)
        {
            if (unit.HexUnitType == HexUnit.UnitType.COMBAT)
            {
                if (unit.GetComponent<Unit>().CityState && unit.GetComponent<Unit>().CityState.CityStateID != ActiveUnit.GetComponent<Unit>().CityState.CityStateID)
                {
                    Attack(unit.unit);
                    return;
                }
            }
        }

        if (city.GetHexCell().CanUnitMoveToCell(ActiveUnit.HexUnit))
        {
            hexGrid.FindPath(ActiveUnit.HexUnit.Location, city.GetHexCell(), ActiveUnit.HexUnit, true, false);
            target = GetFirstCellFromPath();
            if (target)
            {
                unit.SetPath(target);
                return;
            }
        }


        foreach(HexCell cell in PathFindingUtilities.GetCellsInRange(city.GetHexCell(),1))
        {
            if (cell.CanUnitMoveToCell(ActiveUnit.HexUnit))
            {
                hexGrid.FindPath(ActiveUnit.HexUnit.Location, city.GetHexCell(), ActiveUnit.HexUnit, true, false);
                target = GetFirstCellFromPath();
                if (target)
                {
                    unit.SetPath(target);
                    return;
                }
            }
        }
    }

    public HexCell Patrol(HexCell hexCell, int radius)
    {
        HexCell target = null;
        List<HexUnit> units = hexGrid.GetUnitsInRange(hexCell, radius);
        foreach (HexUnit unit in units)
        {
            if (unit.HexUnitType == HexUnit.UnitType.COMBAT)
            {
                if (unit.GetComponent<Unit>().CityState && unit.GetComponent<Unit>().CityState.CityStateID != ActiveUnit.GetComponent<Unit>().CityState.CityStateID)
                {
                    hexGrid.FindPath(ActiveUnit.HexUnit.Location, unit.Location, ActiveUnit.HexUnit, true, false);
                    target = GetFirstCellFromPath();
                    if (target)
                    {
                        return target;
                    }
                }
            }
        }
        if (hexCell.coordinates.DistanceTo(unit.HexUnit.Location.coordinates) > radius)
        {
            List<HexCell> cells = PathFindingUtilities.GetCellsInRange(hexCell, radius).OrderBy(c => c.coordinates.DistanceTo(unit.HexUnit.Location.coordinates)).ToList();
            foreach(HexCell cell in cells)
            {
                if(cell.CanUnitMoveToCell(unit.HexUnit))
                {
                    hexGrid.FindPath(unit.HexUnit.Location, cell, unit.HexUnit);
                    return GetFirstCellFromPath();
                }
            }

        }

        List<HexDirection> directions = Enum.GetValues(typeof(HexDirection)).Cast<HexDirection>().ToList();
        while(directions.Count > 0 && !target)
        {
            HexDirection direction = directions[UnityEngine.Random.Range(0, directions.Count)];
            directions.Remove(direction);
            HexCell neighbour = unit.HexUnit.Location.GetNeighbor(direction);
            if(hexCell.coordinates.DistanceTo(neighbour.coordinates) <= radius && unit.HexUnit.IsValidDestination(neighbour,true))
            {
                target = neighbour;
            }
        }
        return target;
    }
}
