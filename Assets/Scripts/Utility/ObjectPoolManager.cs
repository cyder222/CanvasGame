using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using CanvasGame.ObjectCreator;

namespace Assets.Utility
{

/**
 * ObjectPoolManagerクラス , ObjectPoolクラス
 * <summary>
 *   オブジェクトプールを作成し、呼び出すクラス。
 *    シングルトンで実装される。
 *    使い方
 *          まず、ObjectPoolManager.getInstance().createNewObjectPool(string オブジェクトプールのキー,
                                                                     int プールのオブジェクト数,Func ゲームオブジェクトをインスタンス化し返す関数);
 *          を使って、オブジェクトプールを作成。後はオブジェクトを作る時は
 *          ObjectPoolManager.getInstance().getPool(string キー名).getNewObject();
 *          オブジェクトを破棄するときは ObjectPoolManager.getInstance().getPool(string キー名).DestroyObject(GameObject 破棄するオブジェクト);        
 *   
 *   </summary>
 *
 **/

    public class ObjectPoolManager : MonoBehaviour
    {

        // Use this for initialization
        private static ObjectPoolManager instance = null;
        private Dictionary<string, GameObjectPool> object_pools;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                GameObject.DontDestroyOnLoad(gameObject);
                object_pools = new Dictionary<string, GameObjectPool>();
            }

        }

        void Update()
        {

        }

        public static ObjectPoolManager getInstance()
        {
            return instance;
        }

        public void DestroyAllObjectFromAllPool()
        {
            //全ての作成済みオブジェクトを消す
            foreach (ObjectType object_type in Enum.GetValues(typeof(ObjectType)))
            {
                Debug.Log("ObjectTYpe = " + object_type.ToString());
                try
                {
                    ObjectPoolManager.getInstance().getPool(object_type.ToString()).DestroyAllObject();
                }
                catch (Exception e)
                {
                    Debug.Log("Error on Destroy Object From All pool");
                }

            }
        }

        public GameObjectPool getPool(string key_name)
        {
            GameObjectPool ret_pool;
            if (object_pools.TryGetValue(key_name, out ret_pool))
                return ret_pool;
            else
                return null;
        }

        /// <summary>
        /// 新しいオブジェクトプールを作成する
        /// </summary>
        /// <param name="key_name">オブジェクトプールの名前</param>
        /// <param name="pool_size">オブジェクトプールのサイズ</param>
        /// <param name="instantiate_func">オブジェクトを生成する関数</param>
        /// <param name="container">Unityのインスペクタ上でオブジェクトの親となるゲームオブジェクト</param>
        public void createNewObjectPool(string key_name, int pool_size, Func<GameObject> instantiate_func,
            GameObject container = null)
        {
            if (!object_pools.ContainsKey(key_name))
                object_pools.Add(key_name,
                    new GameObjectPool(pool_size, instantiate_func,
                        container == null ? this.transform : container.transform));
        }
    }

    public class GameObjectPool
    {
        private string pool_type;
        private List<GameObject> prepare_list, executing_list;
        private Mutex mutex_lock = new Mutex();

        public GameObjectPool(int size, Func<GameObject> instantiate_func, Transform parent = null)
        {
            prepare_list = new List<GameObject>(size);
            executing_list = new List<GameObject>(size);

            for (int i = 0; i < size; i++)
            {
                GameObject game_object = instantiate_func();
                if (game_object == null) continue;
                if ((parent != null))
                {
                    game_object.transform.parent = parent;

                }
                game_object.SetActive(false);
                prepare_list.Add(game_object);
            }

            pool_type = typeof(GameObject).ToString();

        }

        public GameObject getNewObject()
        {
            lock (mutex_lock)
            {
                if (prepare_list.Count > 0)
                {

                    GameObject return_object = prepare_list[0];
                    prepare_list.RemoveAt(0);
                    return_object.SetActive(true);
                    executing_list.Add(return_object);

                    return return_object;
                }
                else
                {
                    throw new System.Exception("Object Pool Overflow Please increase your objectpool size of" +
                                               pool_type);
                }
            }

        }

        public int getExexutingObjectCount()
        {
            return executing_list.Count;
        }

        public int getPrepareObjectCount()
        {
            return prepare_list.Count;
        }

        public void DestroyObject(GameObject destroy_object)
        {
            lock (mutex_lock)
            {
                if (executing_list.Contains(destroy_object))
                {
                    executing_list.Remove(destroy_object);
                    destroy_object.SetActive(false);
                    prepare_list.Add(destroy_object);
                }
            }
        }

        public void DestroyAllObject()
        {
            GameObject[] array_for_loop = new GameObject[executing_list.Count];
            executing_list.CopyTo(array_for_loop);
            lock (mutex_lock)
            {
                foreach (GameObject game_object in array_for_loop)
                {
                    executing_list.Remove(game_object);
                    game_object.SetActive(false);
                    prepare_list.Add(game_object);
                }
            }
        }


    }

}