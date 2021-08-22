using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

/// <summary>
/// 场景里用的传送门（多人模式未适配）
/// </summary>
public class Portal : MonoBehaviour
{
    Transform tr;

    /// <summary>
    /// 把相机传送到约束的哪个点
    /// </summary>
    public int CameraPointInRestraint = -1;
    /// <summary>
    /// 把玩家传送到哪个点
    /// </summary>
    public Vector2 PlayerTo = Vector2.zero;

    private void Awake()
    {
        tr = transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        UpdateManager.updateManager.SlowUpdate.AddListener(SlowUpdate);
    }

    // Update is called once per frame
    void SlowUpdate()
    {

        //靠的足够近 不是灵魂球 没死，传送
        if ( tr != null && Mathf.Abs(MountGSS.gameScoreSettings.PlayersPosition[0].x - tr.position.x) <= 1f && !MountGSS.gameScoreSettings.IsBodyDieInGame[0] && !MountGSS.gameScoreSettings.IsSoulBallInGame[0])
                {
            tr = null;
            StartCoroutine(TP());
                }
    }


    private IEnumerator TP()
    {
        //禁止玩家输入
        MountGSS.gameScoreSettings.BanInput = true;
        //淡出相机
        yield return StartCoroutine(UICtrl.uiCtrl.NextFragmentFadeOut());
        //瞬移
       CameraCtrl.cameraCtrl.cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo].JumpToPoint(CameraPointInRestraint);
        //解除瞬移之后相机可能不移动的现象
        CameraCtrl.cameraCtrl.RecoverMoving();
        //所有玩家瞬移
        PlayerRootCtrl.playerRootCtrl.JumpToPoint(PlayerTo);

        //淡入
        yield return StartCoroutine(UICtrl.uiCtrl.NextFragmentFadeIn());
        MountGSS.gameScoreSettings.BanInput = false;
        Destroy(this.gameObject);
    }

}
