using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PureAmaya.General
{
    public class ObjectPoolManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
          //  new ObjectPoolForGameObject()
        }

        // Update is called once per frame
        void Update()
        {

        }
    }


    public class ObjectPoolForGameObject 
    {
        public Queue container;
        /// <summary>
        /// ��Ϸ�д�����Щ�����ĸ�����
        /// </summary>
        public Transform ContainerTr;

        public ObjectPoolForGameObject(string PoolName,int InitialCapacity,GameObject Member,Transform Root)
        {
            ContainerTr = new GameObject(PoolName).AddComponent<Transform>();
        }

    }


}
