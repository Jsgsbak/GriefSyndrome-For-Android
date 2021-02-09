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
    public static GameScoreSettingsIO gameScoreSettings;//尽在这里弄一个单利
    public static StageCtrl stageCtrl;

    
    [Header("玩家激活设置")]
    public GameObject[] Players;
    public Transform Point;

    public GameObject Stage;

#if UNITY_EDITOR
    [Header("检查视图中的预设")]
    public EasyBGMCtrl PerfebInAsset;
#endif

    /// <summary>
    /// 玩家人数
    /// </summary>
    int playerNumber = 0;
    /// <summary>
    /// 所选的玩家死亡人数
    /// </summary>
    int deadPlayerNumber = 0;

    /// <summary>
    /// 打这个魔女的时间
    /// </summary>
   [HideInInspector] public  int ThisMajoTime = 0;
   
    public int BGMid = 5;//通常都为5，是道中曲

    #region 事件组
    public Variable.IntEvent Player1Hurt = new Variable.IntEvent();
    public Variable.IntEvent Player2Hurt = new Variable.IntEvent();
    public Variable.IntEvent Player3Hurt = new Variable.IntEvent();

    /// <summary>
    /// 击败魔女
    /// </summary>
    public Variable.OrdinaryEvent MajoDefeated = new Variable.OrdinaryEvent();
    /// <summary>
    /// 魔法少女被击败（所选全死）
    /// </summary>
    public Variable.OrdinaryEvent AllGirlsInGameDie = new Variable.OrdinaryEvent();

    #endregion
    private void Awake()
    {
        stageCtrl = this;

        MajoDefeated.RemoveAllListeners();
        AllGirlsInGameDie.RemoveAllListeners();


        gameScoreSettings = (GameScoreSettingsIO)Resources.Load("GameScoreAndSettings");
        Application.targetFrameRate = gameScoreSettings.MaxFps;

#if UNITY_EDITOR

        //检查是否存在BGMCtrl
        if (GameObject.FindObjectOfType<EasyBGMCtrl>() == null)
        {
            EasyBGMCtrl easyBGMCtrl = Instantiate(PerfebInAsset).GetComponent<EasyBGMCtrl>();
            easyBGMCtrl.IsClone = true;
        }
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        //事件组注册
        UICtrl.uiCtrl.PauseGame.AddListener(PauseGameForStage);

        //初始化
        gameScoreSettings.MajoInitial();
        //音量初始化
        EasyBGMCtrl.easyBGMCtrl.ChangeVol(gameScoreSettings.BGMVol, true);
        EasyBGMCtrl.easyBGMCtrl.ChangeVol(gameScoreSettings.SEVol, false);

        //播放BGM（这里用的还是旧版的BGM播放器）
        if(gameScoreSettings.BattlingMajo != Variable.Majo.Walpurgisnacht && gameScoreSettings.BattlingMajo != Variable.Majo.Oktavia)
        {
            BGMid = 5;
        }
        else if(gameScoreSettings.BattlingMajo == Variable.Majo.Walpurgisnacht)
        {
            BGMid = 6;
        }
        else
        {
            BGMid = 2;
        }
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(BGMid);

        //先禁用所有玩家
        foreach (var item in Players)
        {
            item.SetActive(false);
        }
        //生成玩家（现在仅用来测试）
        for (int i = 0; i < 3; i++)
        {
            if(gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.Null)
            {
                Players[(int)gameScoreSettings.SelectedGirlInGame[i]].transform.SetPositionAndRotation(Point.position, Point.rotation);
                Players[(int)gameScoreSettings.SelectedGirlInGame[i]].SetActive(true);
                Players[(int)gameScoreSettings.SelectedGirlInGame[i]].transform.SetParent(Stage.transform);


                if (gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.QB)
                {
                    playerNumber++;//玩家数记录（排除QB）
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

        //初始化计时器
        InvokeRepeating("Timer", 1f, 1f);
    }


    public void Timer()
    {
        ThisMajoTime++;
    }

    /// <summary>
    /// 为Stage写的暂停游戏或否的方法
    /// </summary>
    public void PauseGameForStage(bool IsPaused)
    {
        Stage.SetActive(!IsPaused);
    }

    /// <summary>
    /// 0.0.7测试用按钮才用的函数，啥时候去掉了那几个按钮才把这个方法去掉
    /// </summary>
    public void Update()
    {
        if (gameScoreSettings.Succeed && !StageCtrl.gameScoreSettings.DoesMajoOrShoujoDie)
        {
            //实际上，魔女hp=0的时候就要调用一次  StageCtrl.gameScoreSettings.DoesMajoOrShoujoDie = true;
            //然后禁用输入按钮和输入
            //之后魔女死亡动画播放
            //掉落悲叹之种
            //执行 GoodbyeMajo();
            GoodbyeMajo();
            StageCtrl.gameScoreSettings.DoesMajoOrShoujoDie = true;
            //单纯为了别多次执行
            gameScoreSettings.Succeed = false;
        }

        /*UI显示，玩家操控，切换场景测试成功，保留备用
        else  if (gameScoreSettings.DoesMajoOrShoujoDie)
        {
            GirlsInGameDie();
            gameScoreSettings.CleanSoul = false;
            enabled = false;
        }*/

    }

    /// <summary>
    /// 场上有一个玩家宝石没了
    /// </summary>
    public void PlayerDie()
    {
        //多人游戏的话，要判断是否三个人都死了
        GirlsInGameDie();

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
        gameScoreSettings.Time += ThisMajoTime;

        //击败的是影之魔女之前的魔女，则开放下一个魔女（不包括人鱼）
        if ((int)gameScoreSettings.BattlingMajo <= 3)
        {
            gameScoreSettings.NewestMajo = (Variable.Majo)((int)gameScoreSettings.NewestMajo + 1);
        }


        //瓦夜逻辑
        if (gameScoreSettings.BattlingMajo == Variable.Majo.Walpurgisnacht)
        {
            //通知gss刷新最高分数，最短时间，最高连击，当前玩的lap
            gameScoreSettings.RefreshBestScoreAndSoOn();
            //存档（放在这里存档是为了防止有的人staff还没出现就关游戏）
            Timing.RunCoroutine(gameScoreSettings.Save());

        }


        //调用击败魔女的事件
        MajoDefeated.Invoke();

    }

    /// <summary>
    /// 游戏中登场的魔法少女死了（每一位死亡之后都调用 QB除外）
    /// </summary>
    public void GirlsInGameDie()
    {
        //真惨。。。加把劲吧

        deadPlayerNumber++;
        //死亡人数达到游戏人数才继续执行
        if (deadPlayerNumber < playerNumber)
        {
            return;
        }

        //停止计时器
        CancelInvoke("Timer");
        //BGM停止播放
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(-1);
        //累计时间增加
        gameScoreSettings.Time += ThisMajoTime;

        //判断是否五色扑街
        gameScoreSettings.AllDie = true;
        for (int i = 0; i < 5; i++)
        {
            if (!gameScoreSettings.MagicalGirlsDie[i])
            {
                //有活着的则修复为false
                gameScoreSettings.AllDie = false;
                break;
            }
        }

        //游戏中的魔法少女全死了的事件调用
            AllGirlsInGameDie.Invoke();
    }


    /// <summary>
    /// 统一控制玩家受伤的逻辑
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="PlayerId"></param>
    public  void HurtPlayer(int damage, int PlayerId)
    {
        if (PlayerId == 1)
        {
            Player1Hurt.Invoke(damage);
        }
        else if (PlayerId == 2)
        {
            Player2Hurt.Invoke(damage);
        }
        else
        {
            Player3Hurt.Invoke(damage);
        }
    }

}
