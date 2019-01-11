using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AIPlayer : Player
{
    

    public void AddAgent()
    {

    }
    public override void Save(BinaryWriter writer)
    {
        writer.Write(PlayerNumber);
        writer.Write(agents.Count);
        foreach(Agent agent in agents)
        {
            agent.Save(writer);
        }
    }

    public static void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {
        AIPlayer instance = gameController.CreateAIPlayer();
        instance.PlayerNumber = reader.ReadInt32();
        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            Agent agent = Agent.Load(reader, hexGrid, header);
            agent.HexUnit.Visible = false;
            instance.AddAgent(agent);
        }
        
    }
}
