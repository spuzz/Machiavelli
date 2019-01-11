using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSetup : MonoBehaviour {

    [SerializeField] SaveLoadMenu saveLoadMenu;
    [SerializeField] NewMapMenu newMapMenu;
    

    public IEnumerator SetupMap()
    {
        if (MapSetupConfig.IsLoadMap)
        {
            if(MapSetupConfig.MapName == "default")
            {
                saveLoadMenu.LoadDefaultMap();
            }
        }
        StartMenu startMenu = FindObjectOfType<StartMenu>();
        if(startMenu)
        {
            startMenu.Finish();
        }
        
        
        yield return new WaitForEndOfFrame();
    }
}
