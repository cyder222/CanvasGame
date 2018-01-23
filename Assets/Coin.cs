using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Coin : MonoBehaviour {
    private bool is_scored = false;
	// Use this for initialization
	void Start () {
        CoinManager.getInstance().AddCoinToManager(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void Awake()
    {
        CoinManager.getInstance().listInit();
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("OnTrigger Enter 2D on Coin" + Time.realtimeSinceStartup);
        if (collider.gameObject.tag == "Player")
            {
                this.gameObject.SetActive(false);
                //二重に取得するのを防ぐ
                if (!is_scored)
                {
                    is_scored = true;   
                    ScoreModel.getInstance().setAddCoinScore();    
                 }
            OneShotSoundManager.getInstance().PlayCoinGet(this.gameObject);
        }
    }
    public void initializeCoin()
    {
        this.is_scored = false;
    }
}
public class CoinManager
{
    private List<Coin> coin_list;
    private static CoinManager __instance = null;

    public static CoinManager getInstance()
    {
        if (__instance == null)
        {
            __instance = new CoinManager();
        }
        return __instance;
    }
    private CoinManager()
    {
        coin_list = new List<Coin>();
    }
    public void listInit()
    {
        coin_list = new List<Coin>();
    }
    public void refleshCoin()
    {

    }
    public int getCoinNum()
    {
        return coin_list.Count;
    }
    
    public void InitializeAllCoin()
    {
        
        foreach (Coin coin in coin_list)
        {    
            coin.gameObject.SetActive(true);
            coin.initializeCoin();
        }
    }
    public void AddCoinToManager(Coin coin)
    {
        if(!coin_list.Contains(coin))
            coin_list.Add(coin);
    }
}
