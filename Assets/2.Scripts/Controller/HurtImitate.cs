using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ϊ����������ť����׼���Ľű� �ܻ�ģ��
/// </summary>
public class HurtImitate : MonoBehaviour
{
    public void HurtAllPlayer(int damage)
    {
        StageCtrl.stageCtrl.Player1Hurt.Invoke(damage);
        StageCtrl.stageCtrl.Player2Hurt.Invoke(damage);
        StageCtrl.stageCtrl.Player2Hurt.Invoke(damage);

    }
}
