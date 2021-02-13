using BayatGames.SaveGameFree;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 控制游戏中的分数，设置，IO (不包含图片)
/// </summary>
public class GameScoreSettingsIO : ScriptableObject
{
    #region 正式玩的变量


    [Header("正式玩的变量")]
    [Header("当前玩家分数")]
    public int[] Score = new int[3] { 0,0, 0 };
    [Header("历史最高分数")]
    public int HiScore = 10000;
    [Header("历史最高分数头像")]
    public Variable.PlayerFaceType[] HiScoreFace = new Variable.PlayerFaceType[]{ Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null , Variable.PlayerFaceType.Null };
    /// <summary>
    /// 当前连击
    /// </summary>
    [Header("当前连击")]
    public int Hits = 0;
    [Header("历史最高hit")]
    public int MaxHits = 0;
    [Header("历史最高hit头像")]
    public Variable.PlayerFaceType[] MaxHitsFace = new Variable.PlayerFaceType[] { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null };
    /// <summary>
    /// 本周目游玩时间
    /// </summary>
    [Header("本周目游玩时间")]
    public int Time = 0;
    [Header("历史最短时间")]
    public int BestTime = 0;
    [Header("历史最短时间头像")]
    public Variable.PlayerFaceType[] BestTimeFace = new Variable.PlayerFaceType[] { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null };
    [Header("正在打的周目数")]
    public int lap = 1;

    /// <summary>
    /// 本地玩家选择的魔法少女的魔法少女id
    /// </summary>
    [Header("本地玩家选择的魔法少女的魔法少女id")]
    public int PlayerSelectedGirlId = 0;
    /// <summary>
    /// 本地玩家是否僵直
    /// </summary>
    public bool LocalIsStiff = false;

    /// <summary>
    /// 本次游戏通关了吗
    /// </summary>
    [HideInInspector]
    public bool Success = false;

    /// <summary>
    /// 上次游戏的周目数
    /// </summary>
    [Header("上次游戏的周目数")]
    public int LastLap = 1;
    /// <summary>
    /// 玩家选择的魔法少女
    /// </summary>
    [Header("玩家选择的魔法少女")]//null用于占位子
    public Variable.PlayerFaceType[] SelectedGirlInGame = new Variable.PlayerFaceType[3] { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null };

    /// <summary>
    /// 魔法少女等级
    /// </summary>
    [Header("魔法少女等级")]
    public int[] GirlsLevel = new int[] { 1,1,1,1,1 };

    /// <summary>
    /// 魔法少女灵魂值
    /// </summary>
    public int[] GirlSoulLimit = new int[] { 0, 0, 0 ,0,0};

    /// <summary>
    /// 魔法少女生命值
    /// </summary>
    public int[] GirlsVit = new int[] { 0, 0, 0 ,0,0};

    /// <summary>
    /// 玩家选择的魔法少女的Pow（power）
    /// </summary>
    public int[] GirlsPow = new int[] { 0, 0, 0,0,0 };
  
    
    
    /// <summary>
    /// 是否按下Magia键（用于玩家信息更新，在玩家自己的脚本中修改）
    /// </summary>
    public bool[] MagiaKeyDown = new bool[] { false, false, false };
    /// <summary>
    /// 玩家受伤损失的vit
    /// </summary>
   // public int[] HurtVitInGame = new int[] { 0, 0, 0 };
    /// <summary>
    /// 受伤了吗
    /// </summary>
    public bool[] GetHurtInGame = new bool[] { false, false, false };
    /// <summary>
    /// 玩家身体死了吗
    /// </summary>
    public bool[] IsBodyDieInGame = new bool[] { false, false, false };
    /// <summary>
    /// 变成灵魂球了吗
    /// </summary>
    public bool[] IsSoulBallInGame = new bool[] { false, false, false };

    /// <summary>
    ///  魔女或者所选的魔法少女都死了吗
    /// </summary>
    [Header("魔女或者所选的魔法少女都死了吗")]
    public bool DoesMajoOrShoujoDie = false;

    /// <summary>
    /// 魔法少女是否变成魔女(2个吼姆拉放在一起了)
    /// </summary>
    [Header("魔法少女是否挂掉")]
    public bool[] MagicalGirlsDie = new bool[] { false,false,false,false,false };

    public Sprite[] PlayerDieImage = new Sprite[6];

    [Header("宝石坏了之后，玩家消失的动画")]
    public RuntimeAnimatorController GemBrokenFadeAnimator;


    /// <summary>
    /// 五色全挂了吗
    /// </summary>
    [Header("五色全挂了吗")]
    public bool AllDie = false;

    /// <summary>
    /// 正在打的魔女
    /// </summary>
    [Header("正在打的魔女")]
    public Variable.Majo BattlingMajo = Variable.Majo.Gertrud;

    /// <summary>
    /// 本次周目最新可食用的魔女
    /// </summary>
    [Header("本次周目最新可食用的魔女")]
    public Variable.Majo NewestMajo = Variable.Majo.Gertrud;

    /// <summary>
    /// 是从魔女场景中返回标题界面吗（不包括返回到主标题按钮）
    /// </summary>
    [HideInInspector]
    public bool MajoSceneToTitle = false;

    #endregion


    #region  主标题 设置  
    [Header("音量")]
    public float BGMVol = 0.6f;
    public float SEVol = 0.7f;
    public int MaxFps = 60;
    #endregion

    #region 玩家设置


    /// <summary>
    /// 魔法少女属性设置
    /// </summary>
    [Header("魔法少女属性设置")]
    [SerializeField] public Variable.MahouShoujo[] mahouShoujos;

    /// <summary>
    /// 虚拟按键-全部按钮位置大小设置（参考分辨率1010x568)
    /// </summary>
    [Header("虚拟按键-全部按钮位置大小设置")]
    [SerializeField] public Variable.VirtualKeyInputPositionAndScale[] KeyPosScale;

    /// <summary>
    /// 使用屏幕模拟键盘输入，0时禁用键盘输入 1按钮移动 2圆盘移动
    /// </summary>
    public int UseScreenInput = 0;


    [HideInInspector] public Vector2 joystick = Vector2.zero;

    #endregion

    #region 输入变量管理（所有的按键/屏幕输入的变量都在这里）
    /// <summary>
    /// 水平输入
    /// </summary>
    [HideInInspector] public int Horizontal = 0;
    /// <summary>
    /// 穿墙（地板）的时候，按↓的时候用的
    /// </summary>
    [HideInInspector] public bool Down = false;
    [HideInInspector] public bool Up = false;
    [HideInInspector] public bool Jump = false;
    [HideInInspector] public bool Zattack = false;
    [HideInInspector] public bool ZattackPressed = false;//长按
    [HideInInspector] public bool Xattack = false;
    [HideInInspector] public bool XattackPressed = false;
    [HideInInspector] public bool Magia = false;
    [HideInInspector] public bool MagiaPressed = false;
    [HideInInspector] public bool Pause = false;
    //一下为本测试版临时添加的
    [HideInInspector] public bool CleanSoul = false;
    [HideInInspector] public bool CleanVit = false;
    [HideInInspector] public bool HurtMyself = false;
    [HideInInspector] public bool Succeed = false;
    [HideInInspector] public bool LevelUp = false;

    #endregion


#if UNITY_EDITOR
    /// <summary>
    /// 全部初始化
    /// </summary>
    [ContextMenu("全部初始化")]
    public void AllInitial()
    {
        TitleInitial();
        MajoInitial();
        MaxFps = 60;
        BGMVol = 0.6f;
        SEVol = 0.7f;
        UseScreenInput = 2;
        Load();
        //做啥角色就换成啥
        SelectedGirlInGame[0] = Variable.PlayerFaceType.Sayaka;
       //删除存档
        SaveGame.DeleteAll();
        MajoSceneToTitle = false;//一定要放到魔女初始化后面

    }

    /// <summary>
    /// ApplyEditPositionToRawPosition
    /// </summary>
    [ContextMenu("ApplyEditPositionToRawPosition")]
    public void ApplyEditPositionToRawPosition()
    {
        for (int i = 0; i < KeyPosScale.Length; i++)
        {
            KeyPosScale[i].RawPosition = KeyPosScale[i].EditPosition;
        }
    }
#endif

    /// <summary>
    /// GSS初始化（主标题part使用）
    /// </summary>
    public void TitleInitial()
    {
        //被赋值的属于临时数据，不保存
        //用于恢复开发时便于调试而弄的参数

        Score = new int[3] { 0, 0, 0 };
        Success = false;
        Time = 0;
        DoesMajoOrShoujoDie = false;
        SelectedGirlInGame = new Variable.PlayerFaceType[3] { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null };
        BattlingMajo = Variable.Majo.Gertrud;
        NewestMajo = Variable.Majo.Gertrud;
        MagicalGirlsDie = new bool[] { false, false, false, false, false };
        GirlsLevel = new int[] { 1, 1, 1 ,1,1};
        AllDie = false;
        //0.07
        Succeed = false;
        Pause = false;
        MagicalGirlsDie = new bool[] { true, true, true, true, false };
    }

    /// <summary>
    /// 每次进入魔女结界时初始化
    /// </summary>
    public void MajoInitial()
    {
        //输入初始化
        Jump = false;
        Zattack = false;
        ZattackPressed = false;
        Xattack = false;
        XattackPressed = false;
        Magia = false;
        MagiaPressed = false;
        Horizontal = 0;
        Up = false;
        Down = false;
        Pause = false;

        MajoSceneToTitle = true;
        Hits = 0;
        //HurtVitInGame = new int[] { 0, 0, 0 };
        DoesMajoOrShoujoDie = false;
        Succeed = false;
        GetHurtInGame = new bool[] { false, false, false };
        MagiaKeyDown = new bool[] { false, false, false };
        IsBodyDieInGame = new bool[] { false, false, false };
        IsSoulBallInGame = new bool[] { false, false, false };
        LastLap = lap;

        Succeed = false;
        //0.0.7
        CleanSoul = false;
        CleanVit = false;
        Pause = false;

    }

    [ContextMenu("使输入方式返回到最初的大小与位置")]
    public void RevokeInputChange()
    {
        for (int i = 0; i < KeyPosScale.Length; i++)
        {
              KeyPosScale[i].EditPosition = KeyPosScale[i].RawPosition;
        }
    }


#if UNITY_EDITOR
    [ContextMenu("保存存档与设置")]
   public void SaveInEditor()
    {
         Timing.RunCoroutine(SaveAll());
    }
#endif



    #region  保存与加载

    //保存设置：标题界面完成设置、返回主标题part的时候调用、打完瓦夜
    //保存存档：打完瓦夜
    //加载：异步加载场景的时候用

    /// <summary>
    /// 向硬盘保存存档与设置（瓦夜结算使用）
    /// </summary>
    public IEnumerator<float> SaveAll()
    {

        //存档
        SaveGame.Save("HighestScore", HiScore);
        SaveGame.Save("HiScoreFace", HiScoreFace);
        SaveGame.Save("MaxHits", MaxHits);
        SaveGame.Save("MaxHitsFace", MaxHitsFace);
        SaveGame.Save("BestTime", BestTime);
        SaveGame.Save("BestTimeFace", BestTimeFace);
        SaveGame.Save("LastLap", lap);
        //设置
        SaveGame.Save("BGMVol", BGMVol);
        SaveGame.Save("SEVol", SEVol);
        SaveGame.Save("MaxFps", MaxFps);
        SaveInput();

        yield return 0f;
    }   

    /// <summary>
    /// 保存设置
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> SaveSettings()
    {
        //设置
        SaveGame.Save("BGMVol", BGMVol);
        SaveGame.Save("SEVol", SEVol);
        SaveGame.Save("MaxFps", MaxFps);
        SaveInput();

        yield return 0f;

    }


    /// <summary>
    /// 保存输入设置
    /// </summary>
    public void SaveInput()
    {
        SaveGame.Save("UseScreenInput", UseScreenInput);
      
        for (int i = 0; i < KeyPosScale.Length; i++)
        {
            SaveGame.Save(string.Format("KeyPosScale_{0}_Rect", i.ToString()), KeyPosScale[i].EditPosition);
        }


    }



    /// <summary>
    /// 从硬盘读取存档与设置（标题界面使用）
    /// </summary>
    [ContextMenu("读取存档与设置")]
    public void Load()
    {
        //存档
        HiScore = SaveGame.Load<int>("HighestScore", 10000);
        HiScoreFace = SaveGame.Load("HiScoreFace", new Variable.PlayerFaceType[] { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null });
        MaxHits = SaveGame.Load<int>("MaxHits", 0);
        MaxHitsFace = SaveGame.Load("MaxHitsFace", new Variable.PlayerFaceType[] { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null });
        BestTime = SaveGame.Load("BestTime", 0);
        BestTimeFace = SaveGame.Load("BestTimeFace", new Variable.PlayerFaceType[] { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null });
        lap = SaveGame.Load("LastLap", 1);
       
        //设置
        BGMVol = SaveGame.Load("BGMVol", 0.6f);
        SEVol = SaveGame.Load("SEVol", 0.7f);
        MaxFps = SaveGame.Load("MaxFps", 60);
        LoadInput();

    }

    /// <summary>
    /// 加载输入设置
    /// </summary>
    public void LoadInput()
    {
        SaveGame.Load("UseScreenInput", 2);

        for (int i = 0; i < KeyPosScale.Length; i++)
        {
            SaveGame.Load(string.Format("KeyPosScale_{0}_Rect", i.ToString()), KeyPosScale[i].RawPosition);
        }

    }

    #endregion

    /// <summary>
    /// 刷新最高分数，最短时间，最高连击，当前玩的lap（瓦夜结算使用）
    /// </summary>
    [ContextMenu("刷新最高分数，最短时间，最高连击，当前玩的lap（瓦夜结算使用）")]
    public void RefreshBestScoreAndSoOn()
    {
        //最高分数刷新
        foreach (var item in Score)
        {
            if(item > HiScore)
            {
                HiScore = item;
                HiScoreFace = SelectedGirlInGame;
            }
        }

        //最短时间刷新
        if(Time < BestTime || BestTime == 0)
        {
            BestTime = Time;
            BestTimeFace = SelectedGirlInGame;
        }

        //最高连击刷新
        if(Hits > MaxHits)
        {
            MaxHits = Hits;
            MaxHitsFace = SelectedGirlInGame;
        }

        //上一次打的lap刷新
        LastLap = lap;
    }
}
