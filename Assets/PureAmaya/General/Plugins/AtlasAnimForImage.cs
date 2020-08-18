using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using MEC;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PureAmaya.General
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class AtlasAnimForImage : MonoBehaviour
    {
        /*放这里一个例子
        [BurstCompile]
        struct ddddddd : IJobParallelForTransform
        {
            public void Execute(int index, TransformAccess transform)
            {
                throw new NotImplementedException();
            }
        }
        */
        [Header("含有动画的图集")]
        public SpriteAtlas spriteAtlas;
        public Image SpriteRenderer;
        [Header("使用新的动画播放器")]
        public bool UseNewAnimPlayer = false;

        [Header("同一组动画的统一id（用于控制动画的停止）")]
        public string GroupName = "DefaultGroup";
        [Header("{0}表示GroupName，{1}表示name,{2}表示Length")]
        public string Format = "{0}_{1}_{2}";
        [Space]


        [Header("一旦被激活就进行默认动画")]
        /// <summary>
        /// 一旦被激活就进行默认动画"
        /// </summary>
        public bool PlayDefaultAnimation = true;
        [Header("默认动画：数组里的id")]
        public int DefaultAnimationNameInAtlas;

        /// <summary>
        /// 正在播放的动画id（数组里的）
        /// </summary>
        private int PlayingId = -1;
        /// <summary>
        /// 上一帧是否被禁用
        /// </summary>
        bool BeDisabled = false;
        /// <summary>
        /// 用数组来储存动画
        /// </summary>
        [SerializeField] AtlasAnimationInEditor[] AtlasAnimations;

        /// <summary>
        /// 在一个动画中正在使用的图片的id
        /// </summary>
        private int PlayingSpriteId = 0;





        #region 事件组
        public class CommonEvent : UnityEvent { }
        /// <summary>
        /// 非循环动画结束事件
        /// </summary>
        public CommonEvent AnimStop = new CommonEvent();
        #endregion


        private void Awake()
        {
            if (SpriteRenderer == null)
            {
                SpriteRenderer = GetComponent<Image>();
            }

            if (PlayDefaultAnimation)
            {
                ChangeAnimation(DefaultAnimationNameInAtlas);
            }

        }

        public void OnEnable()
        {
            //重启动画
            ChangeAnimation(PlayingId);
        }
        private void OnDisable()
        {
            BeDisabled = true;
            Timing.KillCoroutines(GroupName);
        }

        /// <summary>
        /// 更换正在播放的动画
        /// </summary>
        /// <param name="AnimationNameId">那个数组中的id</param>
        public void ChangeAnimation(int AnimationId)
        {

            //垃圾代码

            if (!BeDisabled)//上一帧正常运行
            {
                if (AnimationId != PlayingId)//阻止多次调用同一动画
                {

                    BeDisabled = false;
                    PlayingId = AnimationId;

                    if (!UseNewAnimPlayer)
                    {
                        //旧的动画播放器
                        Timing.KillCoroutines(GroupName);
                        Timing.RunCoroutine(PlayAnimation(AnimationId), GroupName);
                    }
                    else
                    {
                        //新的动画播放器
                        CancelInvoke("NewPlayAnimation");
                        InvokeRepeating("NewPlayAnimation", 0f, AtlasAnimations[AnimationId].Interval);
                    }

                    //判断该动画是否从0开始，但是仅用来在检查视图中显示
                    if (AtlasAnimations[AnimationId].StartFromZero)
                    {
                        PlayingSpriteId = 0;
                    }
                    else
                    {
                        PlayingSpriteId = 1;
                    }

                }
            }
            else//如果上一帧是因为被禁用物体而停止播放动画，则强制播放动画
            {

                if (!UseNewAnimPlayer)
                {
                    //旧的动画播放器
                    Timing.KillCoroutines(GroupName);
                    Timing.RunCoroutine(PlayAnimation(AnimationId), GroupName);
                }
                else
                {
                    //新的动画播放器
                    CancelInvoke("NewPlayAnimation");
                    InvokeRepeating("NewPlayAnimation", 0f, AtlasAnimations[AnimationId].Interval);
                }

                if (AtlasAnimations[AnimationId].StartFromZero)
                {
                    PlayingSpriteId = 0;
                }
                else
                {
                    PlayingSpriteId = 1;
                }
                BeDisabled = false;
                PlayingId = AnimationId;
            }

        }

        /// <summary>
        /// 控制播放的东西
        /// </summary>
        /// <param name="id">数组中一组动画的id</param>
        /// <returns></returns>
        void NewPlayAnimation()
        {
            //  while (true)
            //  {

            //设置图像
            SpriteRenderer.sprite = spriteAtlas.GetSprite(string.Format(Format, GroupName,
                                                                         AtlasAnimations[PlayingId].name, PlayingSpriteId.ToString()));

            //处理 另一个循环（修复起始点循环和常规循环）
            if (PlayingSpriteId == AtlasAnimations[PlayingId].Length - 1)
            {
                if (AtlasAnimations[PlayingId].Cycle)//允许循环
                {

                    //修复起始点循环
                    if (AtlasAnimations[PlayingId].BeginningRepair) { PlayingSpriteId = AtlasAnimations[PlayingId].RepairedFirstAtlasId; }
                    //不修复起始点循环
                    else
                    {
                        if (AtlasAnimations[PlayingId].StartFromZero)
                        {
                            PlayingSpriteId = 0;
                        }
                        else
                        {
                            PlayingSpriteId = 1;
                        }
                    }
                }

                //不循环动画
                else
                {
                    //调用非循环动画结束事件
                    AnimStop.Invoke();
                    //停止该动画播放
                    CancelInvoke("NewPlayAnimation");

                    //  break;
                }
            }

            //动画没有播放到最后一张图片，继续
            else
            {
                PlayingSpriteId++;
            }



            //  }
        }


        /// <summary>
        /// 控制播放的东西
        /// </summary>
        /// <param name="id">数组中一组动画的id</param>
        /// <returns></returns>
        [Obsolete("short time animation won't work properly")]
        IEnumerator<float> PlayAnimation(int id)
        {

            string AnimationName = string.Empty;

            while (true)
            {
                //异常报道用。提前获取变量
                AnimationName = string.Format(Format, GroupName, AtlasAnimations[id].name, PlayingSpriteId.ToString());
                if (AnimationName == null)
                {
                    Debug.LogWarning(string.Format("{0} {1}", "该图集中不包含：", AtlasAnimations[id].name));
                }

                yield return Timing.WaitForSeconds(AtlasAnimations[id].Interval);

                SpriteRenderer.sprite = spriteAtlas.GetSprite(string.Format(Format, GroupName,
                                                                             AtlasAnimations[id].name, PlayingSpriteId.ToString()));

                //处理 另一个循环（修复起始点循环和常规循环）
                if (PlayingSpriteId == AtlasAnimations[id].Length - 1)
                {
                    if (AtlasAnimations[id].Cycle)//允许循环
                    {

                        if (AtlasAnimations[id].BeginningRepair) { PlayingSpriteId = AtlasAnimations[id].RepairedFirstAtlasId; }
                        else
                        {
                            if (AtlasAnimations[id].StartFromZero)
                            {
                                PlayingSpriteId = 0;
                            }
                            else
                            {
                                PlayingSpriteId = 1;
                            }
                        }
                    }
                    else
                    {
                        //调用非循环动画结束事件
                        AnimStop.Invoke();
                        break;
                    }
                }
                else
                {
                    PlayingSpriteId++;
                }



            }
        }


        /// <summary>
        /// 提供一个数组来进行编辑
        /// </summary>
        [System.Serializable]
        public class AtlasAnimationInEditor
        {
            public string name;
            [Space(10)]
            public string Description;
            [Space(10)]

            public int Length;
            [Header("图像的id(Length)是否从0开始")]
            public bool StartFromZero = true;
            /// <summary>
            /// 动画循环吗
            /// </summary>
            [Header("动画能循环播放吗")]
            public bool Cycle = true;
            /// <summary>
            /// 循环间隔
            /// </summary>
            public float Interval = 0.1f;
            /// <summary>
            /// 起始点修复
            /// </summary>
            [Header("起始点修复")]
            public bool BeginningRepair = false;
            /// <summary>
            /// 完成一次循环且启用起始点修复后，一个周期内第一个图片的ID
            /// </summary> 
            [Header("完成一次循环且启用起始点修复后，一个周期内第一个图片的数组中的ID")]
            public int RepairedFirstAtlasId;

        }
    }
}