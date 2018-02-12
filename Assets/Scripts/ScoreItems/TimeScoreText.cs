using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TimeScoreText : MonoBehaviour {

    private ScoreModel score_model;
	// Use this for initialization

	void Start () {
        score_model = ScoreModel.getInstance();
        
	}

    // Update is called once per frame
    void Update() {
        int time = score_model.getTimeScore();
        Text time_text = this.gameObject.GetComponent<Text>();
        time_text.text = String.Format("{0:D2}", time / 60) + ":" + String.Format("{0:D2}",(time % 60));
	}
}
