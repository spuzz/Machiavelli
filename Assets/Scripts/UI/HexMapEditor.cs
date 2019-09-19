using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

public class HexMapEditor : MonoBehaviour {

    enum OptionalToggle
    {
        Ignore, Yes, No
    }

    [SerializeField] Dropdown playerUnits;
    [SerializeField] Dropdown players;
    [SerializeField] Dropdown cityStateUnits;
    [SerializeField] Dropdown cityStates;
    [SerializeField] Toggle editModeToggle;
    [SerializeField] Toggle animationsToggle;
    public HexGrid hexGrid;
    public GameController gameController;
	public Material terrainMaterial;

	int activeElevation;
	int activeWaterLevel;

	int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeSpecialIndex;

	int activeTerrainTypeIndex = -1;

	int brushSize;

	bool applyElevation = false;
	bool applyWaterLevel = false;
    bool applyCityState = false;
    bool applyCityStatePlayer = false;
    bool applyCityUnit = false;
    bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex, applyExplored;

	OptionalToggle riverMode, roadMode, walledMode, exploredMode;

	bool isDrag;
	HexDirection dragDirection;
	HexCell previousCell;

	public void SetTerrainTypeIndex (int index) {
		activeTerrainTypeIndex = index;
	}

	public void SetApplyElevation (bool toggle) {
		applyElevation = toggle;
	}

	public void SetElevation (float elevation) {
		activeElevation = (int)elevation;
	}

	public void SetApplyWaterLevel (bool toggle) {
		applyWaterLevel = toggle;
	}

	public void SetWaterLevel (float level) {
		activeWaterLevel = (int)level;
	}

	public void SetApplyUrbanLevel (bool toggle) {
		applyUrbanLevel = toggle;
	}

	public void SetUrbanLevel (float level) {
		activeUrbanLevel = (int)level;
	}

	public void SetApplyFarmLevel (bool toggle) {
		applyFarmLevel = toggle;
	}

	public void SetFarmLevel (float level) {
		activeFarmLevel = (int)level;
	}

	public void SetApplyPlantLevel (bool toggle) {
		applyPlantLevel = toggle;
	}

	public void SetPlantLevel (float level) {
		activePlantLevel = (int)level;
	}

	public void SetApplySpecialIndex (bool toggle) {
		applySpecialIndex = toggle;
	}

    public void SetApplyExplored(bool toggle)
    {
        applyExplored = toggle;
    }

    public void SetApplyCityState(bool toggle)
    {
        applyCityState = toggle;
    }
    public void SetApplyCityStatePlayer(bool toggle)
    {
        applyCityStatePlayer = toggle;
    }

    public void SetApplyCityUnit(bool toggle)
    {
        applyCityUnit = toggle;
    }

    public void SetSpecialIndex (float index) {
		activeSpecialIndex = (int)index;
	}

	public void SetBrushSize (float size) {
		brushSize = (int)size;
	}

	public void SetRiverMode (int mode) {
		riverMode = (OptionalToggle)mode;
	}

	public void SetRoadMode (int mode) {
		roadMode = (OptionalToggle)mode;
	}

	public void SetWalledMode (int mode) {
		walledMode = (OptionalToggle)mode;
	}

    public void SetExploredMode(int mode)
    {
        exploredMode = (OptionalToggle)mode;
    }

    public void SetEditMode (bool toggle) {
		enabled = toggle;
        editModeToggle.isOn = toggle;
	}

    public void SetAnimations(bool toggle)
    {
        GameConsts.playAnimations = toggle;
    }
    void Awake()
    {
        terrainMaterial.DisableKeyword("GRID_ON");
        Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
        SetEditMode(true);

        string myPath = "Assets/Resources/AgentConfigs";
        DirectoryInfo dir = new DirectoryInfo(myPath);
        FileInfo[] info = dir.GetFiles("*.asset");
        List<string> files = new List<string>();
        foreach (FileInfo f in info)
        {
            files.Add(f.Name.Split('.')[0]);
        }
        playerUnits.ClearOptions();
        playerUnits.AddOptions(files);
        playerUnits.value = 0;

        myPath = "Assets/Resources/CombatUnitConfigs";
        dir = new DirectoryInfo(myPath);
        info = dir.GetFiles("*.asset");
        files = new List<string>();
        foreach (FileInfo f in info)
        {
            files.Add(f.Name.Split('.')[0]);
        }
        cityStateUnits.ClearOptions();
        cityStateUnits.AddOptions(files);
        cityStateUnits.value = 0;

        cityStates.ClearOptions();


        GameConsts.playAnimations = false;
    }


	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButton(0)) {
				HandleInput();
				return;
			}
            if (Input.GetMouseButton(1))
            {
                HandleRightClickInput();
                return;
            }
            if (Input.GetKeyDown(KeyCode.U)) {
				if (Input.GetKey(KeyCode.LeftShift)) {
					DestroyUnit();
				}
				else {
					CreateUnit();
				}
				return;
			}
            if (Input.GetKeyDown(KeyCode.H))
            {
                CreateCityStateUnit();
            }

        }

        if(cityStates.options.Count != gameController.CityStateCount())
        {
            cityStates.ClearOptions();
            cityStates.AddOptions(gameController.GetCityNames());
        }

        if (players.options.Count != gameController.PlayerCount() || gameController.PlayerCount() == 3)
        {
            players.ClearOptions();
            players.AddOptions(gameController.PlayerNames());
        }

        previousCell = null;
	}
    public void ShowGrid(bool visible)
    {
        if (visible)
        {
            terrainMaterial.EnableKeyword("GRID_ON");
        }
        else
        {
            terrainMaterial.DisableKeyword("GRID_ON");
        }
    }

    HexCell GetCellUnderCursor () {
		return
			hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	}

	void CreateUnit () {
		HexCell cell = GetCellUnderCursor();
		if (players.options.Count > 0 && cell && cell.CanUnitMoveToCell(Unit.UnitType.AGENT)) {
            string name = playerUnits.options[playerUnits.value].text;
            string playerName = players.options[players.value].text;
            Player player;
            if(playerName == "Human Player")
            {
                player = gameController.HumanPlayer;
            }
            else
            {
                int playerID = System.Convert.ToInt32(players.options[players.value].text);
                player = gameController.GetPlayer(playerID);
            }
            


            HexUnit unit = gameController.CreateAgent(name, cell, player);

        }
    }

    public void CreateAIPlayer()
    {
        gameController.CreateAIPlayer();
    }
	void DestroyUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && cell.GetTopUnit()) {
			hexGrid.DestroyUnit(cell.GetTopUnit());
		}
	}

    void CreateCityStateUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cityStates.options.Count > 0 && cell && cell.CanUnitMoveToCell(Unit.UnitType.COMBAT))
        {
            int cityID = System.Convert.ToInt32(cityStates.options[cityStates.value].text);
            City city = gameController.GetCity(cityID);
            if(city)
            {
                string name = cityStateUnits.options[cityStateUnits.value].text;
                gameController.CreateCityStateUnit(name, cell, city);
            }

        }
    }

    public void SetCityPlayer(City city)
    {
        
        string playerName = players.options[players.value].text;
        Player player;
        if (playerName == "Human Player")
        {
            player = gameController.HumanPlayer;
        }
        else
        {
            int playerID = System.Convert.ToInt32(players.options[players.value].text);
            player = gameController.GetPlayer(playerID);
        }


        if(player && city.Player != player)
        {
            city.Player = player;
        }
    }

    void HandleInput () {
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell) {
			if (previousCell && previousCell != currentCell) {
				ValidateDrag(currentCell);
			}
			else {
				isDrag = false;
			}
			EditCells(currentCell);
			previousCell = currentCell;
		}
		else {
			previousCell = null;
		}
	}

    void HandleRightClickInput()
    {
        HexCell currentCell = GetCellUnderCursor();
        if (currentCell)
        {
            if (currentCell.City)
            {
                if(applyCityStatePlayer)
                {
                    SetCityPlayer(currentCell.City);
                }

            }
        }
    }

	void ValidateDrag (HexCell currentCell) {
		for (
			dragDirection = HexDirection.NE;
			dragDirection <= HexDirection.NW;
			dragDirection++
		) {
			if (previousCell.GetNeighbor(dragDirection) == currentCell) {
				isDrag = true;
				return;
			}
		}
		isDrag = false;
	}

	void EditCells (HexCell center) {
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;

		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
			for (int x = centerX - r; x <= centerX + brushSize; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
		for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
			for (int x = centerX - brushSize; x <= centerX + r; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
	}

	void EditCell (HexCell cell) {
		if (cell) {
			if (activeTerrainTypeIndex >= 0) {
				cell.TerrainTypeIndex = activeTerrainTypeIndex;
			}
			if (applyElevation) {
				cell.Elevation = activeElevation;
			}
			if (applyWaterLevel) {
				cell.WaterLevel = activeWaterLevel;
			}
			if (applySpecialIndex) {
                if(activeSpecialIndex == 2)
                {
                    hexGrid.CreateCity(cell);
                }
                else
                {
                    if(cell.City)
                    {
                        hexGrid.DestroyCity(cell.City);
                    }
                    
                }

                cell.SpecialIndex = activeSpecialIndex;
			}
			if (applyUrbanLevel) {
				cell.UrbanLevel = activeUrbanLevel;
			}
			if (applyFarmLevel) {
				cell.FarmLevel = activeFarmLevel;
			}
			if (applyPlantLevel) {
				cell.PlantLevel = activePlantLevel;
			}
			if (riverMode == OptionalToggle.No) {
				cell.RemoveRiver();
			}
			if (roadMode == OptionalToggle.No) {
				cell.RemoveRoads();
			}
			if (walledMode != OptionalToggle.Ignore) {
				cell.Walled = walledMode == OptionalToggle.Yes;
			}
            if (exploredMode != OptionalToggle.Ignore)
            {
                bool originalImmediateMode = cell.ShaderData.ImmediateMode;
                cell.ShaderData.ImmediateMode = true;
                cell.IsExplored = exploredMode == OptionalToggle.Yes;
                cell.ShaderData.ImmediateMode = originalImmediateMode;
            }

			if (isDrag) {
				HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
				if (otherCell) {
					if (riverMode == OptionalToggle.Yes) {
						otherCell.SetOutgoingRiver(dragDirection);
					}
					if (roadMode == OptionalToggle.Yes) {
						otherCell.AddRoad(dragDirection);
					}
				}
			}
		}
	}
}