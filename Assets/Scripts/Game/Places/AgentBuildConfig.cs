using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = ("Build/AgentBuildConfig"))]
public class AgentBuildConfig : BuildConfig
{
    [Header("Agent Build Config Specific")]
    [SerializeField] AgentConfig agentConfig;


    public AgentConfig AgentConfig
    {
        get
        {
            return agentConfig;
        }

        set
        {
            agentConfig = value;
        }
    }

    public override BUILDTYPE GetBuildType()
    {
        return BUILDTYPE.AGENT;
    }
}

