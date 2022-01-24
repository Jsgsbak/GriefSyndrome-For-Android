using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

/// <summary>
/// 电视机魔女 电视被电 调出场景
/// </summary>
public class Stage3Teleport : MonoBehaviour
{
    Transform tr
        ;
    Transform  MainCamera;

    public bool BecomeBall = true;

    // Start is called before the first frame update
    void Start()
    {
        UpdateManager.updateManager.SlowUpdate.AddListener(Wdnmd);
        tr = transform;
        MainCamera = Camera.main.transform;

    }

    // Update is called once per frame
    void Wdnmd()
    {
        for (int i = 0; i < 3; i++)
        {
            if(MountGSS.gameScoreSettings.PlayersPosition[i] != Vector2.zero && tr.position.y >= MountGSS.gameScoreSettings.PlayersPosition[i].y && !MountGSS.gameScoreSettings.IsSoulBallInGame[i])
            {
                if (BecomeBall)
                {
                    PlayerRootCtrl.playerRootCtrl.PlayerBecomeBall(i);
                    PlayerRootCtrl.playerRootCtrl.OneBloodNiiiiiiice(i);
                    PlayerRootCtrl.playerRootCtrl.JumpToPoint(i, MainCamera.position);
                }
                else
                {
                    //最后一关那里，直接穿越到最上面
                    PlayerRootCtrl.playerRootCtrl.JumpToPoint(i, new Vector2 (MountGSS.gameScoreSettings.PlayersPosition[i].x,-4f));
                }
            }
            else if (MountGSS.gameScoreSettings.PlayersPosition[i] == Vector2.zero)
            {
                break;
            }
        }
    }
}
