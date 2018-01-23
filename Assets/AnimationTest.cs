using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AnimationTest : MonoBehaviour {
    [SerializeField]
    Animator text_animator;
    [SerializeField]
    Animator camera_animator;
    private IEnumerator frameDelayFunc(Action action,int frame_delay=1)
    {
        Debug.Log("aiueo");
        for (int i = 0; i < frame_delay; i++)
        {
            yield return null;
        }
        action();
    }
    private IEnumerator WaitForAnimationEnd(Action action,Animator[] animatores,int last_state_hash)
    {

        yield return null;
        Debug.Log("Wait anim"+animatores.Length);
        for (int i = 0; i < animatores.Length; i++)
        {
            bool tmp_is_playing = true;
            while (tmp_is_playing)
            {
                var current_state = animatores[i].GetCurrentAnimatorStateInfo(0);
                tmp_is_playing = (current_state.normalizedTime < 1.0f);
                Debug.Log("Wait animation info" + current_state.normalizedTime);
                if (tmp_is_playing)
                {
                    yield return null;
                    Debug.Log("Wait one");
                }
            }
        }
        Debug.Log("Wait end");
        action();
    }
	// Use this for initialization
	void Start () {
       /* camera_animator.enabled = true;
        int hash = Animator.StringToHash("Start");
        Debug.Log(camera_animator.HasState(0, 1));
        camera_animator.Play(hash);
        IEnumerator coroutine = WaitForAnimationEnd(() => { camera_animator.Stop();
            text_animator.enabled = true;
            text_animator.Play(hash);
        IEnumerator innner_coroutine = WaitForAnimationEnd(() => { Debug.Log("End Wait ALL");
            text_animator.enabled = false;
        },
            new Animator[] { text_animator }, hash
                );
            StartCoroutine(innner_coroutine);
        }, new Animator[] { camera_animator }, hash);
        StartCoroutine(coroutine);
        */
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
}
