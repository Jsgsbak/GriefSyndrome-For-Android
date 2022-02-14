using System.Collections.Generic;
using UnityEngine;

//相机那些球球的间隔不要太小，玩家的Raw输入为Move函数输入到的最终结果

/// <summary>
/// 约束相机的单向移动轨迹（线性约束）
/// </summary>
public class CameraRestraint : MonoBehaviour
{
    /// <summary>
    /// 相机线性移动限制区域的锚点
    /// </summary>
    [Header("相机线性移动限制区域的锚点")]
    public LinarPoint[] CameraPoints;//下面的数组都是指这个

    public int InitialPosition;
    /// <summary>
    /// 相机的变换组件
    /// </summary>
    Transform tr;
    /// <summary>
    /// 储存相机的位置（防止反复NEW）
    /// </summary>
    Vector2 vector2;
    /// <summary>
    /// 刚刚路过哪一个点？
    /// </summary>
    int PassedIndex;
    /// <summary>
    /// 确定相机可能会向着的点，并进行划线处理
    /// </summary>
    int NextPointDrawingLine;
    /// <summary>
    /// 下一个点在左面吗
    /// </summary>
    bool NextPointRight = false;

    public Transform Target;
    /// <summary>
    /// 初始化
    /// </summary>
    public void Initialize(Transform Camera, Transform target)
    {
        Target = target;
        tr = Camera;
        //初始化位置
        tr.SetPositionAndRotation(new Vector3(CameraPoints[InitialPosition].Point.x, CameraPoints[InitialPosition].Point.y, -10f), Quaternion.identity);
        PassedIndex = InitialPosition;

        
        for (int i = 0; i < CameraPoints.Length; i++)
        {
            //修复点0→1但没有1→0而引起的不能移动的问题（其实应该双向都能移动）
            foreach (var item in CameraPoints[i].ConnectPointIndex)
            {
                if (CameraPoints[item].ConnectPointIndex.Contains(i))
                {
                    continue;
                }
                CameraPoints[item].ConnectPointIndex.Add(i);
            }
        }

    }

    /// <summary>
    /// 直接跳跃到某个点
    /// </summary>
    public void JumpToPoint(int index)
    {
        tr.SetPositionAndRotation(new Vector3(CameraPoints[index].Point.x, CameraPoints[index].Point.y, -10f), Quaternion.identity);
        PassedIndex = index;
        NextPointDrawingLine = index + 1;
    }

    public void UpdatePoint()
    {
        //更新上一次经过的点的序号            
        vector2 = tr.position;

        //到达一对允许往回走的点中的小的点时（序号大的允许小的不允许），不更新刚经过的点序号
        if (CameraPoints[Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1)].AllowGoBack && !CameraPoints[PassedIndex].AllowGoBack)
        {

        }
        else
        {
            //某个点和相机的位置离得足够近，则认为这个点是新的“刚经过的点”
            if ((vector2 - CameraPoints[Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1)].Point).sqrMagnitude <= 0.1f)
            {
                PassedIndex = Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1);
            }

        }


        //直接取大一个作为下一个要经过的点
        NextPointDrawingLine = Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1);
        //确定序号大一个的点在左面还是右面
        NextPointRight = CameraPoints[NextPointDrawingLine].Point.x - CameraPoints[PassedIndex].Point.x > 0f;





    }

    /*
    public Vector2 RepairCameraMoveDirection(Vector2 Target)
    {
        switch (CameraPoints[PassedIndex].BanLeftRight)
    }*/

    /// <summary>
    /// 修正相机移动方向
    /// </summary>
    /// <param name="Raw">玩家输入的移动方向</param>
    /// <param name="Player">玩家的位置（全局）</param>
    public Vector2 RepairCameraMoveDirection(Vector2 Raw, Vector2 Player)
    {
        //相机在某个点上了
        if ((vector2 - CameraPoints[PassedIndex].Point).sqrMagnitude <= 0.1f)
        {
            //确定（修改）水平移动方向
            if (Raw.x > 0 && !CameraPoints[PassedIndex].BanLeftRight[1])
            {
                return MoveCamera(Raw);
            }
            else if (Raw.x < 0 && !CameraPoints[PassedIndex].BanLeftRight[0])
            {
                return MoveCamera(Raw);
            }
            else
            {
                //RAW移动方向与点左右移动限制有冲突，则不移动相机
                return Vector2.zero;
            }
        }
        //相机在线上，按着线随便动
        else
        {
            //那些允许往回走的线，直接按照玩家输入的方向走好了（还要求距离两个点有一定的举例
            if (CameraPoints[NextPointDrawingLine].AllowGoBack)
            {
                switch (NextPointRight)
                {
                    //下一个点在刚经过的点的右面
                    case true:

                        //玩家向右走，并且玩家在屏幕右侧，离着右面的下一个点很近
                        if (Raw.x > 0f && Player.x > tr.position.x && (CameraPoints[NextPointDrawingLine].Point - vector2).sqrMagnitude <= 0.1F)
                        {
                            return Vector2.zero;
                        }
                        //玩家向左走，并且玩家在屏幕左侧，离着左面的刚过的点很近
                        else if (Raw.x < 0f && Player.x < tr.position.x && (CameraPoints[PassedIndex].Point - vector2).sqrMagnitude <= 0.1F)
                        {
                            return Vector2.zero;
                        }
                        //玩家向左走，并且玩家在屏幕左侧，离着左面的刚经过的点很远
                        else if (Raw.x < 0f && Player.x < tr.position.x && (CameraPoints[PassedIndex].Point - vector2).sqrMagnitude > 0.1F)
                        {
                            return MoveCamera(Raw);
                        }
                        //玩家向右走，并且玩家在屏幕右侧，离着右面的下一个点很远
                        else if (Raw.x > 0f && Player.x > tr.position.x && (CameraPoints[NextPointDrawingLine].Point - vector2).sqrMagnitude > 0.1F)
                        {
                            //快点动，追上去
                            return MoveCamera(Raw * 1.2f);
                        }
                        else
                        {
                            return Vector2.zero;
                        }
                    //下一个点在刚经过的点的左面
                    case false:
                        //玩家向右走，并且玩家在屏幕右侧，离着右面的刚经过的点很近
                        if (Raw.x > 0f && Player.x > tr.position.x && (CameraPoints[PassedIndex].Point - vector2).sqrMagnitude <= 0.1F)
                        {
                            Debug.Log("2");
                            return Vector2.zero;
                        }
                        //玩家向左走，并且玩家在屏幕左侧，离着左面的要经过的点很近
                        else if (Raw.x < 0f && Player.x < tr.position.x && (CameraPoints[NextPointDrawingLine].Point - vector2).sqrMagnitude <= 0.1F)
                        {
                            Debug.Log("3");
                            return Vector2.zero;
                        }
                        //玩家向左走，并且玩家在屏幕左侧，离着左面的下一个过的点很远
                        else if (Raw.x < 0f && Player.x < tr.position.x && (CameraPoints[NextPointDrawingLine].Point - vector2).sqrMagnitude > 0.1F)
                        {
                            Debug.Log("5");
                            return MoveCamera(Raw);
                        }
                        //玩家向右走，并且玩家在屏幕右侧，离着右面的刚经过的点很远
                        else if (Raw.x > 0f && Player.x > tr.position.x && (CameraPoints[PassedIndex].Point - vector2).sqrMagnitude > 0.1F)
                        {
                            Debug.Log("6");
                            return MoveCamera(Raw);
                        }
                        else
                        {
                            return Vector2.zero;
                        }
                }


            }

            //不能回头
            else
            {
                switch (NextPointRight)
                {
                    //下一个点在刚经过的点的右面
                    case true:
                        //玩家向右走，并且玩家在屏幕中间
                        if (Raw.x > 0f && Mathf.Abs(Player.x - tr.position.x) <= 0.1f)
                        {
                            //相机顺着移动过去
                            return MoveCamera(Raw);
                        }
                        //玩家向右走，并且玩家在屏幕右侧
                        else if (Raw.x >= 0f && Player.x - tr.position.x > 0.1f)
                        {
                            //相机多移动一点，为了跟上玩家
                            return MoveCamera(Raw * 1.2F);
                        }
                        else
                        {
                            return Vector2.zero;
                        }
                    //下一个点在刚经过的点的左面
                    case false:
                        //玩家向左走，并且玩家在屏幕中间
                        if (Raw.x < 0f && Mathf.Abs(Player.x - tr.position.x) <= 0.1f)
                        {
                            return MoveCamera(Raw);
                        }
                        //玩家向左走，并且玩家在屏幕左侧
                        else if (Raw.x <= 0f && Player.x - tr.position.x < -0.1f)
                        {
                            //相机多移动一点，为了跟上玩家
                            return MoveCamera(Raw * 1.2F);
                        }
                        else
                        {
                            return Vector2.zero;
                        }
                }
            }



        }


    }

    /// <summary>
    /// 移动相机（这个能执行，说明相机在线上跑）
    /// </summary>
    /// <param name="Raw">玩家原始输入（一点也不原始，被AplayerCtrl加工过了）</param>
    /// <returns></returns>
    Vector2 MoveCamera(Vector2 Raw)
    {
        //确定刚经过的点与获取的下一个点的方向（左右不定）
        Vector2 direction = (CameraPoints[NextPointDrawingLine].Point - CameraPoints[PassedIndex].Point).normalized;
        //计算玩家输入的移动向量与向量direction的角度
        float angle = Vector2.Angle(Raw, direction);
        //返回修正值
        return Raw.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad) * direction;
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < CameraPoints.Length; i++)
        {
            //点那里画个球 不这样子写位置TM不显示
            Gizmos.DrawSphere(new Vector3(CameraPoints[i].Point.x, CameraPoints[i].Point.y, -3f), 0.6f);

            //准备19个应该够了  标号
            if (i <= 9)
            {

                Gizmos.DrawIcon(new Vector3(CameraPoints[i].Point.x, CameraPoints[i].Point.y + 1f, 1f), string.Format("{0}.psd", i.ToString()), true, Color.black);
            }
            else
            {
                Gizmos.DrawIcon(new Vector3(CameraPoints[i].Point.x, CameraPoints[i].Point.y + 1f, 1f), "1.psd", true, Color.black);
                Gizmos.DrawIcon(new Vector3(CameraPoints[i].Point.x, CameraPoints[i].Point.y + 1f, 1f), string.Format("{0}.psd", i.ToString()), true, Color.black);
            }



            foreach (var item in CameraPoints[i].ConnectPointIndex)
            {
                //找到所有可以与这个点连接的点，然后划线
                if (CameraPoints[i].AllowGoBack)
                {
                    //允许走回去，画绿线
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(CameraPoints[i].Point, CameraPoints[item].Point);
                }
                else
                {
                    //允许走回去，画红线
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(CameraPoints[i].Point, CameraPoints[item].Point);
                }
                //检查：如果有两个相连的点允许玩家返回，则报错
                if (CameraPoints[item].AllowGoBack && CameraPoints[i].AllowGoBack)
                {
                    Debug.LogErrorFormat("编号{0}与{1}的AllowGoBack设定冲突或不必要", item.ToString(), i.ToString());
                }

            }

        }

    }
#endif



    [System.Serializable]
    public class LinarPoint
    {
        [Header("锚点")]
        public Vector2 Point = new();
        [Header("能连接的点的序号")]
        public List<int> ConnectPointIndex = new();
        /// <summary>
        /// 点的左右能否移动 0左1右
        /// </summary>
        public bool[] BanLeftRight = { false, false };

        /// <summary>
        /// 是否允许玩家往回走（必须是前往排序序号比较小的地方）
        /// </summary>
        public bool AllowGoBack = false;
    }


}
