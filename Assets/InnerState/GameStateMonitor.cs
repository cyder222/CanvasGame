using System;
using System.Collections;
using System.Collections.Generic;
using Assets.innerState;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Utility;

public interface IGameStateMonitoring : IEventSystemHandler
{
    void OnDeathZoneTouch();
    void OnTimeOver();
    void OnCoinAllGet();
    void OnGiveUp();
}

namespace Assets.innerState
{
    /// <summary>
    /// ゲームの状態を監視、また、他のオブジェクトからイベントを受信して、実際にゲームの状態の変更を発生させる
    /// </summary>
    public class GameStateMonitor : MonoBehaviour, IGameStateMonitoring
    {
        [SerializeField] StateAccess access;
        [SerializeField] AnimatedStateChanger state_getter;

        public void OnCoinAllGet()
        {
            ExecuteEvents.Execute<IRecieveStateChange>(gameObject, null,
                (inter, rec) => { inter.OnChangeState(access.getState(AnimatedStateType.GAMECLEAR)); }
            );
        }

        public void OnDeathZoneTouch()
        {
            ExecuteEvents.Execute<IRecieveStateChange>(gameObject, null,
                (inter, rec) => { inter.OnChangeState(access.getState(AnimatedStateType.GAMEOVER)); }
            );
        }

        public void OnGiveUp()
        {
            throw new NotImplementedException();
        }

        public void OnTimeOver()
        {
            ExecuteEvents.Execute<IRecieveStateChange>(gameObject, null,
                (inter, rec) => { inter.OnChangeState(access.getState(AnimatedStateType.GAMEOVER)); }
            );
        }

        // Use this for initialization
        void Start()
        {
            TaskSystem.getInstance().addTask(() =>
            {
                ScoreModel.getInstance().StopTimer();
                ExecuteEvents.Execute<IRecieveStateChange>(gameObject, null,
                    (inter, rec) => { inter.OnChangeState(access.getState(AnimatedStateType.NORMAL)); }
                );
            });
        }

        // Update is called once per frame
        void Update()
        {
            //スコア状態の監視
            Debug.Log(ScoreModel.getInstance().getCoinScore());

            if (state_getter.getCurrentStateType() == AnimatedStateType.NORMAL)
            {
                if (ScoreModel.getInstance().getCoinScore() >= CoinManager.getInstance().getCoinNum())
                {
                    if (CoinManager.getInstance().getCoinNum() != 0)
                        this.OnCoinAllGet();
                }
                if (ScoreModel.getInstance().getTimeScore() <= 0)
                {
                    this.OnTimeOver();
                }
            }
        }
    }
}