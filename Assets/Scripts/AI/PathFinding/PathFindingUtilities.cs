using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class PathFindingUtilities
{
    public static List<HexCell> GetMovableCells(Unit unit, bool allowUnexplored = false)
    {
        int movementLeft = unit.GetMovementLeft();
        List<HexCell> hexCells = new List<HexCell>();
        List<HexCellNode> openList = new List<HexCellNode>();
        List<HexCellNode> closedList = new List<HexCellNode>();
        HexCellNode startNode = new HexCellNode();
        startNode.HexCell = unit.HexUnit.Location;
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            openList = openList.OrderBy(c => c.Cost).ToList();
            HexCellNode nextNode = openList.First();
            closedList.Add(nextNode);
            openList.Remove(nextNode);
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbour = nextNode.HexCell.GetNeighbor(d);
                if (neighbour)
                {
                    HexCellNode neighbourNode = openList.Find(c => c.HexCell == neighbour);

                    int cost = nextNode.Cost;
                    if (!unit.HexUnit.IsValidDestination(neighbour, allowUnexplored) && !unit.HexUnit.IsValidAttackDestination(neighbour))
                    {
                        continue;
                    }

                    int moveCost = unit.HexUnit.GetMoveCost(nextNode.HexCell, neighbour, d, allowUnexplored);
                    if (moveCost < 0)
                    {
                        continue;
                    }
                    cost += moveCost;
                    if(cost > movementLeft)
                    {
                        continue;
                    }
                    if (neighbourNode == null || neighbourNode.Cost > cost)
                    {
                        if (neighbourNode == null)
                        {
                            neighbourNode = new HexCellNode();
                            openList.Add(neighbourNode);
                        }
                        neighbourNode.HexCell = neighbour;
                        neighbourNode.HexCellNodeParent = nextNode;
                        neighbourNode.Cost = cost;
                        
                    }

                }
            } 
        }
        foreach(HexCellNode hexCellNode in closedList)
        {
            hexCells.Add(hexCellNode.HexCell);
        }
        return hexCells;
    }

    public static List<HexCell> GetCellsInRange(HexCell hexCell, int range)
    {

        List<HexCell> cells = new List<HexCell>();
        List<HexCell> lastCells = new List<HexCell>();
        List<HexCell> currentCells = new List<HexCell>();
        cells.Add(hexCell);
        currentCells.Add(hexCell);
        for (int a =0; a< range; a++)
        {
            foreach(HexCell cell in currentCells)
            {
                lastCells.Add(cell);
            }
            
            currentCells.Clear();
            foreach (HexCell lastCell in lastCells)
            {
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = lastCell.GetNeighbor(d);
                    if (neighbor && !lastCells.Contains(neighbor) && !cells.Contains(neighbor))
                    {
                        currentCells.Add(neighbor);
                        cells.Add(neighbor);
                    }
                }
            }

        }
        
        return cells;
    }

    public static HexCell FindFreeCell(HexUnit hexUnit, HexCell hexCell)
    {
        if (hexCell.hexUnits.Count == 0)
        {
            return hexCell;
        }
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbour = hexCell.GetNeighbor(d);
            if (neighbour && neighbour.CanUnitMoveToCell(hexUnit))
            {
                return neighbour;
            }
        }
        return null;
    }

    public static HexCell FindNearestUnexplored(Agent agent, int maxRange = 5, bool movable = true)
    {
        List<HexCell> cellsChecked = new List<HexCell>();
        List<HexCell> cellsToLookFrom= new List<HexCell>();
        cellsToLookFrom.Add(agent.HexUnit.Location);
        cellsChecked.Add(agent.HexUnit.Location);
        for (int a = 0; a < maxRange; a++)
        {
            List<HexCell> neighboursChecked = new List<HexCell>();
            foreach (HexCell cellToLookFrom in cellsToLookFrom)
            {
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbour = cellToLookFrom.GetNeighbor(d);
                    if(neighbour && cellsChecked.Contains(neighbour) == false)
                    {
                        if (!neighbour.IsUnderwater && agent.GetPlayer().exploredCells.Contains(neighbour) == false)
                        {
                            return neighbour;
                        }
                        neighboursChecked.Add(neighbour);
                    }

                }
                
            }
            cellsToLookFrom.Clear();
            foreach(HexCell cell in neighboursChecked)
            {
                cellsToLookFrom.Add(cell);
                cellsChecked.Add(cell);
            }

        }
        return null;
    }

    public static List<City> FindAllSeenCities(IEnumerable<HexCell> exploredCells)
    {
        List<City> discoveredCities = new List<City>();
        foreach(HexCell cell in exploredCells)
        {
            if(cell.City)
            {
                discoveredCities.Add(cell.City);
            }
        }
        return discoveredCities;
    }

}
