using MEC;
using System.Collections;
using System.Collections.Generic;
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
        UpAttackMove = false;
        UpAttackCount = 0;
         MagiaDash = false;
        MagiaDashSpeedRatio = 1f;
        //攻击动画/状态消除
        for (int i = 0; i < 3; i++)
        {
            IsAttack[i] = false;
        }
    }



    public override void Magia()
    {     
        //带有攻击的magia，并且防止多次运行
        if (MagiaDashSpeedRatio == 1f && StageCtrl.gameScoreSettings.Xattack && IsAttack[2])
        {
            MagiaDashSpeedRatio = 1.5f;
            playerStatus = Variable.PlayerStatus.Magia_2;
        }

        //初始发动magia
        if(!IsAttack[2] && StageCtrl.gameScoreSettings.Magia)
        {
            IsAttack[2] = true;
            CancelJump();
            BanWalk = true;
            BanTurnAround = true;
            BanGravity = true;
          //  BanGravityRay = true;

            playerStatus = Variable.PlayerStatus.Magia_1;

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
                Move(8f * MagiaDashSpeedRatio, true,  Vector2.right);
            }
            else
            {
                Move(8f * MagiaDashSpeedRatio, true, Vector2.left);
            }
        }
    }

    public override void HorizontalX()
    {
        if (StageCtrl.gameScoreSettings.Horizontal != 0 && !IsAttack[1]  && StageCtrl.gameScoreSettings.Xattack)
        {

            CancelJump();//直接中断跳跃并且不恢复
            BanGravity = true;
            BanInput = true;
            IsAttack[1] = true;
            StopAttacking = false;
            playerStatus = Variable.PlayerStatus.HorizontalStrong_1;//移动
        }

        if (playerStatus == Variable.PlayerStatus.HorizontalStrong_1)
        {
             BanInput = true; //BUG修复

            //移动
            if (DoLookRight)
            {
                Move(8f, true, Vector3.right);
            }
            else
            {
                Move(8f, true, Vector3.left);
            }
        }
    }


    public override void OrdinaryX()
    {
        //从通常状态进入到X攻击准备状态
        if ( StageCtrl.gameScoreSettings.Horizontal == 0 && !IsAttack[1] && !StageCtrl.gameScoreSettings.Up && !StageCtrl.gameScoreSettings.Down  && StageCtrl.gameScoreSettings.XattackPressed  && !XordinaryDash)
        {
            playerStatus = Variable.PlayerStatus.Strong_1;
            CancelJump();//直接中断跳跃并且不恢复
            IsAttack[1] = true;
            BanWalk = true;
            BanTurnAround = true;

            //保存一下时间，用于得到蓄力的效果
            OrdinaryXTimer = Time.timeSinceLevelLoad;

            GravityRatio = 0.3f;
        }
        //松开X键，但仍然处于X攻击状态，所以能往前冲
        else if (!StageCtrl.gameScoreSettings.XattackPressed && IsAttack[1]&& playerStatus == Variable.PlayerStatus.Strong_1 && !XordinaryDash)
        {
            playerStatus = Variable.PlayerStatus.Strong_2;

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
                Move(6F - OrdinaryXTimer, true, Vector2.right);
            }
            else
            {
                Move(6F - OrdinaryXTimer, true, Vector2.left);
            }
        }
    }
    public override void DownX()
    {
        if (StageCtrl.gameScoreSettings.Horizontal == 0 && !IsAttack[1] && StageCtrl.gameScoreSettings.Xattack && StageCtrl.gameScoreSettings.Down )
        {
            CancelJump();//直接中断跳跃并且不恢复
            IsAttack[1] = true;
            playerStatus = Variable.PlayerStatus.DownStrong_1;//上升动作
            BanInput = true;//在这一套攻击里，就靠取消僵直来把这个设置为false了
            BanGravity = true;
            DownAttackMovingUpward = 1;
        }

        //上升
        if (DownAttackMovingUpward == 1)
        {
            if (IsGround)
            {
                Debug.Log("2");
                Move(100f, true, Vector2.up);
            }
            else
            {
                Move(4f, true, Vector2.up);
            }
        }
        //下降
        else if (DownAttackMovingUpward == -1)
        {
            if (DoLookRight)
            {
                Move(13f, true,  Vector2.right);
            }
            else
            {
                Move(13f, true,  Vector2.left);
            }

            //碰到地了（仅执行一次）
            if (IsGround && playerStatus != Variable.PlayerStatus.DownStrong_3)
            {
                //反弹
              Timing.RunCoroutine(XattackBound());

                DownAttackMovingUpward = 2;
            }
        }

        //触地反弹
        else if (DownAttackMovingUpward == 2)
        {
            if (DoLookRight)
            {
                Move(5f, true, new Vector2(-1f, 1f));
            }
            else
            {
                Move(5f, true,  new Vector2(1f, 1f));
            }
        }

    }

    public override void UpX()
    {
        if (IsGround) { UpAttackCount = 0; }

        if (StageCtrl.gameScoreSettings.Horizontal == 0  && UpAttackCount < 2  && StageCtrl.gameScoreSettings.Xattack && StageCtrl.gameScoreSettings.Up)
        {
            CancelJump();//直接中断跳跃并且不恢复
            UpAttackCount++;
            UpAttackMove = true;
            IsAttack[1] = true;
            BanWalk = true;
            BanTurnAround = true;
            BanGravity = true;
            BanJump = true;

            playerStatus = Variable.PlayerStatus.UpStrong_1;
        }

        if (UpAttackMove)
        {
            if (DoLookRight)
            {
                Move(4f, true, new Vector2(1f, 3f));
            }
            else
            {
                Move(4f, true,  new Vector2(-1f, 3f));
            }
        }
    }

    public override void OrdinaryZ()
    { 
        if (StageCtrl.gameScoreSettings.ZattackPressed)
        {
            if (playerStatus != Variable.PlayerStatus.Weak_1 && playerStatus != Variable.PlayerStatus.Weak_2) CancelJump();//直接中断跳跃并且不恢复
            if (playerStatus != Variable.PlayerStatus.Weak_2 )  playerStatus = Variable.PlayerStatus.Weak_1;
            IsAttack[0] = true;
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

                break;

            //Z攻击的动画处于两端攻击的连接处，可以中断，中断处允许切换到其他动画和状态
            case "ZattackCouldStop":
                //如果还在攻击那就不能解除移动和跳跃禁止
                StopAttacking = true;//可以中断攻击
                BanTurnAround = false;//连接处可以转身
                IsAttack[0] = StageCtrl.gameScoreSettings.ZattackPressed;//连接处不属于攻击阶段，可以切换到其他动画和状态

                GravityRatio = 1f;

                if (!IsAttack[0])
                {
                    VariableInitialization();
                }

                break;

            //Z攻击打完，
            case "ZattackDone":
                StopAttacking = true;//可以中断攻击
                IsAttack[0] = StageCtrl.gameScoreSettings.ZattackPressed;//连接处不属于攻击阶段，可以切换到其他动画和状态
                BanTurnAround = false;//可以转身

                //攻击完了恢复移动速度与重力
                GravityRatio = 1F;

                if (StageCtrl.gameScoreSettings.ZattackPressed)
                {
                    //并且按着Z，满足条件后进入Z攻击最后阶段
                    //仅在地面上能发动最后一击
                    if (IsGround) ZattackCount++;

                    if (ZattackCount == 2 && IsGround)//仅在地面上并且达到要求了才能发动
                    {
                        playerStatus = Variable.PlayerStatus.Weak_2;
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
                    Move(0.4f, false, Vector2.right);
                }
                else
                {
                    Move(0.4f, false, Vector2.left);
                }
                break;

            //Z攻击最后阶段结束
            case "ZattackFinDone":
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
    
    /// <summary>
    /// 普通重工结束
    /// </summary>
    /// <param name="AnimationName"></param>
    public override void XattackAnimationEvent(string AnimationName)
    {        
                //结束
                GravityRatio = 1F;
                BanTurnAround = false;
                BanInput = false;
                XordinaryDash = false;
                IsAttack[1] = false;

                VariableInitialization();
                //僵直
                Stiff(0.08f);
              


    }

    public override void HorizontalXattackAnimationEvent(string AnimationName)
    {
        //结束
        BanInput = true;
        StopAttacking = true;
        IsAttack[1] = false;
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
                playerStatus = Variable.PlayerStatus.DownStrong_2;//下移动作

                break;

                //下移
            case "Doing-Down":
                GravityRatio = 1.5F;
                BanGravity = false;
                DownAttackMovingUpward = -1;
                break;

                //着陆
            case "Land":
              Timing.RunCoroutine(XattackBound());
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
                Move(0.02f, false, Vector2.right);
            }
            else
            {
                Move(0.02f, false, Vector2.left);
            }
        }

    }


    /// <summary>
    /// DownX攻击触地之后的反弹
    /// </summary>
    /// <returns></returns>
    IEnumerator<float> XattackBound()
    {
        playerStatus = Variable.PlayerStatus.DownStrong_3;//反弹动作

        BanGravity = true;
        DownAttackMovingUpward = 2;
        //  IsStiff = true;这个也不能要，不然不会触发反弹效果
        yield return Timing.WaitForSeconds(0.2f);
        DownAttackMovingUpward = 0;
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




