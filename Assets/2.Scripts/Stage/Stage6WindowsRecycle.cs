using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage6WindowsRecycle : MonoBehaviour
{
    //移动到x=11的时候最右面的一组到最左面填充 local position

    [Header("从左向右")]
    public Transform[] WindowsGroups;

    /// <summary>
    /// 开始滚动的时间
    /// </summary>
    float RollStartTime = 0f;
    /// <summary>
    /// WindowsGroups哪一个在最左面
    /// </summary>
    int Left = 0;

    /// <summary>
    /// WindowsGroups中哪一个在最右面
    /// </summary>
    int Right = 3;



    public void LetsRoll()
    {
        RollStartTime = Time.timeSinceLevelLoad;
    }


    //这个场景很快就没了，直接用这个方法好了
    void Update()
    {
        //玩家到位，禁止输入，并且开始滚动
        if(ExMath.Approximation(1.0F, -25.87552F, MountGSS.gameScoreSettings.PlayersPosition[0].x) && !MountGSS.gameScoreSettings.BanInput)
        {
            MountGSS.gameScoreSettings.BanInput = true;
            LetsRoll();
        }

        if(RollStartTime == 0f)
        {
            return;
        }

        for (int i = 0; i < WindowsGroups.Length; i++)
        {
            //不断加快移动
            WindowsGroups[i].Translate( Time.deltaTime * 15f * (Time.timeSinceLevelLoad - RollStartTime) * Vector2.right, Space.World);
        }


        //如果最右面的到达x=11了 25.13
        if (ExMath.Approximation(2.0f, WindowsGroups[Right].localPosition.x, 11f))
        {
            //把到位的移动到最左面的左侧
            WindowsGroups[Right].localPosition = new Vector3(WindowsGroups[Left].localPosition.x - 25.13f, WindowsGroups[Left].localPosition.y, WindowsGroups[Left].localPosition.z);

            //修正左右两侧的序号
            Left = Right;
            if (Right == 0)
            {
                Right = 3;
            }
            else
            {
                Right--;

            }
        }
    }
}
