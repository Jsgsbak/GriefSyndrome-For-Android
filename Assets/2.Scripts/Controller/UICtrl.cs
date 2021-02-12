using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PureAmaya.General;
using UnityEngine.UI;
using UnityEngine.U2D;
using MEC;

//击杀完魔女之后的 XXX is defeated需要用到魔女的脚本，但是还没写）

/// <summary>
/// 包含安卓输入在内的UI控制（是否启用在UICtrl中控制）
/// </summary>
[DisallowMultipleComponent]
public class UICtrl : MonoBehaviour
{
    public static UICtrl uiCtrl;

    /// <summary>
    /// 虚拟输入
    /// </summary>
    public GameObject VirtualInput;

    //控制方法：调用相应的PlayerInf方法，更新UI信息
    [Header("玩家信息预设")]
    public PlayerInfUpdate PlayerInf;
    public Transform PIUParent;
    public PausePlayerInf pausePlayerInf;
    public SpriteAtlas PauseAtlas;
    public Transform PPIParent;
    
    public GameObject Pause;

    public GameObject DebugUI;

    [Header("音量滑条")]
    public Slider BGMVol;
    public Slider SEVol;

    /// <summary>
    /// 结算界面
    /// </summary>
    [Header("结算界面")]
    public CanvasGroup ConcInMajo;
    public TMP_Text MajoDieText;
    public TMP_Text ThisMajoTimeText;
    public TMP_Text TotalTimeText;

    /// <summary>
    /// 真正起到更新作用的在这里
    /// </summary>
    PlayerInfUpdate[] PlayerInfInGame;
    PausePlayerInf[] pausePlayerInfInGame;
    /// <summary>
    /// 非QB玩家计数
    /// </summary>
    int PlayerCount = 0;



#if UNITY_EDITOR
    [Header("调试用")]
    public Text ShowRandomBGM;
#endif


    #region 事件组
    /// <summary>
    /// 更新玩家信息
    /// </summary>
    public Variable.OrdinaryEvent UpdateInf = new Variable.OrdinaryEvent();
    /// <summary>
    /// 是否暂停游戏
    /// </summary>
    public Variable.BoolEvent PauseGame = new Variable.BoolEvent();
    #endregion

    private void Awake()
    {
        uiCtrl = this;

        UpdateInf.RemoveAllListeners();



    }

    private void Start()
    {

        UpdateManager.updateManager.SlowUpdate.AddListener(SlowUpdate);
        UpdateManager.updateManager.FastUpdate.AddListener(FastUpdate);

        #region 注册事件
        //音量事件注册与设置
        BGMVol.onValueChanged.AddListener(BGMVolChange);
        SEVol.onValueChanged.AddListener(SEVolChange);
        BGMVol.value =StageCtrl. gameScoreSettings.BGMVol;
        SEVol.value = StageCtrl.gameScoreSettings.SEVol;

        //魔女被击败
        StageCtrl.stageCtrl.MajoDefeated.AddListener(delegate() {/*修改状态，防止游戏暂停*/StageCtrl.gameScoreSettings. DoesMajoOrShoujoDie = true; /*启用结算界面*/Timing.RunCoroutine(Conclusion());});
        //魔法少女被击败（所选全死）
        StageCtrl.stageCtrl.AllGirlsInGameDie.AddListener(delegate() {/*修改状态，防止游戏暂停*/StageCtrl.gameScoreSettings.DoesMajoOrShoujoDie = true; /*启用结算界面*/Timing.RunCoroutine(ShoujoDie()); });
        #endregion

        #region 场景UI初始化
        //关闭结算界面
        ConcInMajo.gameObject.SetActive(false);
        ConcInMajo.alpha = 0f;
        //禁用暂停界面
        Pause.SetActive(false);
        #endregion

        #region 初始化UI界面

        
        //设置好虚拟按键是否启用
        if(StageCtrl.gameScoreSettings.UseScreenInput != 2)
        {
            Destroy(VirtualInput);
        }

        //先记录下玩家人数（当然现在是单机，不过还是为多人留点东西）
        for (int i = 0; i < 3; i++)
        {
            if (StageCtrl.gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.Null && StageCtrl.gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.QB)
            {
                PlayerCount++;
            }
        }

        //创建相应的玩家信息预设，展示并且便于管理
        PlayerInfInGame = new PlayerInfUpdate[PlayerCount];
        pausePlayerInfInGame = new PausePlayerInf[PlayerCount];

        //设置名称，注册事件，设置灵魂宝石图片，顺便剔除qb
        for (int i = 0; i < PlayerCount; i++)
        {
            //玩家信息
            PlayerInfInGame[i] = Instantiate(PlayerInf);
            PlayerInfInGame[i].PlayerId = i +1;
            PlayerInfInGame[i].transform.SetParent(PIUParent);//设置父对象，排版
            PlayerInfInGame[i].transform.localScale = 0.8f * Vector2.one;//修正规模
            PlayerInfInGame[i].SetNameAndSG(StageCtrl.gameScoreSettings.SelectedGirlInGame[i].ToString());
            PlayerInfInGame[i].RegEvent();
           
            //暂停界面的
            pausePlayerInfInGame[i] = Instantiate(pausePlayerInf);
            pausePlayerInfInGame[i].transform.SetParent(PPIParent);
            pausePlayerInfInGame[i].transform.localScale = 1.1f * Vector2.one;//修正规模
            pausePlayerInfInGame[i].SetNameAndImage(StageCtrl.gameScoreSettings.SelectedGirlInGame[i].ToString(), PauseAtlas); ;

        }

        //暂停界面隐藏
        Pause.SetActive(false);

        #endregion


    }


    void SlowUpdate()
    {
        UpdateInf.Invoke();

    }

    void FastUpdate()
    { 
        //响应安卓返回按键
        if(Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
        {
            StageCtrl.gameScoreSettings.Pause = true;
        }

        ///触发暂停
        if (StageCtrl.gameScoreSettings.Pause && Time.timeScale != 0)
        {
            GamePauseSwitch();
        }

    }


    #region 音量滑块
    public void BGMVolChange(float vol)
    {
        StageCtrl.gameScoreSettings.BGMVol = vol;
        EasyBGMCtrl.easyBGMCtrl.ChangeVol(vol, true);
    }
    public void SEVolChange(float vol)
    {
        StageCtrl.gameScoreSettings.SEVol = vol;
        EasyBGMCtrl.easyBGMCtrl.ChangeVol(vol, false);

    }

    #endregion

    /// <summary>
    /// 游戏暂停切换（按钮检查面板注入）
    /// </summary>
    [ContextMenu("游戏暂停切换")]
    public void GamePauseSwitch()
    {
        
        PauseGame.Invoke(Time.timeScale != 0);

        //游戏暂停
        if (Time.timeScale != 0 && !StageCtrl.gameScoreSettings.DoesMajoOrShoujoDie)
        {
            DebugUI.SetActive(false);
            //暂停音效
            EasyBGMCtrl.easyBGMCtrl.PlaySE(2);
            Time.timeScale = 0;
            EasyBGMCtrl.easyBGMCtrl.BGMPlayer.Pause();
            Pause.SetActive(true);
            MEC.Timing.TimeBetweenSlowUpdateCalls = 999999999999999f;

        }
        //暂停恢复
        else if(Time.timeScale == 0)
        {
            DebugUI.SetActive(true);
            StageCtrl.gameScoreSettings.Pause = false;
            Time.timeScale = 1;
            //确认音效
            EasyBGMCtrl.easyBGMCtrl.PlaySE(0);
            EasyBGMCtrl.easyBGMCtrl.BGMPlayer.UnPause();
            Pause.SetActive(false);
            MEC.Timing.TimeBetweenSlowUpdateCalls = 1f / 7f;
        }
    }

    /// <summary>
    /// 非结算界面返回标题界面（按钮检查面板注入）
    /// </summary>
    public void ReturnToTitle()
    {
        //恢复被暂停的时间
        GamePauseSwitch();

        //设置为false，使回到主标题part
        StageCtrl.gameScoreSettings.MajoSceneToTitle = false;


        //返回音效
        EasyBGMCtrl.easyBGMCtrl.PlaySE(1);
        Time.timeScale = 1;//回复时间
        UnityEngine.SceneManagement.SceneManager.LoadScene(1,UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    /// <summary>
    /// 随机播放bgm（按钮检查面板注入）
    /// </summary>
    public void RandomPlayBGM()
    {
        //确认音效
        EasyBGMCtrl.easyBGMCtrl.PlaySE(0);
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(Random.Range(0, EasyBGMCtrl.easyBGMCtrl.BGM.Length));
    }

    /// <summary>
    /// 所选都魔法少女死亡
    /// </summary>
    public IEnumerator<float> ShoujoDie()
    {

        //判断是否五色扑街
        if (StageCtrl.gameScoreSettings.AllDie)
        {
            //借助结算界面的文本框通知玩家你成功打出了be
            MajoDieText.text = "    Se kai saraba...";
        }
        else
        {
            //借助结算界面的文本框通知玩家你成功打出了be
            MajoDieText.text = "  Select another mahoshoujo to continue...";

        }

        //存活时间
        ThisMajoTimeText.text = string.Format("Surivial Time:{0}", TitleCtrl.IntTimeFormat(StageCtrl.stageCtrl.ThisMajoTime));
        //总用时                                
        TotalTimeText.text = string.Format("Total Time:{0}", TitleCtrl.IntTimeFormat(StageCtrl.gameScoreSettings.Time));


        //展开结算界面
        ConcInMajo.gameObject.SetActive(true);
        //淡入
        for (int i = 0; i < 50; i++)
        {
            ConcInMajo.alpha += 0.02f;
            yield return Timing.WaitForSeconds(0.01f);
        }


        //返回方法
        Invoke("ReturnToMajoOrStaff", 3f);

    }

    /// <summary>
    /// 击败魔女后的结果（显示挑战时间与总时间）
    /// </summary>
    /// <returns></returns>
    IEnumerator<float> Conclusion()
    {
        /*这里说明一下，所有魔女打完之后都会先展示结算界面，最后展示staff（仅瓦夜击败后有staff）
 * 游戏中的魔法少女死亡后说明一下然后退出到魔女选择part
 * 全员死亡后说一下凉透了就跳转到staff
 */
        //此处仅执行顺利打完魔女的结算

        //击败提示
        if (StageCtrl.gameScoreSettings.BattlingMajo != Variable.Majo.Walpurgisnacht)
        {
            MajoDieText.text = string.Format("{0} was defeated\n                                   and left griefseed.", StageCtrl.gameScoreSettings.BattlingMajo.ToString());
        }
        else
        {
            MajoDieText.text = string.Format("   {0} was over.", StageCtrl.gameScoreSettings.BattlingMajo.ToString());
        }

        //这个魔女被击败的用时
        ThisMajoTimeText.text = string.Format("Clear Time:{0}", TitleCtrl.IntTimeFormat(StageCtrl.stageCtrl.ThisMajoTime));
        //总用时
        TotalTimeText.text = string.Format("Total Time:{0}", TitleCtrl.IntTimeFormat(StageCtrl.gameScoreSettings.Time));

        //展开结算界面
        ConcInMajo.gameObject.SetActive(true);
        //淡入
        for (int i = 0; i < 50; i++)
        {
            ConcInMajo.alpha += 0.02f;
            yield return Timing.WaitForSeconds(0.01f);
        }

        //返回方法
        Invoke("ReturnToMajoOrStaff", 3f);
    
    }

    /// <summary>
    /// （顺利打完某魔女）从结算界面返回到魔女选择part或者staff
    /// </summary>
    void ReturnToMajoOrStaff()
    {
        //瓦夜打完，结算界面结束后进入staff / 或者五色全挂，进入staff
        if(StageCtrl.gameScoreSettings.BattlingMajo == Variable.Majo.Walpurgisnacht || StageCtrl.gameScoreSettings.AllDie)
        {
            LoadingCtrl.LoadScene(4, false);
        }
        //其他魔女打完，结算界面结束后进入魔女选择part
        else if(StageCtrl.gameScoreSettings.BattlingMajo != Variable.Majo.Walpurgisnacht)
        {
            LoadingCtrl.LoadScene(1, false);
        }
    }


    #region 0.0.7临时用
    public void JumpUp()
    {
        StageCtrl.gameScoreSettings.Jump = false;
    }
    public void CleanSoulUp()
    {
        StageCtrl.gameScoreSettings.CleanSoul = false;
    }

    public void cleanvitUp()
    {
        StageCtrl.gameScoreSettings.CleanVit = false;
    }

    public void HurtMeUp()
    {
        StageCtrl.gameScoreSettings.HurtMyself = false;
    }
    public void SucceedUp()
    {
        StageCtrl.gameScoreSettings.Succeed = false;
    }
    public void JumpDown()
    {
        StageCtrl.gameScoreSettings.Jump = true;
    }
    public void CleanSoulDOwn()
    {
        StageCtrl.gameScoreSettings.CleanSoul = true;
    }

    public void cleanvitDown()
    {
        StageCtrl.gameScoreSettings.CleanVit = true;
    }

    public void HurtMeDown()
    {
        StageCtrl.gameScoreSettings.HurtMyself = true;
    }
    public void SucceedDOwn()
    {
        StageCtrl.gameScoreSettings.Succeed = true;
    }
    #endregion
}
