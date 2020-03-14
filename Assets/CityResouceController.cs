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

    [SerializeField] EffectsController effectsController;

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

    public EffectsController EffectsController
    {
        get
        {
            return effectsController;
        }

        set
        {
            effectsController = value;
        }
    }

    public void AddBuilding(CityBuilding building)
    {
        building.ResourceBenefit.EffectName = building.BuildConfig.Name;
        EffectsController.AddEffect(building.gameObject, building.ResourceBenefit);
        if (building.ResourceBenefit.Happiness != 0)
        {
            UpdateHappinessEffects();
        }
        city.NotifyInfoChange();
    }

    public void RemoveBuilding(CityBuilding building)
    {
        EffectsController.RemoveEffect(building.gameObject, building.ResourceBenefit.EffectName);
        if (building.ResourceBenefit.Happiness != 0)
        {
            UpdateHappinessEffects();
        }
        city.NotifyInfoChange();
    }

    public void AddEffect(GameObject obj, GameEffect effect)
    {
        EffectsController.AddEffect(obj, effect);
        if (effect.Happiness != 0)
        {
            UpdateHappinessEffects();
        }
        city.NotifyInfoChange();
    }


    public void RemoveEffect(GameObject obj, GameEffect gameEffect)
    {
        EffectsController.AddEffect(obj, gameEffect);
        if (gameEffect.Happiness != 0)
        {
            UpdateHappinessEffects();
        }
        city.NotifyInfoChange();
    }

    private void Start()
    {
        gameSettings = FindObjectOfType<GameSettings>();
        AddEffect(this.gameObject, gameSettings.Happiness.GetHappinessEffect(0));
    }

    private void UpdateHappinessEffects()
    {
        RemoveEffect(this.gameObject, gameSettings.Happiness.GetHappinessEffect(0));
        AddEffect(this.gameObject, gameSettings.Happiness.GetHappinessEffect(GetHappiness()));
    }

    public int GetProduction(FocusType focusType = 0)
    {

        float production = baseProduction;
        foreach(HexCell cell in city.GetWorkedCells())
        {
            production += cell.HexCellGameData.Production;
        }

        production += EffectsController.TotalEffects.Production.x;
        GameEffect.FocusBonus bonus = EffectsController.TotalEffects.FocusProductionBonus.Find(c => c.type == focusType);
        production += bonus.prodBonus.x;

        float bonusPerc = EffectsController.TotalEffects.Production.y;

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
        food += EffectsController.TotalEffects.Food.x;
        float bonusPerc = EffectsController.TotalEffects.Food.y;

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

        science += EffectsController.TotalEffects.Science.x;
        float bonusPerc = EffectsController.TotalEffects.Science.y;

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

        gold += EffectsController.TotalEffects.Gold.x;
        float bonusPerc = EffectsController.TotalEffects.Gold.y;

        bonusPerc = 1.0f + bonusPerc / 100.0f;
        gold = gold * bonusPerc;
        gold = (float)System.Math.Floor(gold);

        return (int)gold;

    }

    public int GetPC()
    {
        float pc = basePC;

        pc += EffectsController.TotalEffects.PoliticalCapital.x;
        float bonusPerc = EffectsController.TotalEffects.PoliticalCapital.y;

        bonusPerc = 1.0f + bonusPerc / 100.0f;
        pc = pc * bonusPerc;
        pc = (float)System.Math.Floor(pc);

        return (int)pc;


    }

    public int GetHappiness()
    {
        int happiness = baseHappiness;

        happiness += EffectsController.TotalEffects.Happiness;

        return happiness;
    }
}
