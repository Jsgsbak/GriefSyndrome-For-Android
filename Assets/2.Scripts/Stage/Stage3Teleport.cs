using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

/// <summary>
/// ���ӻ�ħŮ ���ӱ��� ��������
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
                    //���һ�����ֱ�Ӵ�Խ��������
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
