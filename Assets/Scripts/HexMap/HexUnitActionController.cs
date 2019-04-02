using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnitActionController : MonoBehaviour {

    List<HexAction> actionQueue = new List<HexAction>();
    [SerializeField] GameObject hexActionObject;
    bool controlRunning = false;
    private void Start()
    {
        StartCoroutine(StartControl());
    }

    public void Clear()
    {
        foreach(HexAction action in actionQueue)
        {
            Destroy(action.gameObject);
        }
        actionQueue.Clear();
    }
    private IEnumerator StartControl()
    {
        controlRunning = true;
        while (controlRunning)
        {
            ClearFinishedActions();
            foreach(HexAction action in actionQueue)
            {
                if(CheckActionValid(action))
                {
                    StartAction(action);
                }
                
            }
            yield return new WaitForEndOfFrame();
        }
        
    }

    public HexAction CreateAction()
    {
        return Instantiate(hexActionObject, gameObject.transform).GetComponent<HexAction>();
    }

    public void AddAction(HexAction hexAction, HexUnit unit)
    {
        HexAction parentAction = actionQueue.FindLast(c => c.ActionsUnit == unit);
        if (parentAction)
        {
            hexAction.Parent = hexAction;
            parentAction.Child = hexAction;
        }

        actionQueue.Add(hexAction);

    }

    public void AddActions(List<HexAction> hexActions, HexUnit unit)
    {
        List<HexAction> moveActions = hexActions.FindAll(c => c.HexActionType == HexAction.ActionType.MOVE);
        while(moveActions.Count >= 2)
        {
            moveActions[0].AddAction(moveActions[1]);
            moveActions.Remove(moveActions[1]);
        }

        if(moveActions.Count > 0)
        {
            hexActions.RemoveAll(c => c.HexActionType == HexAction.ActionType.MOVE);
            hexActions.Insert(0, moveActions[0]);
        }

        foreach(HexAction action in hexActions)
        {
            AddAction(action,unit);
        }

    }



    public bool FinishedActions()
    {
        if(actionQueue.Count > 0)
        {
            return false;
        }
        return true;
    }

    public bool CheckActionValid(HexAction action)
    {
        if(action.ActionStatus == HexAction.Status.RUNNING || action.ActionStatus == HexAction.Status.FINISHED)
        {
            return false;
        }
        if(action.Parent)
        {
            return false;
        }
        foreach(HexCell cell in action.GetPath())
        {
            if(actionQueue.GetRange(0, actionQueue.IndexOf(action)).FindAll(c => c.ActionCell == cell).Count != 0)
            {
                return false;
            }
        }
        if(action.ActionCell)
        {
            HexUnit unit = action.UnitTarget;
            if(!action.ActionCell.City && actionQueue.GetRange(0,actionQueue.IndexOf(action)).FindAll(c => c.ActionsUnit == unit).Count != 0)
            {
                return false;
            }
        }
        if(action.CityTarget && action.KillTarget && action.CityStateTarget.GetCityCount() == 0)
        {
            if (actionQueue.GetRange(0, actionQueue.IndexOf(action)).FindAll(c => c.UnitTarget && c.UnitTarget.unit.GetCityState() == action.CityStateTarget).Count != 0)
            {
                return false;
            }
        }
        return true;
    }

    public void StartAction(HexAction action)
    {
        action.ActionStatus = HexAction.Status.RUNNING;
        StartCoroutine(action.Run());
    }

    private void ClearFinishedActions()
    {
        List<HexAction> finishedActions = actionQueue.FindAll(c => c.ActionStatus == HexAction.Status.FINISHED);
        foreach(HexAction action in finishedActions)
        {
            if(action.Child)
            {
                action.Child.Parent = null;
            }
            actionQueue.Remove(action);
            Destroy(action.gameObject);
        }
    }
}
