using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpCentreBuilding : MonoBehaviour {
    [SerializeField] ResourceBenefit resourceBenefit;
    OperationCentre opCentreBuiltIn;
    OpCentreBuildConfig buildConfig;

    [SerializeField] List<OpCentreBuildConfig> allowedBuilds;
    [SerializeField] List<AgentBuildConfig> allowedAgents;
    [SerializeField] List<CombatUnitBuildConfig> allowedMercs;
    public OperationCentre OpCentreBuiltIn
    {
        get
        {
            return opCentreBuiltIn;
        }

        set
        {
            opCentreBuiltIn = value;
            if(opCentreBuiltIn)
            {
                AddConfigs();
            }
            
        }
    }

    public void AddConfigs()
    {
        foreach(OpCentreBuildConfig config in allowedBuilds)
        {
            opCentreBuiltIn.AddOpCentreBuildConfigs(config);
        }
        foreach (AgentBuildConfig config in allowedAgents)
        {
            opCentreBuiltIn.AddAgentBuildConfigs(config);
        }
        foreach (CombatUnitBuildConfig config in allowedMercs)
        {
            opCentreBuiltIn.AddCombatUnitBuildConfigs(config);
        }
    }

    public ResourceBenefit ResourceBenefit
    {
        get
        {
            return resourceBenefit;
        }

        set
        {
            resourceBenefit = value;
        }
    }

    public OpCentreBuildConfig BuildConfig
    {
        get
        {
            return buildConfig;
        }

        set
        {
            buildConfig = value;
        }
    }

}
