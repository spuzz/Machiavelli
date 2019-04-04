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

    public enum ActionType
    {
        ATTACKCITY,
        ATTACKUNIT,
        MOVE,
        USEABILITY
    }
    ActionType hexActionType = ActionType.MOVE;
    List<HexCell> path = new List<HexCell>();
    HexCell actionCell;
    HexCell finalMove;
    HexAction child;
    HexAction parent;
    HexUnit actionsUnit;
    City cityTarget;
    HexUnit unitTarget;
    CityState cityStateTarget;
    AbilityConfig abilityConfigToShow;
    bool meleeAction = true;
    bool killTarget = false;
    bool killSelf = false;
    int damageToSelf = 0;
    int damageToTarget = 0;

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

    public HexUnit UnitTarget
    {
        get
        {
            return unitTarget;
        }

        set
        {
            unitTarget = value;
        }
    }

    public City CityTarget
    {
        get
        {
            return cityTarget;
        }

        set
        {
            cityTarget = value;
        }
    }

    public bool KillTarget
    {
        get
        {
            return killTarget;
        }

        set
        {
            killTarget = value;
        }
    }

    public bool KillSelf
    {
        get
        {
            return killSelf;
        }

        set
        {
            killSelf = value;
        }
    }

    public CityState CityStateTarget
    {
        get
        {
            return cityStateTarget;
        }

        set
        {
            cityStateTarget = value;
        }
    }

    public ActionType HexActionType
    {
        get
        {
            return hexActionType;
        }

        set
        {
            hexActionType = value;
        }
    }

    public void SetKillTarget()
    {
        KillTarget = true;
    }

    public void SetKillSelf()
    {
        KillSelf = true;
    }

    public void AddAction(List<HexCell> path)
    {
        this.path = path;
        HexActionType = ActionType.MOVE;
    }

    public bool AddAction(HexAction action)
    {
        if(action.HexActionType != ActionType.MOVE)
        {
            return false;
        }
        bool initial = true;
        foreach (HexCell cell in action.GetPath())
        {
            if(initial == false)
            {
                path.Add(cell);
            }
            else
            {
                initial = false;
            }
        }
        return true;
    }

    public void AddAction(HexCell actionCell, HexCell finalMove, City cityTarget, int damageToSelf, int damageToTarget, CityState cityState)
    {
        path.Add(actionsUnit.Location);
        path.Add(actionCell);
        HexActionType = ActionType.ATTACKCITY;
        this.CityStateTarget = cityState;
        this.CityTarget = cityTarget;
        this.actionCell = actionCell;
        this.damageToSelf = damageToSelf;
        this.damageToTarget = damageToTarget;
        this.FinalMove = finalMove;
        this.UnitTarget = cityTarget.GetHexCell().hexUnits.Find(c => c.HexUnitType == HexUnit.UnitType.COMBAT);
    }

    public void AddAction(HexCell actionCell, HexCell finalMove, HexUnit unitTarget, int damageToSelf, int damageToTarget)
    {
        path.Add(actionsUnit.Location);
        path.Add(actionCell);
        HexActionType = ActionType.ATTACKUNIT;
        this.UnitTarget = unitTarget;
        this.actionCell = actionCell;
        this.damageToSelf = damageToSelf;
        this.damageToTarget = damageToTarget;
        this.FinalMove = finalMove;
    }

    public void AddAction(HexCell actionCell, AbilityConfig abilityConfig)
    {
        path.Add(actionsUnit.Location);
        path.Add(actionCell);
        HexActionType = ActionType.USEABILITY;
        abilityConfigToShow = abilityConfig;
        this.actionCell = actionCell;
    }

    public IEnumerator Run()
    {
        t = Time.deltaTime * HexUnit.TravelSpeed;
        if (HexActionType == ActionType.MOVE)
        {
            yield return StartCoroutine(DoMove());
        }
        if (HexActionType == ActionType.ATTACKCITY || HexActionType == ActionType.ATTACKUNIT)
        {
            yield return StartCoroutine(DoAction());
        }

        if (HexActionType == ActionType.USEABILITY)
        {
            yield return StartCoroutine(DoAbility());
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
        HexCell currentTravelLocation = actionsUnit.Location;
        yield return actionsUnit.LookAt(ActionCell.Position);
        if (meleeAction)
        {
            a = currentTravelLocation.Position;
            b = currentTravelLocation.Position;
            c = currentTravelLocation.Position;
            
            yield return StartCoroutine(MoveUnit(currentTravelLocation, ActionCell));
        }

        if (actionsUnit.HexUnitType == HexUnit.UnitType.COMBAT)
        {
            if (CityTarget)
            {
                yield return StartCoroutine(actionsUnit.FightCity(ActionCell.City, KillTarget, CityStateTarget, UnitTarget));
            }
            else if (UnitTarget)
            {
                yield return StartCoroutine(actionsUnit.FightUnit(UnitTarget, damageToTarget, KillTarget));
            }
            actionsUnit.UpdateUnit(damageToSelf, KillSelf);

        }
        if (meleeAction && KillSelf == false)
        {
            yield return StartCoroutine(MoveUnitEnd(currentTravelLocation, finalMove));
        }
    }

    public IEnumerator DoAbility()
    {
        abilityConfigToShow.Show(actionCell);
        t = Time.deltaTime * HexUnit.TravelSpeed;
        for (; t < 1f; t += Time.deltaTime * 1)
        {
            yield return null;
        }
        abilityConfigToShow.Finish(actionCell);

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
            actionsUnit.Animator.SetBool("Walking", true);
            actionsUnit.HexVision.Visible = true;
            for (; t < 1f; t += Time.deltaTime * HexUnit.TravelSpeed)
            {
                actionsUnit.transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                actionsUnit.transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            actionsUnit.Animator.SetBool("Walking", false);
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
            actionsUnit.Animator.SetBool("Walking", true);
            actionsUnit.HexVision.Visible = true;
            for (; t < 1f; t += Time.deltaTime * HexUnit.TravelSpeed)
            {
                actionsUnit.transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                actionsUnit.transform.localRotation = Quaternion.LookRotation(d);
                yield return null;

            }
            actionsUnit.Animator.SetBool("Walking", false);
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