using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class CoinScoreText : MonoBehaviour {
    private ScoreModel score_model;
	// Use this for initialization
	void Start () {
        score_model = ScoreModel.getInstance();
	
	}
	
	// Update is called once per frame
	void Update () {
        var text = this.gameObject.GetComponent<Text>();
        text.text = score_model.getCoinScore().ToString() + "/" + score_model.getMaxCoin();
	}
}
