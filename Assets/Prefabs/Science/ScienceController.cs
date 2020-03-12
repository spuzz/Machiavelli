using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScienceController : MonoBehaviour {

    [SerializeField] Player player;
    List<Research> finishedResearch = new List<Research>();

    Research currentResearch;
    int overflowScience;
    public Research CurrentResearch
    {
        get
        {
            return currentResearch;
        }

        set
        {
            currentResearch = value;
        }
    }

    public IEnumerable<Research> FinishedResearch
    {
        get
        {
            return finishedResearch;
        }

    }
    public int GetTotalReseached()
    {
        return finishedResearch.Count;
    }

    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
        }
    }

    public void AddFinishedResearch(Research research)
    {
        finishedResearch.Add(research);
        Player.AddResearch(research);
    }
    public void StartTurn()
    {
        if(currentResearch)
        {
            currentResearch.AddProgress(player.GetScience());
            if(currentResearch.Finished)
            {
                overflowScience = currentResearch.OverflowScience;
                AddFinishedResearch(currentResearch);
                currentResearch = null;
            }
        }
    }

    public void SelectResearch(Research research)
    {
        currentResearch = research;
        player.NotifyInfoChange();
    }

    public int GetTierCost(ResearchTier tier)
    {
        return 10;
    }

}
