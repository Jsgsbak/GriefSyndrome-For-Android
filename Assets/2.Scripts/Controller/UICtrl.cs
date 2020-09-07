using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PureAmaya.General;
using UnityEngine.UI;
using UnityEngine.U2D;

//击杀完魔女之后的 XXX is defeated需要用到魔女的脚本，但是还没写）

[DisallowMultipleComponent]
public class UICtrl : MonoBehaviour
{

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
    /// 真正起到管理作用的在这里
    /// </summary>
    PlayerInfUpdate[] PlayerInfInGame;
    PausePlayerInf[] pausePlayerInfInGame;
    /// <summary>
    /// 非QB玩家计数
    /// </summary>
    int PlayerCount = 0;

    #region 事件组
    public static Variable.OrdinaryEvent UpdateInf = new Variable.OrdinaryEvent();
    #endregion

    //awake不能用
    private void Start()
    {
        UpdateManager.SlowUpdate.AddListener(SlowUpdate);
        UpdateManager.FastUpdate.AddListener(FastUpdate);
      
        //音量事件注册与设置
        BGMVol.onValueChanged.AddListener(BGMVolChange);
        SEVol.onValueChanged.AddListener(SEVolChange);
        BGMVol.value =StageCtrl. gameScoreSettings.BGMVol;
        SEVol.value = StageCtrl.gameScoreSettings.SEVol;


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
        if(Time.timeScale != 0)
        {
            Time.timeScale = 0;
            EasyBGMCtrl.easyBGMCtrl.BGMPlayer.Pause();
            Pause.SetActive(true);
            MEC.Timing.TimeBetweenSlowUpdateCalls = 0f;
        }
        //暂停恢复
        else
        {
            Time.timeScale = 1;
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
        //  LoadingCtrl.LoadScene(0);
        Time.timeScale = 1;//回复时间
        UnityEngine.SceneManagement.SceneManager.LoadScene(0,UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
