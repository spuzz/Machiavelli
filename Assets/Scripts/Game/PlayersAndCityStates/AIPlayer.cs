using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AIPlayer : Player
{
    [SerializeField] PlayerAIController playerAIController;
    protected List<CityState> friendlyCityStates = new List<CityState>();
    protected List<CityState> enemyCityStates = new List<CityState>();
    public override void AddAgent(Agent agent)
    {
        base.AddAgent(agent);
        agent.GetComponent<AgentBehaviourTree>().StartTree();
    }

    public override void AddVisibleCell(HexCell cell)
    {
        base.AddVisibleCell(cell);
        UpdateFriendlyCityStates();
    }

    private void UpdateFriendlyCityStates()
    {
        foreach(CityState state in cityStatesMet)
        {
            if (!friendlyCityStates.Contains(state) && !enemyCityStates.Contains(state))
            {
                if (friendlyCityStates.Count == 0)
                {
                    friendlyCityStates.Add(state);
                }
                else
                {
                    if (UnityEngine.Random.value < .2)
                    {
                        friendlyCityStates.Add(state);
                    }
                    else
                    {
                        enemyCityStates.Add(state);
                    }
                }
            }
        }
    }

    public IEnumerator TakeTurn()
    {
        agents.RemoveAll(c => c.Alive == false);
        yield return StartCoroutine(playerAIController.UpdateUnits(agents));
        gameController.PlayerTurnFinished(this);
    }
    public override void PlayerDefeated()
    {
        Alive = false;
    }

    public override bool IsCityStateFriendly(CityState state)
    {
        if(friendlyCityStates.Contains(state))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void Save(BinaryWriter writer)
    {
        writer.Write(ColorID);
        writer.Write(PlayerNumber);
        writer.Write(agents.Count);
        foreach (Agent agent in agents)
        {
            agent.Save(writer);
        }
        writer.Write(exploredCells.Count);
        for (int i = 0; i < exploredCells.Count; i++)
        {
            writer.Write(exploredCells[i].Index);
        }
        SavePlayer(writer);
    }

    public static void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {
        AIPlayer instance;
        if (header >= 6)
        {
            instance = gameController.CreateAIPlayer(reader.ReadInt32());
        }
        else
        {
            instance = gameController.CreateAIPlayer();
        }
        instance.PlayerNumber = reader.ReadInt32();
        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            Agent agent = Agent.Load(reader, gameController, hexGrid, header, instance);
            instance.AddAgent(agent);
        }
        instance.LoadPlayer(reader, gameController, hexGrid, header);

    }
}
