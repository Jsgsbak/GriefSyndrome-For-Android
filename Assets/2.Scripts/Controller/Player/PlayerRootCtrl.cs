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
    /// ѡ��Ľ�ɫ�����ID
    /// </summary>
    [HideInInspector] Variable.PlayerFaceType[] PlayersId = { Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null, Variable.PlayerFaceType.Null };

    private void Awake()
    {
        playerRootCtrl = this;//������˽�Ǥ�
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(PlayersId.Length);
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
/// ��������Ҵ��͵��ĸ��㣨�������꣩
/// </summary>
/// <param name="vector2"></param>
public void JumpToPoint(Vector2 vector2)
    {
        for (int i = 0; i < PlayerNumber; i++)
        {
            PlayerRoots[(int)PlayersId[i]].position = new Vector3(vector2.x + 1.7F * i, vector2.y, 2F);
        }
    }
}
