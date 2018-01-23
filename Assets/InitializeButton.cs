using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CanvasGame.ObjectCreator;
using System;
using Assets.innerState;

public class InitializeButton : MonoBehaviour {

    [SerializeField]
    GameStateMonitor monitor;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnClick()
    {
        //Initializer.getInstance().InitializeAll();
        monitor.OnDeathZoneTouch();
        
    }
}
