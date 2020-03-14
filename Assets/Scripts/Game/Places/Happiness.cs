using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Happiness : MonoBehaviour {

    [SerializeField] List<int> happinessBoundariesPositive;
    [SerializeField] List<int> happinessBoundariesNegative;
    [SerializeField] List<GameEffect> happinessEffectPositive;
    [SerializeField] List<GameEffect> happinessEffectNegative;

    public GameEffect GetHappinessEffect(int happiness)
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

    private GameEffect GetPositiveEffect(int happiness)
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
        return new GameEffect();
    }

    private GameEffect GetNegativeEffect(int happiness)
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
        return new GameEffect();
    }
}
