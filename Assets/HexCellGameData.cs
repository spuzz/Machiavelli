using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HexCellGameData : MonoBehaviour
{

    [SerializeField] HexCellUI hexCellUI;

    City cityOwner;
    int production;
    int gold;
    int science;
    int food;

    bool isWorked;
    public int Production
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

    public int Gold
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

    public int Science
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

    public int Food
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

    public bool IsWorked
    {
        get
        {
            return isWorked;
        }

        set
        {
            isWorked = value;
            hexCellUI.SetWorked(isWorked);
        }
    }

    public City CityOwner
    {
        get
        {
            return cityOwner;
        }

        set
        {
            cityOwner = value;
        }
    }

    public void Awake()
    {
        RandomiseYields();
    }

    private void RandomiseYields()
    {
        Production = UnityEngine.Random.Range(0, 2);
        Food = UnityEngine.Random.Range(0, 2);
        Science = UnityEngine.Random.Range(0, 2);
        Gold = UnityEngine.Random.Range(0, 2);
        hexCellUI.UpdateUI();
    }

    public int GetTotalYield()
    {
        return Production + Gold + Science + Food;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(Production);
        writer.Write(Food);
        writer.Write(Science);
        writer.Write(Gold);
    }
    public void Load(BinaryReader reader, int header)
    {
        Production = reader.ReadInt32();
        Food = reader.ReadInt32();
        Science = reader.ReadInt32();
        Gold = reader.ReadInt32();
        hexCellUI.UpdateUI();
        hexCellUI.EnableCanvas(false);
    }
}