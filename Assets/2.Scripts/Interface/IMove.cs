using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌机/玩家用的移动接口
/// </summary>
public interface IMove 
{
    /// <summary>
    /// 移动方法
    /// </summary>
    /// <param name="Speed">移动速度</param>
    /// <param name="UseTimeDelta">是否使用TimeDelta</param>
    /// <param name="Slope">斜坡的角度（单位向量）</param>
    /// <param name="Direction">移动方向</param>
   void Move(float Speed, bool UseTimeDelta, Vector2 Direction);
}
