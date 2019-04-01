using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColourChanger : MonoBehaviour {

    public enum MaterialType
    {
        UNIT,
        BUILDING
    }
    [SerializeField] MaterialType type;

    public void ChangeMaterial(PlayerColour colour)
    {
        Renderer[] children;
        children = GetComponentsInChildren<Renderer>();
        Material newMat;
        if(type == MaterialType.UNIT)
        {
            newMat = colour.UnitMaterial;
        }
        else
        {
            newMat = colour.BuildingMaterial;
        }
        foreach (Renderer rend in children)
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = newMat;
            }
            rend.materials = mats;
        }
    }

    public void ChangeMaterial(Material newMat)
    {
        Renderer[] children;
        children = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in children)
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = newMat;
            }
            rend.materials = mats;
        }
    }

}
