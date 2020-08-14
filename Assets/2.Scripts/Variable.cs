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

       
    }
}
