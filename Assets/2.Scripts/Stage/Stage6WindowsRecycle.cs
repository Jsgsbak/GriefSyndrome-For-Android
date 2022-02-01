using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage6WindowsRecycle : MonoBehaviour
{
    //�ƶ���x=11��ʱ���������һ�鵽��������� local position

    [Header("��������")]
    public Transform[] WindowsGroups;

    /// <summary>
    /// ��ʼ������ʱ��
    /// </summary>
    float RollStartTime = 0f;
    /// <summary>
    /// WindowsGroups��һ����������
    /// </summary>
    int Left = 0;

    /// <summary>
    /// WindowsGroups����һ����������
    /// </summary>
    int Right = 3;



    public void LetsRoll()
    {
        RollStartTime = Time.timeSinceLevelLoad;
    }


    //��������ܿ��û�ˣ�ֱ���������������
    void Update()
    {
        //��ҵ�λ����ֹ���룬���ҿ�ʼ����
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
            //���ϼӿ��ƶ�
            WindowsGroups[i].Translate( Time.deltaTime * 15f * (Time.timeSinceLevelLoad - RollStartTime) * Vector2.right, Space.World);
        }


        //���������ĵ���x=11�� 25.13
        if (ExMath.Approximation(2.0f, WindowsGroups[Right].localPosition.x, 11f))
        {
            //�ѵ�λ���ƶ�������������
            WindowsGroups[Right].localPosition = new Vector3(WindowsGroups[Left].localPosition.x - 25.13f, WindowsGroups[Left].localPosition.y, WindowsGroups[Left].localPosition.z);

            //����������������
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
