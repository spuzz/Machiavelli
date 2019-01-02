using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    [SerializeField] GameController gameController;
    [SerializeField] Text turn;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        turn.text = "Turn : " + gameController.GetTurn().ToString();

    }

    public void EndTurn()
    {
        gameController.EndPlayerTurn();
    }
}
