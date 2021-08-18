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
    /// 相机
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
        //初始化相机约束
        cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo].Initialize(tr);
    }

    /// <summary>
    /// 解除停止点的限制，恢复相机移动
    /// </summary>
    public void RecoverMoving()
    {
        StopPoints[(int)MountGSS.gameScoreSettings.BattlingMajo].CancelStop();
    }


    //由AplayerCtrl控制
    public void UpdateCamera()
    {
        //检查停止点
        StopPoints[(int)MountGSS.gameScoreSettings.BattlingMajo].CheckStopPoint(tr.position);
        //到停止点，停下
        if (!StopPoints[(int)MountGSS.gameScoreSettings.BattlingMajo].StopCamera)
        {
            Restraint();
        }

    }

    /// <summary>
    /// 相机约束
    /// </summary>
    void Restraint()
    {
      Vector2 c =  cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo].RepairCameraMoveDirection(MountGSS.gameScoreSettings.PlayerMove,Player.position);

        tr.Translate(c, Space.World);

    }
}
