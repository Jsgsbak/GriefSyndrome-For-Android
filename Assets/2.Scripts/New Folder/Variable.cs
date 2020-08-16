using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public enum PlayerFaceType
    {
        Homura =0,
        Kyoko,
        Madoka,
        Mami,
        Sayaka,
        Homura_m,
        Null,   
        QB,
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

        [Space]
        [Header("回复消耗的Soul Limit关于损失Vit的倍数")]
        public int Recovery = 18;
        [Header("复活消耗的Soul Limit关于损失Vit最大值的倍数")]
        public int Rebirth = 30;

        [Space]
        [Header("发动时Maiga消耗Vit数")]
        public int MaigaVit = 45;
    }
}
