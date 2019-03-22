using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexAction : MonoBehaviour
{

    public enum Status
    {
        WAITING,
        RUNNING,
        FINISHED
    }

    List<HexCell> path;
    HexCell actionCell;
    HexCell finalMove;
    HexAction child;
    HexAction parent;
    HexUnit actionsUnit;
    bool meleeAction = true;
    Vector3 a, b, c = new Vector3();
    Status actionStatus = Status.WAITING;
    float t;

    public IEnumerable<HexCell> GetPath()
    {
        return path;
    }
    public HexUnit ActionsUnit
    {
        get
        {
            return actionsUnit;
        }

        set
        {
            actionsUnit = value;
        }
    }

    public HexAction Parent
    {
        get
        {
            return parent;
        }

        set
        {
            parent = value;
        }
    }

    public HexAction Child
    {
        get
        {
            return child;
        }

        set
        {
            child = value;
        }
    }

    public Status ActionStatus
    {
        get
        {
            return actionStatus;
        }

        set
        {
            actionStatus = value;
        }
    }

    public HexCell ActionCell
    {
        get
        {
            return actionCell;
        }
    }

    public HexCell FinalMove
    {
        get
        {
            return finalMove;
        }

        set
        {
            finalMove = value;
        }
    }

    public bool MeleeAction
    {
        get
        {
            return meleeAction;
        }

        set
        {
            meleeAction = value;
        }
    }

    public void AddAction(HexCell actionCell, HexCell finalMove)
    {
        this.actionCell = actionCell;
        this.FinalMove = finalMove;
    }
    public void SetPath(List<HexCell> path)
    {
        this.path = path;
    }
    public IEnumerator Run()
    {
        if (actionsUnit.Location.IsVisible)
        {
            t = Time.deltaTime * HexUnit.TravelSpeed;
            if (path.Count > 1)
            {
                yield return StartCoroutine(DoMove());
                if (ActionCell)
                {
                    yield return StartCoroutine(DoAction());
                }
            }
            actionsUnit.Animator.SetBool("Walking", false);
        }
        
        ActionStatus = Status.FINISHED;
    }

    private IEnumerator DoMove()
    {
        HexCell currentTravelLocation = path[0];
        a = currentTravelLocation.Position;
        b = currentTravelLocation.Position;
        c = currentTravelLocation.Position;
        yield return actionsUnit.LookAt(path[1].Position);
        actionsUnit.Animator.SetBool("Walking", true);

        for (int i = 1; i < path.Count; i++)
        {
            yield return StartCoroutine(MoveUnit(currentTravelLocation, path[i]));
            currentTravelLocation = path[i];

        }
        yield return StartCoroutine(MoveUnitEnd(currentTravelLocation, path[path.Count - 1]));
        actionsUnit.Location.UpdateVision();
    }

    private IEnumerator DoAction()
    {
        HexCell currentTravelLocation = path[path.Count - 1];
        if (meleeAction)
        {
            a = currentTravelLocation.Position;
            b = currentTravelLocation.Position;
            c = currentTravelLocation.Position;
            yield return actionsUnit.LookAt(ActionCell.Position);
            yield return StartCoroutine(MoveUnit(currentTravelLocation, ActionCell));
        }

        if (actionsUnit.HexUnitType == HexUnit.UnitType.COMBAT)
        {
            HexUnit unit = ActionCell.GetFightableUnit(actionsUnit);
            if (ActionCell.City)
            {
                yield return StartCoroutine(actionsUnit.FightCity(ActionCell.City));
            }
            else if (unit)
            {
                yield return StartCoroutine(actionsUnit.FightUnit(unit));
            }
            actionsUnit.UpdateUnit();
        }
        if (meleeAction)
        {
            yield return StartCoroutine(MoveUnitEnd(currentTravelLocation, finalMove));
        }
    }
    public IEnumerator MoveUnit(HexCell currentTravelLocation, HexCell newTravelLocation)
    {
        a = c;
        b = currentTravelLocation.Position;
        int currentColumn = currentTravelLocation.ColumnIndex;
        int nextColumn;
        nextColumn = newTravelLocation.ColumnIndex;
        if (currentColumn != nextColumn)
        {
            if (nextColumn < currentColumn - 1)
            {
                a.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
                b.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
            else if (nextColumn > currentColumn + 1)
            {
                a.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
                b.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
            actionsUnit.Grid.MakeChildOfColumn(actionsUnit.transform, nextColumn);
            currentColumn = nextColumn;
        }

        c = (b + newTravelLocation.Position) * 0.5f;

        
        actionsUnit.HexVision.AddCells(actionsUnit.Grid.GetVisibleCells(newTravelLocation, actionsUnit.VisionRange));

        if (currentTravelLocation.IsVisible == true)
        {
            actionsUnit.HexVision.Visible = true;
            for (; t < 1f; t += Time.deltaTime * HexUnit.TravelSpeed)
            {
                actionsUnit.transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                actionsUnit.transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }

            t -= 1f;
        }
        else
        {
            actionsUnit.HexVision.Visible = false;
            actionsUnit.transform.localPosition = c;
            t = Time.deltaTime * HexUnit.TravelSpeed;
        }
        actionsUnit.HexVision.ClearCells();
        actionsUnit.HexVision.AddCells(actionsUnit.Grid.GetVisibleCells(newTravelLocation, actionsUnit.VisionRange));

    }

    public IEnumerator MoveUnitEnd(HexCell currentTravelLocation, HexCell newTravelLocation)
    {
        a = c;
        b = newTravelLocation.Position;
        c = b;
        int currentColumn = currentTravelLocation.ColumnIndex;
        int nextColumn;
        nextColumn = newTravelLocation.ColumnIndex;

        if (currentColumn != nextColumn)
        {
            if (nextColumn < currentColumn - 1)
            {
                a.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
            else if (nextColumn > currentColumn + 1)
            {
                a.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
            actionsUnit.Grid.MakeChildOfColumn(actionsUnit.transform, nextColumn);
            currentColumn = nextColumn;
        }

        actionsUnit.HexVision.AddCells(actionsUnit.Grid.GetVisibleCells(newTravelLocation, actionsUnit.VisionRange));
        t = Time.deltaTime * HexUnit.TravelSpeed;
        if (newTravelLocation.IsVisible == true)
        {
            actionsUnit.HexVision.Visible = true;
            for (; t < 1f; t += Time.deltaTime * HexUnit.TravelSpeed)
            {
                actionsUnit.transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                actionsUnit.transform.localRotation = Quaternion.LookRotation(d);
                yield return null;

            }
            t -= 1f;
        }
        else
        {
            actionsUnit.HexVision.Visible = false;
            actionsUnit.transform.localPosition = c;
            t = Time.deltaTime * HexUnit.TravelSpeed;
        }
        

        actionsUnit.HexVision.ClearCells();
        actionsUnit.HexVision.AddCells(actionsUnit.Grid.GetVisibleCells(newTravelLocation, actionsUnit.VisionRange));
    }
}