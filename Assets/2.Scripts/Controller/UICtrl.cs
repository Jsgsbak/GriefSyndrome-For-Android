using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PureAmaya.General;
using UnityEngine.UI;
using UnityEngine.U2D;
using MEC;

//击杀完魔女之后的 XXX is defeated需要用到魔女的脚本，但是还没写）

[DisallowMultipleComponent]
public class UICtrl : MonoBehaviour
{
    public static UICtrl uiCtrl;

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


    /// <summary>
    /// 真正起到更新作用的在这里
    /// </summary>
    PlayerInfUpdate[] PlayerInfInGame;
    PausePlayerInf[] pausePlayerInfInGame;
    /// <summary>
    /// 非QB玩家计数
    /// </summary>
    int PlayerCount = 0;

    /// <summary>
    ///  魔女死了吗
    /// </summary>
    bool DoesMajoDie = false;



#if UNITY_EDITOR
    [Header("调试用")]
    public Text ShowRandomBGM;
#endif


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
        StageCtrl.stageCtrl.MajoDefeated.AddListener(MajoDie);
        #endregion

        #region 初始化UI界面
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
            pausePlayerInfInGame[i].PlayerId = i + 1;
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

    /// <summary>
    /// 提供键盘支持
    /// </summary>
    void FastUpdate()
    { 
        if (RebindableInput.GetKeyDown("Pause"))
        {
            Debug.Log("PAUISE");
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


    [ContextMenu("游戏暂停切换")]
    public void GamePauseSwitch()
    {


        //游戏暂停
        if (Time.timeScale != 0 && !DoesMajoDie)
        {       
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
            Time.timeScale = 1;
            //确认音效
            EasyBGMCtrl.easyBGMCtrl.PlaySE(0);
            EasyBGMCtrl.easyBGMCtrl.BGMPlayer.UnPause();
            Pause.SetActive(false);
            MEC.Timing.TimeBetweenSlowUpdateCalls = 1f / 7f;
        }
    }

    /// <summary>
    /// 非结算界面返回标题界面
    /// </summary>
    public void ReturnToTitle()
    {
        //恢复被暂停的时间
        GamePauseSwitch();

        //返回音效
        EasyBGMCtrl.easyBGMCtrl.PlaySE(1);
        Time.timeScale = 1;//回复时间
        UnityEngine.SceneManagement.SceneManager.LoadScene(1,UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    /// <summary>
    /// 随机播放bgm
    /// </summary>
    public void RandomPlayBGM()
    {
        //确认音效
        EasyBGMCtrl.easyBGMCtrl.PlaySE(0);
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(Random.Range(0, EasyBGMCtrl.easyBGMCtrl.BGM.Length));
        ShowRandomBGM.text = string.Format("正在播放：{0}", EasyBGMCtrl.easyBGMCtrl.BGMPlayer.clip.name);
    }

    /// <summary>
    /// 击败魔女后ui的逻辑
    /// </summary>
    public void MajoDie()
    {
        //修改状态，防止游戏暂停
        DoesMajoDie = true;

        //结算界面
        Timing.RunCoroutine(Conclusion());
    }

    /*
    /// <summary>
    /// 击败魔女后的结果（显示挑战时间与总时间）
    /// </summary>
    /// <returns></returns>
    IEnumerator<float> Conclusion()
    {
        MajoDieText.text = string.Format("Clear Time:{0}",)
    }*/
}
