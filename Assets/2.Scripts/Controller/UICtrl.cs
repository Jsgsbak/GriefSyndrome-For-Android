using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PureAmaya.General;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UICtrl : MonoBehaviour
{

    //控制方法：调用相应的PlayerInf方法，更新UI信息
    [Header("预设生成")]
    public PlayerInfUpdate PlayerInf;
    public Transform Parent;
    /// <summary>
    /// 真正起到管理作用的在这里
    /// </summary>
    PlayerInfUpdate[] PlayerInfInGame;

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

        //设置名称，注册事件，设置灵魂宝石图片，顺便剔除qb
        for (int i = 0; i < PlayerCount; i++)
        {
            PlayerInfInGame[i] = Instantiate(PlayerInf);
            PlayerInfInGame[i].transform.SetParent(Parent);
            PlayerInfInGame[i].transform.localScale = 0.8f * Vector2.one;//修正规模
            PlayerInfInGame[i].SetNameAndSG(StageCtrl.gameScoreSettings.SelectedGirlInGame[i].ToString());
            PlayerInfInGame[i].RegEvent();
        }
        #endregion
    }

    //其实是慢速的Update
    void FastUpdate()
    {
        UpdateInf.Invoke();
    }


}
