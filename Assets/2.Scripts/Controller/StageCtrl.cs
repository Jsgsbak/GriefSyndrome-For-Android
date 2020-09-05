using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ！不负责敌人的控制，游戏里敌人是固定生成的
/// </summary>
public class StageCtrl : MonoBehaviour
{
    public static GameScoreSettingsIO gameScoreSettings;

   public int BGMid = 1;//通常都为1，是魔女狩猎

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

        //初始化
        gameScoreSettings.MajoInitial();

    }

    // Start is called before the first frame update
    void Start()
    {


        //注册事件
     //   UpdateManager.FastUpdate.AddListener(FastUpdate);

        //播放BGM
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(BGMid);
    }

    public void FastUpdate()
    {
    }

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
