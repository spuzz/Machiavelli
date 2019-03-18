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
            foreach(HexCell cell in player.exploredCells.FindAll(c => c.City))
            {
                if(!cell.City.PlayerBuildingControl.HasOutpost(player))
                {
                    buildList.Add(opCentre.Location, opCentre.GetAgentBuildConfigs("Builder"));
                    return;
                }
            }
            List<BuildConfig> opCentreBuilds = new List<BuildConfig>();
            if (opCentre.IsConstructingBuilding() == false && opCentre.buildingSpaceAvailable())
            {
                IEnumerable<OpCentreBuildConfig> configs = opCentre.availableBuilds;
                if (configs.Count() > 0)
                {
                    opCentreBuilds.Add(opCentre.availableBuilds[UnityEngine.Random.Range(0, opCentre.availableBuilds.Count)]);
                }
            }
            if(opCentre.GetAgentBuildConfigs().Count() > 0)
            {
                IEnumerable<AgentBuildConfig> configs = opCentre.GetAgentBuildConfigs(new List<string>() { "Builder" });
                if(configs.Count() > 0)
                {
                    opCentreBuilds.Add(IListExtensions.RandomElement(configs));
                }
                
            }

            if (opCentre.GetCombatUnitBuildConfigs().Count() > 0)
            {
                opCentreBuilds.Add(IListExtensions.RandomElement(opCentre.GetCombatUnitBuildConfigs()));
            }
            if(opCentreBuilds.Count > 0)
            {
                buildList.Add(opCentre.Location, opCentreBuilds[UnityEngine.Random.Range(0, opCentreBuilds.Count)]);
            }
            
        }
        
    }
}
