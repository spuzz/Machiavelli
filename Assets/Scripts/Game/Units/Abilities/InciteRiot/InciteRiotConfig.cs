using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = ("Units/Ability/InciteRiot"))]
public class InciteRiotConfig : AbilityConfig
{
    [Header("Incite Riot Specific")]
    [SerializeField] int happiness = -20;

    public override AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo)
    {
        return gameObjectToAttachTo.AddComponent<InciteRiotBehaviour>();
    }
    public int GetHappinessChange()
    {
        return happiness;
    }
}


