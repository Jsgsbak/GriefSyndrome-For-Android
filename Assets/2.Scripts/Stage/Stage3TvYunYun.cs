using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3TvYunYun : MonoBehaviour
{
    //���ˡ��硱�ӣ���Ϊ������ǿ������������Ȱ���������Ǳ��������׵�֮��

    [Header("����ƫ��")]
    public float OffsetCycle = 0f;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
            animator.SetFloat("OffsetCycle", OffsetCycle);
    }

   
}
