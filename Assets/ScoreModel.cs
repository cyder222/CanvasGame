using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Threading;
using UnityEngine.EventSystems;

public class ScoreModel : MonoBehaviour{
    private static ScoreModel __instance = null;
    private int m_coinScore = 0;
    private int m_maxCoinScore = 0;
    private bool m_enableTimer = true;
    public EventHandler OnCoinChange;
    private int m_timeScore = 0;
    private float m_floatTimeScore = 0.0f;
    private const float MAX_TIME = 90.0f;
    public EventHandler OnTimeChange;

    public enum TimerType
    {
        UP_TIMER,
        DOWN_TIMER
    }
    private TimerType timer_type = TimerType.UP_TIMER;
    void Awake()
    {
        if (__instance != null && __instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            __instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        Type type = Type.GetType("Mono.Runtime");
        if (type != null)
        {
            MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
            if (displayName != null)
                Debug.Log(displayName.Invoke(null, null));
        }


    }
    private ScoreModel()
    {
        m_coinScore = 0;
        m_maxCoinScore = CoinManager.getInstance().getCoinNum();   
        OnTimeChange += (ss, ee) => { };
        OnCoinChange += (ss, ee) => { };
        this.timer_type = TimerType.DOWN_TIMER;
        this.m_timeScore = 90;
        this.m_floatTimeScore = 90.0f;
    }
    public void InitScore(int max_coin,int current_coin)
    {
        this.m_floatTimeScore = 0.0f;
        m_maxCoinScore = CoinManager.getInstance().getCoinNum();
        this.m_coinScore = 0;
        this.m_timeScore = (int)MAX_TIME;
        this.m_floatTimeScore = MAX_TIME;
    }
    public void InitScoreAndEvent(int max_coin,int current_coin)
    {
        m_coinScore = 0;
        m_maxCoinScore = 14;
        this.timer_type = TimerType.DOWN_TIMER;
        this.m_timeScore = (int)MAX_TIME;
        this.m_floatTimeScore = MAX_TIME;
    }
    public static ScoreModel getInstance()
    {
        if (__instance == null)
            __instance = new ScoreModel();
        return __instance;
    }
    public int getCoinScore()
    {
        return m_coinScore;
    }
    public int getMaxCoin()
    {
        return m_maxCoinScore;
    }
    public void setAddCoinScore()
    {
        this.m_coinScore += 1;
      
        OnCoinChange(this, EventArgs.Empty);
       
    }
    public int getTimeScore()
    {
        return m_timeScore;
    }
    public void setTimeScore(int time)
    {
        if ( (this.m_timeScore != time) && (this.m_timeScore>=0) )
        {
            this.m_timeScore = time;
            
            OnTimeChange(this, EventArgs.Empty);
        }
    }
    public void StopTimer()
    {
        this.m_enableTimer = false;
    }
    public void StartTimer()
    {
        this.m_enableTimer = true;
    }
    void Update()
    {
        if (this.m_enableTimer)
        {
            if (timer_type == TimerType.UP_TIMER)
                this.m_floatTimeScore += Time.deltaTime;
            else
                this.m_floatTimeScore -= Time.deltaTime;
            this.setTimeScore((int)m_floatTimeScore);
        }
    }
}