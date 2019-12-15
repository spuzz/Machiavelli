using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightPanel : MonoBehaviour {

    [SerializeField] HumanPlayer player;

    [SerializeField] ExistingUnit existingUnitPrefab;
    List<ExistingUnit> existingUnits = new List<ExistingUnit>();
    [SerializeField] GameObject existingUnitsContent;

    private void Awake()
    {
        player.onInfoChange += UpdateUI;
    }

    private void UpdateUI(Player player)
    {
        foreach (ExistingUnit panel in existingUnits)
        {
            Destroy(panel.gameObject);
        }

        existingUnits.Clear();

        foreach (Agent unit in player.GetAgents())
        {
            AddExisting(unit);
        }
    }

    public void AddExisting(Agent unit)
    {
        ExistingUnit panel = Instantiate(existingUnitPrefab, existingUnitsContent.transform);
        panel.Unit = unit;
        panel.OptionImage.sprite = unit.GetAgentConfig().Portrait;
        panel.OptionName.text = unit.GetAgentConfig().Name;
        existingUnits.Add(panel);
    }
}
