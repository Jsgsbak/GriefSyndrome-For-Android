using UnityEngine;

/// <summary>
/// Ϊ����������ť����׼���Ľű� �ܻ�ģ��
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
