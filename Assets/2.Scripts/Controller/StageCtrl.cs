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
    public static GameScoreSettingsIO gameScoreSettings;

   public int BGMid = 5;//通常都为5，是道中曲

    [Header("玩家生成设置")]
    public GameObject[] Players;
    public Transform Point;

    [Header("检查视图中的预设")]
    public EasyBGMCtrl PerfebInAsset;


    #region 事件组
    public class intEvent : UnityEvent<int> { }
    public static intEvent Player1Hurt = new intEvent();
    public static intEvent Player2Hurt = new intEvent();
    public static intEvent Player3Hurt = new intEvent();
    #endregion
    private void Awake()
    {
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


        //注册事件
        //   UpdateManager.FastUpdate.AddListener(FastUpdate);

        //播放BGM
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(BGMid);

        //生成玩家（现在仅用来测试）
        for (int i = 0; i < 3; i++)
        {
            if(gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.Null)
            {
                Instantiate(Players[(int)gameScoreSettings.SelectedGirlInGame[i]], Point);
            }
        }

        //初始化计时器
        InvokeRepeating("Timer", 1f, 1f);
    }


    public void Timer()
    {
        gameScoreSettings.Time++;
    }


    /// <summary>
    /// 打完魔女之后的结算逻辑
    /// </summary>
    [ContextMenu("结算")]
    public void GoodbyeMajo()
    {
        //停止计时器
        CancelInvoke("Timer");
        

        //瓦夜逻辑
       if (gameScoreSettings.MajoBeingBattled == Variable.Majo.Walpurgisnacht)
        {
            //通知gss刷新最高分数，最短时间，最高连击，当前玩的lap
            gameScoreSettings.RefreshBestScoreAndSoOn();
            //存档
            Timing.RunCoroutine(gameScoreSettings.Save());

        }
    }


    /// <summary>
    /// 统一控制玩家受伤的逻辑
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="PlayerId"></param>
    public static void HurtPlayer(int damage, int PlayerId)
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
