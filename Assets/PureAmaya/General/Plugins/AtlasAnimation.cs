using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using MEC;
using UnityEngine.Events;

namespace PureAmaya.General
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
  public class AtlasAnimation : MonoBehaviour
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
        public SpriteRenderer SpriteRenderer;

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

        #region 用于储存暂停的动画
        bool IsPaused = false;
        /// <summary>
        /// 被暂停的动画的id
        /// </summary>
        #endregion



        #region 事件组
        public class CommonEvent : UnityEvent<string> { }
        /// <summary>
        /// 非循环动画结束事件
        /// </summary>
        public CommonEvent AnimStop = new CommonEvent();
        #endregion


        private void Awake()
        {
            if(SpriteRenderer == null)
            {
                SpriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (PlayDefaultAnimation)
            {
                ChangeAnimation(DefaultAnimationNameInAtlas);
            }

        }

        public void OnEnable()
        {
            //重启动画
                ChangeAnimation(PlayingId,true);
        }
        private void OnDisable()
        {
            BeDisabled = true;
            CancelInvoke("NewPlayAnimation");
        }

        [ContextMenu("播放DefaultAnimationNameInAtlas动画")]
        public void PlayAnimInEditor()
        {
          ChangeAnimation(DefaultAnimationNameInAtlas);
        }


        /// <summary>
        /// 更换正在播放的动画
        /// </summary>
        /// <param name="AnimationNameId">那个数组中的id</param>
        /// <param name="Forced">强制播放</param>
        public void ChangeAnimation(int AnimationId,bool Forced = false)
        {
            //id小于-1直接无视
            if(AnimationId < 0) { return; }

            //垃圾代码

            if(!BeDisabled)//上一帧正常运行
            {
                if (AnimationId != PlayingId || Forced)//阻止多次调用同一动画
                {

                    BeDisabled = false;
                    PlayingId = AnimationId;

                        //新的动画播放器
                        CancelInvoke("NewPlayAnimation");
                        InvokeRepeating("NewPlayAnimation", 0f, AtlasAnimations[AnimationId].Interval);
                    

                    //判断该动画是否从哪里开始，但是仅用来在检查视图中显示
                  PlayingSpriteId = AtlasAnimations[AnimationId].StartFromId;
                   


                }
            }

            else//如果上一帧是因为被禁用物体而停止播放动画，则强制播放动画
            {

                    //新的动画播放器
                    CancelInvoke("NewPlayAnimation");
                    InvokeRepeating("NewPlayAnimation", 0f, AtlasAnimations[AnimationId].Interval);

                PlayingSpriteId = AtlasAnimations[AnimationId].StartFromId;
                BeDisabled = false;
                PlayingId = AnimationId;
            }

        }

        /// <summary>
        /// 暂停动画
        /// </summary>
        /// <param name="time"></param>
        public void PauseAnimation()
        {
            //取消播放
            CancelInvoke("NewPlayAnimation");
            //修改状态
            IsPaused = true;
        }

        /// <summary>
        /// 从暂停位置继续播放
        /// </summary>
        public void ContinueAnimation()
        {
            if (IsPaused)
            {
                InvokeRepeating("NewPlayAnimation", 0f, AtlasAnimations[PlayingId].Interval);
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

            int d = 0;
            //处理 另一个循环（修复起始点循环和常规循环）
            if (AtlasAnimations[PlayingId].EndWithId != -1)
            {
                //到预定位置停止播放以后的图像
                d = AtlasAnimations[PlayingId].EndWithId;
            }
            else
            {
                //直接戳出来
                d = AtlasAnimations[PlayingId].Length - 1;

            }

            //如果动画播放完了
            if (PlayingSpriteId == d)
            {
                if (AtlasAnimations[PlayingId].Cycle)//允许循环
                {

                    //修复起始点循环
                    if (AtlasAnimations[PlayingId].BeginningRepair) { PlayingSpriteId = AtlasAnimations[PlayingId].RepairedFirstAtlasId; }
                    //不修复起始点循环
                    else
                    {
                        //从指定位置开始播放动画
                        PlayingSpriteId = AtlasAnimations[PlayingId].StartFromId;
                    }
                }

                //不循环动画
                else
                {
                    //调用非循环动画结束事件
                    AnimStop.Invoke(AtlasAnimations[PlayingId].name);
                    //停止该动画播放
                    CancelInvoke("NewPlayAnimation");

                    //  break;
                }
            }

            //动画没有播放到最后一张图片（或预定照片），继续
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
        [Obsolete("use NewPlayAnimation instead",true)]
        IEnumerator<float> PlayAnimation(int id)
        {
            yield return Timing.WaitForOneFrame;
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
            [Header("动画从哪开始")]
            public int StartFromId = 0;
            [Header("动画到哪里结束(-1:直接播放完)")]
            public int EndWithId = -1;
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