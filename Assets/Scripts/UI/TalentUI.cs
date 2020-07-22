using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalentUI : MonoBehaviour {

    [SerializeField] int level;
    [SerializeField] Talent talent;
    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] TalentTreeUI talentTreeUI;

    private void Start()
    {
        talentTreeUI = FindObjectOfType<TalentTreeUI>();
    }

    public void SetTalent(Talent talentToDisplay)
    {
        talent = talentToDisplay;
        image.sprite = talent.Sprite;
    }

    public void buttonClicked()
    {

    }
}
