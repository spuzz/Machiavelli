using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedGlow : MonoBehaviour {

    Camera cameraToLookAt;
    [SerializeField] GameObject obj;

    private void Awake()
    {
        cameraToLookAt = Camera.main;
    }

        // Update is called once per frame 
    void LateUpdate()
    {
        transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
        //transform.LookAt(cameraToLookAt.transform);
        //transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
        //transform.Translate(new Vector3(0, 0, 6));
        
    }
}
