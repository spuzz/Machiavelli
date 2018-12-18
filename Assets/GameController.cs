using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    [SerializeField] Player humanPlayer;
    int turn = 1;
	void Start () {
        turn = 1;
	}

    public Player GetPlayer(int playerNumber)
    {
        return humanPlayer;
    }

    public void EndPlayerTurn()
    {
        humanPlayer.EndTurn();
        // Do AI
        turn += 1;
        humanPlayer.StartTurn();
    }

    public int GetTurn()
    {
        return turn;
    }

    public void EndGame()
    {

    }
}
