using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// n标题管理
/// </summary>
public class TitleCtrl : MonoBehaviour
{
    #region 分数管理
    public TMP_Text HiScore;
    public TMP_Text BestTime;
    public TMP_Text MaxHits;
    #endregion




    public static TitleCtrl titleCtrl;

    private void Awake()
    {
        titleCtrl = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        //BGM
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(0);
    }

public void StartGame(int lap)
    {

    }

    public void Exit()
    {
        Application.Quit();
    }



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
