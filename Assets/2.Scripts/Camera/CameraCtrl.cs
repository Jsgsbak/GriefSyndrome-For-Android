using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

public class CameraCtrl : MonoBehaviour
{
    public static CameraCtrl cameraCtrl;

    public Transform[] AirWalls;

    /// <summary>
    /// 相机
    /// </summary>
  [HideInInspector] public  Transform tr;

    public Transform[] Players;
    /// <summary>
    /// 目标玩家（其实是多人游戏相机的目标位置啦）
    /// </summary>
   public Transform TargetedPlayer;

    public StopPoint[] StopPoints;

    public CameraRestraint[] cameraRestraints;

    private void Awake()
    {
        cameraCtrl = this;
        FixAirWallPos();
    }

    /// <summary>
    /// 修正空气墙的位置
    /// </summary>
    void FixAirWallPos()
    {
        //得到屏幕比例
        float width = Screen.width;
        float height = Screen.height;
        //修正空气墙位置
        AirWalls[0].localPosition = new Vector2(-5.1525f * width / height, AirWalls[0].localPosition.y);
        AirWalls[1].localPosition = new Vector2(5.1525f * width / height, AirWalls[1].localPosition.y);
    }

    public void Start()
    {
        //单人游戏先用这个把相机上绑定好玩家
        TargetedPlayer = Players[(int)MountGSS.gameScoreSettings.SelectedGirlInGame[0]].transform;
        tr = transform;

        //初始化相机约束
        cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo].Initialize(tr,TargetedPlayer);
        
       /*
        //将相机上绑定好玩家
        for (int i = 0; i < 3; i++)
        {
            //后续玩家没了，跳出循环
            if(MountGSS.gameScoreSettings.SelectedGirlInGame[i] == Variable.PlayerFaceType.Null)
            {
                break;
            }
        }*/
    }

    /// <summary>
    /// 解除停止点的限制，恢复相机移动
    /// </summary>
    public void RecoverMoving()
    {
        StopPoints[(int)MountGSS.gameScoreSettings.BattlingMajo].CancelStop();
    }

    public void LateUpdate()
    {
        //更新限制点
       cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo]. UpdatePoint();
        //检查停止点
        StopPoints[(int)MountGSS.gameScoreSettings.BattlingMajo].CheckStopPoint(tr.position);
        //到停止点，停下
        if (!StopPoints[(int)MountGSS.gameScoreSettings.BattlingMajo].StopCamera)
        {
            Restraint();
        }

    }

    /// <summary>
    /// 相机约束后移动
    /// </summary>
    void Restraint()
    {
      Vector3 c =  cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo].RepairCameraMoveDirection(MountGSS.gameScoreSettings.PlayerMove,TargetedPlayer.position);

      tr.Translate(c, Space.World);

    }

    #region 调试模式（相机一侧）
    /// <summary>
    /// 调式模式按钮：允许相机继续移动
    /// </summary>
    [ContextMenu("AllowCameraMoving")]
    public void AllowCameraMoving()
    {
        RecoverMoving();
    }
    #endregion
}


