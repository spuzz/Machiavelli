using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AIPlayer : Player
{
    [SerializeField] PlayerAIController playerAIController;
    public override void AddAgent(Agent agent)
    {
        base.AddAgent(agent);
        agent.GetComponent<AgentBehaviourTree>().StartTree();
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
