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
            animator.SetBool("OrdinaryXattackPrepare", StageCtrl.gameScoreSettings.Xattack && animator.GetBool("OrdinaryXattack"));
    }

    public override void OrdinaryZ()
    {
        if (StageCtrl.gameScoreSettings.Zattack)
        {
            // StopAttacking = false; 先这样吧，无力了
            GravityRatio = 0.8f;
        }

        animator.SetBool("Zattack", StageCtrl.gameScoreSettings.Zattack && !StageCtrl.gameScoreSettings.Jump);
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
        //攻击状态下移动
        if(StageCtrl.gameScoreSettings.Horizontal == 1 && DoLookRight)
        {
            tr.Translate(Vector2.right * 0.07f);
        }
        else if (StageCtrl.gameScoreSettings.Horizontal == -1 && !DoLookRight)
        {
            tr.Translate(Vector2.right * 0.07f);
        }

        //Z攻击的动画正处于攻击状态，不能中断
        if (AnimationName.Equals("ZattackDoing"))
        {
            StopAttacking = false;
            BanTurnAround = true;//攻击状态不能转身
        }
        //Z攻击的动画处于两端攻击的连接处，可以中断
        else if (AnimationName.Equals("ZattackCouldStop"))
        {
            StopAttacking = true;
            BanTurnAround = false;//连接处可以转身


        }
        //Z攻击打完，并且按着Z，满足条件后进入Z攻击最后阶段
        else if (AnimationName.Equals("ZattackDone") && StageCtrl.gameScoreSettings.Zattack && !animator.GetBool("ZattackFin"))
        {
            //攻击完了恢复移动速度与重力
            GravityRatio = 1F;

            //仅在地面上能发动最后一击
            if (IsGround) ZattackCount++;
            BanTurnAround = false;//向前跳之前可以转身

            if (ZattackCount == 2)
            {
                animator.SetBool("ZattackFin", true);
            }
            StopAttacking = true;
        }
        //Z攻击最后一阶段向前跳
        else if (AnimationName.Equals("ZattackFinJump"))
        {
            BanTurnAround = true;//向前跳的时候不能转身
            BanWalk = true;
           
           tr.Translate(Vector3.right * 0.6f, Space.Self);
           

        }
        //Z攻击最后阶段结束
        else if (AnimationName.Equals("ZattackFinDone"))
        {
            animator.SetBool("ZattackFin", false);
            animator.SetBool("Zattack", false);
            StopAttacking = true;
            //修改计数器重新循环动画
            ZattackCount = 0;
            BanTurnAround = false;//打完了可以转身
            BanWalk = false;


            //因为这里不会产生动画未结束松开Z导致动画结束的情况，所以不修改IsZattacking
        }
    }

    public override void XattackAnimationEvent(string AnimationName)
    {

    }
}




