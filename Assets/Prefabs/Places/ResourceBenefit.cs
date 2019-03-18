using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBenefit : MonoBehaviour {

    [SerializeField] Vector2 production;
    [SerializeField] Vector2 gold;
    [SerializeField] Vector2 science;
    [SerializeField] Vector2 food;
    [SerializeField] Vector2 defence;
    [SerializeField] Vector2 playerGold;

    public Vector2 Production
    {
        get
        {
            return production;
        }

        set
        {
            production = value;
        }
    }

    public Vector2 Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
        }
    }

    public Vector2 Science
    {
        get
        {
            return science;
        }

        set
        {
            science = value;
        }
    }

    public Vector2 Food
    {
        get
        {
            return food;
        }

        set
        {
            food = value;
        }
    }

    public Vector2 Defence
    {
        get
        {
            return defence;
        }

        set
        {
            defence = value;
        }
    }

    public Vector2 PlayerGold
    {
        get
        {
            return playerGold;
        }

        set
        {
            playerGold = value;
        }
    }

    public void AddBenefit(ResourceBenefit benefit)
    {
        Gold += benefit.Gold;
        Food += benefit.Food;
        Production += benefit.Production;
        Science += benefit.Science;
        Defence += benefit.Defence;
        PlayerGold += benefit.PlayerGold;
    }

    public void ResetBenefit()
    {
        Gold = new Vector2(0, 0);
        Food = new Vector2(0, 0);
        Production = new Vector2(0, 0);
        Science = new Vector2(0, 0);
        Defence = new Vector2(0, 0);
        PlayerGold = new Vector2(0, 0);
    }
}
