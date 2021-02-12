using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MEC;
using UnityEngine.SceneManagement;
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

    [Header("版本号")]
    public TMP_Text Version;

    [Space]
    [Header("主标题按钮控制")]
    public Button StartGameButton;
    public TMP_InputField LapInput;
    public Button Settings;
    public Button ExitButton;
    public Button RandomStaff;

    [Space]
    [Header("设置界面")]
    public Slider BGMVol;
    public Slider SEVol;
    public TMP_InputField MaxFpsField;
    public Button[] SettingsReturnToTitle = new Button[2];

    /// <summary>
    /// 场景切换控制 顺序：MainTitle，SelectMajo，SelectMaigcalGirl
    /// </summary>
    [Space]
    //场景切换控制
    [Header("场景切换控制 顺序：MainTitle，SelectMajo，SelectMaigcalGirl Settings")]
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
    public Image[] Mahoshaojo;
    /// <summary>
    /// 溜了溜了
    /// </summary>
    public Button ExitMagicalGirls;
    #endregion


    [Header("检查视图中的预设")]
    public EasyBGMCtrl PerfebInAsset;


    /// <summary>
    /// 尽量所有的属性/设置都从这里读取/写入
    /// </summary>
    public static GameScoreSettingsIO gameScoreSettingsIO;//进了别的场景之后托管了就
    public static TitleCtrl titleCtrl;
    /// <summary>
    /// 控制EventSystem用于阻止用户非正常输入
    /// </summary>
    [Space(50)]
    public GameObject eventSystem;
    private void Awake()
    {
        //组件获取
        titleCtrl = this;
        gameScoreSettingsIO = Resources.Load("GameScoreAndSettings") as GameScoreSettingsIO;

        //版本 号
        Version.text = string.Format("Ver.{0}", Application.version);
    }


    // Start is called before the first frame update
    void Start()
    {
        //限制帧率
        Application.targetFrameRate = gameScoreSettingsIO.MaxFps;


#if UNITY_EDITOR
        //检查是否存在BGMCtrl（仅供调试）
        if (GameObject.FindObjectOfType<EasyBGMCtrl>() == null)
        {
            EasyBGMCtrl easyBGMCtrl = Instantiate(PerfebInAsset).GetComponent<EasyBGMCtrl>();
            easyBGMCtrl.IsClone = true;
        }
#endif

        //用于防止多次不必要执行一些操作
        //刚打开游戏/staff返回到标题，并不是从魔女场景中返回
        if (!gameScoreSettingsIO.MajoSceneToTitle)
        {

            //存档与设置获取
          //  gameScoreSettingsIO.Load();//这里确实要堵一下主线程
            /*初始化游戏（用于游戏刚开始的初始化。）
             * 游戏中途返回标题：Majo场景的Return to Title按钮执行初始化
             * 游戏结束（显示staff）：由StaffCtrl执行在返回标题的一瞬间初始化
             */
            gameScoreSettingsIO.TitleInitial();

            //计分板调整
            AdjustScoreAndTime(Variable.ScoreType.BestTime, gameScoreSettingsIO.BestTime.ToString(), gameScoreSettingsIO.BestTimeFace);
            AdjustScoreAndTime(Variable.ScoreType.HiScore, gameScoreSettingsIO.HiScore.ToString(), gameScoreSettingsIO.HiScoreFace);
            AdjustScoreAndTime(Variable.ScoreType.MaxHits, gameScoreSettingsIO.MaxHits.ToString(), gameScoreSettingsIO.MaxHitsFace);


            //淡入MainTitle part（用于刚刚打开游戏）
            ChangePart[0].gameObject.SetActive(true);
            ChangePart[0].alpha = 0;
            Timing.RunCoroutine(ChangePartMethod(-1, 0));
            //禁用其他Part
            ChangePart[1].gameObject.SetActive(false);
            ChangePart[2].gameObject.SetActive(false);
            ChangePart[3].gameObject.SetActive(false);
        }

        //从魔女场景返回，直接打开魔女选择part
        else
        {
            ChangePart[0].gameObject.SetActive(false);
            ChangePart[3].gameObject.SetActive(false);

            Timing.RunCoroutine(ChangePartMethod(-1, 1));

        }


        //BGM和音效的音量从gss中读取，并同步显示在界面中
        EasyBGMCtrl.easyBGMCtrl.PlayBGM(0);
        EasyBGMCtrl.easyBGMCtrl.ChangeVol(gameScoreSettingsIO.BGMVol, true);
        EasyBGMCtrl.easyBGMCtrl.ChangeVol(gameScoreSettingsIO.SEVol, false);
        //最大帧率从GSS中获取，并同步显示在界面中
        MaxFpsField.text = gameScoreSettingsIO.MaxFps.ToString();
        //输入的话有自己的脚本

        #region  注册组件
        //主标题part
        LapInput.onValueChanged.AddListener(delegate (string lap) { gameScoreSettingsIO.lap = int.Parse(lap); });//向GSS中写入周目数

        //设置即时保存
        //音量
        BGMVol.onValueChanged.AddListener(BGMVolChange);
        SEVol.onValueChanged.AddListener(SEVolChange);
        //帧率输入合法性检查（仅支持非负数）
        MaxFpsField.onValueChanged.AddListener(delegate (string s) 
        {
            int.TryParse(s, out int d);

            if (d < 0)
            {
                d = -d;
            }
            else if(d < 30 && d!=0)
            {
                d = 30;
            }

            MaxFpsField.text = d.ToString();

        });
        MaxFpsField.onEndEdit.AddListener(delegate (string s) { int.TryParse(s, out gameScoreSettingsIO.MaxFps); Application.targetFrameRate = gameScoreSettingsIO.MaxFps;});

        //进入魔女选择part的音效放在了ChangePartMethod中
        StartGameButton.onClick.AddListener(delegate () { EasyBGMCtrl.easyBGMCtrl.PlaySE(0); Timing.RunCoroutine(ChangePartMethod(0, 1)); });//进入魔女选择part
        ExitButton.onClick.AddListener(delegate () { Timing.RunCoroutine( gameScoreSettingsIO.SaveSettings());/*这里保存一下*/   Application.Quit(0); });//关闭游戏
        RandomStaff.onClick.AddListener(delegate () { EasyBGMCtrl.easyBGMCtrl.PlaySE(0); RandomKillGirl(); });
        Settings.onClick.AddListener(delegate () { EasyBGMCtrl.easyBGMCtrl.PlaySE(0); Timing.RunCoroutine(ChangePartMethod(0, 3)); });
        SettingsReturnToTitle[0].onClick.AddListener(delegate () { EasyBGMCtrl.easyBGMCtrl.PlaySE(0); Timing.RunCoroutine(gameScoreSettingsIO.SaveSettings()); Timing.RunCoroutine(ChangePartMethod(3, 0)); });
        SettingsReturnToTitle[1].onClick.AddListener(delegate () { EasyBGMCtrl.easyBGMCtrl.PlaySE(0); Timing.RunCoroutine(gameScoreSettingsIO.SaveSettings()); Timing.RunCoroutine(ChangePartMethod(3, 0)); });
        //魔女选择part
        ExitMajo.onClick.AddListener(delegate () { EasyBGMCtrl.easyBGMCtrl.PlaySE(1); Timing.RunCoroutine(ChangePartMethod(1, 0)); });//返回到主标题part

        //魔法少女选择part
        ExitMagicalGirls.onClick.AddListener(delegate () { EasyBGMCtrl.easyBGMCtrl.PlaySE(1); Timing.RunCoroutine(ChangePartMethod(2, -1)); });//范围到魔女选择part
       
        
        
        #endregion
    }

    #region 用于处理的按钮的方法

    //不放在那个委托里是因为每次显示这个part的时候都要检查一次
    /// <summary>
    /// 检查魔法少女是否可用
    /// </summary>
    public void CheckMahoshoujo()
    {
        for (int i = 0; i < 6; i++)
        {

            //麻花焰单独处理
            if(i == 5)
            {
                //如果魔法少女扑街，那么变灰
                if (gameScoreSettingsIO.MagicalGirlsDie[0])
                {
                    Mahoshaojo[i].color = new Color(0.2075f, 0.2075472f, 0.2075472f);
                }
                else
                {
                    Mahoshaojo[i].color = Color.white;
                }
            }
            else
            {
                //如果魔法少女扑街，那么变灰
                if (gameScoreSettingsIO.MagicalGirlsDie[i])
                {
                    Mahoshaojo[i].color = new Color(0.2075f, 0.2075472f, 0.2075472f);
                }
                else
                {
                    Mahoshaojo[i].color = Color.white;
                }

            }
        }
    }

    //不放在那个委托里是因为每次显示这个part的时候都要检查一次
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
                if (gameScoreSettingsIO.MagicalGirlsDie[4])
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
    public void SelectedMajo(int MajoId)
    {
        //储存要打的魔女&切换到选择魔法少女part
        if (MajoId <= (int)gameScoreSettingsIO.NewestMajo && MajoId != 5) { EasyBGMCtrl.easyBGMCtrl.PlaySE(0); gameScoreSettingsIO.BattlingMajo = (Variable.Majo)MajoId; Timing.RunCoroutine(ChangePartMethod(-1, 2)); }
        else if (MajoId == 5 && gameScoreSettingsIO.MagicalGirlsDie[4]) { EasyBGMCtrl.easyBGMCtrl.PlaySE(0); gameScoreSettingsIO.BattlingMajo = (Variable.Majo)MajoId; Timing.RunCoroutine(ChangePartMethod(-1, 2)); }

    }

    //没有适配多人游戏！
    /// <summary>
    /// 选择魔法少女（检查视图注入） 
    /// </summary>
    public void SelectedMahoshoujo(int id)
    {

        //确认音效 多人游戏的时候前2个玩家选择后播放。
        //  EasyBGMCtrl.easyBGMCtrl.PlaySE(0);
        //最后一个玩家，播放进入魔女结界的音效
        EasyBGMCtrl.easyBGMCtrl.PlaySE(4);


        //麻花焰单独处理
        if (id == 5)
        {
            if (gameScoreSettingsIO.MagicalGirlsDie[0])
            {
                //挂了，选择为QB
                gameScoreSettingsIO.SelectedGirlInGame[0] = Variable.PlayerFaceType.QB;//玩家1，联机的话要在处理 0：玩家1
            }
            else
            {
                //没挂，正常选择
                gameScoreSettingsIO.SelectedGirlInGame[0] = (Variable.PlayerFaceType)id;//玩家1，联机的话要在处理0：玩家1


                //应该所有玩家都选择完在进行处理
                //切换场景
                LoadingCtrl.LoadScene(2);

            }

            return;
        }


      else  if (gameScoreSettingsIO.MagicalGirlsDie[id])
        {
            //挂了，选择为QB 0.0.7暂时禁用
            //gameScoreSettingsIO.SelectedGirlInGame[0] = Variable.PlayerFaceType.QB;//玩家1，联机的话要在处理 0：玩家1
        }
        else
        {
            //没挂，正常选择
            gameScoreSettingsIO.SelectedGirlInGame[0] = (Variable.PlayerFaceType)id;//玩家1，联机的话要在处理0：玩家1

            //应该所有玩家都选择完在进行处理
            //切换场景
            LoadingCtrl.LoadScene(2);

        }


    }

    /// <summary>
    /// 随机staff用。随机杀死魔法少女（？）
    /// </summary>
    public void RandomKillGirl()
    {
        int i = Random.Range(0, 11);
        // i == 0 :全员幸存
        if (i == 1) //只有鹿目圆死亡
        {
            gameScoreSettingsIO.MagicalGirlsDie[2] = true;
        }
        //只有可怜的蓝毛死亡
        else if (i == 2)
        {
            gameScoreSettingsIO.MagicalGirlsDie[4] = true;
        }
        //只有杏子死亡
        else if (i == 3)
        {
            gameScoreSettingsIO.MagicalGirlsDie[1] = true;
        }
        //除了蓝毛红毛都死了
        else if (i == 4)
        {
            gameScoreSettingsIO.MagicalGirlsDie[0] = true;
            gameScoreSettingsIO.MagicalGirlsDie[2] = true;
            gameScoreSettingsIO.MagicalGirlsDie[3] = true;
        }
        //除了学姐都死了
        else if (i == 5)
        {
            gameScoreSettingsIO.MagicalGirlsDie[0] = true;
            gameScoreSettingsIO.MagicalGirlsDie[1] = true;
            gameScoreSettingsIO.MagicalGirlsDie[2] = true;
            gameScoreSettingsIO.MagicalGirlsDie[4] = true;
        }
        //只有学姐死了
        else if (i == 6)
        {
            gameScoreSettingsIO.MagicalGirlsDie[3] = true;
        }
        //只有鹿目圆和沙耶加死亡
        else if (i == 7)
        {
            gameScoreSettingsIO.MagicalGirlsDie[4] = true;
            gameScoreSettingsIO.MagicalGirlsDie[2] = true;

        }
        //除了粉焰全挂了
        else if (i == 8)
        {
            gameScoreSettingsIO.MagicalGirlsDie[1] = true;
            gameScoreSettingsIO.MagicalGirlsDie[3] = true;
            gameScoreSettingsIO.MagicalGirlsDie[4] = true;
        }
        //全挂了
        else if (i == 9)
        {
            gameScoreSettingsIO.MagicalGirlsDie[0] = true;
            gameScoreSettingsIO.MagicalGirlsDie[1] = true;
            gameScoreSettingsIO.MagicalGirlsDie[2] = true;
            gameScoreSettingsIO.MagicalGirlsDie[3] = true;
            gameScoreSettingsIO.MagicalGirlsDie[4] = true;
        }
        //轮回吧，吼姆拉
        else
        {
            gameScoreSettingsIO.MagicalGirlsDie[0] = true;
        }

        LoadingCtrl.LoadScene(4, false);
    }

    #endregion

    #region 音量滑块
    public void BGMVolChange(float vol)
    {
        gameScoreSettingsIO.BGMVol = vol;
        EasyBGMCtrl.easyBGMCtrl.ChangeVol(vol, true);
    }
    public void SEVolChange(float vol)
    {
        gameScoreSettingsIO.SEVol = vol;
        EasyBGMCtrl.easyBGMCtrl.ChangeVol(vol, false);

    }

    #endregion

    /// <summary>
    /// 调整标题界面展示的分数与最佳时间
    /// </summary>
    /// <param name="scoreType"></param>
    /// <param name="Parameter"></param>
    /// <param name="PlayerFaces"></param>
    public void AdjustScoreAndTime(Variable.ScoreType scoreType, string Parameter, Variable.PlayerFaceType[] PlayerFaces)
    {
        if (scoreType == Variable.ScoreType.BestTime)
        {
            BestTime.text = string.Format("{0} {1}  {2} {3} {4}", " Best Time", IntTimeFormat(int.Parse(Parameter)), PlayerFaceToRichText(PlayerFaces)[0], PlayerFaceToRichText(PlayerFaces)[1], PlayerFaceToRichText(PlayerFaces)[2]);

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

    /// <summary>
    /// 将整数时间格式化
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string IntTimeFormat(int time)
    {

        //总秒数转换为时分秒
        int h = Mathf.FloorToInt(time / 3600);
        int m = Mathf.FloorToInt(time / 60 - h * 60);
        int s = Mathf.FloorToInt(time - m * 60 - h * 3600);
       return string.Format("{0}:{1}:{2}", h.ToString("00"), m.ToString("00"), s.ToString("00"));

    }

    public  void SetMaxFps()
    {
        Application.targetFrameRate = gameScoreSettingsIO.MaxFps;
    }

    //<sprite="PlayerFace" index=1> 
    #region 内部方法



    /// <summary>
    /// 切换part和每个part需要的数据处理（在MainTitle SelectMajo SelectMaigcalGirl InputSettings四个part中切换）
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
            Debug.Log("检查");
            //音频
            EasyBGMCtrl.easyBGMCtrl.PlaySE(3);
        }
        else if (InId == 2)
        {
            //如果来的是SelectMagicalGirls，则检查一下马猴烧酒
            CheckMahoshoujo();

        }
        //主标题part
        else
        {
            #region 从存档中读取主标题part中的保存数据，lap ,音量
            BGMVol.value = gameScoreSettingsIO.BGMVol;
            SEVol.value = gameScoreSettingsIO.SEVol;
            LapInput.text = gameScoreSettingsIO.LastLap.ToString();
            #endregion
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
    public static string[] PlayerFaceToRichText(Variable.PlayerFaceType[] playerFaceType)
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
