using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DeathZone : MonoBehaviour {

    private GameObject gameStateControler;
	// Use this for initialization
	void Start () {
        gameStateControler = GameObject.Find("GameState");
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject.tag == "Player")
        {
            ExecuteEvents.Execute<IGameStateMonitoring>(gameStateControler, null, (inter, y) => { inter.OnDeathZoneTouch(); });
            
        }
    }
}
