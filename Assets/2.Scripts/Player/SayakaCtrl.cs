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
    bool XordinaryDash = false;

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
        //从通常状态进入到X攻击准备状态
        if(StageCtrl.gameScoreSettings.Xattack && !animator.GetBool("OrdinaryXattack") )
        {
            animator.SetBool("OrdinaryXattackPrepare", true);
            XattackAnimationEvent("OrdinaryPrepare");
        }
        //松开X键，但仍然处于X攻击状态，所以能往前冲
        else if (!StageCtrl.gameScoreSettings.Xattack && animator.GetBool("OrdinaryXattackPrepare"))
        {
            animator.SetBool("OrdinaryXattackPrepare",false);
            XattackAnimationEvent("OrdinaryDash");
            
        }

        //冲刺移动
        if (XordinaryDash)
        {
            tr.Translate(Vector3.right * 10f * Time.deltaTime, Space.Self);

        }
    }

    public override void OrdinaryZ()
    {
        if (StageCtrl.gameScoreSettings.Zattack /*|| Time.timeSinceLevelLoad -  AttackTimer[0] <= PressAttackInteral && AttackTimer[0] != 0*/)
        {
            // StopAttacking = false; 先这样吧，无力了
            GravityRatio = 0.8f;
            animator.SetBool("Zattack", true);
            animator.SetBool("Fall", false);
            CancelJump();//直接中断跳跃并且不恢复
        }
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
            animator.SetBool("Zattack", StageCtrl.gameScoreSettings.Zattack);

        }
        //Z攻击的动画处于两端攻击的连接处，可以中断，中断处允许切换到其他动画和状态
        else if (AnimationName.Equals("ZattackCouldStop"))
        {
            StopAttacking = true;
            BanTurnAround = false;//连接处可以转身
             animator.SetBool("Zattack", StageCtrl.gameScoreSettings.Zattack);
            animator.SetBool("Fall", !IsGround && !StageCtrl.gameScoreSettings.Zattack);

        }
        //Z攻击打完，并且按着Z，满足条件后进入Z攻击最后阶段
        else if (AnimationName.Equals("ZattackDone") && StageCtrl.gameScoreSettings.Zattack && !animator.GetBool("ZattackFin"))
        {
            //攻击完了恢复移动速度与重力
            GravityRatio = 1F;
            //取消Z攻击状态，方便转换到idle或者ZattackFin
            animator.SetBool("Zattack", StageCtrl.gameScoreSettings.Zattack);
            //允许下落状态
             animator.SetBool("Fall", !IsGround && !StageCtrl.gameScoreSettings.Zattack);

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
           
           tr.Translate(Vector3.right * 0.4f, Space.Self);
           

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
        #region 通常攻击段
        //攻击准备阶段
        if (AnimationName.Equals("OrdinaryPrepare"))
        {
            BanTurnAround = true;
            BanWalk = true;
            GravityRatio = 0.4F;
            BanJump = true;
            animator.SetBool("OrdinaryXattackPrepare", true);
        }
        //冲刺阶段
        else if (AnimationName.Equals("OrdinaryDash"))
        {
            animator.SetBool("OrdinaryXattackPrepare", false);
            animator.SetBool("OrdinaryXattack", true); 
            XordinaryDash = true;
        }
        //冲刺阶段结束
        else if (AnimationName.Equals("OrdinaryDashDone"))
        {
            BanWalk = false;
            GravityRatio = 1F;
            BanJump = false;
            animator.SetBool("OrdinaryXattack", false);
            BanTurnAround = false;
            XordinaryDash = false;
        }
        #endregion
    }
}




