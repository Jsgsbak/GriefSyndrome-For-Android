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
    /// 选择的角色的玩家ID
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
/// 将所有玩家传送到哪个点（世界坐标）
/// </summary>
/// <param name="vector2"></param>
public void JumpToPoint(Vector2 vector2)
    {
        for (int i = 0; i < PlayerNumber; i++)
        {
            PlayerRoots[(int)PlayersId[i]].position = new Vector3(vector2.x + 1.7F * i, vector2.y, 2F);
        }
    }

    /// <summary>
    /// 对全体玩家造成伤害
    /// </summary>
    /// <param name="Damage"></param>
    public void HurtAllPlayers(int Damage)
    {
        if (sayakaCtrl != null) 
        {
            sayakaCtrl.GetHurt(Damage);
        }
        if(kyokoCtrl != null)
        {
            kyokoCtrl.GetHurt(Damage);
        }
        /*
        if (mamiCtrl != null)
        {
            mamiCtrl.GetHurt(Damage);
        }
        if (kyokoCtrl != null)
        {
            kyokoCtrl.GetHurt(Damage);
        }
        if (sayakaCtrl != null)
        {
            sayakaCtrl.GetHurt(Damage);
        }
        if (kyokoCtrl != null)
        {
            kyokoCtrl.GetHurt(Damage);
        }*/


    }

    /// <summary>
    /// 清除所有玩家的血量
    /// </summary>
    public void CleanAllPlayersVit()
    {
        if (sayakaCtrl != null)
        {
            sayakaCtrl.CleanVit();
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
