using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PureAmaya.General;
using UnityEngine.UI;

public class UICtrl : MonoBehaviour
{
    //顺序按照已有顺序
    [Header("预设管理")]
    public GameObject[] PlayerInf;

    [Header("游戏UI设置")]
    public TMP_Text[] PlayerScore;

    public Image[] Player1Hp;
    public Image[] Player2Hp;
    public Image[] Player3Hp;


    //awake不能用
    private void Start()
    {
        #region 初始化UI界面
        //分数初始化
        for (int i = 0; i < 3; i++)
        {
            PlayerScore[i].text  = string.Format("{0}p Score {1}  {2}", (i+1).ToString(),StageCtrl.gameScoreSettings.Score[i] ,TitleCtrl.PlayerFaceToRichText(StageCtrl.gameScoreSettings.SelectedGirlInGame)[i]);


        }
        #endregion
    }


}
