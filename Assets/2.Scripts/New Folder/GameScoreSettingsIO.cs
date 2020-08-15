using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制游戏中的分数，设置，IO
/// </summary>
public class GameScoreSettingsIO : ScriptableObject
{
    #region 正式玩的时候用来储存变量
    [Header("正式玩的时候用来储存变量")]
    [Header("玩家分数")]
    public int[] Score = new int[3] { -1, -1, -1 };//-1:该位子没有玩家
    [Header("当前连击")]
    public int Hits = 0;
    [Header("本周目游玩时间")]
    public int[] Time = new int[3] { 0, 0, 0 };
    [Header("周目数")]
    public int lap = 1;
    /// <summary>
    /// 上次游戏的周目数
    /// </summary>
    public int LastLap = 1;
    [Header("玩家类型")]//null用于占位子
    public Variable.PlayerFaceType[] PlayerType = new Variable.PlayerFaceType[3] { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null };
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
    #endregion

    #region 玩家设置
    [Header("魔法少女属性设置")]
    [SerializeField] public Variable.MahouShoujo[] mahouShoujos;
    #endregion



    /// <summary>
    /// GSS初始化
    /// </summary>
    public void Initial()
    {
        Score = new int[3] { -1, -1, -1 };//-1:该位子没有玩家
        Hits = 0;
        Time = new int[3] { 0, 0, 0 };
        //读取上次游戏的周目数
        lap = LastLap;
        PlayerType = new Variable.PlayerFaceType[3] { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null };
        MajoBeingBattled = Variable.Majo.Gertrud;
        NewestMajo = Variable.Majo.Gertrud;
        AllowOktavia = false;
    }

}
