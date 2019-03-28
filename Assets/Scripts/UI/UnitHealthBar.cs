using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    RawImage healthBarRawImage = null;
    Unit unit = null;
    int currentHealth;
    int maxHealth;
    public Unit Unit
    {
        get
        {
            return unit;
        }

        set
        {
            unit = value;
            currentHealth = unit.GetBaseHitpoints();
            maxHealth = unit.GetBaseHitpoints();
            UpdateHealth(0);
        }
    }

    // Use this for initialization
    void Awake()
    {
        healthBarRawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    public void UpdateHealth(int healthChange)
    {
        float healthAfterChange = (float)currentHealth + (float)healthChange;
        if(healthAfterChange < 0) { healthAfterChange = 0;  }
        float healthAsPerc = healthAfterChange / (float)maxHealth;
        float xValue = -(healthAsPerc / 2f) - 0.5f;
        healthBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        currentHealth = (int)healthAfterChange;
    }
}
