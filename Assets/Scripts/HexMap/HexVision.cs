using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexVision : MonoBehaviour
{

    List<HexCell> visibleCells = new List<HexCell>();

    bool editMode = false;
    bool hasVision = false;
    bool visible = false;
    VisionSystem visionSystem;

    List<GameObject> visibleObjects = new List<GameObject>();
    public bool EditMode
    {
        get
        {
            return editMode;
        }

        set
        {
            editMode = value;
        }
    }

    public bool HasVision
    {
        get
        {
            return editMode || hasVision;
        }

        set
        {
            UpdateVision(value);
            hasVision = value;
        }
    }

    public bool Visible
    {
        get
        {
            return visible;
        }

        set
        {
            if (visible != value)
            {
                UpdateVisibleObjects(value);
            }
            visible = value;

        }
    }

    private void UpdateVisibleObjects(bool isVisible)
    {
        foreach (GameObject visibleObject in visibleObjects)
        {
            visibleObject.SetActive(isVisible);
        }
    }

    public void ResetVision()
    {
        if(hasVision == true)
        {
            IncreaseVisionInCells();
        }
    }
    private void UpdateVision(bool value)
    {
        if(editMode == false && hasVision != value)
        {
            if(value == true)
            {
                IncreaseVisionInCells();
            }
            else if(value == false)
            {
                DecreaseVisionInCells();
            }
        }
    }

    private void IncreaseVisionInCells()
    {
        foreach(HexCell cell in visibleCells)
        {
            cell.IncreaseVisibility(editMode);
        }
    }

    private void DecreaseVisionInCells()
    {
        foreach (HexCell cell in visibleCells)
        {
            cell.DecreaseVisibility();
        }
    }


    public void SetVisionSystem(VisionSystem visSystem)
    {
        visionSystem = visSystem;
    }

    public void AddCell(HexCell cell)
    {
        if (editMode == false && hasVision)
        {
            cell.IncreaseVisibility(editMode);
        }
        visibleCells.Add(cell);

    }

    public void AddCells(List<HexCell> cells)
    {
        foreach(HexCell cell in cells)
        {
            if (editMode == false && hasVision)
            {
                cell.IncreaseVisibility(editMode);
            }
            visibleCells.Add(cell);
        }
    }

    public void SetCells(List<HexCell> cells)
    {
        ClearCells();
        AddCells(cells);
    }
    public void RemoveCell(HexCell cell)
    {
        if (editMode == false && hasVision)
        {
            cell.DecreaseVisibility();
        }
        visibleCells.Remove(cell);

    }

    public void RemoveCells(List<HexCell> cells)
    {
        foreach (HexCell cell in cells)
        {
            if (editMode == false && hasVision)
            {
                cell.DecreaseVisibility();
            }

           visibleCells.Remove(cell);
        }
    }

    public void ClearCells()
    {
        if(editMode == false && hasVision)
        {
            DecreaseVisionInCells();
        }
        visibleCells.Clear();
    }


    public void AddVisibleObject(GameObject gameObject)
    {
        visibleObjects.Add(gameObject);
        gameObject.SetActive(Visible);
    }
}