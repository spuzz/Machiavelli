using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SciencePanel : MonoBehaviour {

    [SerializeField] List<ResearchPathPanel> researchPathPanels;
    [SerializeField] Player player;
    public void OnEnable()
    {
        player.onInfoChange += UpdateUI;
        UpdateUI(player);
    }

    public void OnDisable()
    {
        player.onInfoChange -= UpdateUI;
    }
    public void UpdateUI(Player player)
    {
        foreach(ResearchPathPanel panel in researchPathPanels)
        {
            panel.UpdatePanel();
        }
    }
}
