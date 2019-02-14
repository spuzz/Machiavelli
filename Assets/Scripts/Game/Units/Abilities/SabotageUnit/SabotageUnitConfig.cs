using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = ("Units/Ability/SabotageUnit"))]
public class SabotageUnitConfig : AbilityConfig
{

    public override AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo)
    {
        return gameObjectToAttachTo.AddComponent<SabotageUnitBehaviour>();
    }

}


