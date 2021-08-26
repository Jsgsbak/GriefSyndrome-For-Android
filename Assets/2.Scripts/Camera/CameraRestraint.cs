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
    Transform  tr;
    /// <summary>
    /// 储存相机的位置（防止反复NEW）
    /// </summary>
    Vector2 vector2;
    /// <summary>
    /// 刚刚路过哪一个点？
    /// </summary>
    public int PassedIndex;
    /// <summary>
    /// 确定相机可能会向着的点，并进行划线处理
    /// </summary>
    public int NextPointDrawingLine;
    /// <summary>
    /// 下一个点在左面吗
    /// </summary>
  public  bool NextPointRight = false;

    public Transform Target;
    /// <summary>
    /// 初始化相机位置
    /// </summary>
    public void Initialize(Transform Camera, Transform target)
    {
        Target = target;
        tr = Camera;
        //初始化位置
        tr.SetPositionAndRotation(CameraPoints[InitialPosition].Point,Quaternion.identity);
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
        tr.SetPositionAndRotation(CameraPoints[index].Point, Quaternion.identity);
        PassedIndex = index;
        NextPointDrawingLine = index + 1;
    }

    public void UpdatePoint()
    {
        //更新上一次经过的点的序号            
        vector2 = tr.position;
       
        //不允许往回走
        if (!CameraPoints[PassedIndex].AllowGoBack)
        {
            //某个点和相机的位置离得足够近，则认为这个点是新的“刚经过的点”
            if ((vector2 - CameraPoints[Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1)].Point).sqrMagnitude <= 0.1f)
            {
                PassedIndex = Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1);
            }

        }
        //允许往回走
        else
        {
            //在上一次经过的点的前后还有他自己各取1个点
            for (int i = -1; i < 2; i++)
            {

                //某个点和相机的位置离得足够近，则认为这个点是新的“刚经过的点”
                if ((vector2 - CameraPoints[Mathf.Clamp(PassedIndex + i, 0, CameraPoints.Length - 1)].Point).sqrMagnitude <= 0.1f)
                {
                    PassedIndex = Mathf.Clamp(PassedIndex + i, 0, CameraPoints.Length - 1);
                }


            }


        }

        //确定刚经过的点之后，根据是否允许往较小的一个点走，获取下一个点
        if (!CameraPoints[PassedIndex].AllowGoBack)
        {
            //如果不允许玩家向序数较小的点（往回）走的话，直接取大一个
            NextPointDrawingLine = Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1);
            //确定序号大一个的点在左面还是右面
            NextPointRight = CameraPoints[NextPointDrawingLine].Point.x - CameraPoints[PassedIndex].Point.x > 0f;
        }
        //如果允许玩家向序数较小的点（往回）走的话，判断
        else
        {
            //仅保留数组更靠后的点，让摄像机不能往回走
            //如果刚路过的点在相机的右面
            if (tr.position.x - CameraPoints[PassedIndex].Point.x < 0)
            {
                //找一个在刚路过的点左面的点作为下一个点，并且找到的这个点与刚经过的点之间由连线
                for (int i = -1; i < 2; i += 2)
                {
                    if (CameraPoints[Mathf.Clamp(PassedIndex + i, 0, CameraPoints.Length - 1)].Point.x - CameraPoints[PassedIndex].Point.x < 0 && CameraPoints[Mathf.Clamp(PassedIndex + i, 0, CameraPoints.Length - 1)].ConnectPointIndex.Contains(PassedIndex))
                    {
                        NextPointRight = true;
                        NextPointDrawingLine = Mathf.Clamp(PassedIndex + i, 0, CameraPoints.Length - 1);
                    }
                }


            }
            //如果刚路过的点在相机的左面
            else
            {
                //找一个在刚路过的点右面的点作为下一个点，并且找到的这个点与刚经过的点之间由连线
                for (int i = -1; i < 2; i += 2)
                {
                    if (CameraPoints[Mathf.Clamp(PassedIndex + i, 0, CameraPoints.Length - 1)].Point.x - CameraPoints[PassedIndex].Point.x > 0 && CameraPoints[Mathf.Clamp(PassedIndex + i, 0, CameraPoints.Length - 1)].ConnectPointIndex.Contains(PassedIndex))
                    {
                        NextPointRight = false;
                        NextPointDrawingLine = Mathf.Clamp(PassedIndex + i, 0, CameraPoints.Length - 1);
                    }
                }
            }
        }

    }


    /// <summary>
    /// 修正相机移动方向
    /// </summary>
    /// <param name="Raw">玩家输入的移动方向</param>
    /// <param name="Player">玩家的位置（全局）</param>
    public Vector2 RepairCameraMoveDirection(Vector2 Raw,Vector2 Player)
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
            //那些允许往回走的线，直接按照玩家输入的方向走好了
            if (CameraPoints[PassedIndex].AllowGoBack)
            {

                if(Raw.x != 0)
                {
                    //相机顺着移动过去
                    return MoveCamera(Raw);
                }
                else
                {
                    return Vector2.zero;

                }

            }
            else
            {
                switch (NextPointRight)
                {
                    //下一个点在刚经过的点的右面
                    case true:
                        //玩家向右走，并且玩家在屏幕中间
                        if (Raw.x > 0 && Mathf.Abs(Player.x - tr.position.x) <= 0.1f)
                        {
                            //相机顺着移动过去
                            return MoveCamera(Raw);
                        }
                        //玩家向右走，并且玩家在屏幕右侧
                        else if (Raw.x >= 0 && Player.x - tr.position.x > 0.1f)
                        {
                            //相机多移动一点，为了跟上玩家
                            return MoveCamera(Raw * 1.2F);
                        }
                        {
                            return Vector2.zero;
                        }
                    //下一个点在刚经过的点的左面
                    case false:
                        //玩家向左走，并且玩家在屏幕中间
                        if (Raw.x < 0 && Mathf.Abs(Player.x - tr.position.x) <= 0.1f)
                        {
                            return MoveCamera(Raw);
                        }
                        //玩家向左走，并且玩家在屏幕左侧
                        else if (Raw.x <= 0 && Player.x - tr.position.x < -0.1f)
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
            Gizmos.DrawSphere(new Vector3(CameraPoints[i].Point.x, CameraPoints[i].Point.y,1f), 0.6f);

            //找到所有可以与这个点连接的点，然后划线
            foreach (var item in CameraPoints[i].ConnectPointIndex)
            {
                Gizmos.DrawLine(CameraPoints[i].Point, CameraPoints[item].Point);
            }

        }

    }
#endif



    [System.Serializable]
    public class LinarPoint
    {
        [Header("锚点")]
        public Vector2 Point;
        [Header("能连接的点的序号")]
        public List<int> ConnectPointIndex;
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
