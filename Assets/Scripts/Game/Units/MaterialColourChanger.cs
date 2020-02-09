using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColourChanger : MonoBehaviour {

    [SerializeField] List<Material> excludedMaterials;
    public enum MaterialType
    {
        UNIT,
        UNIT2,
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
        else if(type == MaterialType.UNIT2)
        {
            newMat = colour.UnitMaterial2;
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
                if(!excludedMaterials.Find(c => rend.materials[j].name.StartsWith(c.name)))
                {
                    mats[j] = newMat;
                }
                else
                {
                    mats[j] = rend.materials[j];
                }

            }
            rend.materials = mats;
        }
    }

}
