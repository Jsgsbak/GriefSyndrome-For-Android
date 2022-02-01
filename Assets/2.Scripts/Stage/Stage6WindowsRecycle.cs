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

    /// <summary>
    /// ��ɫ�������Ķ���
    /// </summary>
    public SpriteRenderer WhiteSwitch;

    /// <summary>
    /// ������
    /// </summary>
    public GameObject Portal;

    private void Awake()
    {
        //���ô���������ֹ���⴫��
        Portal.SetActive(false);
    }

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

            //�Ȳ���һ����Ч
            SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.Stage6Rolling,0.2f);
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

        //ʱ���㹻���ˣ���ʼ����
        if(Time.timeSinceLevelLoad - RollStartTime >= 4F)
        {

        }
        //�л��ɵڶ�������
        if(Time.timeSinceLevelLoad - RollStartTime > 5.5F)
        {
            //��ʱ���ˣ����Դ�����
            Portal.SetActive(true);
        }

        //���������ĵ���x=11�� 25.13
        if (ExMath.Approximation(2.0f, WindowsGroups[Right].localPosition.x, 11f))
        {
            //�ѵ�λ���ƶ�������������
            WindowsGroups[Right].localPosition = new Vector3(WindowsGroups[Left].localPosition.x - 25.13f, WindowsGroups[Left].localPosition.y, WindowsGroups[Left].localPosition.z);
            //������Ч
            SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.Stage6Rolling,0.3F);

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
