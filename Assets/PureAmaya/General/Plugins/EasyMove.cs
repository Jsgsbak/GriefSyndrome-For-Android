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

        [Header("单摆转动")]
        public bool AllowPendulum = false;
        public float Amplitude = 1f;
        public float Cycle = 10f;
        public Vector3 AxisForPendulum = Vector3.forward;
        public Space PendulumSpace = Space.World;
        AnimationCurve ac;

        Transform tr;
        GameObject go;

        // Start is called before the first frame update
        void Start()
        {
            tr = transform;
            go = gameObject;

            //周期旋转
            if (AllowRotateTime)
            {
                InvokeRepeating("RotateForTime", 0f, Interval);
            }

            //初始化单摆的函数曲线
            if (AllowPendulum)
            {
                ac = new AnimationCurve(new Keyframe(0f, Amplitude, 0f, 0f), new Keyframe(Cycle / 4, 0,-Amplitude, -Amplitude), new Keyframe(Cycle / 2, -Amplitude, 0f, 0f), new Keyframe(Cycle *3 / 4, 0, Amplitude, Amplitude), new Keyframe(Cycle, Amplitude, 0f, 0f));
                ac.preWrapMode = WrapMode.Loop;
                ac.postWrapMode = WrapMode.Loop;
            }
        }

        void RotateForTime()
        {
            tr.Rotate(AxisForTime * Angle, RotSpaceForTime);
        }


        private void OnEnable()
        {
            //注册Update
            UpdateManager.FastUpdate.AddListener(FastUpdate);
        }

        private void OnDisable()
        {
            //移除Update
            UpdateManager.FastUpdate.RemoveListener(FastUpdate);
        }

        // Update is called once per frame
        void FastUpdate()
        {
            //仅限被激活后转动
            if (go.activeInHierarchy)
            {
                //移动
                if (AllowMove)
                {
                    tr.Translate(Direction * Speed * Time.deltaTime, MoveSpace);
                }
                else if (AllowFanFuHengTiao)//反复横跳
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
                //转动
                if (AllowRotate)
                {
                    tr.Rotate(Axis * RotSpeed * Time.deltaTime, RotSpace);
                }

                //单摆
                if (AllowPendulum)
                {
                    tr.Rotate(AxisForPendulum * ac.Evaluate(Time.timeSinceLevelLoad), PendulumSpace);
                }

            }
        }
    }
}
