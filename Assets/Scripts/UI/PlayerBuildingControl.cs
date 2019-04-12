using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerBuildingControl : MonoBehaviour {

    GameController gameController;
    Dictionary<Player, BuildingManager> playerBuildingManagers = new Dictionary<Player, BuildingManager>();
    Dictionary<Player, CityPlayerBuilding[]> playerBuildings = new Dictionary<Player, CityPlayerBuilding[]>();
    Dictionary<Player, CityPlayerBuilding> outposts = new Dictionary<Player, CityPlayerBuilding>();

    [SerializeField] CityPlayerBuildConfig outpostConfig;
    [SerializeField] City city;
    [SerializeField] ResourceBenefit benefit;
    public CityPlayerBuildConfig OutpostConfig
    {
        get
        {
            return outpostConfig;
        }

        set
        {
            outpostConfig = value;
        }
    }

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
    }
    public BuildingManager GetPlayerBuildingManager(Player player)
    {
        if(playerBuildingManagers.ContainsKey(player))
        {
            return playerBuildingManagers[player];
        }

        return null;
    }

    public void StartTurn()
    {
        foreach(Player player in playerBuildingManagers.Keys)
        {
            PlayerBuildingManagerStartTurn(player);
        }
        
    }

    public void BuildBuilding(int buildingID, Player player,int slotID)
    {
        if(outposts.ContainsKey(player))
        {
            CityPlayerBuildConfig config = player.GetCityPlayerBuildConfig(buildingID);
            if(player.Gold >= config.BasePurchaseCost)
            {
                player.Gold -= config.BasePurchaseCost;
                playerBuildingManagers[player].AddBuild(config, slotID);
            }
            
        }
        city.NotifyInfoChange();
    }
    private void PlayerBuildingManagerStartTurn(Player player)
    {
        BuildingManager buildingManager = playerBuildingManagers[player];
        buildingManager.DayPassed(1);
        BuildConfig buildConfig = buildingManager.GetCompletedBuild();
        while (buildConfig)
        {
            if (buildConfig.GetBuildType() == BuildConfig.BUILDTYPE.CITY_PLAYER_BUILDING)
            {
                AddBuilding(buildConfig as CityPlayerBuildConfig, player, buildingManager.GetIDofLastCompletedBuild());
            }
            buildConfig = buildingManager.GetCompletedBuild();
        }

        foreach(var buildings in playerBuildings)
        {
            foreach(CityPlayerBuilding building in buildings.Value)
            {
                if(building)
                {
                    building.StartTurn();
                }
                
            }
        }
    }

    private void AddBuilding(CityPlayerBuildConfig cityPlayerBuildConfig, Player player, int id)
    {
        playerBuildings[player][id] = gameController.CreateCityPlayerBuilding(cityPlayerBuildConfig);
        playerBuildings[player][id].CityBuildIn = city;
        playerBuildings[player][id].PlayersBuilding = player;
        playerBuildings[player][id].Init();
        city.RefreshYields();
        city.NotifyInfoChange();
    }

    public ResourceBenefit GetTotalEffects()
    {
        benefit.ResetBenefit();
        foreach(Player player in playerBuildings.Keys)
        {
            foreach(CityPlayerBuilding building in playerBuildings[player])
            {
                if(building)
                {
                    benefit.AddBenefit(building.ResourceBenefit);
                }
                
            }
        }

        return benefit;
    }

    public CityPlayerBuilding GetPlayerBuilding(int slotID, Player player)
    {
        if(HasOutpost(player))
        {
            return playerBuildings[player][slotID];
        }
        return null;
    }

    public bool HasBuilding(string name, Player player)
    {
        CityPlayerBuilding[] buildings = playerBuildings[player];
        foreach(CityPlayerBuilding building in buildings)
        {
            if(building && building.BuildConfig.Name == name)
            {
                return true;
            }
        }
        return false;
    }

    public IEnumerable<CityPlayerBuilding> GetPlayerBuildings(Player player)
    {
        if (HasOutpost(player))
        {
            return playerBuildings[player];
        }
        return null;
    }

    public IEnumerable<Player> GetPlayersWithOutposts()
    {

        return outposts.Keys;
    }



    public bool IsConstructingBuilding(int slotID, Player player)
    {
        if(HasOutpost(player))
        {
            if (playerBuildingManagers[player].IsIDInQueue(slotID))
            {
                return true;
            }
        }
        return false;
    }

    public int TimeLeftOnConstruction(int slotID, Player player)
    {
        if (HasOutpost(player))
        {
            if(playerBuildingManagers[player].IDInConstruction() == slotID)
            {
                return playerBuildingManagers[player].TimeLeftOnBuild(1);
            }
            else
            {
                if(playerBuildingManagers[player].GetConfigInQueueByID(slotID))
                {
                    return playerBuildingManagers[player].GetConfigInQueueByID(slotID).BaseBuildTime;
                }
                
            }
            
        }
        return -1;
    }

    public void AddOutpost(Player player)
    {
        if(!outposts.ContainsKey(player))
        {
            outposts.Add(player, gameController.CreateCityPlayerBuilding(OutpostConfig));
            playerBuildingManagers.Add(player, new BuildingManager());
            playerBuildings.Add(player, new CityPlayerBuilding[5]);
            HexVision hexVision = outposts[player].gameObject.AddComponent<HexVision>();
            if(player.IsHuman)
            {
                hexVision.HasVision = true;
            }

            List<HexCell> cells = (PathFindingUtilities.GetCellsInRange(city.GetHexCell(), 1));
            hexVision.SetCells(cells);
            for (int i = 0; i < cells.Count; i++)
            {
                player.AddVisibleCell(cells[i]);
            }
            gameController.VisionSystem.AddHexVision(hexVision);
            player.AddCityWithOutpost(city);
            city.NotifyInfoChange();
        }
    }
    public bool HasOutpost(Player player)
    {
        if(outposts.ContainsKey(player))
        {
            return true;
        }
        return false;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(outposts.Count);
        foreach (Player player in outposts.Keys)
        {
            writer.Write(player.PlayerNumber);
            playerBuildingManagers[player].Save(writer);
            foreach (CityPlayerBuilding building in playerBuildings[player])
            {
                if (building)
                {
                    writer.Write(building.BuildConfig.Name);
                }
                else
                {
                    writer.Write("No Building");
                }
            }
        }
    }

    public void Load(BinaryReader reader, GameController gameController, int header)
    {
        if(header >= 5)
        {
            int playerCount = reader.ReadInt32();
            for(int a=0;a<playerCount;a++)
            {
                int playerNumber = reader.ReadInt32();
                Player player = gameController.GetPlayer(playerNumber);
                AddOutpost(player);
                BuildingManager manager = new BuildingManager();
                manager.Load(reader, gameController, header);
                playerBuildingManagers[player] = manager;
                for (int b = 0; b < 5; b++)
                {
                    BuildConfig config = gameController.GetBuildConfig(reader.ReadString());
                    if (config)
                    {
                        AddBuilding(config as CityPlayerBuildConfig, player, b);
                    }

                }

            }
        }
    }
}
