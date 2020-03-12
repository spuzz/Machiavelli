using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Happiness : MonoBehaviour {

    [SerializeField] List<int> happinessBoundariesPositive;
    [SerializeField] List<int> happinessBoundariesNegative;
    [SerializeField] List<ResourceBenefit> happinessEffectPositive;
    [SerializeField] List<ResourceBenefit> happinessEffectNegative;

    public ResourceBenefit GetHappinessEffect(int happiness)
    {
        if(happiness >= 0)
        {
            return GetPositiveEffect(happiness);
        }
        else
        {
            return GetNegativeEffect(happiness);
        }
    }

    private ResourceBenefit GetPositiveEffect(int happiness)
    {
        int count = 0;
        foreach( int boundary in happinessBoundariesPositive)
        {
            if(happiness >= boundary)
            {
                return happinessEffectPositive[count];
            }
            count++;
        }
        return new ResourceBenefit();
    }

    private ResourceBenefit GetNegativeEffect(int happiness)
    {
        int count = 0;
        foreach (int boundary in happinessBoundariesNegative)
        {
            if (happiness < boundary)
            {
                return happinessEffectNegative[count];
            }
            count++;
        }
        return new ResourceBenefit();
    }
}
