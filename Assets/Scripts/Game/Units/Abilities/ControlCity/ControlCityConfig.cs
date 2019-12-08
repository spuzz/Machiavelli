using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = ("Units/Ability/BuildOutpost"))]
public class ControlCityConfig : AbilityConfig
{

    public override AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo)
    {
        return gameObjectToAttachTo.AddComponent<ControlCityBehaviour>();
    }

}


