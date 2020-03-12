using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySummaryTable : SearchableTable {


    private Player player;
    List<City> cities = new List<City>();

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

    public override void SortBy(int column = 0, bool ascending = false)
    {
        switch (column)
        {
            case 0:
                cities.Sort((x, y) => string.Compare(x.GetCityState().CityStateName, y.GetCityState().CityStateName));
                break;
            case 1:
                cities.Sort((x, y) => x.Population.CompareTo(y.Population));
                break;
            case 2:
                cities.Sort((x, y) => x.CityResouceController.GetHappiness().CompareTo(y.CityResouceController.GetHappiness()));
                break;
            case 3:
                cities.Sort((x, y) => x.GetCityState().PoliticiansByPlayer(x.GetCityState().Player).CompareTo(y.GetCityState().PoliticiansByPlayer(x.GetCityState().Player)));
                break;
            case 4:
                cities.Sort((x, y) => x.GetTotalUnits().CompareTo(y.GetTotalUnits()));
                break;
            case 5:
                cities.Sort((x, y) => x.GetIncomePerTurn().CompareTo(y.GetIncomePerTurn()));
                break;
            case 6:
                cities.Sort((x, y) => x.CityResouceController.GetProduction().CompareTo(y.CityResouceController.GetProduction()));
                break;
            case 7:
                cities.Sort((x, y) => x.CityResouceController.GetScience().CompareTo(y.CityResouceController.GetScience()));
                break;
            case 8:
                cities.Sort((x, y) => x.CityResouceController.GetPC().CompareTo(y.CityResouceController.GetPC()));
                break;
            default:
                cities.Sort((x, y) => string.Compare(x.GetCityState().CityStateName, y.GetCityState().CityStateName));
                break;
        }
        if(ascending == false)
        {
            cities.Reverse();
        }
        FillTable();
    }

    public void FillList(Player playerToList)
    {
        cities.Clear();
        player = playerToList;
        foreach (City city in player.cities)
        {
            cities.Add(city);
        }
    }

    public override void UpdateTableList()
    {
        Player player = FindObjectOfType<HumanPlayer>();
        FillList(player);
    }

    public override void FillTable(int column = 0, bool ascending = true)
    {

        ClearObjects();

        foreach(City city in cities)
        {
            CityStateSummary objectAdded = Instantiate(TableObjectPrefab, transform).GetComponent<CityStateSummary>();
            objectAdded.SetCity(city);
            SearchableTableObjects.Add(objectAdded);
        }
    }

}
