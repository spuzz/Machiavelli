using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpCentreAgentBuildMod {

    int costPerc;
    int cap;
    int xpPerc;
    AgentBuildConfig agentBuildConfig;

    public OpCentreAgentBuildMod(OpCentreAgentBuildModConfig config)
    {
        AddMod(config);
    }
    public int CostPerc
    {
        get
        {
            return costPerc;
        }

        set
        {
            costPerc = value;
        }
    }

    public int XpPerc
    {
        get
        {
            return xpPerc;
        }

        set
        {
            xpPerc = value;
        }
    }

    public AgentBuildConfig AgentBuildConfig
    {
        get
        {
            return agentBuildConfig;
        }

        set
        {
            agentBuildConfig = value;
        }
    }

    public int Cap
    {
        get
        {
            return cap;
        }

        set
        {
            cap = value;
        }
    }

    public void AddMod(OpCentreAgentBuildModConfig config)
    {
        if(!agentBuildConfig)
        {
            agentBuildConfig = config.AgentBuildConfig;
        }
        else if(config.AgentBuildConfig != agentBuildConfig)
        {
            throw new ArgumentException("Invalid Config");
        }

        costPerc += config.CostReductionPerc;
        if(costPerc > 100)
        {
            costPerc = 100;
        }
        xpPerc += config.XpIncreasePerc;
        cap += config.CapIncrease;
    }

    public void RemoveMod(OpCentreAgentBuildModConfig config)
    {
        if (config.AgentBuildConfig != agentBuildConfig)
        {
            throw new ArgumentException("Invalid Config");
        }

        costPerc -= config.CostReductionPerc;
        xpPerc -= config.XpIncreasePerc;
        cap -= config.CapIncrease;
    }
}
