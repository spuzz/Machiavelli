using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour {

    [SerializeField] int baseMovement = 2;
    [SerializeField] int movementLeft = 0;
    [SerializeField] HexUnit hexUnit;
    [SerializeField] int baseMovementFactor = 5;
    [SerializeField] int baseHitPoints = 100;
    List<HexCell> path = new List<HexCell>();
    Player player;
    HexGrid hexGrid;
    HexCell fightInCell;
    CityState cityState;
    public CityState CityState
    {
        get
        {
            return cityState;
        }

        set
        {
            cityState = value;
        }
    }

    int hitPoints = 1;
    public int HitPoints
    {
        get
        {
            return hitPoints;
        }

        set
        {
            hitPoints = value;
        }
    }

    public int GetMovementLeft()
    {
        return movementLeft;
    }
    void Start () {
        hexUnit.Speed = (baseMovement * baseMovementFactor);
        hitPoints = baseHitPoints;
        hexGrid = FindObjectOfType<HexGrid>();
        StartTurn();
    }
	
    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void SetPath(List<HexCell> path)
    {
        this.path = path;
        MoveUnit();

    }

    public void StartTurn()
    {
        movementLeft = baseMovement * baseMovementFactor;
    }

    public bool CheckPath()
    {
        if (path.Count == 0)
        {
            return false;
        }

        hexGrid.FindPath(hexUnit.Location, path[path.Count - 1],hexUnit);
        path = hexGrid.GetPath();
        if (path.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void MoveUnit()
    {
        if(path.Count == 0)
        {
            return;
        }
        List<HexCell> move = new List<HexCell>();
        move.Add(path[0]);
        int cellNumber = 1;
        while (movementLeft > 0 && path.Count > 1)
        {
           if(path.Count > cellNumber)
            {
                int movementCost = hexUnit.GetMoveCost(path[cellNumber - 1], path[cellNumber], path[cellNumber - 1].GetNeighborDirection(path[cellNumber]));
                if (movementCost == -1 || movementCost > movementLeft)
                {
                    break;
                }
                else
                {
                    move.Add(path[cellNumber]);
                    movementLeft -= movementCost;
                    cellNumber++;
                }
            }
            else
            {
                break;
            }
        }
        
        if(move.Count > 1)
        {
            HexCell attackCell = null;
            HexUnit unitToFight = move[move.Count - 1].GetFightableUnit(hexUnit);
            if (unitToFight)
            {
                attackCell = move[move.Count - 1];
                unitToFight.GetComponent<Unit>().HitPoints -= 50;
                move[move.Count - 1] = move[move.Count - 2];
            }
            hexUnit.Travel(move, attackCell);
            path.RemoveRange(0, move.Count - 1);
        }

    }

    public abstract bool CanAttack(Unit unit);
}
