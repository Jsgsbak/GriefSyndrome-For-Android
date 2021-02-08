using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 变量声明（？）
/// </summary>
public class Variable
{
    /// <summary>
    /// 分数类型
    /// </summary>
    ///
   public enum ScoreType
    {
        /// <summary>
        /// 最高分
        /// </summary>
        HiScore =0,
        /// <summary>
        /// 最佳用时
        /// </summary>
        BestTime,
        /// <summary>
        /// 最高连击
        /// </summary>
        MaxHits,
    }

    /// <summary>
    /// 魔法少女的全局id，用于面部设置，肖像设置与结局图判断
    /// </summary>
    public enum PlayerFaceType
    {
        Homura =0,
        Kyoko,
        Madoka =2,
        Mami,
        Sayaka =4,
        Homura_m,
        QB,
        Null,   
    }

    public enum Majo
    {
        /// <summary>
        /// 蔷薇魔女
        /// </summary>
        Gertrud =0,
        /// <summary>
        /// 零食魔女
        /// </summary>
        Charlotte =1,
        /// <summary>
        /// 箱之魔女
        /// </summary>
        Elly = 2,
        /// <summary>
        /// 影之魔女
        /// </summary>
        ElsaMaria =3,
        /// <summary>
        /// 舞台装置魔女
        /// </summary>
        Walpurgisnacht = 4,
        /// <summary>
        /// 人鱼魔女
        /// </summary>
        Oktavia = 5,

    }

    /// <summary>
    /// 魔法少女属性设置
    /// </summary>
    [System.Serializable]
    public class MahouShoujo
    {
        public string name;
        [Header("初始攻击力")]
        public int BasicPow = 10;
        [Header("攻击力成长值")]
        public int PowGrowth = 1;
        [Header("攻击力最大值")]
        public int MaxPow = 108;

        [Header("移动速度")][SerializeField]
        public float MoveSpeed = 45f;

        [Space]
        [Header("初始灵魂值")]
        public int BasicSoulLimit;
        [Header("灵魂成长值")]
        public int SoulGrowth;
        [Header("灵魂最大值")]
        public int MaxSoul;

        [Space]
        [Header("初始HP")]
        public int BasicVit;
        [Header("HP成长值")]
        public int[] VitGrowth;
        [Header("HP成长阶段下限")]
        public int[] VitGrowthLevelLimit;
        [Header("HP最大值")]
        public int MaxVit;

        /// <summary>
        /// 回复消耗的Soul Limit关于损失Vit的倍数
        /// </summary>
        [Space]
        [Header("回复消耗的Soul Limit关于损失Vit的倍数")]
        public int Recovery = 18;
        /// <summary>
        /// 复活消耗的Soul Limit关于损失Vit最大值的倍数
        /// </summary>
        [Header("复活消耗的Soul Limit关于损失Vit最大值的倍数")]
        public int Rebirth = 30;

        [Space]
        [Header("发动时Magia消耗Vit数")]
        public int MaigaVit = 45;

        [Space]
        [Header("是能够蓄力的Magia吗")]
        public bool LongMagia = true;
    }

    /// <summary>
    /// 没有参数的通常事件
    /// </summary>
    public class OrdinaryEvent : UnityEvent { }
    public class BoolEvent : UnityEvent<bool> { }
    public class IntEvent : UnityEvent<int> { }


    /// <summary>
    /// 虚拟按键-全部按钮位置大小设置（参考分辨率1010x568)
    /// </summary>
    [System.Serializable]
    public class VirtualKeyInputPositionAndScale
    {
        public string name = "Button";
        /// <summary>
        /// UI内显示的文本
        /// </summary>
        [Header("UI内显示的文本")]
        public string UIName = "Button";
        /// <summary>
        /// 设置中用于编辑的Rect
        /// </summary>
        [Space]
        public Rect EditPosition;
        /// <summary>
        /// EditPosition经过分辨率修正之后的Rect
        /// </summary>
        [HideInInspector]
        public Rect PositionInUse;

        /// <summary>
        /// 默认设置，重置游戏之后读取这个设置
        /// </summary>
        public Rect RawPosition;

        /// <summary>
        /// 允许长按
        /// </summary>
        [Header("允许长按")]
        public bool AllowPress = true;
        }




}
