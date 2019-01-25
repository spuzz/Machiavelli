using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationCentre : MonoBehaviour
{
    
    [SerializeField] HexGrid hexGrid;

    List<City> cities = new List<City>();
    GameController gameController;

    [SerializeField] BuildingManager buildingManager;
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


    HexCell location;
    public HexCell Location
    {
        get
        {
            return location;
        }

        set
        {
            location = value;
        }
    }

    Player player;
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
        BuildingManager.DayPassed();
        BuildConfig buildConfig = BuildingManager.GetCompletedBuild();
        while (buildConfig)
        {
            GameObject gameObjectPrefab = buildConfig.GameObjectPrefab;
            if (gameObjectPrefab.GetComponent<CombatUnit>())
            {
                CreateAgent(buildConfig);
            }
            buildConfig = BuildingManager.GetCompletedBuild();
        }
    }
    public bool CreateAgent(BuildConfig buildConfig)
    {
        HexUnit hexUnit = Instantiate(buildConfig.GameObjectPrefab.GetComponent<HexUnit>());
        hexUnit.UnitPrefabName = buildConfig.UnitPreFabName;
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
