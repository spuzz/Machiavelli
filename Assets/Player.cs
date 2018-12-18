using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] int playerNumber = 0;
    List<Unit> units = new List<Unit>();

    public int GetPlayerNumber()
    {
        return playerNumber;
    }
    public void StartTurn()
    {
        foreach(Unit unit in units)
        {
            unit.StartTurn();
        }
    }

    public void EndTurn()
    {
        foreach (Unit unit in units)
        {
            unit.MoveUnit();
        }
    }

    public void AddUnit(HexUnit hexUnit)
    {
        Unit unit = hexUnit.GetComponent<Unit>();
        hexUnit.GetComponent<Unit>().SetPlayer(this);
        units.Add(unit);
    }

    public void RemoveUnit(HexUnit hexUnit)
    {
        Unit unit = hexUnit.GetComponent<Unit>();
        hexUnit.GetComponent<Unit>().SetPlayer(null);
        units.Remove(unit);
    }
}
