using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSplashScreen : MonoBehaviour {

    [SerializeField] Text text;
    int anim = 0;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        text.text = "Loading";
    }
    public void Open()
    {
        gameObject.SetActive(true);
        //HexMapCamera.Locked = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        //HexMapCamera.Locked = false;
    }

}
