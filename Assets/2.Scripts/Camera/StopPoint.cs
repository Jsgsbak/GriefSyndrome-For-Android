using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制打怪相机停止点，不含地图两端空气墙那里的停止点
/// </summary>
public class StopPoint : MonoBehaviour
{
    /// <summary>
    /// 停止点
    /// </summary>
    [Tooltip("停止点")]
    public List<Vector2> PointsEditor;//保存世界坐标

#if UNITY_EDITOR
    [Header("游戏中实际用的点的颜色")]
    public Color GameUseColor = Color.red;
#endif

    /// <summary>
    /// 刚刚用过的停止点
    /// </summary>
    public int UsedPointIndex = -1;

    /// <summary>
    /// 停止相机移动
    /// </summary>
    public bool StopCamera = false;


    /// <summary>
    ///  检查停止点(世界坐标）
    /// </summary>
    /// <param name="Camera"></param>
    /// <returns>true：在停止点上，停止移动相机</returns>
    public void CheckStopPoint(Vector2 Camera)
    {
        //最后一个停止点激活后，不再检查停止点
        if(UsedPointIndex + 1 >= PointsEditor.Count)
        {
            return;
        }


        //靠近下一个停止点，停下相机
        if ((PointsEditor[UsedPointIndex +1] - Camera).sqrMagnitude <= 0.1f)
        {
            UsedPointIndex++;
            StopCamera = true;
        }
    }

    /// <summary>
    /// 停止停止点对相机的限制
    /// </summary>
    public void CancelStop()
    {
        StopCamera = false;
    }

#if UNITY_EDITOR

    public void OnDrawGizmos()
    {
        Gizmos.color = GameUseColor;


        for (int i = 0; i < PointsEditor.Count; i++)
        {
            //点那里画个球
            Gizmos.DrawSphere(new Vector3(PointsEditor[i].x, PointsEditor[i].y,1f), 0.8f);


        }

    }





#endif
}
