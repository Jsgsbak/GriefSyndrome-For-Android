using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ר���ṩһ��GSS���ô���ã�˳���ʼ��GSS��
/// </summary>
public class MountGSS : MonoBehaviour
{
    public static GameScoreSettingsIO gameScoreSettings;//��������Ūһ������

    // Start is called before the first frame update
    void Awake()
    {
        gameScoreSettings = (GameScoreSettingsIO)Resources.Load("GameScoreAndSettings");
        //��ʼ��
        gameScoreSettings.MajoInitial();

    }
}
