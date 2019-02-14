using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = ("Units/Ability/BuildOpCentre"))]
public class BuildOpCentreConfig : AbilityConfig
{

    public override AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo)
    {
        return gameObjectToAttachTo.AddComponent<BuildOpCentreBehaviour>();
    }

}


