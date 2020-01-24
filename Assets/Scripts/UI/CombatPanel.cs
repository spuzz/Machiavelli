using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatPanel : MonoBehaviour {

    List<HexCell> ownerCombatCells = new List<HexCell>();
    List<HexCell> targetCombatCells = new List<HexCell>();
    HexCell owner;
    HexCell target;
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI attackerText;
    [SerializeField] TextMeshProUGUI defenderText;
    [SerializeField] TextMeshProUGUI estimatedBattleOutcomeText;
    [SerializeField] Image estimatedBattleOutcome;

    public void Activate(HexCell owner, HexCell target)
    {

        this.owner = owner;
        this.target = target;
        ClearCells();
        Combat combat = CombatSystem.Fight(owner, target, false);
        foreach (HexUnit unit in combat.attackSupport)
        {
            ownerCombatCells.Add(unit.Location);
            unit.Location.EnableHighlight(Color.cyan);
        }

        attackerText.text = combat.attackerStrength.ToString();

        foreach(HexUnit unit in combat.defendSupport)
        {
            targetCombatCells.Add(unit.Location);
            unit.Location.EnableHighlight(Color.magenta);
        }
        defenderText.text = combat.defenderStrength.ToString();

        switch (combat.likely_outcome)
        {
            case Combat.BATTLE_LIKELY_OUTCOME.CERTAIN_VICTORY:
                estimatedBattleOutcomeText.text = "Certain Victory";
                estimatedBattleOutcome.color = Color.green;
                break;
            case Combat.BATTLE_LIKELY_OUTCOME.CLOSE_VICTORY:
                estimatedBattleOutcomeText.text = "Close Victory";
                estimatedBattleOutcome.color = Color.yellow;
                break;
            case Combat.BATTLE_LIKELY_OUTCOME.CLOSE_DEFEAT:
                estimatedBattleOutcomeText.text = "Close Defeat";
                estimatedBattleOutcome.color = new Color(1, 0.6f, 0.0f);
                break;
            case Combat.BATTLE_LIKELY_OUTCOME.CERTAIN_DEFEAT:
                estimatedBattleOutcomeText.text = "Certain Defeat";
                estimatedBattleOutcome.color = Color.red;
                break;
        }

        panel.SetActive(true);
    }

    public void Deactivate(List<HexCell> path)
    {
        ClearCells(path);

        panel.SetActive(false);
    }

    private void ClearCells(List<HexCell> path = null)
    {
        if(path == null)
        {
            path = new List<HexCell>();
        }

        foreach (HexCell cell in ownerCombatCells)
        {
            if (cell != owner && !path.Contains(cell))
            {
                cell.DisableHighlight();
            }
        }

        foreach (HexCell cell in targetCombatCells)
        {
            if (cell != target && !path.Contains(cell))
            {
                cell.DisableHighlight();
            }
        }

        ownerCombatCells.Clear();
        targetCombatCells.Clear();
    }
}
