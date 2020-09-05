using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PureAmaya.General;
using UnityEngine.UI;
using UnityEngine.U2D;

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
        UpdateManager.SlowUpdate.AddListener(FastUpdate);

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
            PlayerInfInGame[i].transform.localScale = 1.1f * Vector2.one;//修正规模
            pausePlayerInfInGame[i].SetNameAndImage(StageCtrl.gameScoreSettings.SelectedGirlInGame[i].ToString(), PauseAtlas); ;

        }

        //暂停界面隐藏
        Pause.SetActive(false);

        #endregion
    }

    //其实是慢速的Update
    void FastUpdate()
    {
        UpdateInf.Invoke();

        if (RebindableInput.GetKeyDown("Pause"))
        {
            GamePauseSwitch();
        }

    }
    [ContextMenu("游戏暂停切换")]
    public void GamePauseSwitch()
    {
        //游戏暂停
        if(Time.timeScale != 0)
        {
            Time.timeScale = 0;
            Pause.SetActive(true);
            MEC.Timing.TimeBetweenSlowUpdateCalls = 0f;
        }
        else
        {
            Time.timeScale = 0;
            Pause.SetActive(false);
            MEC.Timing.TimeBetweenSlowUpdateCalls = 1f / 7f;
        }
    }
}
