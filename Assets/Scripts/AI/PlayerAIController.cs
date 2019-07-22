using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAIController : MonoBehaviour
{
    Dictionary<HexCell, BuildConfig> buildList = new Dictionary<HexCell, BuildConfig>();
    [SerializeField] Player player;

    public IEnumerator UpdateUnits(IEnumerable<Agent> agents)
    {
        List<Agent> agentsAtStart = new List<Agent>();

        foreach (Agent agent in agents)
        {
            agentsAtStart.Add(agent);
        }

        foreach (Agent agent in agentsAtStart)
        {

            agent.GetComponent<AgentBehaviourTree>().TakeTurn();
            while (agent && agent.GetComponent<AgentBehaviourTree>().IsFinished() == false)
            {
                yield return new WaitForEndOfFrame();
            }
            if (agent)
            {
                agent.EndTurn();
            }
        }

        UpdateBuilds();
    }

    private void UpdateBuilds()
    {
        UpdateBuildList();

        bool buildSuccessful = true;
        while(buildList.Count != 0)
        {
            IEnumerable<HexCell> cell = IListExtensions.RandomKeys(buildList);
            HexCell cellToBuildOn = cell.First();
            if (cellToBuildOn.OpCentre)
            {
                if(cellToBuildOn.OpCentre.IsAvailableToBuild(buildList[cellToBuildOn]))
                {
                    cellToBuildOn.OpCentre.BuildUsingBuildConfig(buildList[cellToBuildOn]);
                }
                buildList.Remove(buildList.First().Key);
            }
            if (cellToBuildOn.City)
            {
                cellToBuildOn.City.PlayerBuildingControl.BuildBuilding(buildList[cellToBuildOn],player, cellToBuildOn.City.PlayerBuildingControl.GetFreeBuildSlot(player));
                buildList.Remove(buildList.First().Key);
            }
        }

    }

    private void UpdateBuildList()
    {
        buildList.Clear();
        GetPriorityBuilds();
        if(buildList.Count > 0)
        {
            return;
        }
        GetAgentBuilds();
        if (buildList.Count > 0)
        {
            return;
        }
        foreach (OperationCentre opCentre in player.opCentres)
        {
            List<BuildConfig> opCentreBuilds = new List<BuildConfig>();

            if (opCentre.IsConstructingBuilding() == false && opCentre.buildingSpaceAvailable())
            {
                IEnumerable<OpCentreBuildConfig> configs = opCentre.availableBuilds;
                if (configs.Count() > 0)
                {
                    opCentreBuilds.Add(opCentre.availableBuilds[UnityEngine.Random.Range(0, opCentre.availableBuilds.Count)]);
                }
            }

            if (opCentreBuilds.Count > 0)
            {
                buildList.Add(opCentre.Location, opCentreBuilds[UnityEngine.Random.Range(0, opCentreBuilds.Count)]);
            }

        }
        foreach (City city in player.citiesWithOutposts)
        {
            if (!city.PlayerBuildingControl.HasBuilding("GovernmentAdvisor", player) && city.GetInfluencePerTurn(player) < 1)
            {
                buildList.Add(city.GetHexCell(), player.GetCityPlayerBuildConfig("GovernmentAdvisor"));
            }
            else
            {
                buildList.Add(city.GetHexCell(), player.GetRandomCityPlayerBuildConfigs());
            }
            
        }
    }

    private void GetPriorityBuilds()
    {
        List<KeyValuePair<HexCell,BuildConfig>> priorityBuilds = new List<KeyValuePair<HexCell, BuildConfig>>();
        foreach (OperationCentre opCentre in player.opCentres)
        {
            foreach (HexCell cell in player.exploredCells.FindAll(c => c.City))
            {
                if (!cell.City.PlayerBuildingControl.HasOutpost(player) && player.IsCityStateFriendly(cell.City.GetCityState()))
                {
                    priorityBuilds.Add(new KeyValuePair<HexCell, BuildConfig>(opCentre.Location, opCentre.GetAgentBuildConfigs("Builder")));
                    break;
                }
            }

            if (player.PlayerAgentTracker.CanRecruit(opCentre.GetAgentBuildConfigs("Diplomat").AgentConfig))
            {
                priorityBuilds.Add(new KeyValuePair<HexCell, BuildConfig>(opCentre.Location, opCentre.GetAgentBuildConfigs("Diplomat")));
            }

            if (player.agents.FindAll(c => c.GetAgentConfig().Name.CompareTo("Scout") == 0).Count < 1)
            {
                priorityBuilds.Add(new KeyValuePair<HexCell, BuildConfig>(opCentre.Location, opCentre.GetAgentBuildConfigs("Scout")));
            }
        }
        foreach (City city in player.citiesWithOutposts)
        {
            if (!city.PlayerBuildingControl.HasBuilding("GovernmentAdvisor", player) && city.GetInfluencePerTurn(player) < 1)
            {
                priorityBuilds.Add(new KeyValuePair<HexCell, BuildConfig>(city.GetHexCell(), player.GetCityPlayerBuildConfig("GovernmentAdvisor")));
            }
        }
        if(priorityBuilds.Count > 0)
        {
            KeyValuePair<HexCell, BuildConfig> build = priorityBuilds[UnityEngine.Random.Range(0, priorityBuilds.Count)];
            buildList.Add(build.Key, build.Value);
        }

    }

    private void GetAgentBuilds()
    {
        foreach (OperationCentre opCentre in player.opCentres)
        {
            if (opCentre.GetAgentBuildConfigs().Count() > 0)
            {
                IEnumerable<AgentBuildConfig> configs = opCentre.GetAgentBuildConfigs(new List<string>() { "Builder", "Scout" });
                List<AgentBuildConfig> buildableConfigs = new List<AgentBuildConfig>();
                foreach (AgentBuildConfig config in configs)
                {
                    if (player.PlayerAgentTracker.CanRecruit(config.AgentConfig))
                    {
                        buildableConfigs.Add(config);
                    }
                }
                if (buildableConfigs.Count() > 0)
                {
                    buildList.Add(opCentre.Location, IListExtensions.RandomElement(buildableConfigs));
                }

            }
        }
    }

}
