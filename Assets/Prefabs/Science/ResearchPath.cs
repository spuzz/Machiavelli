using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ResearchPathType
{
    Military,
    Infrastructure,
    Agents,
    Science,
    Economy
}

[System.Serializable]
public class TierList
{
    [SerializeField] List<Research> research;

    public List<Research> Research
    {
        get
        {
            return research;
        }

        set
        {
            research = value;
        }
    }
}
public class ResearchPath : MonoBehaviour {

    public static int MIN_PREVIOUSTIER_REQ = 2;

    [SerializeField] ScienceController scienceController;
    [SerializeField] ResearchPathType researchPathType;
    [SerializeField] Research baseResearch;
    [SerializeField] List<TierList> researchTiers;

    List<Research> finishedResearch = new List<Research>();

    public Research BaseResearch
    {
        get
        {
            return baseResearch;
        }

        set
        {
            baseResearch = value;
        }
    }

    public bool isTierAvailable(Research research)
    {
        int previousTier = (int)research.ResearchTier - 1;
        if(previousTier < 0)
        {
            return true;
        }
        if(previousTier == 0)
        {
            if(baseResearch.Finished == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        if (!Enum.IsDefined(researchPathType.GetType(), previousTier))
        {
            throw new InvalidOperationException("Invalid Tier");
        }
        else
        {
            ResearchTier tier = (ResearchTier)previousTier;
            if(finishedResearch.FindAll(c => c.ResearchTier == tier).Count < MIN_PREVIOUSTIER_REQ)
            {
                return false;
            }
        }
        return true;
    }

    public List<TierList> GetResearchTiers()
    {
        return researchTiers;
    }
    public void SelectResearch(int tier, int number)
    {
        scienceController.SelectResearch(researchTiers[tier].Research[number]);
    }
}
