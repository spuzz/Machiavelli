using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour {

    public HexGrid grid;

    HexCell currentCell;

    HexUnit selectedUnit;

    [SerializeField] GameController gameController;
    [SerializeField] HUD HUD;

    private bool editMode;

    public void SetEditMode(bool toggle) {
        editMode = toggle;
        enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
        grid.EditMode = toggle;

        if (toggle) {
			Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
		}
		else {
			Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
		}
	}

	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown(0)) {
				DoSelection();
			}
			else if (selectedUnit) {
				if (Input.GetMouseButtonDown(1)) {
					DoMove();
				}
				else {
					DoPathfinding();
				}
			}
		}

        if(grid.EditMode != editMode)
        {
            SetEditMode(grid.EditMode);
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
                selectedUnit = currentCell.GetTopUnit();
                HUD.Unit = selectedUnit.GetComponent<Unit>();
            }
            else if (currentCell.City)
            {
                selectedUnit = null;
                HUD.City  = currentCell.City;
            }
            else if(currentCell.OpCentre)
            {
                selectedUnit = null;
                HUD.OpCentre = currentCell.OpCentre;
            }
        }
	}

	void DoPathfinding () {
		if (UpdateCurrentCell()) {
			if (currentCell && selectedUnit.IsValidDestination(currentCell)) {
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
			grid.ClearPath();
            HUD.UpdateUI();
		}
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
}