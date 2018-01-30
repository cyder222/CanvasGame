using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	public void OnClick(){
		SocialConnector.SocialConnector.Share ("It's my score!!", "", Application.persistentDataPath +"/"+GlobalConstantValue.ScreenShotImageName);
	
	}
	// Update is called once per frame
	void Update () {
		
	}
}
