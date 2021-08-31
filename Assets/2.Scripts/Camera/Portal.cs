using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;
using UnityEngine.Events;

/// <summary>
/// �������õĴ����ţ�����ģʽδ���䣩
/// </summary>
public class Portal : MonoBehaviour
{
    Transform tr;

    public GameObject[] NeedToDestroy;
    public GameObject[] NeedToEnable;

    /// <summary>
    /// �Ƿ�Ҫ��һ��Z�����ܽ�ȥ
    /// </summary>
    public bool NeedPressW = false;
    /// <summary>
    /// ����ʹ��Y����Ϊ�ж�����
    /// </summary>
    public bool AllowY = false;

    /// <summary>
    /// ��������͵�Լ�����ĸ���
    /// </summary>
    public int CameraPointInRestraint = -1;
    /// <summary>
    /// ����Ҵ��͵��ĸ���
    /// </summary>
    public Vector2 PlayerTo = Vector2.zero;

    public UnityEvent OnEnable;

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
        if (NeedPressW)
        {
            if (!MountGSS.gameScoreSettings.Up)
            {
                return;
            }
        }

        if (AllowY)
        {
            //��Y������ ��������� û��������
            if (tr != null && MountGSS.gameScoreSettings.PlayersPosition[0].y < tr.position.y  && !MountGSS.gameScoreSettings.IsBodyDieInGame[0] && !MountGSS.gameScoreSettings.IsSoulBallInGame[0])
            {
                tr = null;
                StartCoroutine(TP());
            }
        }

        //�����㹻�� ��������� û��������
        if ( tr != null && Mathf.Abs(MountGSS.gameScoreSettings.PlayersPosition[0].x - tr.position.x) <= 1f && !MountGSS.gameScoreSettings.IsBodyDieInGame[0] && !MountGSS.gameScoreSettings.IsSoulBallInGame[0])
                {
            tr = null;
            StartCoroutine(TP());
            OnEnable.Invoke();
                }
    }


    private IEnumerator TP()
    {
        //��ֹ�������
        MountGSS.gameScoreSettings.BanInput = true;
        //�������
        yield return StartCoroutine(UICtrl.uiCtrl.NextFragmentFadeOut());
        //˲��
       CameraCtrl.cameraCtrl.cameraRestraints[(int)MountGSS.gameScoreSettings.BattlingMajo].JumpToPoint(CameraPointInRestraint);
        //���˲��֮��������ܲ��ƶ�������
        CameraCtrl.cameraCtrl.RecoverMoving();
        //�������˲��
        PlayerRootCtrl.playerRootCtrl.JumpToPoint(PlayerTo);
        //������Ҫ���������
        for (int i = 0; i < NeedToEnable.Length; i++)
        {
            if (NeedToEnable[i] != null) { NeedToEnable[i].SetActive(true); }
        }

        //����
        yield return StartCoroutine(UICtrl.uiCtrl.NextFragmentFadeIn());

        MountGSS.gameScoreSettings.BanInput = false;

        //ɾ����Ҫɾ��������
        for (int i = 0; i < NeedToDestroy.Length; i++)
        {
            if (NeedToDestroy[i] != null) { Destroy(NeedToDestroy[i]); }
        }
        Destroy(this.gameObject);
    }

}
