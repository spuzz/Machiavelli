using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = ("Units/Ability/AssasinateAgent"))]
public class AssassinateAgentConfig : AbilityConfig
{
    [Header("Assasinate Agent Specific")]
    [SerializeField] int damage = 50;

    public override AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo)
    {
        return gameObjectToAttachTo.AddComponent<AssassinateAgentBehaviour>();
    }

    public int GetDamage()
    {
        return damage;
    }
}


