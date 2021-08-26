using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2OpenDoor : MonoBehaviour
{
    /// <summary>
    /// 属于哪个测试点
    /// </summary>
    public int BelongToWhichStopPoint = -1;

    public Animator animator;
    public GameObject Portal;

    //在添加敌机之前，用这个来开门
    private void OnBecameVisible()
    {
        animator.enabled = true;
        Portal.SetActive(true);
    }

   
}
