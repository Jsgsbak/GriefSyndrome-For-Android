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


    [Header("玩家生成设置")]
    public GameObject[] Players;
    public Transform Point;

    [Header("检查视图中的预设")]
    public EasyBGMCtrl PerfebInAsset;

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
    public class IntEvent : UnityEvent<int> { }
    public IntEvent Player1Hurt = new IntEvent();
    public IntEvent Player2Hurt = new IntEvent();
    public IntEvent Player3Hurt = new IntEvent();

    /// <summary>
    /// 击败魔女
    /// </summary>
    public Variable.OrdinaryEvent MajoDefeated = new Variable.OrdinaryEvent();
    /// <summary>
    /// 魔法少女被击败（所选全死）
    /// </summary>
    public Variable.OrdinaryEvent AllGirlsDieInGame = new Variable.OrdinaryEvent();

    #endregion
    private void Awake()
    {
        stageCtrl = this;

        MajoDefeated.RemoveAllListeners();
        AllGirlsDieInGame.RemoveAllListeners();


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

        //初始化
        gameScoreSettings.MajoInitial();
        //音量初始化
        EasyBGMCtrl.easyBGMCtrl.ChangeVol(gameScoreSettings.BGMVol, true);
        EasyBGMCtrl.easyBGMCtrl.ChangeVol(gameScoreSettings.SEVol, false);

        //播放BGM
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(BGMid);

        //生成玩家（现在仅用来测试）
        for (int i = 0; i < 3; i++)
        {
            if(gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.Null)
            {
             GameObject player =   Instantiate(Players[(int)gameScoreSettings.SelectedGirlInGame[i]], Point);
                if(gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.QB)
                {
                    playerNumber++;//玩家数记录（排除QB）
                }
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
    /// 顺利打完魔女之后的结算逻辑
    /// </summary>
    [ContextMenu("顺利结算")]
    public void GoodbyeMajo()
    {
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
    public void GirlDieInGame()
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
        AllGirlsDieInGame.Invoke();
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
