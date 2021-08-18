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
        /// ��Ϸ�д�����Щ�����ĸ�����
        /// </summary>
        public Transform root;

        /// <summary>
        /// �����������Ԥ��
        /// </summary>
        public GameObject goPrefeb;

        /// <summary>
        /// ��ʼ��һ�������
        /// </summary>
        /// <param name="PoolName">���������</param>
        /// <param name="InitialCapacity">��ʼ�����г���</param>
        /// <param name="Root">������</param>
        public ObjectPoolForGameObject(string PoolName, int InitialCapacity, GameObject gameObject, Transform Root = null)
        {
            //δ���ø����壬�Զ�����һ��
            if (Root == null)
            {
                root = new GameObject(PoolName).AddComponent<Transform>();
            }
            else
            {
                //���ø�����
                root = Root;
                //�����趨���������
                root.gameObject.name = PoolName;
            }

            //����Ԥ��
            goPrefeb = gameObject;

            //��ʼ������
            container = new Queue(InitialCapacity);
            //װ�϶���
            AddObjectsToPool(InitialCapacity);


        }


        /// <summary>
        /// ��ӵ������壨�Ѿ�ʵ���������岻���� ��ӣ�
        /// </summary>
        /// <param name="gameObject">Ҫ��ӵ�����</param>
        public void AddObjectToPool()
        {
            GameObject go = GameObject.Instantiate(goPrefeb);
            go.SetActive(false);
            go.transform.SetParent(root);
            container.Enqueue(go);
        }

        /// <summary>
        /// ��Ӷ�����壨�Ѿ�ʵ���������岻���� ��ӣ�
        /// </summary>
        /// <param name="gameObjects">Ҫ��ӵ�����</param>
        public void AddObjectsToPool(int Number)
        {
            for (int i = 0; i < Number; i++)
            {
                AddObjectToPool();
            }
        }

        /// <summary>
        /// �ӳ����а���˳��ȡ��һ������
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
        /// �ӳ�����ȡ���������
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
        /// ��������
        /// </summary>
        public void RecycleObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            container.Enqueue(gameObject);
        }

    }


}
