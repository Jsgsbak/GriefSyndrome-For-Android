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
    /// <summary>
    /// 普通X攻击计时器，用于记录普通X准备用时与设定普通X攻击冲刺速度还有普通X冲刺完之后间隔0.3s才能再充一次
    /// </summary>
    float OrdinaryXTimer = 0f;

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
        if(StageCtrl.gameScoreSettings.Xattack && !animator.GetBool("OrdinaryXattack") && !animator.GetBool("OrdinaryXattackPrepare") &&!BanWalk && !XordinaryDash && Time.timeSinceLevelLoad -OrdinaryXTimer >= 0.3F)
        {
            animator.SetBool("OrdinaryXattackPrepare", true);
            XattackAnimationEvent("OrdinaryPrepare");
            BanWalk = true;
        }
        //松开X键，但仍然处于X攻击状态，所以能往前冲
        else if (!StageCtrl.gameScoreSettings.Xattack && animator.GetBool("OrdinaryXattackPrepare") && !XordinaryDash)
        {
            animator.SetBool("OrdinaryXattackPrepare",false);
            XattackAnimationEvent("OrdinaryDash");
            
        }

        //冲刺移动（放在这里是为了移动流畅）
        if (XordinaryDash)
        {
            //使用正负号的不同来防止多次计算
            if (OrdinaryXTimer >= 0F)
            {
                OrdinaryXTimer = -Mathf.Clamp((Time.timeSinceLevelLoad - OrdinaryXTimer) / 1.5F, 0F, 1F);
                Debug.Log(8F - OrdinaryXTimer);

            }
            tr.Translate(Vector3.right *(8F  -OrdinaryXTimer) *Time.deltaTime, Space.Self);
        }
    }

    public override void OrdinaryZ()
    {
        if (StageCtrl.gameScoreSettings.Zattack && !animator.GetBool("OrdinaryXattack") && !animator.GetBool("OrdinaryXattackPrepare") /*|| Time.timeSinceLevelLoad -  AttackTimer[0] <= PressAttackInteral && AttackTimer[0] != 0*/)
        {
            // StopAttacking = false; 先这样吧，无力了
            GravityRatio = 0.8f;
            animator.SetBool("Zattack", true);
            animator.SetBool("Fall", false);
            CancelJump();//直接中断跳跃并且不恢复
        }
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
            IsAttack[0] = true;
            StopAttacking = false;
            BanWalk = true;
            BanTurnAround = true;//攻击状态不能转身
            animator.SetBool("Zattack", true);//不能中断动画

        }
        //Z攻击的动画处于两端攻击的连接处，可以中断，中断处允许切换到其他动画和状态
        else if (AnimationName.Equals("ZattackCouldStop"))
        {
            //如果还在攻击那就不能解除移动禁止
            BanWalk = StageCtrl.gameScoreSettings.Zattack;
            StopAttacking = true;//可以中断攻击
            BanTurnAround = false;//连接处可以转身
            IsAttack[0] = false;//连接处不属于攻击阶段，可以切换到其他动画和状态
            animator.SetBool("Zattack", StageCtrl.gameScoreSettings.Zattack);//现在可以中断动画
            animator.SetBool("Fall", !IsGround && !StageCtrl.gameScoreSettings.Zattack);

            //攻击连接处可以移动
            if (StageCtrl.gameScoreSettings.Horizontal == 1 && DoLookRight)
            {
                tr.Translate(Vector2.right * 0.02f);
            }
            else if (StageCtrl.gameScoreSettings.Horizontal == -1 && !DoLookRight)
            {
                tr.Translate(Vector2.right * 0.02f);
            }



        }
        //Z攻击打完，并且按着Z，满足条件后进入Z攻击最后阶段
        else if (AnimationName.Equals("ZattackDone") && StageCtrl.gameScoreSettings.Zattack && !animator.GetBool("ZattackFin"))
        {
            StopAttacking = true;//可以中断攻击
            IsAttack[0] = false;//连接处不属于攻击阶段，可以切换到其他动画和状态

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
            IsAttack[0] = false;//连接处不属于攻击阶段，可以切换到其他动画和状态


            //因为这里不会产生动画未结束松开Z导致动画结束的情况，所以不修改IsZattacking
        }
    }

    public override void XattackAnimationEvent(string AnimationName)
    {
        #region 通常攻击段
        //攻击准备阶段
        if (AnimationName.Equals("OrdinaryPrepare"))
        {
            IsAttack[1] = true;
            BanTurnAround = true;
            BanWalk = true;
            GravityRatio = 0.4F;
            BanJump = true;
            animator.SetBool("OrdinaryXattackPrepare", true);

            //保存一下时间，用于得到蓄力的效果
            OrdinaryXTimer = Time.timeSinceLevelLoad;
        }
        //冲刺阶段
        else if (AnimationName.Equals("OrdinaryDash"))
        {
            BanWalk = true;
            animator.SetBool("OrdinaryXattackPrepare", false);
            animator.SetBool("OrdinaryXattack", true); 
            XordinaryDash = true;

        }
        //冲刺阶段结束
        else if (AnimationName.Equals("OrdinaryDashDone"))
        {
            IsAttack[1] = false;
            BanWalk = false;
            GravityRatio = 1F;
            BanJump = false;
            animator.SetBool("OrdinaryXattack", false);
            BanTurnAround = false;
            XordinaryDash = false;
            //普通X冲刺完之后间隔0.3s才能再充一次，先保存一下时间
            OrdinaryXTimer = Time.timeSinceLevelLoad;
        }
        #endregion
    }
}




