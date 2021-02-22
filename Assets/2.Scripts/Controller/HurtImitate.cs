using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 为了那三个按钮特意准备的脚本 受击模拟
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
