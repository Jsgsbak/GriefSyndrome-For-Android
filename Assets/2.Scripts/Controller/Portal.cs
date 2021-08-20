using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

/// <summary>
/// �������õĴ����ţ�����ģʽδ���䣩
/// </summary>
public class Portal : MonoBehaviour
{
    Transform tr;

    /// <summary>
    /// ��������͵�Լ�����ĸ���
    /// </summary>
    public int CameraPointInRestraint = -1;
    /// <summary>
    /// ����Ҵ��͵��ĸ���
    /// </summary>
    public Vector2 PlayerTo = Vector2.zero;

    private void Awake()
    {
        tr = transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        UpdateManager.updateManager.SlowUpdate.AddListener(SlowUpdate);
    }

    // Update is called once per frame
    void SlowUpdate()
    {

        //�����㹻�� ��������� û��������
        if ( tr != null && Mathf.Abs(MountGSS.gameScoreSettings.PlayersPosition[0].x - tr.position.x) <= 1f && !MountGSS.gameScoreSettings.IsBodyDieInGame[0] && !MountGSS.gameScoreSettings.IsSoulBallInGame[0])
                {
            tr = null;
            StartCoroutine(TP());
                }
    }


    private IEnumerator TP()
    {
        //�������
        yield return StartCoroutine(UICtrl.uiCtrl.NextFragmentFadeOut());
        //˲��
       CameraCtrl.cameraCtrl.cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo].JumpToPoint(CameraPointInRestraint);
        //�������˲��
        PlayerRootCtrl.playerRootCtrl.JumpToPoint(PlayerTo);

        //����
        yield return StartCoroutine(UICtrl.uiCtrl.NextFragmentFadeIn());
        Destroy(this.gameObject);
    }

}
