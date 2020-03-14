using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Units/Agents/Skill"))]
public class AgentSkillConfig : ScriptableObject
{
    [SerializeField] string skillName;
    [SerializeField] int maxPoints = 1;
    [SerializeField] GameEffect effect;

}
