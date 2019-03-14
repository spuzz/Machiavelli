using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HumanPlayer : Player {

    private void Awake()
    {
        IsHuman = true;
        gameController = FindObjectOfType<GameController>();
    }

    public override void PlayerDefeated()
    {
        Alive = false;
    }

    public override void Save(BinaryWriter writer)
    {
        writer.Write(ColorID);
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
        if (header >= 6)
        {
            ColorID = gameController.GetNewPlayerColor(reader.ReadInt32());
        }
        else
        {
            ColorID = gameController.GetNewPlayerColor();
        }
        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            Agent agent = Agent.Load(reader, gameController, hexGrid, header, this);
        }
        ClearExploredCells();
        ClearOperationCentres();
        LoadPlayer(reader, gameController, hexGrid, header);
    }



}
