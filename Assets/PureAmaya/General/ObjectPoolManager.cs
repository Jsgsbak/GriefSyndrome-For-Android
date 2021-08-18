using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PureAmaya.General
{
    public class ObjectPoolForGameObject
    {
        public Queue container;
        /// <summary>
        /// 游戏中储存那些东西的父对象
        /// </summary>
        public Transform root;

        /// <summary>
        /// 池子中物体的预设
        /// </summary>
        public GameObject goPrefeb;

        /// <summary>
        /// 初始化一个缓冲池
        /// </summary>
        /// <param name="PoolName">缓冲池名字</param>
        /// <param name="InitialCapacity">初始化队列长度</param>
        /// <param name="Root">根物体</param>
        public ObjectPoolForGameObject(string PoolName, int InitialCapacity, GameObject gameObject, Transform Root = null)
        {
            //未设置根物体，自动生成一个
            if (Root == null)
            {
                root = new GameObject(PoolName).AddComponent<Transform>();
            }
            else
            {
                //设置根物体
                root = Root;
                //重新设定缓冲池名字
                root.gameObject.name = PoolName;
            }

            //储存预设
            goPrefeb = gameObject;

            //初始化队列
            container = new Queue(InitialCapacity);
            //装上东西
            AddObjectsToPool(InitialCapacity);


        }


        /// <summary>
        /// 添加单个物体（已经实例化的物体不够用 添加）
        /// </summary>
        /// <param name="gameObject">要添加的物体</param>
        public void AddObjectToPool()
        {
            GameObject go = GameObject.Instantiate(goPrefeb);
            go.SetActive(false);
            go.transform.SetParent(root);
            container.Enqueue(go);
        }

        /// <summary>
        /// 添加多个物体（已经实例化的物体不够用 添加）
        /// </summary>
        /// <param name="gameObjects">要添加的物体</param>
        public void AddObjectsToPool(int Number)
        {
            for (int i = 0; i < Number; i++)
            {
                AddObjectToPool();
            }
        }

        /// <summary>
        /// 从池子中按照顺序取出一个物体
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public GameObject GetObjectFromPool()
        {
            if (container.Count == 0)
            {
                AddObjectToPool();
            }

            GameObject go = container.Dequeue() as GameObject;
            go.SetActive(true);

            return go;

        }

        /// <summary>
        /// 从池子中取出多个物体
        /// </summary>
        /// <returns></returns>
        public GameObject[] GetObjectsFromPool(int Number)
        {
            GameObject[] goes = new GameObject[Number];
            for (int i = 0; i < Number; i++)
            {
                goes[i] = GetObjectFromPool();
            }
            return goes;
        }

        /// <summary>
        /// 回收物体
        /// </summary>
        public void RecycleObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            container.Enqueue(gameObject);
        }

    }


}
