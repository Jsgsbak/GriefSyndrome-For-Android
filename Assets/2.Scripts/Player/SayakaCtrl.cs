using MEC;
using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

//先暂时不继承
public class SayakaCtrl : APlayerCtrl
{
    public override void HorizontalX()
    {

    }

    public override void HorizontalZ()
    {

    }

    public override void Magia()
    {

    }

    public override void OrdinaryX()
    {

    }

    public override void OrdinaryZ()
    {
            animator.SetBool("Zattack", StageCtrl.gameScoreSettings.Zattack);
        BanWalk = StageCtrl.gameScoreSettings.Zattack;
    }

    public override void VerticalX()
    {

    }

    public override void VerticalZ()
    {

    }

    public override void  AttackAnimationEvent(string AnimationName)
    {
        //Z攻击打完，并且按着Z，进入Z攻击最后阶段
        if(AnimationName.Equals("ZattackDone") && StageCtrl.gameScoreSettings.Zattack && !animator.GetBool("ZattackFin"))
        {
            animator.SetBool("ZattackFin",true);
        }
        //Z攻击最后阶段结束
        else if (AnimationName.Equals("ZattackFinDone"))
        {
            animator.SetBool("ZattackFin", false);
            animator.SetBool("Zattack", false);
        }
        //已经满足执行idle动画的条件了，初始化状态机和动画参数减少Bug
        else if(AnimationName.Equals("Idle"))
        {
            animator.SetBool("ZattackFin", false);
            animator.SetBool("Walk", false);
            animator.SetBool("Zattack", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Fall", false);

            IsMoving = false;
            BanGravity = true;
            IsGround = true;
            


        }
    }
}




