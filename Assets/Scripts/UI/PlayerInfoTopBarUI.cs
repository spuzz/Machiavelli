using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoTopBarUI : MonoBehaviour {

    [SerializeField] Text goldText;
    [SerializeField] Text scienceText;
    [SerializeField] Text politicalText;
    Player player;
    GameController gameController;

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        player = gameController.GetPlayer(0);
        player.onInfoChange += playerChanged;
    }


    private void playerChanged(Player player)
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        goldText.text = player.Gold.ToString() + "(+" + player.GoldPerTurn.ToString() + ")";
        scienceText.text = "+" + player.GetScience().ToString();
        politicalText.text = player.PoliticalCapital.ToString() ;
    }
}
