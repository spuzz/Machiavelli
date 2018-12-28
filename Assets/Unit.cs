using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    [SerializeField] int baseMovement = 2;
    [SerializeField] int movementLeft = 0;
    [SerializeField] HexUnit hexUnit;
    [SerializeField] int baseMovementFactor = 5;
    List<HexCell> path;
    Player player;

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

    public int GetMovementLeft()
    {
        return movementLeft;
    }
    void Start () {
        hexUnit.Speed = (baseMovement * baseMovementFactor);
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
            hexUnit.Travel(move);
            path.RemoveRange(0, move.Count - 1);
        }

    }
}
