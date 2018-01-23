using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using Assets.Utility;

public class Initializer : MonoBehaviour {

    [SerializeField]
    private GameObject m_mainCharactor;
    [SerializeField]
    private Vector3 m_charactorInitializePosition = new Vector3(-16, -6, 0);
    [SerializeField]
    private Camera m_mainCamera;
    [SerializeField]
    private Vector3 m_mainCameraInitializePosition = new Vector3(-16, -6, 0);
    
    private static Initializer instance;
    // Use this for initialization
    void OnSceneWasLoaded()
    {
        instance = null;
    }
    void Awake()
    {
        //InitializeAll();
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            print("Duplicate Initializer! self destruct!");
        }
        else
        {
            instance = this;
            //GameObject.DontDestroyOnLoad(gameObject);
        }
    }
    public static Initializer getInstance()
    {
        return instance;
    }
    void Start () {
        InitializeAll();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void InitializeAll()
    {
        CoinManager.getInstance().InitializeAllCoin();
        ScoreModel.getInstance().InitScore(CoinManager.getInstance().getCoinNum(), 90);
        ObjectPoolManager.getInstance().DestroyAllObjectFromAllPool();
    }
    
}
