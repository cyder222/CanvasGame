//-----------------------------------------------------------------------
// <
//-----------------------------------------------------------------------
using UnityEngine;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Utility
{
/**
 * <summary>
 * タスクシステムクラス　
 * このクラスにタスクをAddすると、次回Updateの時に実行してくれる。
 * 描画系の、別スレッドでは動かせない関数を別スレッドから呼び出すときに使える
 * 例 (別スレッドのプログラムなどから)
 *     TaskSystem.getInstance().addTask(  ()=>{  gameObject.transform.position = new Vector(0,0,0);   }  );
 *   
 *  </summary>
 **/

    public class TaskSystem : MonoBehaviour
    {

        Queue<Action> action_queue;
        Queue<Action> fixed_action_queue;
        private static TaskSystem instance;
        // Use this for initialization
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                print("Duplicate TaskSytem! self destruct!");
            }
            else
            {
                instance = this;
                action_queue = new Queue<Action>();
                fixed_action_queue = new Queue<Action>();
                //DontDestroyOnLoad(gameObject);
            }
        }

        public static TaskSystem getInstance()
        {
            return instance;
        }

        public void addTask(Action func)
        {
            lock (action_queue)
            {
                action_queue.Enqueue(func);
            }
        }

        public void addTaskToFixedUpdate(Action func)
        {
            lock (fixed_action_queue)
            {
                fixed_action_queue.Enqueue(func);
            }
        }

        // Update is called once per frame
        void Update()
        {
            lock (action_queue)
            {
                while (action_queue.Count > 0)
                {
                    try
                    {
                        action_queue.Dequeue()();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }

        }

        void FixedUpdate()
        {
            lock (fixed_action_queue)
            {
                while (fixed_action_queue.Count > 0)
                    fixed_action_queue.Dequeue()();
            }
        }
    }
}