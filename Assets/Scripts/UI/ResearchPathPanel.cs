using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchPathPanel : MonoBehaviour {

    [SerializeField] ScienceController scienceController;
    [SerializeField] ResearchPath researchPath;
    [SerializeField] Button researchPathBaseButton;
    [SerializeField] List<Button> tier1Buttons;
    [SerializeField] List<Button> tier2Buttons;
    [SerializeField] List<Button> tier3Buttons;
    [SerializeField] List<Button> tier4Buttons;
    [SerializeField] List<Button> tier5Buttons;

    public void Start()
    {
        UnityEngine.Events.UnityAction action = () => { SelectResearch(researchPath.BaseResearch); };
        researchPathBaseButton.onClick.AddListener(action);//find the button and set
        researchPathBaseButton.GetComponentInChildren<Text>().text = researchPath.BaseResearch.ResearchName;
        InitButtons(tier1Buttons, 0);
        InitButtons(tier2Buttons, 1);
        InitButtons(tier3Buttons, 2);
        InitButtons(tier4Buttons, 3);
        InitButtons(tier5Buttons, 4);

    }

    private void InitButtons(List<Button> buttons, int tier)
    {
        

        int buttonNumber = 0;
        foreach (Button button in buttons)
        {
            Research research = researchPath.GetResearchTiers()[tier].Research[buttonNumber];
            if(research)
            {
                UnityEngine.Events.UnityAction action1 = () => { SelectResearch(research); };
                button.onClick.AddListener(action1);//find the button and set
                button.GetComponentInChildren<Text>().text = research.ResearchName;
            }
            else
            {
                button.gameObject.SetActive(false);
            }
            buttonNumber++;
        }
        UpdateButtons(buttons, tier);
    }

    public void UpdateButtons(List<Button> buttons, int tier)
    {

        int buttonNumber = 0;
        foreach (Button button in buttons)
        {
            Research research = researchPath.GetResearchTiers()[tier].Research[buttonNumber];
            CheckResearchButton(button, research);
            buttonNumber++;
        }
    }

    private void CheckResearchButton(Button button, Research research)
    {
        if (research)
        {
            if (research.Finished || !researchPath.isTierAvailable(research) || scienceController.CurrentResearch == research)
            {
                button.interactable = false;
                if(research.Finished)
                {
                    button.image.color = Color.blue;
                }
                else if(scienceController.CurrentResearch == research)
                {
                    button.image.color = Color.green;
                }
                else if(!researchPath.isTierAvailable(research))
                {
                    button.image.color = Color.white;
                }
            }
            else
            {
                button.interactable = true;
                button.image.color = Color.white;
            }
        }
        else
        {
            button.gameObject.SetActive(false);
        }
    }

    public void UpdatePanel()
    {
        CheckResearchButton(researchPathBaseButton, researchPath.BaseResearch);
        UpdateButtons(tier1Buttons, 0);
        UpdateButtons(tier2Buttons, 1);
        UpdateButtons(tier3Buttons, 2);
        UpdateButtons(tier4Buttons, 3);
        UpdateButtons(tier5Buttons, 4);
    }

    public void SelectResearch(Research research)
    {
        scienceController.SelectResearch(research);
    }
}
