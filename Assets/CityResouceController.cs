using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityResouceController : MonoBehaviour {

    [SerializeField] int baseProduction = 5;
    [SerializeField] int baseFood = 0;
    [SerializeField] int baseScience = 2;
    [SerializeField] int baseGold = 5;
    [SerializeField] int basePC = 0;
    [SerializeField] int baseHappiness = 0;

    [SerializeField] City city;

    [SerializeField] ResourceBenefit resourceBenefits;
    Dictionary<string, ResourceBenefit> combinedBenefits;

    GameSettings gameSettings;

    public int BaseProduction
    {
        get
        {
            return baseProduction;
        }

        set
        {
            baseProduction = value;
        }
    }

    public int BaseFood
    {
        get
        {
            return baseFood;
        }

        set
        {
            baseFood = value;
        }
    }

    public int BaseScience
    {
        get
        {
            return baseScience;
        }

        set
        {
            baseScience = value;
        }
    }

    public int BaseGold
    {
        get
        {
            return baseGold;
        }

        set
        {
            baseGold = value;
        }
    }

    public ResourceBenefit ResourceBenefits
    {
        get
        {
            return resourceBenefits;
        }

        set
        {
            resourceBenefits = value;
        }
    }

    public int BaseHappiness
    {
        get
        {
            return baseHappiness;
        }

        set
        {
            baseHappiness = value;
        }
    }

    public void AddBuilding(CityBuilding building)
    {
        ResourceBenefits.AddBenefit(building.ResourceBenefit);
        combinedBenefits.Add(building.BuildConfig.Name, building.ResourceBenefit);
        if (building.ResourceBenefit.Happiness != 0)
        {
            UpdateHappinessEffects();
        }
        city.NotifyInfoChange();
    }

    public void RemoveBuilding(CityBuilding building)
    {
        ResourceBenefits.RemoveBenefit(building.ResourceBenefit);
        if (combinedBenefits.ContainsKey(building.BuildConfig.Name))
        {
            combinedBenefits.Remove(building.BuildConfig.Name);
        }
        if (building.ResourceBenefit.Happiness != 0)
        {
            UpdateHappinessEffects();
        }
        city.NotifyInfoChange();
    }

    public void AddEffect(string effectName, ResourceBenefit benefit)
    {
        resourceBenefits.AddBenefit(benefit);
        combinedBenefits.Add(effectName, benefit);
        if (benefit.Happiness != 0)
        {
            UpdateHappinessEffects();
        }
        city.NotifyInfoChange();
    }


    public void RemoveEffect(string effectName)
    {
        if(combinedBenefits.ContainsKey(effectName))
        {
            ResourceBenefit effect = combinedBenefits[effectName];
            resourceBenefits.RemoveBenefit(effect);
            combinedBenefits.Remove(effectName);
            if(effect.Happiness != 0)
            {
                UpdateHappinessEffects();
            }
            city.NotifyInfoChange();
        }
    }

    private void Start()
    {
        gameSettings = FindObjectOfType<GameSettings>();
        combinedBenefits = new Dictionary<string, ResourceBenefit>();
        AddEffect("Happiness", gameSettings.Happiness.GetHappinessEffect(0));
    }

    private void UpdateHappinessEffects()
    {
        RemoveEffect("Happiness");
        AddEffect("Happiness", gameSettings.Happiness.GetHappinessEffect(GetHappiness()));
    }

    public int GetProduction(FocusType focusType = 0)
    {

        float production = baseProduction;
        foreach(HexCell cell in city.GetWorkedCells())
        {
            production += cell.HexCellGameData.Production;
        }

        production += ResourceBenefits.Production.x;
        ResourceBenefit.FocusBonus bonus = ResourceBenefits.FocusProductionBonus.Find(c => c.type == focusType);
        production += bonus.prodBonus.x;

        float bonusPerc = ResourceBenefits.Production.y;

        bonusPerc += bonus.prodBonus.y;

        bonusPerc = 1.0f + bonusPerc / 100.0f;
        production = production * bonusPerc;
        production = (float)System.Math.Floor(production);


        return (int)production;
    }

    public int GetFood()
    {
        float food = baseFood;
        foreach (HexCell cell in city.GetWorkedCells())
        {
            food += cell.HexCellGameData.Food;
        }
        food += ResourceBenefits.Food.x;
        float bonusPerc = ResourceBenefits.Food.y;

        bonusPerc = 1.0f + bonusPerc / 100.0f;
        food = food * bonusPerc;
        food = (float)System.Math.Floor(food);


        return (int)food;

    }

    public int GetScience()
    {

        float science = BaseScience;
        foreach (HexCell cell in city.GetWorkedCells())
        {
            science += cell.HexCellGameData.Science;
        }

        science += ResourceBenefits.Science.x;
        float bonusPerc = ResourceBenefits.Science.y;

        bonusPerc = 1.0f + bonusPerc / 100.0f;
        science = science * bonusPerc;
        science = (float)System.Math.Floor(science);

        return (int)science;
    }

    public int GetGold()
    {
        float gold = baseGold;
        foreach (HexCell cell in city.GetWorkedCells())
        {
            gold += cell.HexCellGameData.Gold;
        }

        gold += ResourceBenefits.Gold.x;
        float bonusPerc = ResourceBenefits.Gold.y;

        bonusPerc = 1.0f + bonusPerc / 100.0f;
        gold = gold * bonusPerc;
        gold = (float)System.Math.Floor(gold);

        return (int)gold;

    }

    public int GetPC()
    {
        float pc = basePC;

        pc += ResourceBenefits.PoliticalCapital.x;
        float bonusPerc = ResourceBenefits.PoliticalCapital.y;

        bonusPerc = 1.0f + bonusPerc / 100.0f;
        pc = pc * bonusPerc;
        pc = (float)System.Math.Floor(pc);

        return (int)pc;


    }

    public int GetHappiness()
    {
        int happiness = baseHappiness;

        happiness += ResourceBenefits.Happiness;

        return happiness;
    }
}
