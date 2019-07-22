using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = ("Units/Ability/Envoy"))]
public class EnvoyConfig : AbilityConfig
{
    [Header("Envoy Specific")]
    [SerializeField] int influence = 20;

    public override AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo)
    {
        return gameObjectToAttachTo.AddComponent<EnvoyBehaviour>();
    }

    public int GetInfluence()
    {
        return influence;
    }
}


