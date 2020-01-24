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
    List<HexUnit> extraUnits = new List<HexUnit>();
    City cityTarget;
    HexUnit unitTarget;
    CityState cityStateTarget;
    AbilityConfig abilityConfigToShow;
    bool meleeAction = true;
    List<FightResult> unitFightResults;
    int energyCost = 0;
    int finishedActions = 0;
    Status actionStatus = Status.WAITING;
    float t;
    List<HexUnit> temporaryVisibleUnits = new List<HexUnit>();
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

    public void AddExtraUnit(HexUnit unit)
    {
        extraUnits.Add(unit);
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

    public int EnergyCost
    {
        get
        {
            return energyCost;
        }

        set
        {
            energyCost = value;
        }
    }

    public bool Compare(HexAction action)
    {
        if(action.HexActionType == ActionType.MOVE && HexActionType == ActionType.MOVE)
        {
            return true;
        }

        if(action.HexActionType == ActionType.USEABILITY && HexActionType == ActionType.USEABILITY)
        {
            if (actionCell == action.ActionCell && action.abilityConfigToShow == abilityConfigToShow)
            {
                return true;
            }
        }
        return false;
    }
    public void AddAction(List<HexCell> path)
    {
        this.path = path;
        HexActionType = ActionType.MOVE;
    }

    public bool AddAction(HexAction action)
    {
        if(action.HexActionType == ActionType.MOVE && HexActionType == ActionType.MOVE)
        {
            bool initial = true;
            foreach (HexCell cell in action.GetPath())
            {
                if (initial == false)
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
        if(action.HexActionType == ActionType.USEABILITY && HexActionType == ActionType.USEABILITY)
        {
            bool result = abilityConfigToShow.Merge();
            if(result == true)
            {
                energyCost += action.EnergyCost;
            }
            return result;
        }
        return false;
    }

    public void AddAction(HexCell actionCell, HexCell finalMove, City cityTarget, int damageToSelf, int damageToTarget, CityState cityState)
    {
        path.Add(actionsUnit.Location);
        path.Add(actionCell);
        HexActionType = ActionType.ATTACKCITY;
        this.CityStateTarget = cityState;
        this.CityTarget = cityTarget;
        this.actionCell = actionCell;
        //this.damageToSelf = damageToSelf;
        //this.damageToTarget = damageToTarget;
        this.FinalMove = finalMove;
        this.UnitTarget = cityTarget.GetHexCell().combatUnit;
    }

    public void AddAction(HexCell actionCell, HexCell finalMove, HexUnit unitTarget, List<FightResult> result)
    {
        path.Add(actionsUnit.Location);
        path.Add(actionCell);
        HexActionType = ActionType.ATTACKUNIT;
        this.UnitTarget = unitTarget;
        this.actionCell = actionCell;
        unitFightResults = result;
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
        int actionsToComplete = 0;
        finishedActions = 0;
        bool tempVisibility = false;
        if(HexActionType == ActionType.ATTACKCITY)
        {

        }

        if (HexActionType == ActionType.ATTACKUNIT)
        {
            if (!ActionCell.IsVisible && path[0].IsVisible)
            {
                if(ActionCell.IsVisible == false)
                {
                    tempVisibility = true;
                    ActionCell.IncreaseVisibility(false);
                }
            }

            foreach(FightResult result in unitFightResults)
            {
                actionsToComplete++;
                StartCoroutine(DoActionMove(result.unit, result.targetCell, result.targetUnit));
            }

            //actionsToComplete++;
            //StartCoroutine(DoActionMove(unitTarget, ActionCell));

            //actionsToComplete++;
            //StartCoroutine(DoActionMove(actionsUnit, ActionCell));

            while (finishedActions != actionsToComplete)
            {
                yield return new WaitForEndOfFrame();
            }
            finishedActions = 0;
            actionsToComplete = 0;

            if (unitFightResults.FindAll(c => c.ambush == true).Count > 0)
            {
                ActionCell.TextEffectHandler.AddTextEffect("AMBUSH!", actionsUnit.transform, Color.red);
            }


            foreach (FightResult result in unitFightResults)
            {
                actionsToComplete++;
                StartCoroutine(DoActionFight(result.unit, result.targetCell, result.targetUnit));
            }

            //actionsToComplete++;
            //StartCoroutine(DoActionFight(unitTarget, ActionCell));

            //actionsToComplete++;
            //StartCoroutine(DoActionFight(actionsUnit, ActionCell));

            while (finishedActions != actionsToComplete)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        actionsUnit.Location.UpdateUnitPositions();

        foreach (HexUnit hexUnit in temporaryVisibleUnits)
        {
            hexUnit.HexVision.Visible = false;
        }
        if(tempVisibility)
        {
            ActionCell.DecreaseVisibility();
        }
        temporaryVisibleUnits.Clear();
        if (HexActionType == ActionType.USEABILITY)
        {
            yield return StartCoroutine(DoAbility());
        }

        ActionStatus = Status.FINISHED;
    }

    private IEnumerator DoMove()
    {
        HexCell currentTravelLocation = path[0];


        bool startMove = true;
        for (int i = 1; i < path.Count; i++)
        {
            yield return StartCoroutine(actionsUnit.MoveUnit(actionsUnit, currentTravelLocation, path[i], t, 1, startMove));
            currentTravelLocation = path[i];
            startMove = false;

        }
        yield return StartCoroutine(actionsUnit.MoveUnitEnd(actionsUnit, currentTravelLocation, path[path.Count - 1]));
        actionsUnit.Location.UpdateVision();
    }

    private IEnumerator DoActionMove(HexUnit unitToDoAction, HexCell targetCell, HexUnit targetUnit)
    {

        if (unitToDoAction.HexVision.Visible == false && (ActionCell.IsVisible || path[0].IsVisible) )
        {
            temporaryVisibleUnits.Add(unitToDoAction);
            unitToDoAction.HexVision.Visible = true;
        }
        unitToDoAction.unit.UnitUI.gameObject.SetActive(false);
        HexCell currentTravelLocation = unitToDoAction.Location;
        if ((unitToDoAction.unit as CombatUnit).CombatType == CombatUnit.CombatUnitType.MELEE)
        {
            float percOfDistanceToTarget = 1;
            if((targetUnit.unit as CombatUnit).CombatType == (CombatUnit.CombatUnitType.MELEE))
            {
                percOfDistanceToTarget = 0.66f;
            }
            else
            {
                percOfDistanceToTarget = 1.33f;
            }
            yield return StartCoroutine(unitToDoAction.MoveUnit(unitToDoAction, currentTravelLocation, targetCell,t, percOfDistanceToTarget, true ));
        }
        else
        {
            //yield return StartCoroutine(unitToDoAction.MoveUnit(unitToDoAction, currentTravelLocation, currentTravelLocation, t, 2.00f, true, unitToDoAction.unit.GetFightPosition(currentTravelLocation,targetCell)));
            yield return StartCoroutine(unitToDoAction.LookAt(targetCell.Position));
            unitToDoAction.Animator.SetBool("Walking", false);
        }

        finishedActions++;
        
    }


    private IEnumerator DoActionFight(HexUnit unitToDoAction, HexCell targetCell, HexUnit targetUnit)
    {

        yield return StartCoroutine(unitToDoAction.Fight(targetCell));

        List<FightResult> results = unitFightResults.FindAll(c => c.unit == unitToDoAction);

        if (results.Count != 0)
        {
            FightResult result = results[0];
            if(result.damageReceived > 0)
            {
                unitToDoAction.UpdateUnit(result.damageReceived, result.isKilled);
            }


            if ((unitToDoAction.unit as CombatUnit).CombatType == CombatUnit.CombatUnitType.MELEE && !result.isKilled)
            {
                yield return StartCoroutine(unitToDoAction.MoveUnitEnd(unitToDoAction, targetCell, unitToDoAction.Location));
                yield return StartCoroutine(unitToDoAction.LookAt(targetCell.Position));
            }

        }

        finishedActions++;
        unitToDoAction.unit.UnitUI.gameObject.SetActive(true);
    }

    public IEnumerator DoCityUpdate(City city)
    {
        yield return new WaitForSeconds(GameConsts.fightSpeed);
        FightResult result = unitFightResults.Find(c => c.unit == null);
        city.GetHexCell().TextEffectHandler.AddTextEffect(result.damageReceived.ToString(), city.GetHexCell().transform, Color.red);
        city.UpdateHealthBar();
        city.UpdateCity();
    }

    public IEnumerator DoAbility()
    {
        HexCell cell = path[0];
        if ((cell.IsVisible || actionCell.IsVisible) && GameConsts.playAnimations)
        {
            cell.IncreaseVisibility(false);
            abilityConfigToShow.Show(actionCell);
            t = Time.deltaTime * HexUnit.TravelSpeed;
            for (; t < 1f; t += Time.deltaTime * 1)
            {
                yield return null;
            }
            cell.DecreaseVisibility();
        }
        abilityConfigToShow.Finish(actionCell);

    }


}