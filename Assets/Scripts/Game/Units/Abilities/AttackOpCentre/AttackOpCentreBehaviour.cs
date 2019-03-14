
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackOpCentreBehaviour : AbilityBehaviour
{

    public override void Use(HexCell target = null)
    {
        if(target.OpCentre)
        {
            Player player = target.OpCentre.Player;
            //target.OpCentre.DestroyOperationCentre();

        }
        PlayParticleEffect();
        PlayAbilitySound();
        PlayAnimation();
    }
    public override bool IsValidTarget(HexCell target)
    {
        if(target.OpCentre && target.OpCentre.Player != gameObject.GetComponent<Unit>().GetPlayer())
        {
            return true;
        }
        return false;
    }
}


