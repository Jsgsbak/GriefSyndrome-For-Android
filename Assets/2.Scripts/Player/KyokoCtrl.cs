using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KyokoCtrl : APlayerCtrl
{
    public override void DownX()
    {
    }

    public override void DownXattackAnimationEvent(string AnimationName)
    {
    }

    public override void HorizontalX()
    {
    }

    public override void HorizontalXattackAnimationEvent(string AnimationName)
    {
    }

    public override void HorizontalZ()
    {
    }

    public override void Magia()
    {
    }

    public override void MagiaAnimationEvent(string AnimationName)
    {
    }

    public override void OrdinaryX()
    {
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
    }

    public override void UpXattackAnimationEvent(string AnimationName)
    {
    }

    public override void VerticalZ()
    {
    }

    public override void XattackAnimationEvent(string AnimationName)
    {
    }

    public override void ZattackAnimationEvent(string AnimationName)
    {
        switch (AnimationName)
        {
            case "CouldStop":
                //停止攻击
                if (!StageCtrl.gameScoreSettings.ZattackPressed)
                {
                    BanWalk = false;
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
}
