using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    RawImage healthBarRawImage = null;
    Unit unit = null;

    public Unit Unit
    {
        get
        {
            return unit;
        }

        set
        {
            unit = value;
        }
    }

    // Use this for initialization
    void Awake()
    {
        healthBarRawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    public void UpdateHealth()
    {
        float xValue = -(Unit.HealthAsPercentage / 2f) - 0.5f;
        healthBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
    }
}
