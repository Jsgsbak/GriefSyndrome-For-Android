using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PlayerRootCtrl : MonoBehaviour
{
    public static PlayerRootCtrl playerRootCtrl;

    public SayakaCtrl sayakaCtrl;
    public KyokoCtrl kyokoCtrl;
    public HomuraCtrl homuraCtrl;
    public MamiCtrl mamiCtrl;
    public MadokaCtrl madokaCtrl;
    // public Homura_mCtrl homura_mCtrl;
    /// <summary>
    /// 对于玩家根对象的管理（对于animation那个东西，用上面的这些类
    /// </summary>
    public Transform[] PlayerRoots;

    /// <summary>
    /// 可用玩家数（含QB） 
    /// </summary>
    int PlayerNumber = 0;

    /// <summary>
    /// 选择的角色的玩家ID  player 1 2 3 
    /// </summary>
    [HideInInspector] Variable.PlayerFaceType[] PlayersId = { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null };

    private void Awake()
    {
        playerRootCtrl = this;//そう、私です
    }

    // Start is called before the first frame update
    void Start()
    {
        //获取选中的玩家的脚本
        for (int i = 0; i < 3; i++)
        {
            if (MountGSS.gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.Null)
            {
                //确定玩家123选择的是啥角色
                PlayersId[i] = MountGSS.gameScoreSettings.SelectedGirlInGame[i];
                PlayerNumber++;
            }
        }

    }


/// <summary>
/// 将全部玩家传送到哪个点（世界坐标）
/// </summary>
/// <param name="vector2"></param>
public void JumpToPoint(Vector2 vector2)
    {
        for (int i = 0; i < PlayerNumber; i++)
        {
            ///不挤在一起
            PlayerRoots[(int)PlayersId[i]].position = new Vector2(vector2.x + 1.7F * i, vector2.y);
        }
    }

    /// <summary>
    /// 将某个玩家传送到哪个点（世界坐标）
    /// </summary>
    /// <param name="PlayerId"></param>
    /// <param name="vector2"></param>
    public void JumpToPoint(int PlayerId,Vector2 vector2)
    {
            ///不挤在一起
            PlayerRoots[(int)PlayersId[PlayerId]].position = vector2;
    }


    public void HurtPlayer(int Damage,int PlayerId)
    {
        //确定玩家 1\2\3 是哪些角色
        switch (PlayersId[PlayerId])
        {
            case Variable.PlayerFaceType.Sayaka:
                if (sayakaCtrl != null)
                {
                    sayakaCtrl.GetHurt(Damage);
                }
                break;

            case Variable.PlayerFaceType.Kyoko:
                if (kyokoCtrl != null)
                {
                    kyokoCtrl.GetHurt(Damage);
                }
                break;

            case Variable.PlayerFaceType.Madoka:
                break;

            case Variable.PlayerFaceType.Homura:
                break;

            case Variable.PlayerFaceType.Homura_m:
                break;

            case Variable.PlayerFaceType.Mami:
                break;

            case Variable.PlayerFaceType.QB:
                break;

        }
       
    }

    /// <summary>
    /// 对全体玩家造成伤害
    /// </summary>
    /// <param name="Damage"></param>
    public void HurtAllPlayers(int Damage)
    {
        for (int i = 0; i < 3; i++)
        {
            HurtPlayer(i, Damage);
        }
    
    }

    /// <summary>
    /// 将玩家变成光球
    /// </summary>
    public void PlayerBecomeBall(int PlayerId)
    {
        //确定玩家 1\2\3 是哪些角色
        switch (PlayersId[PlayerId])
        {
            case Variable.PlayerFaceType.Sayaka:
                if (sayakaCtrl != null)
                {
                    sayakaCtrl.RebirthOrGemBroken();
                }
                break;

            case Variable.PlayerFaceType.Kyoko:
                if (kyokoCtrl != null)
                {
                    kyokoCtrl.RebirthOrGemBroken();
                }
                break;

            case Variable.PlayerFaceType.Madoka:
                break;

            case Variable.PlayerFaceType.Homura:
                break;

            case Variable.PlayerFaceType.Homura_m:
                break;

            case Variable.PlayerFaceType.Mami:
                break;

            case Variable.PlayerFaceType.QB:
                break;
        }

    }

    /// <summary>
    /// 见面一滴血（将玩家设置为1滴血）
    /// </summary>
    /// <param name="PlayerId"></param>
    public void OneBloodNiiiiiiice(int PlayerId)
    {
        //确定玩家 1\2\3 是哪些角色
        switch (PlayersId[PlayerId])
        {
            case Variable.PlayerFaceType.Sayaka:
                if (sayakaCtrl != null)
                {
                    sayakaCtrl.OneBlood();
                }
                break;

            case Variable.PlayerFaceType.Kyoko:
                if (kyokoCtrl != null)
                {
                    kyokoCtrl.OneBlood();
                }
                break;

            case Variable.PlayerFaceType.Madoka:
                break;

            case Variable.PlayerFaceType.Homura:
                break;

            case Variable.PlayerFaceType.Homura_m:
                break;

            case Variable.PlayerFaceType.Mami:
                break;

            case Variable.PlayerFaceType.QB:
                break;
        }

    }
    /// <summary>
    /// 清除某一玩家的血量
    /// </summary>
    public void CleanPlayerVit(int PlayerId)
    {
        //确定玩家 1\2\3 是哪些角色
        switch (PlayersId[PlayerId])
        {
            case Variable.PlayerFaceType.Sayaka:
                if (sayakaCtrl != null)
                {
                    sayakaCtrl.CleanVit();
                }
                break;

            case Variable.PlayerFaceType.Kyoko:
                if (kyokoCtrl != null)
                {
                    kyokoCtrl.CleanVit();
                }
                break;

            case Variable.PlayerFaceType.Madoka:
                break;

            case Variable.PlayerFaceType.Homura:
                break;

            case Variable.PlayerFaceType.Homura_m:
                break;

            case Variable.PlayerFaceType.Mami:
                break;

            case Variable.PlayerFaceType.QB:
                break;

        }
    }

    /// <summary>
    /// 清除所有玩家的血量
    /// </summary>
    public void CleanAllPlayersVit()
    {
        for (int i = 0; i < 3; i++)
        {
            CleanPlayerVit(i);
        }
    }

    /// <summary>
    /// 清除所有玩家的灵魂值
    /// </summary>
    public void CleanAllPlayersSoul()
    {
        if (sayakaCtrl != null)
        {
            sayakaCtrl.CleanSoul();
        }
    }

    /// <summary>
    /// 场上所有人升级
    /// </summary>
    /// <param name="level">升几级</param>
    public void AllPlayersLevelUp()
    {
        if (sayakaCtrl != null)
        {
            sayakaCtrl.LevelUp();
        }
    }
}
