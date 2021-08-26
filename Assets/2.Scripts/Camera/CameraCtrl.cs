using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

public class CameraCtrl : MonoBehaviour
{
    public static CameraCtrl cameraCtrl;

    public Transform[] AirWalls;

    /// <summary>
    /// ���
    /// </summary>
  [HideInInspector] public  Transform tr;

    public Transform[] Players;
  [HideInInspector]  public Transform TargetedPlayer;

    public StopPoint[] StopPoints;

    public CameraRestraint[] cameraRestraints;

    private void Awake()
    {
        cameraCtrl = this;
        FixAirWallPos();
    }

    /// <summary>
    /// ��������ǽ��λ��
    /// </summary>
    void FixAirWallPos()
    {
        //�õ���Ļ����
        float width = Screen.width;
        float height = Screen.height;
        //��������ǽλ��
        AirWalls[0].localPosition = new Vector2(-5.1525f * width / height, AirWalls[0].localPosition.y);
        AirWalls[1].localPosition = new Vector2(5.1525f * width / height, AirWalls[1].localPosition.y);
    }

    public void Start()
    {
        //������Ϸ�������������ϰ󶨺����
        TargetedPlayer = Players[(int)MountGSS.gameScoreSettings.SelectedGirlInGame[0]].transform;
        tr = transform;

        //��ʼ�����Լ��
        cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo].Initialize(tr,TargetedPlayer);

        UpdateManager.updateManager.FastUpdate.AddListener(UpdateCamera);
        
       /*
        //������ϰ󶨺����
        for (int i = 0; i < 3; i++)
        {
            //�������û�ˣ�����ѭ��
            if(MountGSS.gameScoreSettings.SelectedGirlInGame[i] == Variable.PlayerFaceType.Null)
            {
                break;
            }
        }*/
    }

    /// <summary>
    /// ���ֹͣ������ƣ��ָ�����ƶ�
    /// </summary>
    public void RecoverMoving()
    {
        StopPoints[(int)MountGSS.gameScoreSettings.BattlingMajo].CancelStop();
    }

    public void UpdateCamera()
    {
        //�������Ƶ�
       cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo]. UpdatePoint();
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
      Vector2 c =  cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo].RepairCameraMoveDirection(MountGSS.gameScoreSettings.PlayerMove,TargetedPlayer.position);

        tr.Translate(c, Space.World);

    }

    #region ����ģʽ�����һ�ࣩ
    /// <summary>
    /// ��ʽģʽ��ť��������������ƶ�
    /// </summary>
    [ContextMenu("AllowCameraMoving")]
    public void AllowCameraMoving()
    {
        RecoverMoving();
    }
    #endregion
}


