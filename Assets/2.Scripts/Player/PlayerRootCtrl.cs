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
    /// ������Ҹ�����Ĺ�������animation�Ǹ����������������Щ��
    /// </summary>
    public Transform[] PlayerRoots;

    /// <summary>
    /// �������������QB�� 
    /// </summary>
    int PlayerNumber = 0;

    /// <summary>
    /// ѡ��Ľ�ɫ�����ID  player 1 2 3 
    /// </summary>
    [HideInInspector] Variable.PlayerFaceType[] PlayersId = { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null };

    private void Awake()
    {
        playerRootCtrl = this;//������˽�Ǥ�
    }

    // Start is called before the first frame update
    void Start()
    {
        //��ȡѡ�е���ҵĽű�
        for (int i = 0; i < 3; i++)
        {
            if (MountGSS.gameScoreSettings.SelectedGirlInGame[i] != Variable.PlayerFaceType.Null)
            {
                //ȷ�����123ѡ�����ɶ��ɫ
                PlayersId[i] = MountGSS.gameScoreSettings.SelectedGirlInGame[i];
                PlayerNumber++;
            }
        }

    }


/// <summary>
/// ��ȫ����Ҵ��͵��ĸ��㣨�������꣩
/// </summary>
/// <param name="vector2"></param>
public void JumpToPoint(Vector2 vector2)
    {
        for (int i = 0; i < PlayerNumber; i++)
        {
            ///������һ��
            PlayerRoots[(int)PlayersId[i]].position = new Vector2(vector2.x + 1.7F * i, vector2.y);
        }
    }

    /// <summary>
    /// ��ĳ����Ҵ��͵��ĸ��㣨�������꣩
    /// </summary>
    /// <param name="PlayerId"></param>
    /// <param name="vector2"></param>
    public void JumpToPoint(int PlayerId,Vector2 vector2)
    {
            ///������һ��
            PlayerRoots[(int)PlayersId[PlayerId]].position = vector2;
    }


    public void HurtPlayer(int Damage,int PlayerId)
    {
        //ȷ����� 1\2\3 ����Щ��ɫ
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
    /// ��ȫ���������˺�
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
    /// ����ұ�ɹ���
    /// </summary>
    public void PlayerBecomeBall(int PlayerId)
    {
        //ȷ����� 1\2\3 ����Щ��ɫ
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
    /// ����һ��Ѫ�����������Ϊ1��Ѫ��
    /// </summary>
    /// <param name="PlayerId"></param>
    public void OneBloodNiiiiiiice(int PlayerId)
    {
        //ȷ����� 1\2\3 ����Щ��ɫ
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
    /// ���ĳһ��ҵ�Ѫ��
    /// </summary>
    public void CleanPlayerVit(int PlayerId)
    {
        //ȷ����� 1\2\3 ����Щ��ɫ
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
    /// ���������ҵ�Ѫ��
    /// </summary>
    public void CleanAllPlayersVit()
    {
        for (int i = 0; i < 3; i++)
        {
            CleanPlayerVit(i);
        }
    }

    /// <summary>
    /// ���������ҵ����ֵ
    /// </summary>
    public void CleanAllPlayersSoul()
    {
        if (sayakaCtrl != null)
        {
            sayakaCtrl.CleanSoul();
        }
    }

    /// <summary>
    /// ��������������
    /// </summary>
    /// <param name="level">������</param>
    public void AllPlayersLevelUp()
    {
        if (sayakaCtrl != null)
        {
            sayakaCtrl.LevelUp();
        }
    }
}
