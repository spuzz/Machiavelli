using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityHealthBar : MonoBehaviour
{
    RawImage healthBarRawImage = null;
    City city = null;

    public City CityObject
    {
        get
        {
            return city;
        }

        set
        {
            city = value;
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
        if(CityObject)
        {
            if (CityObject.HealthAsPercentage <= 0)
            {
                healthBarRawImage.uvRect = new Rect(-0.5f, 0f, 0.5f, 1f);
            }
            else
            {
                float xValue = -(CityObject.HealthAsPercentage / 2f) - 0.5f;
                healthBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
            }
        }


    }
}
