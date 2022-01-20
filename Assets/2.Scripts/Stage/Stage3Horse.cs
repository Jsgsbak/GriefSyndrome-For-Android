using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3Horse : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //动画周期偏移，让🐎进行不同步的动画播放
        animator.SetFloat("CycleOffset", Random.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
