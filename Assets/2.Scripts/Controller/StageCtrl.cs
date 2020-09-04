using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageCtrl : MonoBehaviour
{
    public static GameScoreSettingsIO gameScoreSettings;


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
    }

    // Start is called before the first frame update
    void Start()
    {

        //初始化
        gameScoreSettings.MajoInitial();
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
