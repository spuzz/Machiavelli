using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationCentre : MonoBehaviour
{
    
    [SerializeField] HexGrid hexGrid;
    [SerializeField] BuildingManager buildingManager;
    [SerializeField] List<AgentConfig> agentConfigs = new List<AgentConfig>();
    [SerializeField] List<CombatUnitConfig> mercConfigs = new List<CombatUnitConfig>();
    [SerializeField] int visionRange = 2;
    [SerializeField] OpCentreUI opCentreUI;

    GameController gameController;
    Player player;
    HexCell location;
    int influence = 2;
    int influenceRange = 2;

    HexVision hexVision;
    public BuildingManager BuildingManager
    {
        get
        {
            return buildingManager;
        }

        set
        {
            buildingManager = value;
        }
    }

    public HexCell Location
    {
        get
        {
            return location;
        }

        set
        {
            location = value;
            location.OpCentre = this;
            HexVision.SetCells(hexGrid.GetVisibleCells(Location, VisionRange));
        }
    }

    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
            transform.parent = player.operationCenterTransformParent.transform;
            Location.CellSecondColor = player.Color;
            opCentreUI.SetPlayerColour(player.Color);

        }
    }

    public HexVision HexVision
    {
        get
        {
            return hexVision;
        }

        set
        {
            hexVision = value;
        }
    }

    public int VisionRange
    {
        get
        {
            return visionRange;
        }

        set
        {
            visionRange = value;
        }
    }

    public IEnumerable<AgentConfig> GetAgentConfigs()
    {
        return agentConfigs;
    }

    public IEnumerable<CombatUnitConfig> GetCombatUnitConfigs()
    {
        return mercConfigs;
    }

    private void Awake()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        gameController = FindObjectOfType<GameController>();
        hexVision = gameObject.AddComponent<HexVision>();
        gameController.VisionSystem.AddHexVision(hexVision);

    }

    public void StartTurn()
    {
        BuildingManager.DayPassed(1);
        BuildConfig buildConfig = BuildingManager.GetCompletedBuild();
        while (buildConfig)
        {
            buildConfig = BuildingManager.GetCompletedBuild();
        }
        foreach (HexCell cityCell in PathFindingUtilities.GetCellsInRange(Location, influenceRange).FindAll(c => c.City))
        {
            cityCell.City.GetCityState().AdjustInfluence(Player, influence);
        }
    }

    public void HireAgent(AgentConfig agentConfig)
    {
        CreateAgent(agentConfig);
    }

    public bool CreateAgent(AgentConfig agentConfig)
    {
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(Location, 2);
        foreach (HexCell cell in cells)
        {
            if (cell.CanUnitMoveToCell(HexUnit.UnitType.AGENT))
            {
                gameController.CreateAgent(agentConfig,cell, Player);
                return true;
            }
        }
        return false;
    }
    public void HireMercenary(CombatUnitConfig mercConfig)
    {
        CreateMercenary(mercConfig);
    }

    public bool CreateMercenary(CombatUnitConfig mercConfig)
    {
        List<HexCell> cells = PathFindingUtilities.GetCellsInRange(Location, 2);
        foreach (HexCell cell in cells)
        {
            if (cell.CanUnitMoveToCell(HexUnit.UnitType.AGENT))
            {
                gameController.CreateMercenary(mercConfig, cell, Player);
                return true;
            }
        }
        return false;
    }

    public void DestroyOperationCentre()
    {
        Destroy(gameObject);
    }


}
