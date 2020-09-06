using BayatGames.SaveGameFree;
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

    [Header("最大帧率")]
    public int MaxFps = 60;

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
    /// 玩家选择的魔法少女的等级
    /// </summary>
    public int[] Level = new int[] { 1,1,1};

    /// <summary>
    /// 玩家选择的魔法少女的soullimit
    /// </summary>
    [HideInInspector]
    public int[] SoulLimitInGame = new int[] { 0, 0, 0 };
    /// <summary>
    /// 玩家选择的魔法少女的VIT
    /// </summary>
    public int[] VitInGame = new int[] { 0, 0, 0 };
    /// <summary>
    /// 玩家选择的魔法少女的Pow
    /// </summary>
    public int[] PowInGame = new int[] { 0, 0, 0 };
    /// <summary>
    /// 是否按下Magia键
    /// </summary>
    public bool[] MagiaKeyDown = new bool[] { false, false, false };
    /// <summary>
    /// 玩家选择的魔法少女的最大vit
    /// </summary>
    public int[] MaxVitInGame = new int[] { 0, 0, 0 };
    /// <summary>
    /// 玩家受伤损失的vit
    /// </summary>
    public int[] HurtVitInGame = new int[] { 0, 0, 0 };
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
    /// 魔法少女是否变成魔女(2个吼姆拉放在一起了)
    /// </summary>
    [Header("魔法少女是否挂掉")]
    public bool[] MagicalGirlsDie = new bool[] { false,false,false,false,false };
    
    /// <summary>
    /// 正在打的魔女
    /// </summary>
    [Header("正在打的魔女")]
    public Variable.Majo MajoBeingBattled = Variable.Majo.Gertrud;
    /// <summary>
    /// 本次周目最新可食用的魔女
    /// </summary>
    [Header("本次周目最新可食用的魔女")]
    public Variable.Majo NewestMajo = Variable.Majo.Gertrud;
    /// <summary>
    /// 本次周目是否开启人鱼魔女
    /// </summary>
    [Header("本次周目是否开启人鱼魔女")]
    public bool AllowOktavia = false;

    /// <summary>
    /// 是从魔女结界中返回魔女选择part吗
    /// </summary>
    [Header("是从魔女结界中返回魔女选择part吗")]
    public bool MajoKeikaiToSelectPart = false;

    #endregion

    #region 玩家设置
    /// <summary>
    /// 魔法少女属性设置
    /// </summary>
    [Header("魔法少女属性设置")]
    [SerializeField] public Variable.MahouShoujo[] mahouShoujos;

    [Header("音量")]
    public float BGMVol = 0.6f;
    public float SEVol = 0.7f;
    #endregion



    /// <summary>
    /// GSS初始化（主标题part使用）
    /// </summary>
    public void TitleInitial()
    {
        //被赋值的属于临时数据，不保存
        //用于恢复开发时便于调试而弄的参数

        Score = new int[3] { 0, 0, 0 };
        Time = 0;
        SelectedGirlInGame = new Variable.PlayerFaceType[3] { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null };
        MajoBeingBattled = Variable.Majo.Gertrud;
        NewestMajo = Variable.Majo.Gertrud;
        AllowOktavia = false;
        MagicalGirlsDie = new bool[] { false, false, false, false, false };
        MajoKeikaiToSelectPart = false;
        Level = new int[] { 1, 1, 1 };
    }

    /// <summary>
    /// 每次进入魔女结界时初始化
    /// </summary>
    public void MajoInitial()
    {
        Hits = 0;
        VitInGame = new int[] { 0, 0, 0 };
        SoulLimitInGame = new int[] { 0, 0, 0 }; 
        MaxVitInGame = new int[] { 0, 0, 0 };
        HurtVitInGame = new int[] { 0, 0, 0 };
        GetHurtInGame = new bool[] { false, false, false };
        MagiaKeyDown = new bool[] { false, false, false };
        IsBodyDieInGame = new bool[] { false, false, false };
        IsSoulBallInGame = new bool[] { false, false, false };
        PowInGame = new int[] { 0, 0, 0 };
    }

    /// <summary>
    /// 保存（结算界面使用）
    /// </summary>
    public void Save()
    {
        //存档
        SaveGame.Save<int>("HighestScore", HiScore);
        SaveGame.Save<int>("MaxHits", MaxHits);
        SaveGame.Save("BestTime", BestTime);
        SaveGame.Save("LastLap", lap);
        //设置
        SaveGame.Save("BGMVol", BGMVol);
        SaveGame.Save("SEVol", SEVol);

    }

    /// <summary>
    /// 读取存档与设置（标题界面使用）
    /// </summary>
    public  void Load()
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
    }

}
