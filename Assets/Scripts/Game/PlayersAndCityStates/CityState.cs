using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CityState : MonoBehaviour
{

    // External Components
    GameController gameController;
    Player player;
    // Internal Components
    [SerializeField] CityStateAIController cityStateAIController;

    // Attributes
    int cityStateID;
    public static int cityStateIDCounter = 1;
    [SerializeField] string cityStateName = "City State";
    [SerializeField] int symbolID;
    bool alive = true;

    City city;


    public delegate void OnInfoChange(CityState cityState);
    public event OnInfoChange onInfoChange;

    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
        }
    }


    public int CityStateID
    {
        get { return cityStateID; }
        set { cityStateID = value; }
    }

    public string CityStateName
    {
        get
        {
            return cityStateName;
        }

        set
        {
            cityStateName = value;
        }
    }

    public int SymbolID
    {
        get
        {
            return symbolID;
        }

        set
        {
            symbolID = value;
        }
    }

    public bool Alive
    {
        get
        {
            return alive;
        }

        set
        {
            alive = value;
        }
    }

    private void cityChanged(City city)
    {
        NotifyInfoChange();
    }


    public City GetCity()
    {
        return city;
    }

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        cityStateID = cityStateIDCounter;
        cityStateIDCounter++;
    }

    public void StartTurn()
    {
        NotifyInfoChange();
    }

    public void TakeTurn()
    {

    }

    public void DestroyCityState()
    {
        Destroy(gameObject);
    }


    public void Save(BinaryWriter writer)
    {
        writer.Write(CityStateID);
        writer.Write(SymbolID);
    }

    public void Load(BinaryReader reader, GameController gameController, HexGrid hexGrid, int header)
    {

        CityStateID = reader.ReadInt32();
        SymbolID = gameController.PickSymbol(reader.ReadInt32());
    }

    public void NotifyInfoChange()
    {
        if (onInfoChange != null)
        {
            onInfoChange(this);
        }
    }
}
