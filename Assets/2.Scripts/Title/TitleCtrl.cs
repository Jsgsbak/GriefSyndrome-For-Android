using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MEC;
using UnityEngine.EventSystems;
/// <summary>
/// n标题管理
/// </summary>
public class TitleCtrl : MonoBehaviour
{
    #region UI管理
    [Space]
    //分数显示
    public TMP_Text HiScore;
    public TMP_Text BestTime;
    public TMP_Text MaxHits;
    [Space]
    [Header("主标题按钮控制")]
    public Button StartGameButton;
    public TMP_InputField LapInput;
    public Button SettingsButton;
    public Button ExitButton;
    /// <summary>
    /// 场景切换控制 顺序：MainTitle，SelectMajo，SelectMaigcalGirl
    /// </summary>
    [Space]
    //场景切换控制
    [Header("场景切换控制 顺序：MainTitle，SelectMajo，SelectMaigcalGirl")]
    public CanvasGroup[] ChangePart;

    [Space]
    [Header("魔女照片控制")]
    /// <summary>
    /// 魔女照片
    /// </summary>
    public Image[] MajoPictures;
    /// <summary>
    /// 可以食用魔女的时候显示的图片
    /// </summary>
    public Sprite[] MajoPictureEnable;
    /// <summary>
    /// 不可以食用魔女的时候显示的图片
    /// </summary>
    public Sprite[] MajoPictureDisable;
    /// <summary>
    /// 溜了溜了
    /// </summary>
    public Button ExitMajo;

    [Header("魔法少女选择控制")]
    /// <summary>
    /// 溜了溜了
    /// </summary>
    public Button ExitMagicalGirls;
    #endregion


    /// <summary>
    /// 尽量所有的属性/设置都从这里读取/写入
    /// </summary>
    public static GameScoreSettingsIO gameScoreSettingsIO;//进了别的场景之后托管了就
    public static TitleCtrl titleCtrl;
    /// <summary>
    /// 控制EventSystem用于阻止用户非正常输入
    /// </summary>
    [Space(50)]
    public  GameObject eventSystem;
    private void Awake()
    {
        //组件获取/初始化
        titleCtrl = this;
        gameScoreSettingsIO = Resources.Load("GameScoreAndSettings") as GameScoreSettingsIO;
        gameScoreSettingsIO.Initial();

    }


    // Start is called before the first frame update
    void Start()
    {
        //淡入进入MainTitle part（用于刚刚打开游戏）
        ChangePart[0].gameObject.SetActive(true);
        ChangePart[0].alpha = 0;
        Timing.RunCoroutine(ChangePartMethod(-1, 0));
        //禁用其他Part
        ChangePart[1].gameObject.SetActive(false);
        ChangePart[2].gameObject.SetActive(false);

        //BGM
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(0);

        #region  注册组件
        //主标题part
        LapInput.onValueChanged.AddListener(delegate (string lap) { gameScoreSettingsIO.lap = int.Parse(lap); });//向GSS中写入周目数
        StartGameButton.onClick.AddListener(delegate () { Timing.RunCoroutine(ChangePartMethod(0, 1)); });//进入魔女选择part
        ExitButton.onClick.AddListener(delegate () { Application.Quit(0); });//关闭游戏

        //魔女选择part
        ExitMajo.onClick.AddListener(delegate () { Timing.RunCoroutine(ChangePartMethod(1, 0)); });//返回到主标题part

        //魔法少女选择part
        ExitMagicalGirls.onClick.AddListener(delegate () { Timing.RunCoroutine(ChangePartMethod(2, 1)); });
        #endregion
    }

    #region 用于处理的按钮的方法

    /// <summary>
    /// 检查魔女是否可以食用
    /// </summary>
    private void CheckMajo()
    {
        //检查最新的魔女，以开启相对应的关卡
        for (int i = 0; i <= 5; i++)
        {
            //不包含人鱼魔女
            if (i != 5)
            {
                if (i <= (int)gameScoreSettingsIO.NewestMajo)
                {
                    MajoPictures[i].sprite = MajoPictureEnable[i];
                }
                else
                {
                    MajoPictures[i].sprite = MajoPictureDisable[i];
                }
            }
            else
            {
                //人鱼魔女单独处理
                if (gameScoreSettingsIO.AllowOktavia)
                {
                    MajoPictures[i].gameObject.SetActive(true);
                }
                else
                {
                    MajoPictures[i].gameObject.SetActive(false);
                }
            }

        }



    }

    /// <summary>
    /// 选择魔女（检查视图注入）
    /// </summary>
    public void StartHuntering(int MajoId)
    {
        //储存要打的魔女
        if (MajoId <= (int)gameScoreSettingsIO.NewestMajo && MajoId != 5) { gameScoreSettingsIO.MajoBeingBattled = (Variable.Majo)MajoId; Timing.RunCoroutine(ChangePartMethod(-1, 2));}
        else if (MajoId == 5 && gameScoreSettingsIO.AllowOktavia) { gameScoreSettingsIO.MajoBeingBattled = (Variable.Majo)MajoId; Timing.RunCoroutine(ChangePartMethod(-1, 2)); }
       //后续处理
    }

    #endregion



    /// <summary>
    /// 调整标题界面展示的分数
    /// </summary>
    /// <param name="scoreType"></param>
    /// <param name="Parameter"></param>
    /// <param name="PlayerFaces"></param>
    public void AdjustScore(Variable.ScoreType scoreType, string Parameter, Variable.PlayerFaceType[] PlayerFaces)
    {
        if (scoreType == Variable.ScoreType.BestTime)
        {
            BestTime.text = string.Format("{0} {1}  {2} {3} {4}", " Best Time", Parameter, PlayerFaceToRichText(PlayerFaces)[0], PlayerFaceToRichText(PlayerFaces)[1], PlayerFaceToRichText(PlayerFaces)[2]);
        }
        else if (scoreType == Variable.ScoreType.HiScore)
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
    /// 切换part（在MainTitle SelectMajo SelectMaigcalGirl三个part中切换）
    /// </summary>
    /// <param name="OutId"></param>
    /// <param name="InId"></param>
    /// <returns></returns>
    public IEnumerator<float> ChangePartMethod(int OutId, int InId)
    {
        //禁用输入防止bug
        eventSystem.SetActive(false);

        //事先处理一些数据
        if (InId == 1)
        {
            //如果来的是SelectMajo，则检查一下马酒
            CheckMajo();
        }

        //一个先变黑另外一个才出来
        if (OutId >= 0)
        {

            for (int i = 0; i < 40; i++)
            {
                ChangePart[OutId].alpha -= 0.025f;
                yield return Timing.WaitForSeconds(0.015f);
            }
            ChangePart[OutId].gameObject.SetActive(false);
        }

        if (InId >= 0)
        {
            ChangePart[InId].alpha = 0;
            ChangePart[InId].gameObject.SetActive(true);

            for (int i = 0; i < 40; i++)
            {
                ChangePart[InId].alpha += 0.025f;
                yield return Timing.WaitForSeconds(0.015f);
            }

        }

        //允许玩家输入
        eventSystem.SetActive(true);

    }


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
                    j[i] = string.Format("{0}{1}{2}", "<sprite=\"PlayerFace\" index=", (int)playerFaceType[i], ">");
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
