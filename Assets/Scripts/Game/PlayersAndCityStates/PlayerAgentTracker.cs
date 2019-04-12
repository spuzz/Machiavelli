using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgentTracker : MonoBehaviour {

    [SerializeField] List<AgentConfig> configsWithcaps;
    [SerializeField] List<AgentConfig> allConfigs;
    List<PlayerAgentCap> playerAgentCaps = new List<PlayerAgentCap>();
    private void Awake()
    {
        foreach(AgentConfig agent in configsWithcaps)
        {
            PlayerAgentCap cap = new PlayerAgentCap();
            cap.Config = agent;
            cap.Cap = 1;
            cap.Current = 0;
            playerAgentCaps.Add(cap);
        }

        foreach (AgentConfig agent in allConfigs)
        {
            if(playerAgentCaps.Find(c => c.Config == agent) == null)
            {
                PlayerAgentCap cap = new PlayerAgentCap();
                cap.Config = agent;
                cap.Cap = -1;
                cap.Current = 0;
                playerAgentCaps.Add(cap);
            }

        }
    }
    public void AddAgent(AgentConfig agentConfig)
    {
        PlayerAgentCap cap = playerAgentCaps.Find(c => c.Config == agentConfig);
        if (cap != null)
        {
            cap.Current += 1;
        }
    }

    public void RemoveAgent(AgentConfig agentConfig)
    {
        PlayerAgentCap cap = playerAgentCaps.Find(c => c.Config == agentConfig);
        if (cap != null)
        {
            cap.Current -= 1;
        }
    }

    public void IncreaseCap(AgentConfig agentConfig, int increaseBy = 1)
    {
        PlayerAgentCap cap = playerAgentCaps.Find(c => c.Config == agentConfig);
        if (cap != null)
        {
            cap.Cap += increaseBy;
        }
    }

    public void DecreaseCap(AgentConfig agentConfig, int decreaseBy = 1)
    {
        PlayerAgentCap cap = playerAgentCaps.Find(c => c.Config == agentConfig);
        if (cap != null && cap.Cap > 0)
        {
            cap.Cap -= decreaseBy;
        }
    }

    public int CurrentCap(AgentConfig agentConfig)
    {
        PlayerAgentCap cap = playerAgentCaps.Find(c => c.Config == agentConfig);
        if (cap != null)
        {
            return cap.Cap;
        }
        else
        {
            return -1;
        }
    }
    public int CurrentUsage(AgentConfig agentConfig)
    {
        PlayerAgentCap cap = playerAgentCaps.Find(c => c.Config == agentConfig);
        if (cap != null)
        {
            return cap.Current;
        }
        else
        {
            return -1;
        }
    }

    public bool CanRecruit(AgentConfig agentConfig)
    {
        PlayerAgentCap cap = playerAgentCaps.Find(c => c.Config == agentConfig);
        if (cap == null)
        {
            return true;
        }
        if(cap.Cap == -1 || cap.Current < cap.Cap)
        {
            return true;
        }
        return false;
    }

    public IEnumerable<AgentConfig> GetCappedAgents()
    {
        return configsWithcaps;
    }

    public IEnumerable<AgentConfig> GetAllAgents()
    {
        return allConfigs;
    }


    public AgentConfig GetRandomCappedAgent()
    {
        return configsWithcaps[UnityEngine.Random.Range(0, configsWithcaps.Count)];
    }

}
