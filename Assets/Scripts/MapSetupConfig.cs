using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class MapSetupConfig
{
    static bool loadMap = true;

    private static string mapName = "default";

    public static string MapName
    {
        get
        {
            return mapName;
        }

        set
        {
            mapName = value;
        }
    }

    public static bool IsLoadMap
    {
        get
        {
            return loadMap;
        }

        set
        {
            loadMap = value;
        }
    }

    public static void LoadMap(string maptoLoad = "default")
    {
        IsLoadMap = true;
        MapName = maptoLoad;
    }

    public static void NewMap()
    {
        IsLoadMap = false;
    }
}

