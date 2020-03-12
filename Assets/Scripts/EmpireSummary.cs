using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
public class EmpireSummary : MonoBehaviour {

    [SerializeField] TextMeshProUGUI cityStates;
    [SerializeField] TextMeshProUGUI agents;
    [SerializeField] TextMeshProUGUI falteringPoliticians;
    [SerializeField] TextMeshProUGUI loyalPoliticians;
    [SerializeField] TextMeshProUGUI units;
    [SerializeField] TextMeshProUGUI happyCities;
    [SerializeField] TextMeshProUGUI unhappyCities;
    [SerializeField] TextMeshProUGUI researchedTech;
    [SerializeField] TextMeshProUGUI income;

    HumanPlayer player;

    private void OnEnable()
    {
        player = FindObjectOfType<HumanPlayer>();
        cityStates.text = player.GetTotalCities().ToString();
        agents.text = player.agents.Count.ToString();
        falteringPoliticians.text = player.GetFalteringPoliticians().ToString();
        loyalPoliticians.text = player.GetLoyalPoliticians().ToString();
        units.text = player.GetTotalUnits().ToString();
        happyCities.text = player.GetHappyCities().ToString();
        unhappyCities.text = player.GetUnhappyCities().ToString();
        researchedTech.text = player.ScienceController.GetTotalReseached().ToString();
        income.text = player.GoldPerTurn.ToString();
    }
}
