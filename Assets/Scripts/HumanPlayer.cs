using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HumanPlayer : Player {

    private void Awake()
    {
        IsHuman = true;
    }
    public override void Save(BinaryWriter writer)
    {
        writer.Write(agents.Count);
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].Save(writer);
        }
    }

    public void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {
        ClearAgents();
        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            Agent agent = Agent.Load(reader, hexGrid, header);
            agent.HexUnit.Visible = true;
            agent.HexUnit.Controllable = true;
            AddAgent(agent);
        }
    }

    public override void PlayerDefeated()
    {
        Alive = false;
    }
}
