using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField] SaveLoadMenu saveLoadMenu;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            HexMapCamera.Locked = false;
        }
        else
        {
            gameObject.SetActive(true);
            HexMapCamera.Locked = true;
        }

    }

    public void NewGame()
    {
        saveLoadMenu.LoadDefaultMap();
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitToDesktop()
    {
        // save any game data here
        #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif
    }

    public void Close()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            HexMapCamera.Locked = false;
        }
    }

    public void Open()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            HexMapCamera.Locked = true;
        }
    }



}
