using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HumanPlayer : Player {

    private void Awake()
    {
        IsHuman = true;
    }

    public override void PlayerDefeated()
    {
        Alive = false;
    }

    public override void Save(BinaryWriter writer)
    {
        writer.Write(agents.Count);
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].Save(writer);
        }
        writer.Write(exploredCells.Count);
        for (int i = 0; i < exploredCells.Count; i++)
        {
            writer.Write(exploredCells[i].Index);
        }
        SavePlayer(writer);
    }

    public void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {
        ClearAgents();
        gameController.ReturnPlayerColor(Color);
        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            Agent agent = Agent.Load(reader, hexGrid, header);
            agent.HexUnit.Visible = true;
            agent.HexUnit.Controllable = true;
            AddAgent(agent);
        }
        ClearExploredCells();
        ClearOperationCentres();
        LoadPlayer(reader, gameController, hexGrid, header);
    }



}
