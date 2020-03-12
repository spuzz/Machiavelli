using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum FocusType
{
    ALL,
    PRODUCTION,
    ECONOMIC,
    SCIENCE,
    GROWTH,
    DEFENCE,
    OFFENCE,
    MILITARY,
    EXPANSION,
    MELEE,
    RANGED,
    MOUNTED,
    SIEGE,
    AGENT

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

    public static int startingGold = 300;
    public static bool playAnimations = true;
    public static List<int> populationFoodReqirements = new List<int> { 0, 30, 60, 90, 120, 150, 180, 210, 240, 270, 300, 330, 360, 390, 420, 450, 480, 510, 540, 570, 600, 630, 660, 690, 720, 750, 780, 810, 840, 870, 900 };
    public static List<int> populationPoliticalCost = new List<int> { 150, 200, 300, 450, 650, 900, 1200, 1550, 1950, 2400, 2950, 3500 };
    public static int HQGold = 5;
    public static int HQPC = 5;
    public static int HQScience = 2;
    public static float ambushBonus = 1.2f;
    public static float fightSpeed = 2f;
    public static int negativePCTurnIncrease = 2;
    public static int maintanencePerPop = 1;
    public static Vector3 agentBaseOffset = new Vector3(0, 0, -4);
    public static Vector3 supportBaseOffset = new Vector3(4, 0, -1);
    public static Vector3 siegeBaseOffset = new Vector3(-4, 0, 0);
    public static int agentLevelXP = 100;
    public static int pointsPerLevel = 3;

}

