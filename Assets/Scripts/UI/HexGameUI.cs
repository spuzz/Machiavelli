using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour {

    public HexGrid grid;

    HexCell currentCell;

    HexUnit selectedUnit;

    [SerializeField] GameController gameController;
    [SerializeField] HUD HUD;

    private bool editMode;
    [SerializeField] HexMapEditor hexMapEditor;
    private bool abilitySelection = false;
    private int abilityIndex;
    private List<HexCell> abilityTargetOptions;
    public void SetEditMode(bool toggle) {
        
        editMode = toggle;
        enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
        grid.EditMode = toggle;
        hexMapEditor.SetEditMode(toggle);

        if (toggle) {
			Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
		}
		else {
			Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
		}
	}

    public bool GetEditMode()
    {
        return editMode;
    }
	void Update () {
        if (abilitySelection == true)
        {
            DoAbilityInput();
        }
        else
        {
            DoSelectionInput();
        }


        if (grid.EditMode != editMode)
        {
            SetEditMode(grid.EditMode);
        }



    }

    private void DoSelectionInput()
    {
        if (gameController.TurnOver == false && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoSelection();
            }
            else if (selectedUnit)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    DoMove();
                }
                else
                {
                    DoPathfinding();
                }
            }
        }
    }


    private void DoAbilityInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FinishAbilitySelection();
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            grid.ClearPath();
            abilitySelection = false;
            abilityTargetOptions.Clear();
            HUD.UpdateUI();
        }
    }

    public void ToggleEditMode()
    {
        SetEditMode(!editMode);
    }

	void DoSelection () {
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) {
            if(currentCell.GetTopUnit() && currentCell.GetTopUnit().Controllable)
            {
                if(selectedUnit)
                {
                    SetLayerRecursively(selectedUnit.gameObject,0);
                }
                selectedUnit = currentCell.GetTopUnit();
                SetLayerRecursively(selectedUnit.gameObject, 9);
                HUD.Unit = selectedUnit.GetComponent<Unit>();
            }
            else if (currentCell.City)
            {
                if (selectedUnit)
                {
                    SetLayerRecursively(selectedUnit.gameObject, 0);
                }
                selectedUnit = null;
                HUD.City  = currentCell.City;
            }
            else if(currentCell.OpCentre)
            {
                if (selectedUnit)
                {
                    SetLayerRecursively(selectedUnit.gameObject, 0);
                }
                selectedUnit = null;
                HUD.OpCentre = currentCell.OpCentre;
            }
        }
	}

    public void SelectOpCentre(OperationCentre opCentre)
    {
        grid.ClearPath();
        selectedUnit = null;
        HUD.OpCentre = opCentre;
    }

    public void SelectCity(City city)
    {
        grid.ClearPath();
        selectedUnit = null;
        HUD.City = city;
    }

    public void SelectUnit(Unit unit)
    {
        grid.ClearPath();
        currentCell = unit.HexUnit.Location;
        selectedUnit = unit.HexUnit;
    }

    void DoPathfinding () {
		if (UpdateCurrentCell()) {
            if (currentCell && (selectedUnit.IsValidDestination(currentCell) || selectedUnit.IsValidAttackDestination(currentCell))) {
				grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
			}
			else {
				grid.ClearPath();
			}
		}
	}

	void DoMove () {
		if (grid.HasPath) {
			selectedUnit.GetComponent<Unit>().SetPath(grid.GetPath());
            selectedUnit.GetComponent<Unit>().DoActions();
            grid.ClearPath();
            HUD.UpdateUI();
		}
	}
    public void DoAbilitySelection(List<HexCell> cellOptions, int index)
    {
        abilityTargetOptions = cellOptions;
        abilitySelection = true;
        abilityIndex = index;
        grid.ClearPath();
        grid.HighlightCells(abilityTargetOptions);
       
    }

    void FinishAbilitySelection()
    {
        HexCell target = grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (selectedUnit && target && abilityTargetOptions.Contains(target))
        {
            selectedUnit.GetComponent<Unit>().RunAbility(abilityIndex, target);
        }
        grid.ClearHighlightedCells(abilityTargetOptions);
        abilitySelection = false;
        abilityTargetOptions.Clear();
        HUD.UpdateUI();
    }

    bool UpdateCurrentCell () {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

}