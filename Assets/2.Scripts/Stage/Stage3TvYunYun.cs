using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

public class Stage3TvYunYun : MonoBehaviour
{
    //吾乃“电”视，作为道中最强场景，操纵着劝退能力，是背负苍蓝雷电之人

    [Header("动画偏移")]
    public float OffsetCycle = 0f;
    public Animator animator;

    private bool StartToFuckYou = false;

    Transform tr;

    // Start is called before the first frame update
    void Start()
    {
            animator.SetFloat("OffsetCycle", OffsetCycle);
        UpdateManager.updateManager.SlowUpdate.AddListener(FuckYou);
        tr = transform;
    }

    public void GuaiGuaiZhanHao(string d)
    {
            StartToFuckYou = d == "Oh♂Yeah";
    }


    void FuckYou()
    {
        if (!StartToFuckYou)
        {
            return;
        }

        Debug.Log(MountGSS.gameScoreSettings.PlayersPosition[0].x - tr.position.x);

        for (int i = 0; i < 3; i++)
        {
            //距离还差点
            if(MountGSS.gameScoreSettings.PlayersPosition[i].x - tr.position.x <= 2.07F && MountGSS.gameScoreSettings.PlayersPosition[i].x - tr.position.x >= -2.18f && MountGSS.gameScoreSettings.PlayersPosition[i].y - tr.position.y <= 3.7F)
            {
                //伤害还要改改
                PlayerRootCtrl.playerRootCtrl.HurtPlayer(20, i);
            }
        }
    }

}
