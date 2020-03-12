﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceBenefit : MonoBehaviour
{
    [System.Serializable]
    public struct FocusBonus
    {
        public FocusType type;
        public Vector2 prodBonus;
        public int percCostReduction;
        public int maintenanceReduction;
    }
    [SerializeField] Vector2 production;
    [SerializeField] Vector2 politicalCapital;
    [SerializeField] Vector2 gold;
    [SerializeField] Vector2 science;
    [SerializeField] Vector2 food;
    [SerializeField] Vector2 defence;
    [SerializeField] List<FocusBonus> focusProductionBonus;
    [SerializeField] int visionRange;
    [SerializeField] int happiness;
    [SerializeField] int loyalty;

    public Vector2 Production
    {
        get
        {
            return production;
        }

        set
        {
            production = value;
        }
    }

    public Vector2 Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
        }
    }

    public Vector2 Science
    {
        get
        {
            return science;
        }

        set
        {
            science = value;
        }
    }

    public Vector2 Food
    {
        get
        {
            return food;
        }

        set
        {
            food = value;
        }
    }

    public Vector2 Defence
    {
        get
        {
            return defence;
        }

        set
        {
            defence = value;
        }
    }

    public Vector2 PoliticalCapital
    {
        get
        {
            return politicalCapital;
        }

        set
        {
            politicalCapital = value;
        }
    }

    public List<FocusBonus> FocusProductionBonus
    {
        get
        {
            return focusProductionBonus;
        }

        set
        {
            focusProductionBonus = value;
        }
    }

    public int VisionRange
    {
        get
        {
            return visionRange;
        }

        set
        {
            visionRange = value;
        }
    }

    public int Happiness
    {
        get
        {
            return happiness;
        }

        set
        {
            happiness = value;
        }
    }

    public int Loyalty
    {
        get
        {
            return loyalty;
        }

        set
        {
            loyalty = value;
        }
    }

    public void AddBenefit(ResourceBenefit benefit)
    {
        Gold += benefit.Gold;
        Food += benefit.Food;
        Production += benefit.Production;
        Science += benefit.Science;
        Defence += benefit.Defence;
        PoliticalCapital += benefit.PoliticalCapital;
        VisionRange += benefit.VisionRange;
        Happiness += benefit.Happiness;
        Loyalty += benefit.Loyalty;

        foreach (FocusBonus bonus in benefit.FocusProductionBonus)
        {
            IEnumerable<FocusBonus> bonuses = focusProductionBonus.Where(c => c.type == bonus.type);
            if (bonuses.Count() > 0)
            {
                FocusBonus updateBonus = bonuses.First();
                updateBonus.prodBonus += bonus.prodBonus;
                updateBonus.maintenanceReduction += bonus.maintenanceReduction;
                updateBonus.percCostReduction += bonus.percCostReduction;
            }
            else
            {
                focusProductionBonus.Add(bonus);
            }
        }
        
    }

    public void RemoveBenefit(ResourceBenefit benefit)
    {
        Gold -= benefit.Gold;
        Food -= benefit.Food;
        Production -= benefit.Production;
        Science -= benefit.Science;
        Defence -= benefit.Defence;
        PoliticalCapital -= benefit.PoliticalCapital;
        VisionRange -= benefit.VisionRange;
        Happiness -= benefit.happiness;
        Loyalty -= benefit.Loyalty;

        foreach (FocusBonus bonus in benefit.FocusProductionBonus)
        {
            IEnumerable<FocusBonus> bonuses = focusProductionBonus.Where(c => c.type == bonus.type);
            if (bonuses.Count() > 0)
            {
                FocusBonus updateBonus = bonuses.First();
                updateBonus.prodBonus -= bonus.prodBonus;
                updateBonus.maintenanceReduction -= bonus.maintenanceReduction;
                updateBonus.percCostReduction -= bonus.percCostReduction;
            }
        }
    }


    public void ResetBenefit()
    {
        Gold = new Vector2(0, 0);
        Food = new Vector2(0, 0);
        Production = new Vector2(0, 0);
        Science = new Vector2(0, 0);
        Defence = new Vector2(0, 0);
        PoliticalCapital = new Vector2(0, 0);
        VisionRange = 0;
        Happiness = 0;
        Loyalty = 0;
    }
}
