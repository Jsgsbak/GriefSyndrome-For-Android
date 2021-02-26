using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KyokoCtrl : APlayerCtrl
{
    bool StrongDash = false;
    int StrongForwardDash = 0;
    float OrdinaryXTimer = 0f;
    bool UpAttackMove;
    int UpAttackCount = 0;

    public override void VariableInitialization()
    {
        base.VariableInitialization();
        StrongDash = false;
        OrdinaryXTimer = 0f;
        UpAttackCount = 0;
        UpAttackMove = true;
    }


    public override void DownX()
    {
    }

    public override void DownXattackAnimationEvent(string AnimationName)
    {
    }

    public override void HorizontalX()
    {
        if(StageCtrl.gameScoreSettings.Horizontal != 0 && StageCtrl.gameScoreSettings.Xattack && !IsAttack[1])
        {
            Debug.Log("???");

            CancelJump();
            GravityRatio = 0.1f;
            BanWalk = true;
            BanTurnAround = true;
            BanJump = true;
            IsAttack[1] = true;
            StrongForwardDash = 0;

            //动画播放，一直播放到结尾
            playerStatus = Variable.PlayerStatus.HorizontalStrong_1;
        }

        //短暂移动
        else if(playerStatus == Variable.PlayerStatus.HorizontalStrong_1 && StrongForwardDash == 1)
        {
            if (DoLookRight)
            {
                Move(3f, true, Vector2.right);
            }
            else
            {
                Move(3f, true, Vector2.left);
            }
        }
        //最后冲刺一下
        else if(playerStatus == Variable.PlayerStatus.HorizontalStrong_1 && StrongForwardDash == 2)
        {
            if (DoLookRight)
            {
                Move(10f, true, Vector2.right);
            }
            else
            {
                Move(10f, true, Vector2.left);
            }
        }
    }

    public override void HorizontalXattackAnimationEvent(string AnimationName)
    {
        switch (AnimationName)
        {
            //第一下，转一圈移动的那个部分
            case "Part_1_Move":
                //低速移动
                StrongForwardDash = 1;
                break;

            //第一下，移动结束后转了一圈
            case "Part_1_Round":
                //停止移动
                StrongForwardDash = 0;
                break;

                //第二下，比较快的移动
            case "Part_2_Move":
                //高速移动
                StrongForwardDash = 2;
                break;

                //攻击结束
            case "attackDone":
                StrongForwardDash = 0;
                IsAttack[1] = false;
                Stiff(0.2f);
                break;

        }
    }



    public override void Magia()
    {
    }

    public override void MagiaAnimationEvent(string AnimationName)
    {
    }

    public override void OrdinaryX()
    {

        //蓄力阶段
        if (StageCtrl.gameScoreSettings.Horizontal == 0 && StageCtrl.gameScoreSettings.XattackPressed && !StrongDash && !StageCtrl.gameScoreSettings.Up && !StageCtrl.gameScoreSettings.Down)
        {
            BanTurnAround = true;
            BanWalk = true;
            BanJump = true;
            GravityRatio = 0.3f;

            playerStatus = Variable.PlayerStatus.Strong_1;
            //冲刺计时器
            if(!IsAttack[1])
            {
                OrdinaryXTimer = Time.timeSinceLevelLoad;
                CancelJump();//取消跳跃状态
            }
            IsAttack[1] = true;

        }
        //松开X
        else if(!StageCtrl.gameScoreSettings.XattackPressed && !StrongDash && IsAttack[1] && playerStatus == Variable.PlayerStatus.Strong_1)
        {
            playerStatus = Variable.PlayerStatus.Strong_2;
            StrongDash = true;
        }
        //冲刺阶段
       else if (StrongDash)
        {
            //使用正负号的不同来防止多次计算
            if (OrdinaryXTimer >= 0F)
            {
                //从开始到蓄力完成有3s
                OrdinaryXTimer = -Mathf.Clamp01((Time.timeSinceLevelLoad - OrdinaryXTimer) / 3F) * 8;
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

    public override void OrdinaryZ()
    {
        if(StageCtrl.gameScoreSettings.ZattackPressed)
        {
            //杏子只有一段攻击动画
            if (playerStatus != Variable.PlayerStatus.Weak_1) CancelJump();
            IsAttack[0] = true;
            playerStatus = Variable.PlayerStatus.Weak_1;
            BanWalk = true;

          
        }
    }

    public override void UpX()
    {
        if (IsGround) { UpAttackCount = 0; }

        if (StageCtrl.gameScoreSettings.Horizontal == 0 && UpAttackCount < 1 && StageCtrl.gameScoreSettings.Xattack && StageCtrl.gameScoreSettings.Up)
        {
            UpAttackCount++;
            CancelJump();//直接中断跳跃并且不恢复
            IsAttack[1] = true;
            BanInput = true;
            BanGravity = true;

            playerStatus = Variable.PlayerStatus.UpStrong_1;
        }

        if (UpAttackMove)
        {
            if (DoLookRight)
            {
                Move(2f, true, new Vector2(0.5f, 3f));
            }
            else
            {
                Move(2f, true, new Vector2(-0.5f, 3f));
            }
        }
    }

    public override void UpXattackAnimationEvent(string AnimationName)
    {
        switch (AnimationName)
        {
            case "Jump":
                UpAttackMove = true;
                break;

            case "Done":
                Stiff(0.05F);
                UpAttackMove = false;
                IsAttack[1] = false;
                break;
        }


    }

    public override void VerticalZ()
    {
    }

    public override void XattackAnimationEvent(string AnimationName)
    {
        //冲刺结束
        StrongDash = false;
        IsAttack[1] = false;
        Stiff(0.05f);
    }

    public override void ZattackAnimationEvent(string AnimationName)
    {
        switch (AnimationName)
        {
            case "CouldStop":
                //停止攻击
                if (!StageCtrl.gameScoreSettings.ZattackPressed)
                {
                    BanJump = false;
                    BanWalk = false;
                    BanTurnAround = false;
                    IsAttack[0] = false;
                } 
                break;
        }

        //水平移动
        if (StageCtrl.gameScoreSettings.Horizontal != 0)
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



    public override void HorizontalZ()
    {
    }
}
