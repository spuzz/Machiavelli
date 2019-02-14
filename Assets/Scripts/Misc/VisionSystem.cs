using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionSystem : MonoBehaviour {

    List<HexVision> hexVisionComponents = new List<HexVision>();
    HexGrid hexGrid;
    bool editMode = false;

    private void Awake()
    {
        hexGrid = FindObjectOfType<HexGrid>();
    }
    public void AddHexVision(HexVision hexVision)
    {
        hexVisionComponents.Add(hexVision);
    }

    public void RemoveHexVision(HexVision hexVision)
    {
        hexVisionComponents.Add(hexVision);
    }

    public void TurnOnEditMode()
    {
        if(editMode != true)
        {
            hexGrid.SetAllCellsToOneVision();
            foreach(HexVision hexVision in hexVisionComponents)
            {
                hexVision.EditMode = true;
            }
            editMode = true;
        }
    }

    public void TurnOffEditMode()
    {
        if (editMode == true)
        {
            foreach (HexVision hexVision in hexVisionComponents)
            {
                hexVision.EditMode = false;
            }
            hexGrid.SetVisibilityToZero();
            foreach (HexVision hexVision in hexVisionComponents)
            {
                hexVision.ResetVision();
            }
            editMode = false;
        }
    }

    public void Clear()
    {
        editMode = false;
        hexVisionComponents.Clear();
        hexGrid.SetVisibilityToZero();
    }

}
