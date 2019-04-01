using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Player/Colour"))]
public class PlayerColour : ScriptableObject {

    [SerializeField] int id;
    [SerializeField] Material unitMaterial;
    [SerializeField] Material buildingMaterial;
    [SerializeField] Color colour;

    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    public Material UnitMaterial
    {
        get
        {
            return unitMaterial;
        }

        set
        {
            unitMaterial = value;
        }
    }

    public Material BuildingMaterial
    {
        get
        {
            return buildingMaterial;
        }

        set
        {
            buildingMaterial = value;
        }
    }

    public Color Colour
    {
        get
        {
            return colour;
        }

        set
        {
            colour = value;
        }
    }
}
