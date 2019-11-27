﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityResouceController : MonoBehaviour {


    [SerializeField] int baseProduction = 5;
    [SerializeField] int baseFood = 0;
    [SerializeField] int baseScience = 2;
    [SerializeField] int baseGold = 5;

    [SerializeField] City city;

    [SerializeField] ResourceBenefit resourceBenefits;

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

    public void AddBuilding(CityBuilding building)
    {
        ResourceBenefits.AddBenefit(building.ResourceBenefit);
    }

    public void RemoveBuilding(CityBuilding building)
    {
        ResourceBenefits.RemoveBenefit(building.ResourceBenefit);
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
}