///
/// FROM 
///
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Rigidbodyの速度を保存しておくクラス
/// </summary>
public class RigidbodyVelocity
{
    public Vector3 velocity;
    public float angularVelocity;
    public RigidbodyVelocity(Rigidbody2D rigidbody)
    {
        velocity = rigidbody.velocity;
        angularVelocity = rigidbody.angularVelocity;
    }
}
public interface IRecievePauseChange : IEventSystemHandler
{
    void OnPause();
    void OnResume();
}
public class Pausable : MonoBehaviour, IRecievePauseChange
{
    /// <summary>
    /// 現在Pause中か？
    /// </summary>
    public bool pausing;

    /// <summary>
    /// 無視するGameObject
    /// </summary>
    public GameObject[] ignoreGameObjects;

    /// <summary>
    /// ポーズ状態が変更された瞬間を調べるため、前回のポーズ状況を記録しておく
    /// </summary>
    bool prevPausing;

    /// <summary>
    /// Rigidbodyのポーズ前の速度の配列
    /// </summary>
    RigidbodyVelocity[] rigidbodyVelocities;

    /// <summary>
    /// ポーズ中のRigidbodyの配列
    /// </summary>
    Rigidbody2D[] pausingRigidbodies;

    /// <summary>
    /// ポーズ中のMonoBehaviourの配列
    /// </summary>
    MonoBehaviour[] pausingMonoBehaviours;
    void Start()
    {
        prevPausing = false;
    }
    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        // ポーズ状態が変更されていたら、Pause/Resumeを呼び出す。
        if (prevPausing != pausing)
        {
            if (pausing) Pause();
            else Resume();
            prevPausing = pausing;
        }
    }
    /// <summary>
    /// 中断
    /// </summary>
    void Pause()
    {
        // Rigidbodyの停止
        // 子要素から、スリープ中でなく、IgnoreGameObjectsに含まれていないRigidbodyを抽出
        Predicate<Rigidbody2D> rigidbodyPredicate =
            obj => !obj.IsSleeping() &&
                   Array.FindIndex(ignoreGameObjects, gameObject => gameObject == obj.gameObject) < 0;
        pausingRigidbodies = Array.FindAll(transform.GetComponentsInChildren<Rigidbody2D>(), rigidbodyPredicate);
        rigidbodyVelocities = new RigidbodyVelocity[pausingRigidbodies.Length];
        for (int i = 0; i < pausingRigidbodies.Length; i++)
        {
            // 速度、角速度も保存しておく
            rigidbodyVelocities[i] = new RigidbodyVelocity(pausingRigidbodies[i]);
            pausingRigidbodies[i].Sleep();
        }

        // MonoBehaviourの停止
        // 子要素から、有効かつこのインスタンスでないもの、IgnoreGameObjectsに含まれていないMonoBehaviourを抽出
        Predicate<MonoBehaviour> monoBehaviourPredicate =
            obj => obj.enabled &&
                   obj != this &&
                   Array.FindIndex(ignoreGameObjects, gameObject => gameObject == obj.gameObject) < 0;
        pausingMonoBehaviours = Array.FindAll(transform.GetComponentsInChildren<MonoBehaviour>(), monoBehaviourPredicate);
        foreach (var monoBehaviour in pausingMonoBehaviours)
        {
            monoBehaviour.enabled = false;
        }
    }
    /// <summary>
    /// 再開
    /// </summary>
    void Resume()
    {
        // Rigidbodyの再開
        for (int i = 0; i < pausingRigidbodies.Length; i++)
        {
            Debug.Log("Resume");
            pausingRigidbodies[i].WakeUp();
            pausingRigidbodies[i].velocity = rigidbodyVelocities[i].velocity;
            pausingRigidbodies[i].angularVelocity = rigidbodyVelocities[i].angularVelocity;
        }

        // MonoBehaviourの再開
        foreach (var monoBehaviour in pausingMonoBehaviours)
        {
            monoBehaviour.enabled = true;
        }
        //スクリプトの再開
        
    }

    public void OnPause()
    {
        this.pausing = true;
    }
    public void OnResume()
    {
        this.pausing = false;
    }
}