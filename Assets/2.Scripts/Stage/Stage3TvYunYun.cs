using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

public class Stage3TvYunYun : MonoBehaviour
{
    //���ˡ��硱�ӣ���Ϊ������ǿ������������Ȱ���������Ǳ��������׵�֮��

    [Header("����ƫ��")]
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
            StartToFuckYou = d == "Oh��Yeah";
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
            //���뻹���
            if(MountGSS.gameScoreSettings.PlayersPosition[i].x - tr.position.x <= 2.07F && MountGSS.gameScoreSettings.PlayersPosition[i].x - tr.position.x >= -2.18f && MountGSS.gameScoreSettings.PlayersPosition[i].y - tr.position.y <= 3.7F)
            {
                //�˺���Ҫ�ĸ�
                PlayerRootCtrl.playerRootCtrl.HurtPlayer(20, i);
            }
        }
    }

}
