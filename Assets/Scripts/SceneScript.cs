using UnityEngine;
using System.Collections;
using Assets.Utility;
public class SceneScript : MonoBehaviour {

    private LevelManager m_levelManager;
	// Use this for initialization
	void Start () {
        GameObject level = GameObject.Find("LevelManager");
        m_levelManager = level.GetComponent<LevelManager>();
        m_levelManager.LoadNextLevelWithFade("01StartMenu");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
