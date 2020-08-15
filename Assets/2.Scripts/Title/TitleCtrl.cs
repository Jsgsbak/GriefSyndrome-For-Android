using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/// <summary>
/// n标题管理
/// </summary>
public class TitleCtrl : MonoBehaviour
{
    #region UI管理
    //分数显示
    public TMP_Text HiScore;
    public TMP_Text BestTime;
    public TMP_Text MaxHits;
    //按钮控制
    public Button StartGameButton;
    public TMP_InputField LapInput;
    public Button SettingsButton;
    public Button ExitButton;
    #endregion


    /// <summary>
    /// 尽量所有的属性/设置都从这里读取/写入
    /// </summary>
    public static GameScoreSettingsIO gameScoreSettingsIO;
    public static TitleCtrl titleCtrl;

    private void Awake()
    {
        //事前处理
        titleCtrl = this;
        gameScoreSettingsIO = Resources.Load("GameScoreAndSettings") as GameScoreSettingsIO;
        gameScoreSettingsIO.Initial();

    }


    // Start is called before the first frame update
    void Start()
    {
        //BGM
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(0);

        //注册组件
        //向GSS中写入周目数
        LapInput.onValueChanged.AddListener(delegate (string lap) { gameScoreSettingsIO.lap = int.Parse(lap);});


        StartGameButton.onClick.AddListener(delegate()
        { 


            //切换场景
         
        
        });
        //关闭游戏
        ExitButton.onClick.AddListener(delegate () { Application.Quit(0); });
    }

    /// <summary>
    /// 调整标题界面展示的分数
    /// </summary>
    /// <param name="scoreType"></param>
    /// <param name="Parameter"></param>
    /// <param name="PlayerFaces"></param>
    public void AdjustScore(Variable.ScoreType scoreType,string Parameter,Variable.PlayerFaceType[] PlayerFaces)
    {
        if(scoreType == Variable.ScoreType.BestTime)
        {
            BestTime.text = string.Format("{0} {1}  {2} {3} {4}", " Best Time", Parameter, PlayerFaceToRichText(PlayerFaces)[0], PlayerFaceToRichText(PlayerFaces)[1], PlayerFaceToRichText(PlayerFaces)[2]);
        }
        else if(scoreType == Variable.ScoreType.HiScore)
        {
            HiScore.text = string.Format("{0} {1}  {2} {3} {4}", "Highest Score", Parameter, PlayerFaceToRichText(PlayerFaces)[0], PlayerFaceToRichText(PlayerFaces)[1], PlayerFaceToRichText(PlayerFaces)[2]);
        }
        else
        {
            MaxHits.text = string.Format("{0} {1}  {2} {3} {4}", "  Max Hits", Parameter, PlayerFaceToRichText(PlayerFaces)[0], PlayerFaceToRichText(PlayerFaces)[1], PlayerFaceToRichText(PlayerFaces)[2]);
        }
    }



    //<sprite="PlayerFace" index=1> 
    #region 内部方法
    /// <summary>
    /// 将玩家面部枚举转化为可用富文本
    /// </summary>
    /// <param name="playerFaceType"></param>
    /// <returns></returns>
    internal string[] PlayerFaceToRichText(Variable.PlayerFaceType[] playerFaceType)
    {
        //用于返回的临时变量
        string[] j = new string[3];

        for (int i = 0; i < 3; i++)
        {
            try
            {
                //如果该位置有玩家，正常转化
                if (playerFaceType[i] != Variable.PlayerFaceType.Null)
                {
                    j[i] = string.Format("{0}{1}{2}", "<sprite=\"PlayerFace\" index=", (int)playerFaceType[i],">");
                }

            }
            catch (System.Exception)
            {

                //如果该位置没有玩家，用空字符串占位
                j[i] = string.Empty;
            }
        }
        

        return j;


    }
    #endregion
}
