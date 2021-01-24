﻿using MEC;
using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

//先暂时不继承
public class SayakaCtrl : APlayerCtrl
{
    [Space]
    int ZattackCount = 0;
   public  bool XordinaryDash = false;
    /// <summary>
    /// 普通X攻击计时器，用于记录普通X准备用时与设定普通X攻击冲刺速度还有普通X冲刺完之后间隔0.3s才能再充一次
    /// </summary>
    float OrdinaryXTimer = 0f;

    /// <summary>
    /// UP X攻击正在往上走
    /// </summary>
    bool UpAttackMovingUpward = false;

    public override void HorizontalX()
    {
        //特意为这个攻击方法重新写一下输入情况emmm
        StageCtrl.gameScoreSettings.Xattack = RebindableInput.GetKeyDown("Xattack") && !BanInput;

        if (StageCtrl.gameScoreSettings.Horizontal != 0 && StageCtrl.gameScoreSettings.Xattack && !animator.GetBool("HorizontalXattack") && !BanWalk)
        {

            CancelJump();//直接中断跳跃并且不恢复
            BanGravity = true;
            BanInput = true;
            BanTurnAround = true;
            StopAttacking = false;
            animator.SetBool("HorizontalXattack", true);
        }

        if (animator.GetBool("HorizontalXattack"))
        {
            tr.Translate(Vector3.right * 8f * Time.deltaTime, Space.Self);
        }
    }

    public override void HorizontalZ()
    {

    }

    public override void Magia()
    {

    }

    public override void OrdinaryX()
    {
        //修复X通常攻击完成后不受控制移动的bug
        XordinaryDash = animator.GetBool("OrdinaryXattack");

        //从通常状态进入到X攻击准备状态
        if (StageCtrl.gameScoreSettings.Horizontal == 0 && StageCtrl.gameScoreSettings.Xattack && !animator.GetBool("OrdinaryXattack") && !animator.GetBool("OrdinaryXattackPrepare") &&!BanWalk && !XordinaryDash && Time.timeSinceLevelLoad -OrdinaryXTimer >= 0.3F)
        {
            //反正这个只执行一次

            animator.SetBool("OrdinaryXattackPrepare", true);
            CancelJump();//直接中断跳跃并且不恢复
            StopAttacking = false;
            GravityRatio = 0.4f;
            IsAttack[1] = true;
            BanWalk = true;
            BanTurnAround = true;

            //保存一下时间，用于得到蓄力的效果
            OrdinaryXTimer = Time.timeSinceLevelLoad;
          

        }
        //松开X键，但仍然处于X攻击状态，所以能往前冲
        else if (!StageCtrl.gameScoreSettings.Xattack && IsAttack[1] &&animator.GetBool("OrdinaryXattackPrepare") && !XordinaryDash)
        {
            animator.SetBool("OrdinaryXattackPrepare",false);
            animator.SetBool("OrdinaryXattack", true);
            XordinaryDash = true;
            GravityRatio = 0.4f;//修复bug


        }

        //冲刺移动（放在这里是为了移动流畅）
        if (XordinaryDash)
        {
            //使用正负号的不同来防止多次计算
            if (OrdinaryXTimer >= 0F)
            {
                OrdinaryXTimer = -Mathf.Clamp((Time.timeSinceLevelLoad - OrdinaryXTimer) / 1.5F, 0F, 1F);

            }
            tr.Translate(Vector3.right *(8F  -OrdinaryXTimer) *Time.deltaTime, Space.Self);
        }
    }
    public override void UpX()
    {
        //特意为这个攻击方法重新写一下输入情况emmm
        StageCtrl.gameScoreSettings.Xattack = RebindableInput.GetKeyDown("Xattack") && !BanInput;

        if (StageCtrl.gameScoreSettings.Xattack && StageCtrl.gameScoreSettings.Up)
        {

        }
    }

    public override void OrdinaryZ()
    {
        if (StageCtrl.gameScoreSettings.Zattack && !animator.GetBool("OrdinaryXattack") && !animator.GetBool("OrdinaryXattackPrepare") /*|| Time.timeSinceLevelLoad -  AttackTimer[0] <= PressAttackInteral && AttackTimer[0] != 0*/)
        {
            if (!animator.GetBool("Zattack") && !animator.GetBool("ZattackFin")) CancelJump();//直接中断跳跃并且不恢复
            GravityRatio = 0.8f;
            animator.SetBool("Zattack", true);
            animator.SetBool("Fall", false);
             BanGravity = IsGround;//修复奇怪的bug
        }
    
        //
    }


    /// <summary>
    /// 攻击用动画逻辑
    /// </summary>
    /// <param name="AnimationName"></param>
    public override void  ZattackAnimationEvent(string AnimationName)
    {
        //虽然跳跃的那个也会用一次这个方法，但是没太大影响
        ZattackMove();

        switch (AnimationName)
        {
            //Z攻击的动画正处于攻击状态，不能中断
            case "ZattackDoing":
                IsAttack[0] = true;
                StopAttacking = false;
                BanTurnAround = true;//攻击状态不能转身
                animator.SetBool("Zattack", true);//不能中断动画


                break;

            //Z攻击的动画处于两端攻击的连接处，可以中断，中断处允许切换到其他动画和状态
            case "ZattackCouldStop":
                //如果还在攻击那就不能解除移动和跳跃禁止
                StopAttacking = true;//可以中断攻击
                BanTurnAround = false;//连接处可以转身
                IsAttack[0] = false;//连接处不属于攻击阶段，可以切换到其他动画和状态
                animator.SetBool("Zattack", StageCtrl.gameScoreSettings.Zattack);//现在可以中断动画
                animator.SetBool("Fall", !IsGround && !StageCtrl.gameScoreSettings.Zattack);

                break;

            //Z攻击打完，
            case "ZattackDone":
                    StopAttacking = true;//可以中断攻击
                    IsAttack[0] = false;//连接处不属于攻击阶段，可以切换到其他动画和状态
                BanTurnAround = false;//可以转身

                //攻击完了恢复移动速度与重力
                GravityRatio = 1F;
                    //取消Z攻击状态，方便转换到idle或者ZattackFin
                    animator.SetBool("Zattack", StageCtrl.gameScoreSettings.Zattack);
                    //允许下落状态
                    animator.SetBool("Fall", !IsGround && !StageCtrl.gameScoreSettings.Zattack);
               
                if (StageCtrl.gameScoreSettings.Zattack)
                {
                    //并且按着Z，满足条件后进入Z攻击最后阶段
                    //仅在地面上能发动最后一击
                    if (IsGround) ZattackCount++;

                    if (ZattackCount == 2 && IsGround)//仅在地面上并且达到要求了才能发动
                    {
                        animator.SetBool("ZattackFin", true);
                    }
                    StopAttacking = true;
                }
                break;

            //Z攻击最后一阶段向前跳
            case "ZattackFinJump":
                BanTurnAround = true;//向前跳的时候不能转身
                StopAttacking = false;//不可以中断攻击

                //向前移动
                tr.Translate(Vector3.right * 0.4f, Space.Self);
                break;

            //Z攻击最后阶段结束
            case "ZattackFinDone":
                animator.SetBool("ZattackFin", false);
                animator.SetBool("Zattack", false);
                StopAttacking = true;
                //修改计数器重新循环动画
                ZattackCount = 0;
                BanTurnAround = false;//打完了可以转身
                IsAttack[0] = false;//连接处不属于攻击阶段，可以切换到其他动画和状态

                //僵直
                Stiff(0.1f);

                //因为这里不会产生动画未结束松开Z导致动画结束的情况，所以不修改IsZattacking
                break;
        }
    }

    public override void XattackAnimationEvent(string AnimationName)
    {
        //结束

            GravityRatio = 1F;
            animator.SetBool("OrdinaryXattack", false);
            BanTurnAround = false;
            StopAttacking = true;
            IsAttack[1] = false;
            //普通X冲刺完之后间隔0.3s才能再充一次，先保存一下时间
            OrdinaryXTimer = Time.timeSinceLevelLoad;
            //僵直
            Stiff(0.1f);
    }

    public override void HorizontalXattackAnimationEvent(string AnimationName)
    {
        //结束
            BanGravity = !true;
            BanInput = !true;
            BanTurnAround = !true;
        StopAttacking = true;
        IsAttack[1] = false;
        animator.SetBool("HorizontalXattack", !true);

            Stiff(0.2f);

    }

    public override void UpXattackAnimationEvent(string AnimationName)
    {
        switch (AnimationName)
        {
            case "Start":
                CancelJump();
                BanGravity = true;
                BanInput = true;
                BanTurnAround = true;
                break;

            case "Doing-Up":
                break;

            case "Doing-Down":
                break;

            case "Done":

                break;
        }
    }


    /// <summary>
    /// 这个用于所有Z动画之中（仅限沙耶加），为了那种一个动画前进一次的效果
    /// </summary>
    void ZattackMove()
    {
        if (StageCtrl.gameScoreSettings.Horizontal != 0 )
        {
            tr.Translate(Vector2.right * 0.02f);
        }

    }







    public override void VerticalZ()
    {

    }

}




