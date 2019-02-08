using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = ("Units/Ability/Bribe"))]
public class BribeConfig : AbilityConfig
{
    [Header("Bribe Specific")]
    [SerializeField] int influence = 20;

    public override AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo)
    {
        return gameObjectToAttachTo.AddComponent<BribeBehaviour>();
    }

    public int GetInfluence()
    {
        return influence;
    }
}


