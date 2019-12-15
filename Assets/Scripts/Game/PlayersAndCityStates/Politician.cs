using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Politician : MonoBehaviour {

    [SerializeField] Player controllingPlayer;
    [SerializeField] Politician politicianPrefab;
    [SerializeField] CityState cityState;
    [SerializeField] [Range(0,100)] int loyalty;

    public Player ControllingPlayer
    {
        get
        {
            return controllingPlayer;
        }

        set
        {
            if(controllingPlayer != value)
            {
                if(controllingPlayer)
                {
                    controllingPlayer.LosePolitician(cityState);
                }

                controllingPlayer = value;
                if (controllingPlayer)
                {
                    controllingPlayer.GainPolitician(cityState);
                }
            }

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

    public int Loyalty
    {
        get
        {
            return loyalty;
        }

        set
        {
            loyalty = value;
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
        writer.Write(loyalty);

    }

    public void Load(BinaryReader reader, GameController gameController, int header)
    {
        int playerNumber = reader.ReadInt32();
        if(playerNumber != -1)
        {
            ControllingPlayer = gameController.GetPlayer(playerNumber);
            loyalty = 100;
        }
        if(header > 2)
        {
            loyalty = reader.ReadInt32();
        }
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
