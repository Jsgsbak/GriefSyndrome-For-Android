using MEC;
using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ！不负责敌人的控制，游戏里敌人是固定生成的
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

#if UNITY_EDITOR
    [Header("检查视图中的预设")]
    public EasyBGMCtrl PerfebInAsset;
#endif


   
    public int BGMid = 5;//通常都为5，是道中曲

    #region 事件组

    #endregion
    private void Start()
    {
        MountGSS.gameScoreSettings.MajoDefeated.RemoveAllListeners();
        MountGSS.gameScoreSettings.AllGirlsInGameDie.RemoveAllListeners();

        //重置玩家死亡数
        MountGSS.gameScoreSettings.deadPlayerNumber = 0;//上限是MountGSS.gameScoreSettings.playerNumber，即不含QB
        //先禁用所有玩家
        foreach (var item in Players)
        {
            item.SetActive(false);
        }
        //生成玩家（现在仅用来测试）
        for (int i = 0; i < 3; i++)
        {
            if (MountGSS.gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.Null)
            {
                Players[(int)MountGSS.gameScoreSettings.SelectedGirlInGame[i]].transform.SetPositionAndRotation(Point.position, Point.rotation);
                Players[(int)MountGSS.gameScoreSettings.SelectedGirlInGame[i]].SetActive(true);
                Players[(int)MountGSS.gameScoreSettings.SelectedGirlInGame[i]].transform.SetParent(Stage.transform);


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
#if UNITY_EDITOR

        //检查是否存在BGMCtrl
        if (GameObject.FindObjectOfType<EasyBGMCtrl>() == null)
        {
            EasyBGMCtrl easyBGMCtrl = Instantiate(PerfebInAsset).GetComponent<EasyBGMCtrl>();
            easyBGMCtrl.IsClone = true;
        }
#endif

        Start2();
    }

    // Start is called before the first frame update
    void Start2()
    {
        //事件组注册
        UICtrl.uiCtrl.PauseGame.AddListener(PauseGameForStage);

        //初始化
        MountGSS.gameScoreSettings.MajoInitial();
        //音量初始化
        EasyBGMCtrl.easyBGMCtrl.ChangeVol(MountGSS.gameScoreSettings.BGMVol, true);
        EasyBGMCtrl.easyBGMCtrl.ChangeVol(MountGSS.gameScoreSettings.SEVol, false);

        //播放BGM（这里用的还是旧版的BGM播放器）
        if(MountGSS.gameScoreSettings.BattlingMajo != Variable.Majo.Walpurgisnacht && MountGSS.gameScoreSettings.BattlingMajo != Variable.Majo.Oktavia)
        {
            BGMid = 5;
        }
        else if(MountGSS.gameScoreSettings.BattlingMajo == Variable.Majo.Walpurgisnacht)
        {
            BGMid = 6;
        }
        else
        {
            BGMid = 2;
        }
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(BGMid);

       

        //初始化计时器
        InvokeRepeating("Timer", 1f, 1f);
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
    /// 为Stage写的暂停游戏或否的方法
    /// </summary>
    public void PauseGameForStage(bool IsPaused)
    {
        //Stage.SetActive(!IsPaused);
    }

    /// <summary>
    /// 0.0.7测试用按钮才用的函数，啥时候去掉了那几个按钮才把这个方法去掉
    /// </summary>
    public void Update()
    {
        if (MountGSS.gameScoreSettings.Succeed && !MountGSS.gameScoreSettings.DoesMajoOrShoujoDie)
        {
            //实际上，魔女hp=0的时候就要调用一次  StageCtrl.MountGSS.gameScoreSettings.DoesMajoOrShoujoDie = true;
            //然后禁用输入按钮和输入
            //之后魔女死亡动画播放
            //掉落悲叹之种
            //执行 GoodbyeMajo();
            GoodbyeMajo();
            MountGSS.gameScoreSettings.DoesMajoOrShoujoDie = true;
            //单纯为了别多次执行
            MountGSS.gameScoreSettings.Succeed = false;
        }

        /*UI显示，玩家操控，切换场景测试成功，保留备用
        else  if (MountGSS.gameScoreSettings.DoesMajoOrShoujoDie)
        {
            GirlsInGameDie();
            MountGSS.gameScoreSettings.CleanSoul = false;
            enabled = false;
        }*/

    }


    /// <summary>
    /// 顺利打完魔女之后的结算逻辑
    /// </summary>
    [ContextMenu("顺利结算")]
    public void GoodbyeMajo()
    {
        //清除场景
        Stage.SetActive(false);

        //停止计时器
        CancelInvoke("Timer");
        //BGM停止播放
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(-1);
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


        //调用击败魔女的事件
        MountGSS.gameScoreSettings.MajoDefeated.Invoke();

    }

   


   

}
