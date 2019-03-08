using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAIController : MonoBehaviour
{

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

    }

}
