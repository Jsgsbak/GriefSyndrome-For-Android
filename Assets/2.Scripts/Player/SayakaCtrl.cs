using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SayakaCtrl : APlayerCtrl
{

    //注意：该方法注册在玩家身上的角色移动动画机，即根据玩家动作动画来进行判断
    public override void CheckAnimStop(string AnimName)
    {
        Debug.Log("SSS");

        //平A 1 2 3 段连段判断，用于阻止玩家在动画结束前再次攻击
        if (AnimName.Equals("AttackA") || AnimName.Equals("Rush") || AnimName.Equals("Rush_fin"))
        {
            AttackCanGoOn = true;
            IsAttacking = false;//动画停止了，也就停止了攻击
            Effect.SetActive(false);//动画停止后，禁用角色效果动画机（有的角色可能无需禁用）
            ChangeGravity(40);//恢复重力


            //最后一击僵直
            if (AnimName.Equals("Rush_fin"))
            {
               PlayerJiangZhi(0.2f);
            }
            else
            {
                PlayerJiangZhi(0.05f);
            }
        }



    }

    public override void PlayerAttack()
    {

        #region 动画
            //Z 一段动画
        if (ZattackCount == 0 || ZattackCount == 2 || ZattackCount == 4)
        {
            //玩家动画
            atlasAnimation.ChangeAnimation(zAttackAnimId[0]);
            ZattackCount++;

            //按Z行走
            rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(RebindableInput.GetAxis("Horizontal"), 0f) * 0.01f * Speed);
            //下降或减缓重力
            

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
            rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(RebindableInput.GetAxis("Horizontal"), 0f) * 0.01f * Speed);
          
            //效果动画
            EffectAnimation.ChangeAnimation(zAttackEffectId[1]);

        }
        //Z 三段动画
        else 
        {
            if (IsHanging)
            {
                //悬空的话不能用第三段
                ZattackCount = 0;
                PlayerAttack();
                return;
            }

            //玩家动画
            atlasAnimation.ChangeAnimation(zAttackAnimId[2]);
            ZattackCount = 0;
            //效果动画
            EffectAnimation.ChangeAnimation(zAttackEffectId[2]);

            //禁止再次攻击
            BanAnyAttack = true;

            //僵直(结束动画调用)，移动
            if (IsLookAtRoght)
            {
                rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(1f, 0f));
            }
            else
            {
                rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(-1f, 0f));
            }
        }
    }

    public override void PlayerGreatAttack()
    {
        //蓄力状态
        if (IsPreparingAttacking)
        {
            atlasAnimation.ChangeAnimation(GreatAttackAnimId[0]);
            
            ChangeGravity(GravityForAttack);

            //真 * 蓄力
            if(Time.timeSinceLevelLoad - GreatAttackTimer > 0.8f && GteatAttackPart <= 2)
            {
                GreatAttackTimer = Time.timeSinceLevelLoad;
                GteatAttackPart++;
            }
        }
        //攻击状态
        else if (IsGreatAttacking)
        {
            //自带防止多次调用的处理
            atlasAnimation.ChangeAnimation(GreatAttackAnimId[1]);

            //冲刺
            if (IsLookAtRoght)
            {
                rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(0.2f, 0f) * ((GteatAttackPart/10) + 0.9f)) ;
            }
            else
            {
                rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(-0.2f, 0f) * ((GteatAttackPart / 10) + 0.9f));
            }

            if(Time.timeSinceLevelLoad - GreatAttackTimer >= 0.3f)
            {
                //停止冲刺
                PlayerJiangZhi(0.1f);
                IsGreatAttacking = false;
                GteatAttackPart = 0;
                ChangeGravity(40);

            }

        }
    }
    #endregion
}



