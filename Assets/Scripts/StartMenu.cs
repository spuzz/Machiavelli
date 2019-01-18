using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

    [SerializeField] LoadingSplashScreen loadingSplashScreen;

    bool loadScene = false;
    public LoadingSplashScreen LoadingSplashScreen
    {
        get
        {
            return loadingSplashScreen;
        }

        set
        {
            loadingSplashScreen = value;
        }
    }

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }
    public void NewGame()
    {
        StartCoroutine(LoadNewScene());

    }

    private IEnumerator LoadNewScene()
    {
        LoadingSplashScreen.Open();
        loadScene = true;

        AsyncOperation async = SceneManager.LoadSceneAsync(1);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            yield return null;
        }
    }

    public void Finish()
    {
        Destroy(gameObject);
    }
}
