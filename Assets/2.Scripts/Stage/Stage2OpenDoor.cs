using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2OpenDoor : MonoBehaviour
{
    /// <summary>
    /// �����ĸ����Ե�
    /// </summary>
    public int BelongToWhichStopPoint = -1;

    public Animator animator;
    public GameObject Portal;

    //����ӵл�֮ǰ�������������
    private void OnBecameVisible()
    {
        animator.enabled = true;
        Portal.SetActive(true);
    }

   
}
