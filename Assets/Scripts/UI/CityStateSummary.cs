using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CityStateSummary : SearchableTableObject {

    [SerializeField] Image cityStateFlag;
    [SerializeField] TextMeshProUGUI pop;
    [SerializeField] TextMeshProUGUI happiness;
    [SerializeField] TextMeshProUGUI politicians;
    [SerializeField] TextMeshProUGUI units;
    [SerializeField] TextMeshProUGUI income;
    [SerializeField] TextMeshProUGUI production;
    [SerializeField] TextMeshProUGUI science;
    [SerializeField] TextMeshProUGUI capital;

    private City city;

    public void SetCity(City city)
    {
        GameController gameController = FindObjectOfType<GameController>();
        cityStateFlag.sprite = gameController.GetCityStateSymbol(city.GetCityState().SymbolID);
        pop.text = city.Population.ToString();
        happiness.text = city.CityResouceController.GetHappiness().ToString();
        politicians.text = city.GetCityState().TotalPoliticians().ToString();
        units.text = city.GetTotalUnits().ToString();
        income.text = city.GetIncomePerTurn().ToString();
        production.text = city.CityResouceController.GetProduction().ToString();
        science.text = city.CityResouceController.GetScience().ToString();
        capital.text = city.CityResouceController.GetPC().ToString();
        this.city = city;

    }

    public void SelectCity()
    {
        FindObjectOfType<HexGameUI>().SelectCity(city);
        FindObjectOfType<InfoButtonMenu>().ClosePanels();
    }
}
