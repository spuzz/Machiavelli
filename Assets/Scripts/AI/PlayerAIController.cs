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
        while(buildList.Count != 0 && buildSuccessful)
        {
            IEnumerable<HexCell> cell = IListExtensions.RandomKeys(buildList);
            HexCell cellToBuildOn = cell.First();
            if (cellToBuildOn.OpCentre)
            {
                buildSuccessful = cellToBuildOn.OpCentre.BuildUsingBuildConfig(buildList[cellToBuildOn]);
                buildList.Remove(buildList.First().Key);
            }
        }

    }

    private void UpdateBuildList()
    {
        buildList.Clear();
        foreach (OperationCentre opCentre in player.opCentres)
        {
            List<BuildConfig> opCentreBuilds = new List<BuildConfig>();
            if (!opCentre.IsTraining())
            {
                foreach (HexCell cell in player.exploredCells.FindAll(c => c.City))
                {
                    if (!cell.City.PlayerBuildingControl.HasOutpost(player))
                    {
                        opCentreBuilds.Add(opCentre.GetAgentBuildConfigs("Builder"));
                        break;
                    }
                }

                if (player.PlayerAgentTracker.CanRecruit(opCentre.GetAgentBuildConfigs("Diplomat").AgentConfig))
                {
                    foreach (HexCell cell in player.exploredCells.FindAll(c => c.City))
                    {
                        if (!cell.City.Player)
                        {
                            opCentreBuilds.Add(opCentre.GetAgentBuildConfigs("Diplomat"));
                            break;
                        }
                    }
                }


                if (opCentre.GetAgentBuildConfigs().Count() > 0)
                {
                    IEnumerable<AgentBuildConfig> configs = opCentre.GetAgentBuildConfigs(new List<string>() { "Builder" });
                    List<AgentBuildConfig> buildableConfigs = new List<AgentBuildConfig>();
                    foreach(AgentBuildConfig config in configs)
                    {
                        if(player.PlayerAgentTracker.CanRecruit(config.AgentConfig))
                        {
                            buildableConfigs.Add(config);
                        }
                    }
                    if (buildableConfigs.Count() > 0)
                    {
                        opCentreBuilds.Add(IListExtensions.RandomElement(buildableConfigs));
                    }

                }
            }

            if (opCentre.IsConstructingBuilding() == false && opCentre.buildingSpaceAvailable())
            {
                IEnumerable<OpCentreBuildConfig> configs = opCentre.availableBuilds;
                if (configs.Count() > 0)
                {
                    opCentreBuilds.Add(opCentre.availableBuilds[UnityEngine.Random.Range(0, opCentre.availableBuilds.Count)]);
                }
            }


            //if (opCentre.GetCombatUnitBuildConfigs().Count() > 0)
            //{
            //    opCentreBuilds.Add(IListExtensions.RandomElement(opCentre.GetCombatUnitBuildConfigs()));
            //}
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
}
