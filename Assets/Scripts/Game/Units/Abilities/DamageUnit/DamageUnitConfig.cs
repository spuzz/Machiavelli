using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = ("Units/Ability/DamageUnit"))]
public class DamageUnitConfig : AbilityConfig
{
    [Header("Damage Unit Specific")]
    [SerializeField] int damage = 20;

    public override AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo)
    {
        return gameObjectToAttachTo.AddComponent<DamageUnitBehaviour>();
    }

    public int GetDamage()
    {
        return damage;
    }
}


