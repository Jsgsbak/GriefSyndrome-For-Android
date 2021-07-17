using UnityEngine;

/// <summary>
/// 为了那三个按钮特意准备的脚本 受击模拟
/// </summary>
public class HurtImitate : MonoBehaviour
{
    public void HurtAllPlayer(int damage)
    {
        MountGSS.gameScoreSettings.Player1Hurt.Invoke(damage);
        MountGSS.gameScoreSettings.Player2Hurt.Invoke(damage);
        MountGSS.gameScoreSettings.Player2Hurt.Invoke(damage);

    }
}
