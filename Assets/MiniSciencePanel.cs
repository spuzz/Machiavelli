using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniSciencePanel : MonoBehaviour {

    [SerializeField] TextMeshProUGUI researchName;
    [SerializeField] TextMeshProUGUI timeRemaining;
    [SerializeField] Image researchImage;

    [SerializeField] Player player;

    private void Awake()
    {
        player.onInfoChange += PlayerUpdated;
    }

    private void PlayerUpdated(Player player)
    {
        Research research = player.ScienceController.CurrentResearch;
        if(research)
        {
            researchName.text = research.ResearchName;
            timeRemaining.text = "Time Remaining: " + research.TimeRemaining(player.GetScience()).ToString();
        }
        else
        {
            researchName.text = "";
            timeRemaining.text = "";
        }
    }
}
