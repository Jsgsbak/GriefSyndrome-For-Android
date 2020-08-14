using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PureAmaya.General
{


    public class EasyMove : MonoBehaviour
    {
        public bool AllowMove = false;
        public Vector2 Direction = Vector2.zero;
        public Space MoveSpace = Space.Self;
        public float Speed = 40f;
        [Space()]
        public bool AllowRotate = false;
        public Vector3 Axis = Vector3.up;
        public float RotSpeed = 30f;
        public Space RotSpace = Space.Self;

        [Header("按照时间的转动")]
        public bool AllowRotateTime = false;
        public Vector3 AxisForTime = Vector3.up;
        public float Interval = 5f;
        public float Angle = 180f;
        public Space RotSpaceForTime = Space.Self;



        [Header("当允许移动时，该移动方式失效")]
        public bool AllowFanFuHengTiao = false;
        public Vector2 HengTiaoDirection = Vector2.up;
        public float HengTiaoSpeed = 40f;
        public Space HengTiaoSpace = Space.Self;
        public float TopBorder;
        public float BottomBorder;

        Transform tr;
        GameObject go;

        // Start is called before the first frame update
        void Start()
        {
            tr = transform;
            go = gameObject;

            UpdateManager.FastUpdate.AddListener(FastUpdate);

            //周期旋转
            if (AllowRotateTime)
            {
                InvokeRepeating("RotateForTime", 0f, Interval);
            }
        }

        void RotateForTime()
        {
            tr.Rotate(AxisForTime * Angle, RotSpaceForTime);
        }

        // Update is called once per frame
        void FastUpdate()
        {
            if (go.activeInHierarchy)
            {
                if (AllowMove)
                {
                    tr.Translate(Direction * Speed * Time.deltaTime, MoveSpace);
                }
                else if (AllowFanFuHengTiao)
                {
                    //修正方向
                    if (tr.position.y >= TopBorder && HengTiaoDirection.y > 0)
                    {
                        HengTiaoDirection = new Vector2(0f, -HengTiaoDirection.y);
                    }
                    else if (tr.position.y <= BottomBorder & HengTiaoDirection.y < 0)
                    {
                        HengTiaoDirection = new Vector2(0f, -HengTiaoDirection.y);
                    }

                    tr.Translate(HengTiaoDirection * HengTiaoSpeed * Time.deltaTime, HengTiaoSpace);
                }

                if (AllowRotate)
                {
                    tr.Rotate(Axis * RotSpeed * Time.deltaTime, RotSpace);
                }

            }
        }
    }
}
