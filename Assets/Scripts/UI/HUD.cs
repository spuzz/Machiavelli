using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    [SerializeField] GameController gameController;
    [SerializeField] Text turn;
    

    Unit unit;

    public Unit Unit
    {
        get
        {
            return unit;
        }

        set
        {
            unit = value;
            TargetCell = unit.HexUnit.Location;
        }
    }

    HexCell targetCell;

    public HexCell TargetCell
    {
        get
        {
            return targetCell;
        }

        set
        {
            targetCell = value;
        }
    }

    // Update is called once per frame
    void Update () {
        turn.text = "Turn : " + gameController.GetTurn().ToString();
        
    }

    public void EndTurn()
    {
        gameController.EndPlayerTurn();
    }

    public void UseAbility(int abilityNumber)
    {
        TargetCell = Unit.HexUnit.Location;
        Unit.UseAbility(abilityNumber, TargetCell);
    }
}
