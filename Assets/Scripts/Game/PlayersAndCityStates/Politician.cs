using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Politician : MonoBehaviour {

    [SerializeField] Player controllingPlayer;
    [SerializeField] Politician politicianPrefab;
    [SerializeField] CityState cityState;

    public Player ControllingPlayer
    {
        get
        {
            return controllingPlayer;
        }

        set
        {
            controllingPlayer = value;
            CityState.UpdatePoliticalLandscape();
        }
    }

    public CityState CityState
    {
        get
        {
            return cityState;
        }

        set
        {
            cityState = value;
        }
    }

    public void Save(BinaryWriter writer)
    {
        if(controllingPlayer)
        {
            writer.Write(controllingPlayer.PlayerNumber);
        }
        else
        {
            writer.Write(-1);
        }

    }

    public void Load(BinaryReader reader, GameController gameController, int header)
    {
        int playerNumber = reader.ReadInt32();
        if(playerNumber != -1)
        {
            controllingPlayer = gameController.GetPlayer(playerNumber);
        }

    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
