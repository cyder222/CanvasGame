using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.innerState
{
    /// <summary>
    /// 各種ゲーム状態にアクセスするための中継クラス
    /// </summary>
    public class StateAccess : MonoBehaviour
    {
        [SerializeField] AnimatedBaseState m_baseState;
        [SerializeField] AnimatedBaseState m_normalState;
        [SerializeField] AnimatedBaseState m_originState;
        [SerializeField] AnimatedBaseState m_gameOverState;
        [SerializeField] AnimatedBaseState m_gameClearState;

        public IAnimatedGameState getState(AnimatedStateType type)
        {
            switch (type)
            {
                case AnimatedStateType.NORMAL:
                    return m_normalState;
                case AnimatedStateType.START:
                    return m_originState;
                case AnimatedStateType.GAMECLEAR:
                    return m_gameClearState;
                case AnimatedStateType.GAMEOVER:
                    return m_gameOverState;
                default:
                    return m_baseState;
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}