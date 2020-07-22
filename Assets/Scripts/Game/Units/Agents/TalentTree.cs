using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = ("Units/Agent/TalentTree"))]
public class TalentTree : ScriptableObject
{


    [System.Serializable]
    public class Level
    {
        [SerializeField] List<TalentBracket> talentBrackets;

        public List<TalentBracket> TalentBrackets
        {
            get
            {
                return talentBrackets;
            }

            set
            {
                talentBrackets = value;
            }
        }
    }

    [System.Serializable]
    public class TalentBracket
    {
        [SerializeField] List<Talent> talents;

        public List<Talent> Talents
        {
            get
            {
                return talents;
            }

            set
            {
                talents = value;
            }
        }
    }
    public List<Level> talentTree;
}
