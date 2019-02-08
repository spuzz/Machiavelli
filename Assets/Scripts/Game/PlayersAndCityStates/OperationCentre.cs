using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationCentre : MonoBehaviour
{
    
    [SerializeField] HexGrid hexGrid;
    [SerializeField] BuildingManager buildingManager;
    [SerializeField] List<BuildConfig> agentConfigs = new List<BuildConfig>();

    GameController gameController;
    Player player;
    HexCell location;
    int influence = 2;
    int influenceRange = 2;
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
        }
    }

    private void Awake()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        gameController = FindObjectOfType<GameController>();
    }

    public void StartTurn()
    {
        BuildingManager.DayPassed(1);
        BuildConfig buildConfig = BuildingManager.GetCompletedBuild();
        while (buildConfig)
        {
            GameObject gameObjectPrefab = buildConfig.GameObjectPrefab;
            if (gameObjectPrefab.GetComponent<Agent>())
            {
                CreateAgent(buildConfig);
            }
            buildConfig = BuildingManager.GetCompletedBuild();
        }
        foreach (HexCell cityCell in PathFindingUtilities.GetCellsInRange(Location, influenceRange).FindAll(c => c.City))
        {
            cityCell.City.GetCityState().AdjustInfluence(Player, influence);
        }
    }

    public void HireAgent(int type)
    {
        BuildingManager.AddBuild(agentConfigs[type]);
    }

    public bool CreateAgent(BuildConfig buildConfig)
    {
        HexUnit hexUnit = Instantiate(buildConfig.GameObjectPrefab.GetComponent<HexUnit>());
        hexUnit.UnitPrefabName = buildConfig.PreFabName;
        HexCell cell = PathFindingUtilities.FindFreeCell(hexUnit,Location);
        if (!cell)
        {
            return false;
        }
        hexGrid.AddUnit(hexUnit, cell, UnityEngine.Random.Range(0f, 360f));
        gameController.CreateAgent(hexUnit, Player);
        return true;
    }

    public void DestroyOperationCentre()
    {
        Destroy(gameObject);
    }


}
