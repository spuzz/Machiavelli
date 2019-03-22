using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnitActionController : MonoBehaviour {

    List<HexAction> actionQueue = new List<HexAction>();
    List<HexCell> blockedCells = new List<HexCell>();
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
            Destroy(action);
        }
        actionQueue.Clear();
        blockedCells.Clear();
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

    public HexAction CreatAction()
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

    public void BlockCell(HexCell cell)
    {
        blockedCells.Add(cell);
    }

    public void BlockCells(IEnumerable<HexCell> cells)
    {
        foreach(HexCell cell in cells)
        {
            BlockCell(cell);
        }
    }

    public void UnblockCell(HexCell cell)
    {
        blockedCells.Remove(cell);
    }

    public void UnblockCells(IEnumerable<HexCell> cells)
    {
        foreach (HexCell cell in cells)
        {
            UnblockCell(cell);
        }
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
            if(blockedCells.Contains(cell))
            {
                return false;
            }
        }
        if(blockedCells.Contains(action.ActionCell))
        {
            return false;
        }

        return true;
    }

    public void StartAction(HexAction action)
    {
        action.ActionStatus = HexAction.Status.RUNNING;
        if(action.ActionCell)
        {
            BlockCells(action.GetPath());
        }
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
            if (action.ActionCell)
            {
                UnblockCells(action.GetPath());
            }
            actionQueue.Remove(action);
            Destroy(action.gameObject);
        }
    }
}
