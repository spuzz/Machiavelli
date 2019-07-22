using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum FocusType
{
    PRODUCTION,
    ECONOMIC,
    SCIENCE,
    GROWTH,
    DEFENCE,
    OFFENCE,
    MILITARY,
    EXPANSION

}

static class TextColors
{
    public static Color lostHitPoints = Color.red;
    public static Color gainedHitPoints = Color.green;
    public static Color humanGainedInfluence = new Color(1.0f, 0.64f, 0.0f);
    public static Color humanlostInfluence = Color.yellow;

    public static Color AIGainedInfluence = Color.cyan;
    public static Color AIlostInfluence = new Color(0.3f, 0, 0.5f);

}

static class GameConsts
{
    public static int capitalUnitCap = 2;
    public static int baseUnitCap = 3;

    public static int startingGold = 300;
    public static bool playAnimations = true;
}

