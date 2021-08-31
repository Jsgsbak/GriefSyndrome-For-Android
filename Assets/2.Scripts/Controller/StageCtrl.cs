using MEC;
using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 总体的场景控制
/// </summary>
/// 
//所选角色全挂掉之后，返回魔女选择界面
//但是五色全挂掉之后，进入CAS场景
public class StageCtrl : MonoBehaviour
{    
    [Header("玩家激活设置")]
    public GameObject[] Players;
    public Transform Point;

    public GameObject Stage;

    public GameObject[] MajoScenes;

    /// <summary>
    /// 这个片段（停止点算一个片段）有多少怪
    /// </summary>
    public int ThisFragmentEnemyNumber = 0;

#if UNITY_EDITOR
    [Header("检查视图中的预设")]
    public SoundEffectCtrl PerfebInAsset;
#endif
   
    #region 事件组

    #endregion


    private void Start()
    {

        //先禁用所有玩家
        foreach (var item in Players)
        {
            item.SetActive(false);
        }

        //激活地图
        for (int i = 0; i < 6; i++)
        {
            //如果是正在打的魔女
            if ((int)MountGSS.gameScoreSettings.BattlingMajo == i)
            {
                MajoScenes[i].SetActive(true);
            }
            else
            {
                //如果不是，则销毁场景
                DestroyImmediate(MajoScenes[i]);
            }

        }

        //生成玩家（现在仅用来测试）
        for (int i = 0; i < 3; i++)
        {
            if (MountGSS.gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.Null)
            {
                Players[(int)MountGSS.gameScoreSettings.SelectedGirlInGame[i]].transform.SetPositionAndRotation(Point.position, Point.rotation);
                Players[(int)MountGSS.gameScoreSettings.SelectedGirlInGame[i]].SetActive(true);
               Players[(int)MountGSS.gameScoreSettings.SelectedGirlInGame[i]].transform.SetParent(null);

                //多人游戏需要重新写一下这一段代码
                if (MountGSS.gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.QB)
                {
                    MountGSS.gameScoreSettings.playerNumber++;//玩家数记录（排除QB）
                }
            }
        }
        //删除其他玩家
        foreach (var item in Players)
        {
            if (!item.activeInHierarchy)
            {
                Destroy(item);
            }
        }


        Start1();
    }

    private void Start1()
    {
       // MountGSS.gameScoreSettings.MajoDefeated.RemoveAllListeners();
        //MountGSS.gameScoreSettings.AllGirlsInGameDie.RemoveAllListeners();

        //重置玩家死亡数
        MountGSS.gameScoreSettings.deadPlayerNumber = 0;//上限是MountGSS.gameScoreSettings.playerNumber，即不含QB
        
#if UNITY_EDITOR

        //检查是否存在BGMCtrl
        if (GameObject.FindObjectOfType<SoundEffectCtrl>() == null)
        {
            SoundEffectCtrl SoundEffectCtrl = Instantiate(PerfebInAsset).GetComponent<SoundEffectCtrl>();
            SoundEffectCtrl.IsClone = true;
        }
#endif

        Start2();
    }

    // Start is called before the first frame update
    void Start2()
    {
        //音量初始化
        SoundEffectCtrl.soundEffectCtrl.ChangeVol(GameScoreSettingsIO.SEVol);       

        //初始化计时器
        InvokeRepeating("Timer", 1f, 1f);

        //注册事件
        MountGSS.gameScoreSettings.MajoDefeated.AddListener(delegate ()
        {
            //实际上，魔女hp=0的时候就要调用一次  StageCtrl.MountGSS.gameScoreSettings.DoesMajoOrShoujoDie = true;
            
            //之后魔女死亡动画播放
            //掉落悲叹之种
            //执行 GoodbyeMajo();
            GoodbyeMajo();
            MountGSS.gameScoreSettings.DoesMajoOrShoujoDie = true;
            MountGSS.gameScoreSettings.BanInput = true;

        });

        MountGSS.gameScoreSettings.AllGirlsInGameDie.AddListener(delegate ()
        {
            MountGSS.gameScoreSettings.DoesMajoOrShoujoDie = true;
            MountGSS.gameScoreSettings.BanInput = true;
            GirlsInGameDie();
        });
    }


    public void Timer()
    {
        if(MountGSS.gameScoreSettings.deadPlayerNumber == MountGSS.gameScoreSettings.playerNumber)
        {
            CancelInvoke("Timer");
            return;
        }
       MountGSS.gameScoreSettings.ThisMajoTime++;
    }

    /// <summary>
    /// 顺利打完魔女之后,悲叹之种精华之后才有的结算逻辑9现在暂时放在了答应魔女之后了）
    /// </summary>
    [ContextMenu("顺利结算")]
    public void GoodbyeMajo()
    {
        if (!Stage.activeInHierarchy)
        {
            return;//不知道为什么这个事件会执行很多次emmmm用这个来防止多次调用，原因不明
        }

        //清除场景
        Stage.SetActive(false);

        //停止计时器
        CancelInvoke("Timer");
        //累计时间增加
        MountGSS.gameScoreSettings.Time += MountGSS.gameScoreSettings.ThisMajoTime;

        //击败的是影之魔女之前的魔女，则开放下一个魔女（不包括人鱼）
        if ((int)MountGSS.gameScoreSettings.BattlingMajo <= 3)
        {
            MountGSS.gameScoreSettings.NewestMajo = (Variable.Majo)((int)MountGSS.gameScoreSettings.NewestMajo + 1);
        }


        //瓦夜逻辑
        if (MountGSS.gameScoreSettings.BattlingMajo == Variable.Majo.Walpurgisnacht)
        {
            //通知gss刷新最高分数，最短时间，最高连击，当前玩的lap
            MountGSS.gameScoreSettings.RefreshBestScoreAndSoOn();
            //存档（放在这里存档是为了防止有的人staff还没出现就关游戏）
            Timing.RunCoroutine(MountGSS.gameScoreSettings.SaveAll());

        }


        //调用击败魔女的事件（为了UI显示结算）
        MountGSS.gameScoreSettings.MajoDefeated.Invoke();

    }

    /// <summary>
    /// 没打过魔女
    /// </summary>
    [ContextMenu("没打过魔女")]
    public void GirlsInGameDie()
    {
        if (!Stage.activeInHierarchy)
        {
            return;//不知道为什么这个事件会执行很多次emmmm用这个来防止多次调用，原因不明
        }

        //清除场景
        Stage.SetActive(false);

        //停止计时器
        CancelInvoke("Timer");
        //累计时间增加
        MountGSS.gameScoreSettings.Time += MountGSS.gameScoreSettings.ThisMajoTime;

        //调用被打败的事件（为了UI显示结算）
        MountGSS.gameScoreSettings.AllGirlsInGameDie.Invoke();

    }




}
