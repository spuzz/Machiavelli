using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour {

    [SerializeField] Happiness happiness;

    public Happiness Happiness
    {
        get
        {
            return happiness;
        }

        set
        {
            happiness = value;
        }
    }
}
