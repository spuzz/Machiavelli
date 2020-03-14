using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loyalty : MonoBehaviour {

    [SerializeField] List<int> loyaltyBoundariesPositive;
    [SerializeField] List<int> loyaltyBoundariesNegative;
    [SerializeField] List<GameEffect> loyaltyEffectPositive;
    [SerializeField] List<GameEffect> loyaltyEffectNegative;

    public GameEffect GetLoyaltyEffect(int loyalty)
    {
        if (loyalty >= 0)
        {
            return GetPositiveEffect(loyalty);
        }
        else
        {
            return GetNegativeEffect(loyalty);
        }
    }

    private GameEffect GetPositiveEffect(int loyalty)
    {
        int count = 0;
        foreach (int boundary in loyaltyBoundariesPositive)
        {
            if (loyalty >= boundary)
            {
                return loyaltyEffectPositive[count];
            }
            count++;
        }
        return new GameEffect();
    }

    private GameEffect GetNegativeEffect(int loyalty)
    {
        int count = 0;
        foreach (int boundary in loyaltyBoundariesNegative)
        {
            if (loyalty < boundary)
            {
                return loyaltyEffectNegative[count];
            }
            count++;
        }
        return new GameEffect();
    }
}
