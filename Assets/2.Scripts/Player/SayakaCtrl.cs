﻿using System.Collections;
using UnityEngine;

//先暂时不继承
public class SayakaCtrl : APlayerCtrl
{

    #region 0.0.7版测试用按钮

    public void cleanSOul()
    {
        if (!StageCtrl.gameScoreSettings.DoesMajoOrShoujoDie)
        {
            GetHurt(56756756);
        }

    }

    public void cleanVit()
    {
        GetHurt(StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId]);

    }

    public void HurtMyself()
    {
        GetHurt(20);
    }

    public void Succeed()
    {
        StageCtrl.gameScoreSettings.Succeed = true;
    }

    #endregion
    /// <summary>
    /// 适用于X攻击蓄力的魔法阵
    /// </summary>
    [Space]
 //   public Animator MagicRing;
    int ZattackCount = 0;
   public  bool XordinaryDash = false;
    bool BanXattack = false;
    /// <summary>
    /// 普通X攻击计时器，用于记录普通X准备用时与设定普通X攻击冲刺速度还有普通X冲刺完之后间隔0.3s才能再充一次
    /// </summary>
    float OrdinaryXTimer = 0f;

    /// <summary>
    /// Down X攻击状态 -1向下 1向上 0没发动攻击 2反弹上升段
    /// </summary>
     int DownAttackMovingUpward = 0;
    /// <summary>
    /// Up X攻击移动
    /// </summary>
    bool UpAttackMove = false;
   public int UpAttackCount = 0;
    bool MagiaDash = false;
    /// <summary>
    /// 魔法按了X键
    /// </summary>
   public float MagiaDashSpeedRatio = 1f;

    public override void VariableInitialization()
    {
        base.VariableInitialization();
       
        //接着上面，自己的变量初始化
        ZattackCount = 0;
        XordinaryDash = false;
        OrdinaryXTimer = 0f;
        DownAttackMovingUpward = 0;
        BanXattack = false;
        UpAttackMove = false;
        UpAttackCount = 0;
         MagiaDash = false;
        MagiaDashSpeedRatio = 1f;
        //攻击动画/状态消除
        for (int i = 0; i < 3; i++)
        {
            //直接无脑遍历一遍好了emmmm，反正也不多
            if (IsAttack[i])
            {
                switch (i)
                {
                    case 0:
                        animator.SetBool("ZattackFin", false);
                        animator.SetBool("Zattack", false);
                        Debug.Log("消除Z攻击");
                        break;

                    case 1:
                        animator.SetBool("OrdinaryXattackPrepare", false);
                        animator.SetBool("OrdinaryXattack", false);
                        animator.SetBool("HorizontalXattack", false);
                        animator.SetBool("DownXattack-MovingUpward", false);
                        animator.SetBool("DownXattack-MovingDownward", false);
                        animator.SetBool("DownXattack-Done", false);
                        animator.SetBool("UpXattack", false);
                        Debug.Log("消除X攻击");
                        break;
                     case 2:
                        animator.SetBool("Magia", false);
                        Debug.Log("消除A攻击");
                        break;
                }
            }
            IsAttack[i] = false;
        }
    }

    public override void Magia()
    {     

        if (MagiaDashSpeedRatio == 1f && StageCtrl.gameScoreSettings.Xattack && IsAttack[2])
        {
            MagiaDashSpeedRatio = 1.5f;
            //重新播放
            animator.Play("MagiaWithAttack", 0, 0f);

        }


        if(!IsAttack[2] && StageCtrl.gameScoreSettings.Magia)
        {
            IsAttack[2] = true;
            CancelJump();
            BanWalk = true;
            BanTurnAround = true;
            BanGravity = true;
            BanGravityRay = true;

            animator.SetBool("Magia", true);

            StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] = StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] - StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].MaigaVit;

            //用于UI中HP血条信息更新
            StageCtrl.gameScoreSettings.MagiaKeyDown[PlayerId] = true;

        }

        //冲刺
        if (MagiaDash)
        {
            //掉落Bug修复
            BanGravity = true;

            if (DoLookRight)
            {
                Move(8f * MagiaDashSpeedRatio, true, PlayerSlope, Vector2.right);
            }
            else
            {
                Move(8f * MagiaDashSpeedRatio, true, PlayerSlope, Vector2.left);
            }
        }
    }

    public override void HorizontalX()
    {
        if (StageCtrl.gameScoreSettings.Horizontal != 0 && !IsAttack[1]  && StageCtrl.gameScoreSettings.Xattack && !animator.GetBool("HorizontalXattack") && !BanWalk)
        {

            CancelJump();//直接中断跳跃并且不恢复
            BanGravity = true;
            BanInput = true;
            IsAttack[1] = true;
            StopAttacking = false;
            animator.SetBool("HorizontalXattack", true);
        }

        if (animator.GetBool("HorizontalXattack"))
        {
             BanInput = true; //BUG修复

            //移动
            if (DoLookRight)
            {
                Move(8f, true, PlayerSlope, Vector3.right);
            }
            else
            {
                Move(8f, true, PlayerSlope, Vector3.left);
            }
        }
    }


    public override void OrdinaryX()
    {
        //该攻击高频率发动卡死bug检查
        if(!animator.GetBool("OrdinaryXattack") && XordinaryDash)
        {
            VariableInitialization();
            Debug.Log("尝试修复");
        }


        //从通常状态进入到X攻击准备状态
        if (!BanXattack &&!animator.GetBool("OrdinaryXattackPrepare") && !animator.GetBool("OrdinaryXattack") && StageCtrl.gameScoreSettings.Horizontal == 0 && !IsAttack[1] && !StageCtrl.gameScoreSettings.Up && !StageCtrl.gameScoreSettings.Down && !StageCtrl.gameScoreSettings.Xattack && StageCtrl.gameScoreSettings.XattackPressed && !BanWalk && !XordinaryDash)
        {
            BanXattack = true;
            animator.SetBool("OrdinaryXattackPrepare", true);
            CancelJump();//直接中断跳跃并且不恢复
            IsAttack[1] = true;
            BanWalk = true;
            BanTurnAround = true;

            //保存一下时间，用于得到蓄力的效果
            OrdinaryXTimer = Time.timeSinceLevelLoad;

            GravityRatio = 0.3f;
        }
        //松开X键，但仍然处于X攻击状态，所以能往前冲
        else if (!StageCtrl.gameScoreSettings.XattackPressed && IsAttack[1]&& !animator.GetBool("OrdinaryXattack") && animator.GetBool("OrdinaryXattackPrepare") && !XordinaryDash)
        {
            animator.SetBool("OrdinaryXattackPrepare",false);
            animator.SetBool("OrdinaryXattack", true);
            XordinaryDash = true;
            GravityRatio = 0.3f;//修复bug
        }

        //冲刺移动（放在这里是为了移动流畅）
        else if (XordinaryDash)
        {
            //使用正负号的不同来防止多次计算
            if (OrdinaryXTimer >= 0F)
            {
                OrdinaryXTimer = -Mathf.Clamp01((Time.timeSinceLevelLoad - OrdinaryXTimer) / 1.5F);
            }

            if (DoLookRight)
            {
                Move(8F - OrdinaryXTimer, true, PlayerSlope.normalized, Vector2.right);
            }
            else
            {
                Move(8F - OrdinaryXTimer, true, PlayerSlope.normalized, Vector2.left);
            }
        }
    }
    public override void DownX()
    {
        if (StageCtrl.gameScoreSettings.Horizontal == 0 && !IsAttack[1] && StageCtrl.gameScoreSettings.Xattack && StageCtrl.gameScoreSettings.Down )
        {
            CancelJump();//直接中断跳跃并且不恢复
            IsAttack[1] = true;
            animator.SetBool("DownXattack-MovingUpward", true);
            BanInput = true;//在这一套攻击里，就靠取消僵直来把这个设置为false了
            BanGravityRay = true;
            BanGravity = true;
            DownAttackMovingUpward = 1;
        }

        //上升
        if (DownAttackMovingUpward == 1)
        {
            if (IsGround)
            {
                Move(13f, true, Vector2.one, Vector2.up);
            }
            else
            {
                Move(5f, true, Vector2.one, Vector2.up);
            }
        }
        //下降
        else if (DownAttackMovingUpward == -1)
        {
            if (DoLookRight)
            {
                Move(13f, true, Vector2.one, Vector2.right);
            }
            else
            {
                Move(13f, true, Vector2.one, Vector2.left);
            }

            //碰到地了（仅执行一次）
            if (IsGround && !animator.GetBool("DownXattack-Done") )
            {
                //反弹
                StartCoroutine("XattackBound");

                animator.SetBool("DownXattack-MovingDownward", false);
                animator.SetBool("DownXattack-Done", true);
                DownAttackMovingUpward = 2;
            }
        }

        //触地反弹
        else if (DownAttackMovingUpward == 2)
        {
            if (DoLookRight)
            {
                Move(12f, true, Vector2.one, new Vector2(-1f, 2f));
            }
            else
            {
                Move(12f, true, Vector2.one, new Vector2(1f, 2f));
            }
        }

    }

    public override void UpX()
    {
        if (IsGround) { UpAttackCount = 0; }

        //UpAttackCount < 1 受上一条IF干扰，第一次起跳不会增加UpAttackCount 
        if (StageCtrl.gameScoreSettings.Horizontal == 0  && !animator.GetBool("UpXattack") && UpAttackCount < 1 && !IsAttack[1] && StageCtrl.gameScoreSettings.Xattack && StageCtrl.gameScoreSettings.Up)
        {
            CancelJump();//直接中断跳跃并且不恢复
            UpAttackCount++;
            UpAttackMove = true;
            IsAttack[1] = true;
            BanWalk = true;
            BanTurnAround = true;
            BanGravityRay = true;
            BanGravity = true;
            BanJump = true;

            animator.SetBool("UpXattack", true);
        }

        if (UpAttackMove)
        {
            if (DoLookRight)
            {
                Move(5f, true, Vector2.one, new Vector2(1f, 3f));
            }
            else
            {
                Move(5f, true, Vector2.one, new Vector2(-1f, 3f));
            }
        }
    }

    public override void OrdinaryZ()
    {
        if (StageCtrl.gameScoreSettings.ZattackPressed && !animator.GetBool("OrdinaryXattack") && !animator.GetBool("OrdinaryXattackPrepare") /*|| Time.timeSinceLevelLoad -  AttackTimer[0] <= PressAttackInteral && AttackTimer[0] != 0*/)
        {
            if (!animator.GetBool("Zattack") && !animator.GetBool("ZattackFin")) CancelJump();//直接中断跳跃并且不恢复
            animator.SetBool("Zattack", true);
            animator.SetBool("Fall", false);
             BanGravity = IsGround;//修复奇怪的bug
        }
    
    }



    //Haruhi Suzumiya

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
                GravityRatio = 0.7f;
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
                IsAttack[0] = StageCtrl.gameScoreSettings.ZattackPressed;//连接处不属于攻击阶段，可以切换到其他动画和状态
                animator.SetBool("Zattack", StageCtrl.gameScoreSettings.ZattackPressed);//现在可以中断动画
                animator.SetBool("Fall", !IsGround && !StageCtrl.gameScoreSettings.ZattackPressed);
                GravityRatio = 1f;

                if (!IsAttack[0])
                {
                    VariableInitialization();
                }

                break;

            //Z攻击打完，
            case "ZattackDone":
                    StopAttacking = true;//可以中断攻击
                    IsAttack[0] = StageCtrl.gameScoreSettings.Zattack;//连接处不属于攻击阶段，可以切换到其他动画和状态
                BanTurnAround = false;//可以转身

                //攻击完了恢复移动速度与重力
                GravityRatio = 1F;
                    //取消Z攻击状态，方便转换到idle或者ZattackFin
                    animator.SetBool("Zattack", StageCtrl.gameScoreSettings.ZattackPressed);
                    //允许下落状态
                    animator.SetBool("Fall", !IsGround && !StageCtrl.gameScoreSettings.ZattackPressed);
               
                if (StageCtrl.gameScoreSettings.ZattackPressed)
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
                BanInput = true;
                BanTurnAround = true;//向前跳的时候不能转身
                StopAttacking = false;//不可以中断攻击
                GravityRatio = 0.7f;

                //向前移动
                if (DoLookRight)
                {
                    Move(0.4f, false, PlayerSlope, Vector2.right);
                }
                else
                {
                    Move(0.4f, false, PlayerSlope, Vector2.left);
                }
                break;

            //Z攻击最后阶段结束
            case "ZattackFinDone":
                animator.SetBool("ZattackFin", false);
                animator.SetBool("Zattack", false);
                StopAttacking = true;
                BanInput = true;
                //修改计数器重新循环动画
                ZattackCount = 0;
                IsAttack[0] = false;//连接处不属于攻击阶段，可以切换到其他动画和状态
                
                //尝试用回复变量的方法来解决bug
                VariableInitialization();
                
                //僵直
                Stiff(0.1f);

                //因为这里不会产生动画未结束松开Z导致动画结束的情况，所以不修改IsZattacking
                break;
        }
    }

    public override void XattackAnimationEvent(string AnimationName)
    {
        switch (AnimationName)
        {
            case "OrdinaryDashDone":
                //结束
                GravityRatio = 1F;
                animator.SetBool("OrdinaryXattack", false);
                animator.SetBool("OrdinaryXattackPrepare", false);
                BanTurnAround = false;
                BanInput = false;
                XordinaryDash = false;
                IsAttack[1] = false;
                BanXattack = false;
                break;

            case "Fuck":
                VariableInitialization();
                //僵直
                Stiff(0.1f);
                break;
        }
               


    }

    public override void HorizontalXattackAnimationEvent(string AnimationName)
    {
        //结束
        BanInput = true;
        StopAttacking = true;
        IsAttack[1] = false;
        animator.SetBool("HorizontalXattack", !true);
        //尝试用回复变量的方法来解决bug
        VariableInitialization();
        Stiff(0.2f);

    }

    public override void DownXattackAnimationEvent(string AnimationName)
    {
        switch (AnimationName)
        {
            //到达顶峰，停顿一下
            case "PeakArrival":
                DownAttackMovingUpward = 0;
                break;

                //蹬了一脚，准备下移
            case "StartMovingDownward":
                animator.SetBool("DownXattack-MovingDownward", true);
                animator.SetBool("DownXattack-MovingUpward", false);
                break;

                //下移
            case "Doing-Down":
                GravityRatio = 1.5F;
                BanGravity = false;
                BanGravityRay = false;
                DownAttackMovingUpward = -1;
                break;

                //着陆
            case "Land":
                StartCoroutine(XattackBound());
                break;
        }
    }

    /// <summary>
    /// 上X攻击结束
    /// </summary>
    /// <param name="AnimationName"></param>
    public override void UpXattackAnimationEvent(string AnimationName)
    {
        UpAttackMove = false;
        IsAttack[1] = false;
        BanGravity = true;//为了悬空效果，僵直结束之后变成false了
        animator.SetBool("UpXattack", false);
        Stiff(0.1f);

    }

    public override void MagiaAnimationEvent(string AnimationName)
    {
        switch (AnimationName)
        {
            case "Finish":
                MagiaDash = false;
                IsAttack[2] = false;
                MagiaDashSpeedRatio = 1f;

                //尝试用回复变量的方法来解决bug
                VariableInitialization();

                BanGravity = false;
                Stiff(0.1f);

                animator.Play("SayakaMagia", 0, 0F);

                animator.SetBool("Magia", false);

                break;

            case "Dash":
                MagiaDash = true;
                break;
        }
    }

    /// <summary>
    /// 这个用于所有Z动画之中（仅限沙耶加），为了那种一个动画前进一次的效果
    /// </summary>
    void ZattackMove()
    {
        if (StageCtrl.gameScoreSettings.Horizontal != 0 && IsGround)
        {
            if (DoLookRight)
            {
                Move(0.02f, false, PlayerSlope, Vector2.right);
            }
            else
            {
                Move(0.02f, false, PlayerSlope, Vector2.left);
            }
        }

    }


    /// <summary>
    /// DownX攻击触地之后的反弹
    /// </summary>
    /// <returns></returns>
    public IEnumerator XattackBound()
    {
        BanGravity = true;
        DownAttackMovingUpward = 2;
      //  IsStiff = true;这个也不能要，不然不会触发反弹效果
        yield return new WaitForSeconds(0.2f);
        DownAttackMovingUpward = 0;
        animator.SetBool("DownXattack-Done", false);
        //   Stiff(0.1f); 自带僵直效果了
      //  IsStiff = false;
        BanGravity = false;
        BanInput = false;
       IsAttack[1] = false;

        //尝试用回复变量的方法来解决bug
        VariableInitialization();
    }



    public override void VerticalZ()
    {

    }
    public override void HorizontalZ()
    {

    }


}




