using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalentTreeUI : MonoBehaviour {

    private Agent currentAgent;

    [SerializeField] TalentTree talentTree;

    [System.Serializable]
    public class UITalentLevel
    {
        [SerializeField] int level;
        [SerializeField] List<TalentUI> talentUIs;

        public int Level
        {
            get
            {
                return level;
            }

            set
            {
                level = value;
            }
        }

        public List<TalentUI> TalentUIs
        {
            get
            {
                return talentUIs;
            }

            set
            {
                talentUIs = value;
            }
        }
    }

    [SerializeField] List<UITalentLevel> levels;

    private void Start()
    {
        int level = 0;
        foreach(var item in talentTree.talentTree)
        {
            if(level < levels.Count)
            {
                for(int a = 0; a < item.TalentBrackets.Count; a++)
                {
                    if (a < levels[level].TalentUIs.Count)
                    {
                        levels[level].TalentUIs[a].SetTalent(item.TalentBrackets[a].Talents[0]);
                    }
                }
            }
            level++;
        }
    }

    public void Display(Agent agent)
    {
        currentAgent = agent;
    }

    public void GiveAgentTalent(Talent talent)
    {
        currentAgent.AddTalent(talent);
    }
}
