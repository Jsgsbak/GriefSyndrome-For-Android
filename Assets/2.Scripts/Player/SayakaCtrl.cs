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
    int ZattackCount = 0;

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
        if (StageCtrl.gameScoreSettings.Zattack)
        {
            // StopAttacking = false; 先这样吧，无力了
            GravityRatio = 0.8f;
        }
        animator.SetBool("Zattack", StageCtrl.gameScoreSettings.Zattack);
        BanWalk = StageCtrl.gameScoreSettings.Zattack;
    }

    public override void VerticalX()
    {

    }

    public override void VerticalZ()
    {

    }


    /// <summary>
    /// 攻击用动画逻辑
    /// </summary>
    /// <param name="AnimationName"></param>
    public override void  ZattackAnimationEvent(string AnimationName)
    {
       //Z攻击的动画正处于攻击状态，不能中断
        if (AnimationName.Equals("ZattackDoing"))
        {
            StopAttacking = false;
        }
        //Z攻击的动画处于两端攻击的连接处，可以中断
        else if (AnimationName.Equals("ZattackCouldStop"))
        {
            StopAttacking = true;
        }
        //Z攻击打完，并且按着Z，满足条件后进入Z攻击最后阶段
        else if (AnimationName.Equals("ZattackDone") && StageCtrl.gameScoreSettings.Zattack && !animator.GetBool("ZattackFin"))
        {
            //仅在地面上能发动最后一击
            if(IsGround) ZattackCount++;

            if (ZattackCount == 2)
            {
                animator.SetBool("ZattackFin", true);
            }
            StopAttacking = true;
        }
        //Z攻击最后一阶段向前跳
        else if (AnimationName.Equals("ZattackFinJump"))
        {
            //动画中已经有时间的限制了
            if (spriteRenderer.flipX) tr.Translate(Vector3.right * 0.6f, Space.Self);
            else { tr.Translate(Vector3.right * 0.6f, Space.Self); }

        }
        //Z攻击最后阶段结束
        else if (AnimationName.Equals("ZattackFinDone"))
        {
            animator.SetBool("ZattackFin", false);
            animator.SetBool("Zattack", false);
            StopAttacking = true;
            //修改计数器重新循环动画
            ZattackCount = 0;

            //因为这里不会产生动画未结束松开Z导致动画结束的情况，所以不修改IsZattacking
        }
    }
}




