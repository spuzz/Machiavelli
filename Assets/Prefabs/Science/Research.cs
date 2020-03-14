using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResearchTier
{
    Base,
    Tier1,
    Tier2,
    Tier3,
    Tier4,
    Tier5
}

public class Research : MonoBehaviour {
    [SerializeField] ResearchTier researchTier;
    [SerializeField] ScienceController scienceController;
    [SerializeField] string researchName;
    [SerializeField] GameEffect resourceBenefit;
    [SerializeField] List<CityPlayerBuildConfig> buildingOptions;
    [SerializeField] List<AgentBuildConfig> agentTrainingOptions;
    [SerializeField] List<CombatUnitBuildConfig> combatUnitTrainingOptions;
    [SerializeField] List<Research> prerequisites;

    int progress;
    bool finished;

    int overFlowScience = 0;
    public ResearchTier ResearchTier
    {
        get
        {
            return researchTier;
        }

        set
        {
            researchTier = value;
        }
    }

    public IEnumerable<CityPlayerBuildConfig> GetBuildingConfigs()
    {
        return buildingOptions;
    }

    public IEnumerable<AgentBuildConfig> GetAgentConfigs()
    {
        return agentTrainingOptions;
    }

    public IEnumerable<CombatUnitBuildConfig> GetCombatUnitConfigs()
    {
        return combatUnitTrainingOptions;
    }
    public int Progress
    {
        get
        {
            return progress;
        }

    }

    public bool Finished
    {
        get
        {
            return finished;
        }

    }

    public int OverflowScience
    {
        get
        {
            return overFlowScience;
        }

    }

    public string ResearchName
    {
        get
        {
            return researchName;
        }

        set
        {
            researchName = value;
        }
    }

    public void AddProgress(int addScience)
    {
        progress += addScience;
        int tierCost = scienceController.GetTierCost(ResearchTier);
        if (progress >= tierCost)
        {
            finished = true;
            overFlowScience = progress - tierCost;
        }
    }

    public int TimeRemaining(int sciencePerTurn)
    {
        int scienceLeft = scienceController.GetTierCost(ResearchTier) - progress;
        int turns = (scienceLeft + sciencePerTurn - 1) / sciencePerTurn;
        return turns;
    }

    public void selectResearch()
    {
        scienceController.SelectResearch(this);
    }

}
