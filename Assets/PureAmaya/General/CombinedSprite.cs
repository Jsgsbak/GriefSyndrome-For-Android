using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


namespace PureAmaya.General
{
    /// <summary>
    /// 用于组合弹幕/敌机的贴图合并
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class CombinedSprite : MonoBehaviour
    {
        [Header("所有的贴图合集")]
        public SpriteAtlas spriteAtlas;
        [Header("要使用贴图的对象")]
        public SpriteRenderer[] Object;
        [Header("每个对象要用到的贴图的名字")]
        public string[] Names;


        private void Start()
        {
            RefreshSprite();
        }

        /// <summary>
        /// 刷新贴图
        /// </summary>
        [ContextMenu("刷新贴图")]
        void RefreshSprite()
        {
            for (int i = 0; i < Object.Length; i++)
            {
                Object[i].sprite = spriteAtlas.GetSprite(Names[i]);
            }
        }
    }

}