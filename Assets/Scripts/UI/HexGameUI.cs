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
        if (Input.GetKeyDown(KeyCode.Escape) && HUD.City)
        {
            ClearSelection();
        }
        if (gameController.TurnOver == false)
        {

            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    DoSelection();
                }

            }
            else if (selectedUnit)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    DoMove();
                }
                else
                {
                    //if (!EventSystem.current.IsPointerOverGameObject())
                    //{
                        DoPathfinding();

                    //}
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
		if (currentCell)
        {
            if (currentCell.agent && currentCell.agent.Controllable)
            {
                SelectUnit(currentCell.agent);
            }
            else if (currentCell.combatUnit && currentCell.combatUnit.Controllable)
            {
                SelectUnit(currentCell.combatUnit);
            }
            else if (currentCell.City)
            {
                ClearSelection();
                if (selectedUnit)
                {

                    SetLayerRecursively(selectedUnit.gameObject, 0);
                    selectedUnit.Location.DisableHighlight();
                }
                SelectCity(currentCell.City);
            }
        }
    }

    public void SelectUnit(HexUnit unit)
    {
        ClearSelection();
        if (selectedUnit)
        {
            SetLayerRecursively(selectedUnit.gameObject, 0);
            selectedUnit.Location.DisableHighlight();
        }
        selectedUnit = unit;
        SetLayerRecursively(selectedUnit.gameObject, 9);
        selectedUnit.Location.EnableHighlight(Color.blue);
        selectedUnit.unit.UpdateUI(0);

        HUD.Unit = selectedUnit.GetComponent<Unit>();

    }

    private void ClearSelection()
    {
        if (HUD.City)
        {
            HUD.City.GetHexCell().HexCellUI.EnableCanvas(false);
            foreach (HexCell cell in HUD.City.GetOwnedCells())
            {
                cell.HexCellUI.EnableCanvas(false);
            }
            HUD.City = null;
        }
    }

    public void SelectCity(City city)
    {

        grid.ClearPath();
        selectedUnit = null;
        HUD.City = city;
        city.GetHexCell().HexCellUI.EnableCanvas(true);
        city.GetHexCell().HexCellUI.CurrentCityOwner = city;
        city.GetHexCell().HexCellUI.SetToggleLocked(true);
        foreach (HexCell cell in city.GetOwnedCells())
        {
            cell.HexCellUI.EnableCanvas(true);
            cell.HexCellUI.CurrentCityOwner = city;
            cell.HexCellUI.SetToggleLocked(false);
        }
    }

    void DoPathfinding () {
		if (UpdateCurrentCell()) {
            if (currentCell && ValidMove(selectedUnit)) { //) {
                grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
                if (grid.GetPath() != null && grid.GetPath().Count != 0 && selectedUnit.IsValidAttackDestination(currentCell) && currentCell.IsVisible)
                {

                    HUD.ShowCombatPanel(selectedUnit.Location, currentCell);
                }
                else
                {
                    HUD.HideCombatPanel(grid.GetPath());
                }
                

            }
			else {
				grid.ClearPath();
                HUD.HideCombatPanel(null);
            }
		}
	}

    bool ValidMove(HexUnit unit)
    {

        if (!unit.IsValidDestination(currentCell) && !unit.IsValidAttackDestination(currentCell))
        {
            return false;
        }
        return true;
    }

	void DoMove () {
		if (grid.HasPath) {
            selectedUnit.Location.DisableHighlight();
            List<HexCell> path = grid.GetPath();
            HUD.HideCombatPanel(null);
            City city = path[path.Count - 1].City;
            if (path[path.Count - 1].GetFightableUnit(selectedUnit))
            {
                selectedUnit.GetComponent<Unit>().SetPath(path.GetRange(0, path.Count - 1));
                selectedUnit.GetComponent<Unit>().AttackCell(path[path.Count - 1]);
                selectedUnit.DoActions();
            }
            else if (city && city.GetCityState().Player && !city.GetCityState().Player.IsHuman && selectedUnit.unit.HexUnitType == Unit.UnitType.COMBAT)
            {
                selectedUnit.GetComponent<Unit>().SetPath(path.GetRange(0, path.Count - 1));
                selectedUnit.GetComponent<Unit>().CaptureCity(path[path.Count - 1]);
                selectedUnit.DoActions();
            }
            else
            {
                selectedUnit.GetComponent<Unit>().SetPath(grid.GetPath());
                selectedUnit.DoActions();
            }
            grid.ClearPath();
            HUD.UpdateUI();
            selectedUnit.Location.EnableHighlight(Color.blue);
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
            selectedUnit.GetComponent<Abilities>().RunAbility(abilityIndex, target, true);
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