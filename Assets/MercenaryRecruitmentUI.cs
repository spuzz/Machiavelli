using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MercenaryRecruitmentUI : OperationCentreInfoPanel
{
    [SerializeField] List<Button> buttons;
    List<CombatUnitConfig> mercConfigs = new List<CombatUnitConfig>();
    OperationCentre opCentre;
    public override void UpdateUI(OperationCentre opCentre)
    {
        this.opCentre = opCentre;
        if (isActiveAndEnabled)
        {

            if (opCentre)
            {
                mercConfigs.Clear();
                foreach (CombatUnitConfig config in opCentre.GetCombatUnitConfigs())
                {
                    mercConfigs.Add(config);
                }
                for (int a = 0; a < buttons.Count; a++)
                {
                    if (a < mercConfigs.Count)
                    {
                        buttons[a].gameObject.SetActive(true);
                        buttons[a].interactable = true;
                        buttons[a].image.sprite = mercConfigs[a].Portrait;
                    }
                    else
                    {
                        buttons[a].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void HireMercenary(int id)
    {
        opCentre.HireMercenary(mercConfigs[id]);
    }
}
