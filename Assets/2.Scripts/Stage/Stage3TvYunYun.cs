using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3TvYunYun : MonoBehaviour
{
    //吾乃“电”视，作为道中最强场景，操纵着劝退能力，是背负苍蓝雷电之人

    [Header("动画偏移")]
    public float OffsetCycle = 0f;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
            animator.SetFloat("OffsetCycle", OffsetCycle);
    }

   
}
