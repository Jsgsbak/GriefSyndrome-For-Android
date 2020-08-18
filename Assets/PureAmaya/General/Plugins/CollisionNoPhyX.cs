using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PureAmaya.General
{

    public class CollisionNoPhyX : MonoBehaviour
    {
        [Header("允许发生碰撞的物体的tag")]
        public string[] AllowCollisionTag = new string[] { "Player", "Enemy" };
        public bool IsTrigger = false;

        [Header("矩形碰撞")]
        public bool AllowRectCollision = false;
        public Vector2 point0;
        public Vector2 point1;

        /// <summary>
        /// 与挂脚本的物体发生碰撞的东西
        /// </summary>
        [HideInInspector] public List<Transform> Colliders = new List<Transform>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}