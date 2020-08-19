using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SayakaCtrl : APlayerCtrl
{


    //注意：该方法注册在玩家身上的角色移动动画机，即根据玩家动作动画来进行判断
    public override void CheckAnimStop(string AnimName)
    {
        //所有这些动画结束后，先允许站立与行走的姿势
        BanStandWalkAnim = false;

        //平A 1 2 3 段连段判断，用于阻止玩家在动画结束前再次攻击
        if (AnimName.Equals("AttackA") || AnimName.Equals("Rush") || AnimName.Equals("AttackD"))
        {
            ZattackCanGoOn = true;
            IsZattacking = false;//动画停止了，也就停止了攻击
        }
    }

    public override void PlayerAttackZ()
    {
        //用于阻止玩家在动画结束前再次攻击 
        ZattackCanGoOn = false;

        #region 动画
        //Z 一段动画
        if (ZattackCount == 0 || ZattackCount == 2 || ZattackCount == 4)
        {
            //玩家动画
            atlasAnimation.ChangeAnimation(zAttackAnimId[0]);
            ZattackCount++;

            //按Z行走
            rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(RebindableInput.GetAxis("Horizontal"), 0f) * 0.05f * Speed);

            //效果动画
            EffectAnimation.ChangeAnimation(zAttackEffectId[0]);
        }
        //Z 二段动画
        else if (ZattackCount == 1 || ZattackCount == 3)
        {
            //玩家动画
            atlasAnimation.ChangeAnimation(zAttackAnimId[1]);
            ZattackCount++;

            //按Z行走
            rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(RebindableInput.GetAxis("Horizontal"), 0f) * 0.05f * Speed);
          
            //效果动画
            EffectAnimation.ChangeAnimation(zAttackEffectId[1]);

        }
        //Z 三段动画
        else
        {
            //玩家动画
            atlasAnimation.ChangeAnimation(zAttackAnimId[2]);
            ZattackCount = 0;
            //效果动画
            EffectAnimation.ChangeAnimation(zAttackEffectId[2]);


            //僵直，移动
        }
    }
        #endregion
    }



