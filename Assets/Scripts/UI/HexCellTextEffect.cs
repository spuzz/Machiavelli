using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexCellTextEffect : MonoBehaviour
{
    [SerializeField] Text text;
    Transform textLocation;
    float defaultDuration = 2;
    float duration;
    float durationLeft;
    Camera cameraToLookAt;
    float scrollDistance = 15;
    Color textColor;
    private void Awake()
    {
        cameraToLookAt = Camera.main;
    }
    public void Show(string newText, Transform transform, Color color, float newDuration = 0)
    {
        textColor = color;
        text.text = newText;
        text.color = color;
        textLocation = transform;
        if(newDuration != 0)
        {
            duration = newDuration;
            durationLeft = newDuration;
        }
        else
        {
            duration = defaultDuration;
            durationLeft = defaultDuration;
        }
        
        
    }

    void LateUpdate()
    {

        if (textLocation)
        {
            durationLeft -= Time.deltaTime;
            if (durationLeft <= 0)
            {
                Destroy(gameObject);
                return;
            }
            float yPos = -10 -(scrollDistance * (1 - durationLeft / duration));
            transform.position = new Vector3(textLocation.position.x, textLocation.position.y + 25, textLocation.position.z);
            transform.LookAt(cameraToLookAt.transform);
            transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
            transform.Translate(new Vector3(0, yPos, 7));
        }



    }
}
