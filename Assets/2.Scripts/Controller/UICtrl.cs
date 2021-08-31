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

    public GameObject NextStopPointLogo;
    [Space]
    public Text PlayerPos;
    /// <summary>
    /// 调试模式面板
    /// </summary>
    [Header("调试界面")]
    public RectTransform DebugMode;
    /// <summary>
    /// 展示的那个按钮
    /// </summary>
    public RectTransform ShowButton;
    /// <summary>
    /// 调试面板里所有的按钮
    /// </summary>
    public Button[] DebugButtons;
    

    [Header("下一分段黑色转场")]
    public Image NextFragment;
    /// <summary>
    /// 真正起到更新作用的在这里
    /// </summary>
    PlayerInfUpdate[] PlayerInfInGame;
    PausePlayerInf[] pausePlayerInfInGame;
    /// <summary>
    /// 非QB玩家计数
    /// </summary>
    int PlayerCount = 0;

    public GameObject[] Players;


    #region 事件组
    /// <summary>
    /// 更新玩家信息
    /// </summary>
    public Variable.OrdinaryEvent UpdateInf = new Variable.OrdinaryEvent();
    #endregion

    private void Awake()
    {
        uiCtrl = this;

        UpdateInf.RemoveAllListeners();

        //奇怪的BUG解决：每次开游戏只显示FPS，然后要微调大Plane Distance才能正常显示
        GetComponent<Canvas>().planeDistance = 10.1f;
        VirtualInput.GetComponent<Canvas>().planeDistance = 11.1f;

    }

    private void Start()
    {
        UpdateManager.updateManager.SlowUpdate.AddListener(SlowUpdate);
        UpdateManager.updateManager.FastUpdate.AddListener(FastUpdate);

        #region 注册事件
        //音量事件注册与设置
        BGMVol.onValueChanged.AddListener(BGMVolChange);
        SEVol.onValueChanged.AddListener(SEVolChange);
        BGMVol.value = GameScoreSettingsIO.BGMVol;
        SEVol.value = GameScoreSettingsIO.SEVol;

        //魔女被击败
        MountGSS.gameScoreSettings.MajoDefeated.AddListener(delegate() {
            /*修改状态，防止游戏暂停*/
            MountGSS.gameScoreSettings. DoesMajoOrShoujoDie = true; /*启用结算界面*/Timing.RunCoroutine(Conclusion());});
        //魔法少女被击败（所选全死）
        MountGSS.gameScoreSettings.AllGirlsInGameDie.AddListener(delegate() {/*修改状态，防止游戏暂停*/MountGSS.gameScoreSettings.DoesMajoOrShoujoDie = true; /*启用结算界面*/Timing.RunCoroutine(ShoujoDie()); });
        #endregion

        #region 场景UI初始化
        //关闭结算界面
        ConcInMajo.gameObject.SetActive(false);
        ConcInMajo.alpha = 0f;
        //禁用暂停界面
        Pause.SetActive(false);
        //暂停使用切换界面
        NextFragment.gameObject.SetActive(false);
        //暂停下一个停止点LOGO的使用
     //   NextStopPointLogo.SetActive(false);
        #endregion

        #region 初始化UI界面
        //初始化调试模式
        initializeDebugMode();

        //设置好虚拟按键是否启用
        if (MountGSS.gameScoreSettings.UseScreenInput != 2)
        {
            Destroy(VirtualInput);
        }

        //先记录下玩家人数（当然现在是单机，不过还是为多人留点东西）
        for (int i = 0; i < 3; i++)
        {
            if (MountGSS.gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.Null && MountGSS.gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.QB)
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
            PlayerInfInGame[i].SetNameAndSG(MountGSS.gameScoreSettings.SelectedGirlInGame[i].ToString());
            PlayerInfInGame[i].RegEvent();
           
            //暂停界面的
            pausePlayerInfInGame[i] = Instantiate(pausePlayerInf);
            pausePlayerInfInGame[i].transform.SetParent(PPIParent);
            pausePlayerInfInGame[i].transform.localScale = 1.1f * Vector2.one;//修正规模
            pausePlayerInfInGame[i].SetNameAndImage(MountGSS.gameScoreSettings.SelectedGirlInGame[i].ToString(), PauseAtlas); ;

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
        //更新玩家位置
        PlayerPos.text = MountGSS.gameScoreSettings.PlayersPosition[0].ToString();//仅适用于单人游戏

        //响应安卓返回按键（游戏界面暂停）
        if(Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
        {
            MountGSS.gameScoreSettings.Pause = true;
        }

        ///触发暂停
        if (MountGSS.gameScoreSettings.Pause && Time.timeScale != 0)
        {
            GamePauseSwitch();
        }

    }


    #region 音量滑块
    public void BGMVolChange(float vol)
    {
        GameScoreSettingsIO.BGMVol = vol;
    }
    public void SEVolChange(float vol)
    {
        GameScoreSettingsIO.SEVol = vol;
    }

    #endregion

    /// <summary>
    /// 游戏暂停切换（按钮检查面板注入）
    /// </summary>
    [ContextMenu("游戏暂停切换")]
    public void GamePauseSwitch()
    {

        MountGSS.gameScoreSettings.PauseGame.Invoke(Time.timeScale != 0);
        DebugMode.gameObject.SetActive(Time.timeScale == 0);
        Pause.SetActive(Time.timeScale != 0);


        //游戏暂停
        if (Time.timeScale != 0 && !MountGSS.gameScoreSettings.DoesMajoOrShoujoDie)
        {
            //暂停音效
           SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.Pause);
            Time.timeScale = 0;
            //无限夸大MEC携程之间的调用间隔
            MEC.Timing.TimeBetweenSlowUpdateCalls = 999999999999999f;

        }
        //暂停恢复
        else if(Time.timeScale == 0)
        {          
            //用返回音效作为接触暂停的音效
           SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.Return);
            MountGSS.gameScoreSettings.Pause = false;
            Time.timeScale = 1;
            //恢复MEC携程之间的调用间隔
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
        MountGSS.gameScoreSettings.MajoSceneToTitle = false;


        //返回音效
        SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.Return);
        Time.timeScale = 1;//回复时间
        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    /// <summary>
    /// 所选都魔法少女死亡
    /// </summary>
    public IEnumerator<float> ShoujoDie()
    {
        //禁用虚拟输入
        if (VirtualInput != null) VirtualInput.SetActive(false);

        //判断是否五色扑街
        if (GameScoreSettingsIO.AllDie)
        {
            //借助结算界面的文本框通知玩家你成功打出了be
            MajoDieText.text = "Sekai saraba...";
        }
        else
        {
            //借助结算界面的文本框通知玩家你成功打出了be
            MajoDieText.text = "Select another mahoshoujo to continue...";

        }

        //存活时间
        ThisMajoTimeText.text = string.Format("Surivial Time:{0}", TitleCtrl.IntTimeFormat(MountGSS.gameScoreSettings.ThisMajoTime));
        //总用时                                
        TotalTimeText.text = string.Format("Total Time:{0}", TitleCtrl.IntTimeFormat(MountGSS.gameScoreSettings.Time));


        //展开结算界面
        ConcInMajo.gameObject.SetActive(true);
        //删掉玩家
        for (int i = 0; i < 7; i++)
        {
            if (Players[i] != null)
            {
                Destroy(Players[i]);
            }
        }
        //删掉调试模式
        if(DebugMode != null) { Destroy(DebugMode.gameObject); }

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
        //禁用虚拟输入
        if (VirtualInput != null) VirtualInput.SetActive(false);
        

        /*这里说明一下，所有魔女打完之后都会先展示结算界面，最后展示staff（仅瓦夜击败后有staff）
 * 游戏中的魔法少女死亡后说明一下然后退出到魔女选择part
 * 全员死亡后说一下凉透了就跳转到staff
 */
        //击败提示
        if (MountGSS.gameScoreSettings.BattlingMajo != Variable.Majo.Walpurgisnacht)
        {
            MajoDieText.text = string.Format("{0} was defeated\nand left griefseed.", MountGSS.gameScoreSettings.BattlingMajo.ToString());
        }
        else
        {
            MajoDieText.text = string.Format("{0} was over.", MountGSS.gameScoreSettings.BattlingMajo.ToString());
        }

        //这个魔女被击败的用时
        ThisMajoTimeText.text = string.Format("Clear Time:{0}", TitleCtrl.IntTimeFormat(MountGSS.gameScoreSettings.ThisMajoTime));
        //总用时
        TotalTimeText.text = string.Format("Total Time:{0}", TitleCtrl.IntTimeFormat(MountGSS.gameScoreSettings.Time));

        //展开结算界面
        ConcInMajo.gameObject.SetActive(true);
        //删掉玩家
        for (int i = 0; i < 7; i++)
        {
            if(Players[i] != null)
            {
                Destroy(Players[i]);
            }
        }
        //删掉调试模式
        if(DebugMode != null) { Destroy(DebugMode.gameObject); }

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
        if(MountGSS.gameScoreSettings.BattlingMajo == Variable.Majo.Walpurgisnacht || GameScoreSettingsIO.AllDie)
        {
            //预先修改一些变量，防止出现Bug
            MountGSS.gameScoreSettings.MajoSceneToTitle = false;
            if(MountGSS.gameScoreSettings.BattlingMajo == Variable.Majo.Walpurgisnacht)
            {
                //通关设置
                MountGSS.gameScoreSettings.Success = true;
            }
            LoadingCtrl.LoadScene(4, false);
        }
        //其他魔女打完，结算界面结束后进入魔女选择part
        else if(MountGSS.gameScoreSettings.BattlingMajo != Variable.Majo.Walpurgisnacht)
        {
            LoadingCtrl.LoadScene(1, false);
        }
    }

    #region 场景下一个分段
    /// <summary>
    /// 场景的下一个分段切换效果
    /// </summary>
    /// <returns></returns>
    public IEnumerator NextFragmentFadeOut()
    {
        MountGSS.gameScoreSettings.BanInput = true;
        //初始化黑色遮罩层
        NextFragment.color = new Color(0f, 0f, 0f, 0f);
        NextFragment.gameObject.SetActive(true);
        //禁用虚拟输入
       if(VirtualInput != null)  VirtualInput.SetActive(false);

        for (int i = 0; i < 50; i++)
        {
            NextFragment.color = new Color(0f, 0f, 0f, NextFragment.color.a + 0.02f);
            yield return new WaitForSeconds(0.02f);
        }
    }
    /// <summary>
    /// 场景的下一个分段切换效果
    /// </summary>
    /// <returns></returns>
    public IEnumerator NextFragmentFadeIn()
    {
       
        //消除遮罩，恢复移动
        for (int i = 0; i < 50; i++)
        {
            NextFragment.color = new Color(0f, 0f, 0f, NextFragment.color.a - 0.02f);
            yield return new WaitForSeconds(0.02f);
        }
        NextFragment.gameObject.SetActive(false);
        //激活虚拟输入
        if (VirtualInput != null) VirtualInput.SetActive(true);

    }

    #endregion

    public IEnumerator<float> ShowNextStopPointLogo()
    {
        NextStopPointLogo.SetActive(true);
        yield return Timing.WaitForSeconds(1.5F);
        NextStopPointLogo.SetActive(false);

    }

    #region 调试模式
    /// <summary>
    /// 初始化调试模式
    /// </summary>
    void initializeDebugMode()
    {
        if (!MountGSS.gameScoreSettings.EnableDebugMode)
        {
            Destroy(DebugMode.gameObject);
            return;
        }
       
        //启用调试模式之后，将调试模式面板移动到屏幕外缘
         DebugMode.localPosition = new Vector3(DebugMode.localPosition.x - 239f, DebugMode.localPosition.y, DebugMode.localPosition.z);
    }

    /// <summary>
    /// 调试按钮按下打开/关闭面板
    /// </summary>
    public void DebugShowButton()
    {
        //根据旋转角度决定调用那个函数
        if(ShowButton.localRotation.eulerAngles.z == 0)
        {
            Timing.RunCoroutine(ShowDebugMenu());
        }
        else
        {
            Timing.RunCoroutine(HideDebugMenu());
        }
    }

   IEnumerator<float> ShowDebugMenu()
    {
        //面板移动过程中不能点击按钮
        for (int i = 0; i < DebugButtons.Length; i++)
        {
            DebugButtons[i].interactable = false;
        }

        for (int i = 0; i < 20; i++)
        {
            //把调试面板移动过来
            DebugMode.localPosition = new Vector3(DebugMode.localPosition.x + 11.95f, DebugMode.localPosition.y, 0f);
            //按钮转一下
            ShowButton.localRotation = Quaternion.Euler(0f, 0f, ShowButton.localRotation.eulerAngles.z + 9f);
            yield return Timing.WaitForSeconds(0.025f);
        }
        ShowButton.localRotation = Quaternion.Euler(0f, 0f, 180f);
       
        //面板移动终了能点击按钮
        for (int i = 0; i < DebugButtons.Length; i++)
        {
            DebugButtons[i].interactable = true;
        }
    }

    IEnumerator<float> HideDebugMenu()
    {        
        //面板移动过程中不能点击按钮
        for (int i = 0; i < DebugButtons.Length; i++)
        {
            DebugButtons[i].interactable = false;
        }

        for (int i = 0; i < 20; i++)
        {
            //把调试面板移动过来
            DebugMode.localPosition = new Vector3(DebugMode.localPosition.x - 11.95f, DebugMode.localPosition.y, 0f);
            //按钮转一下
            ShowButton.localRotation = Quaternion.Euler(0f, 0f, ShowButton.localRotation.eulerAngles.z - 9f);
            yield return Timing.WaitForSeconds(0.025f);
        }

        ShowButton.localRotation = Quaternion.Euler(0f, 0f, 0f);
      
        //面板移动终了能点击按钮
        for (int i = 0; i < DebugButtons.Length; i++)
        {
            DebugButtons[i].interactable = true;
        }
    }


    public void RandomStaff()
    {
        //结局随机数
        int i = Random.Range(0, 11);

        switch (i)
        {
            //全员死亡
            case 0:
                GameScoreSettingsIO.AllDie = true;
                break;
            //全员幸存
            case 1:
                MountGSS.gameScoreSettings.MagicalGirlsDie =  new bool[] { false, false, false, false, false };
                break;
            //只有鹿目圆死亡
            case 2:
                MountGSS.gameScoreSettings.MagicalGirlsDie = new bool[] { false, false, true, false, false };
                break;
            //只有沙耶加死亡
            case 3:
                MountGSS.gameScoreSettings.MagicalGirlsDie = new bool[] { false, false,false, false, true};
                break;
            //只有杏子死亡
            case 4:
                MountGSS.gameScoreSettings.MagicalGirlsDie = new bool[] { false, true, false, false, false };
                break;
            //只有学姐死亡
            case 5:
                MountGSS.gameScoreSettings.MagicalGirlsDie = new bool[] { false, false, false, true, false };
                break;
            //除了红蓝都死亡
            case 6:
                MountGSS.gameScoreSettings.MagicalGirlsDie = new bool[] { true, false, true, true, false };
                break;
            //除了学姐都死亡
            case 7:
                MountGSS.gameScoreSettings.MagicalGirlsDie = new bool[] { !false, !false, !false, !true, !false };
                break;
            //只有鹿目圆和沙耶加死亡
            case 8:
                MountGSS.gameScoreSettings.MagicalGirlsDie = new bool[] { false, false, !false, !true, !false };
                break;
            //除了鹿目圆全死亡
            case 9:
                MountGSS.gameScoreSettings.MagicalGirlsDie = new bool[] { !false, !false, !true, !false, !false };
                break;
                //黑长直轮回
            case 10:
                MountGSS.gameScoreSettings.MagicalGirlsDie = new bool[] { true, true, true, true, false };
                break;
        }

        LoadingCtrl.LoadScene(4, false);

 
    }

    #endregion
}
