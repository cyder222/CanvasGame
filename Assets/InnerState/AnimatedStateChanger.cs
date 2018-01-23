using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Assets.innerState;
namespace Assets.innerState
{
    public enum AnimatedStateType
    {
        BASE,
        NORMAL,
        START,
        END,
        GAMEOVER,
        GAMECLEAR
    }

    public interface IAnimatedGameState
    {
        AnimatedStateType getStateType();
        void endState();
        void beginState();
        void prepareEndState();
        void prepareBeginState();
        List<Animator> getAnimatorList();
    }

    public interface IRecieveStateChange : IEventSystemHandler
    {
        void OnChangeState(IAnimatedGameState to_state);
    }

    public interface ICurrentStateGetter
    {
        AnimatedStateType getCurrentStateType();
    }

    /// <summary>
    /// アニメーションを行いながら、ゲームの内部状態を変化させる。
    /// 実行するアニメーションは、Animatorの配列に入れたもので、ステート名で制御する。
    /// 例) "START"状態を終了させ、"GAMEOVER"に移行するとき
    /// 各AnimatorのSTART_END状態を参照
    /// START_END状態があるすべてのアニメーターのEND状態をPlayする。
    /// その後、アニメーションの終了を待つ。終了したら、STARTのendStateを呼ぶ、
    /// 次にSTART状態が登録されているアニメーターを探索し、それらのアニメーションを実行。
    /// それらの終了を待ち、終了次第 gameover->beginState()を呼ぶ.
    /// </summary>
    /// <!--
    /// 注意点として、
    /// 1.ここで使うアニメーターはloopなし、遷移なしにする必要がある。
    /// 2.呼ばれる順番は、prepareEnd→endAnimation→endState→prepareBegin→startAnimation→beginState
    /// -->
    public class AnimatedStateChanger : MonoBehaviour, IRecieveStateChange, ICurrentStateGetter
    {

        [SerializeField] StateAccess state_access_funcs;
        // Use this for initialization
        IAnimatedGameState current_state;

        enum StateInnnerState
        {
            NORMAL,
            START,
            END
        }

        private string getAnimationName(StateInnnerState innner_state, AnimatedStateType animated_state)
        {
            return innner_state.ToString();
        }

        public IEnumerator changeState(IAnimatedGameState to_state)
        {
            current_state.prepareEndState();
            /**
             * Start and wait Animation and wait until
            **/

            List<Animator> state_using_animatores = new List<Animator>();
            List<Animator> current_state_animator = current_state.getAnimatorList();
            int end_animation_hash =
                Animator.StringToHash(getAnimationName(StateInnnerState.END, current_state.getStateType()));

            foreach (Animator animator in current_state_animator)
            {
                if (animator.HasState(0, end_animation_hash))
                {
                    animator.enabled = true;
                    state_using_animatores.Add(animator);
                    animator.Play(end_animation_hash);
                }
            }
            var first_wait_routine = WaitForAnimationEnd(state_using_animatores.ToArray(), end_animation_hash);
            yield return StartCoroutine(first_wait_routine);
            foreach (Animator animator in state_using_animatores)
            {
                animator.enabled = false;
            }
            current_state.endState();
            current_state = to_state;
            current_state.prepareBeginState();
            int start_animation_hash =
                Animator.StringToHash(getAnimationName(StateInnnerState.START, current_state.getStateType()));
            current_state_animator = current_state.getAnimatorList();
            state_using_animatores.Clear();
            foreach (Animator animator in current_state_animator)
            {
                if (animator.HasState(0, start_animation_hash))
                {
                    state_using_animatores.Add(animator);
                    animator.enabled = true;
                    animator.Play(start_animation_hash);
                }
            }
            var second_wait_coroutine = WaitForAnimationEnd(state_using_animatores.ToArray(),
                start_animation_hash);
            yield return StartCoroutine(second_wait_coroutine);
            foreach (Animator animator in state_using_animatores)
            {
                animator.enabled = false;
            }
            current_state.beginState();
        }

        private IEnumerator frameDelayFunc(Action action, int frame_delay = 1)
        {
            for (int i = 0; i < frame_delay; i++)
            {
                yield return null;
            }
            action();
        }

        /// <summary>
        /// animatores配列に入っているアニメーターのアニメーションが全て終了状態(normalized_timeが1.0以上)になるのを待つ。
        /// アニメーションが自動で遷移する設定のときは動作がおかしくなる可能性があるため、各種アニメーターに登録するアニメーションは、
        /// 単独のものにする必要がある。
        /// </summary>
        /// <param name="animatores"></param>
        /// <param name="last_state_hash"></param>
        /// <returns></returns>
        private IEnumerator WaitForAnimationEnd(Animator[] animatores, int last_state_hash)
        {
            yield return null;
            Debug.Log("Wait anim" + animatores.Length);
            for (int i = 0; i < animatores.Length; i++)
            {
                bool tmp_is_playing = true;
                while (tmp_is_playing)
                {
                    var current_state = animatores[i].GetCurrentAnimatorStateInfo(0);
                    tmp_is_playing = (current_state.normalizedTime < 1.0f);

                    if (tmp_is_playing)
                    {
                        yield return null;
                        Debug.Log("Wait one");
                    }
                }
            }
            Debug.Log("Wait end");
            yield return true;
        }

        // Use this for initialization
        void Start()
        {
            this.current_state = state_access_funcs.getState(AnimatedStateType.START);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public AnimatedStateType getCurrentStateType()
        {
            return this.current_state.getStateType();
        }

        public void OnChangeState(IAnimatedGameState to_state)
        {
            Debug.Log("call onchangestate");
            StartCoroutine(this.changeState(to_state));
        }
    }
}