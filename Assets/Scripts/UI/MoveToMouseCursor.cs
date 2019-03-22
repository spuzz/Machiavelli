using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToMouseCursor : MonoBehaviour {

    [SerializeField] int xOffset = 5;
    [SerializeField] int yOffset = 5;
    // Update is called once per frame
    void Update ()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        gameObject.transform.position = new Vector3(Input.mousePosition.x + xOffset + GetComponent<RectTransform>().rect.width / 2, Input.mousePosition.y + yOffset + GetComponent<RectTransform>().rect.height / 2);
    }

}
