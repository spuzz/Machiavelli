using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourChange : MonoBehaviour {

    public void ChangeColour(Color colour)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = colour;
    }
}
