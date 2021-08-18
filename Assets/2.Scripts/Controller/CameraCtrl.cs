using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

public class CameraCtrl : MonoBehaviour
{
    public static CameraCtrl cameraCtrl;

    public float XMargin;
    public float YMargin;
    public float XSmooth = 8.0f;
    public float YSmooth = 8.0f;
    public Vector3 MaxXAndY;
    public Vector3 MinXAndY;

    /// <summary>
    /// ���
    /// </summary>
   public  Transform tr;

    public Transform Player;

    public StopPoint[] StopPoints;

    public CameraRestraint[] cameraRestraints;

    private void Awake()
    {
        cameraCtrl = this;
    }


    public void Start()
    {
        tr = transform;
        //��ʼ�����Լ��
        cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo].Initialize(tr);
    }

    /// <summary>
    /// ���ֹͣ������ƣ��ָ�����ƶ�
    /// </summary>
    public void RecoverMoving()
    {
        StopPoints[(int)MountGSS.gameScoreSettings.BattlingMajo].CancelStop();
    }


    //��AplayerCtrl����
    public void UpdateCamera()
    {
        //���ֹͣ��
        StopPoints[(int)MountGSS.gameScoreSettings.BattlingMajo].CheckStopPoint(tr.position);
        //��ֹͣ�㣬ͣ��
        if (!StopPoints[(int)MountGSS.gameScoreSettings.BattlingMajo].StopCamera)
        {
            Restraint();
        }

    }

    /// <summary>
    /// ���Լ��
    /// </summary>
    void Restraint()
    {
      Vector2 c =  cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo].RepairCameraMoveDirection(MountGSS.gameScoreSettings.PlayerMove,Player.position);

        tr.Translate(c, Space.World);

    }
}
