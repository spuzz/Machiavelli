using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MercenaryRecruitmentUI : OperationCentreInfoPanel
{
    [SerializeField] List<Button> buttons;
    List<CombatUnitBuildConfig> mercBuildConfigs = new List<CombatUnitBuildConfig>();
    public override void UpdateUI(OperationCentre opCentre)
    {
        mercBuildConfigs.Clear();
        foreach (CombatUnitBuildConfig config in opCentre.GetCombatUnitBuildConfigs())
        {
            mercBuildConfigs.Add(config);
        }
        for (int a = 0; a < buttons.Count; a++)
        {
            if (a < mercBuildConfigs.Count)
            {
                buttons[a].gameObject.SetActive(true);
                buttons[a].interactable = true;
                buttons[a].image.sprite = mercBuildConfigs[a].CombatUnitConfig.Portrait;
            }
            else
            {
                buttons[a].gameObject.SetActive(false);
            }
        }
    }

    public void HireMercenary(int id)
    {
        opCentre.HireMercenary(mercBuildConfigs[id]);
    }
}
