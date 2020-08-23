using MEC;
using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SayakaCtrl : APlayerCtrl
{
    private void Start()
    {
        //注册私有事件
        UpdateManager.FastUpdate.AddListener(PlayerUpX);
    }

    //注意：该方法注册在玩家身上的角色移动动画机，即根据玩家动作动画来进行判断
    public override void CheckAnimStop(string AnimName)
    {


        //平A 1 2 3 段连段判断，用于阻止玩家在动画结束前再次攻击
        if (AnimName.Equals("AttackA") || AnimName.Equals("Rush") || AnimName.Equals("Rush_fin"))
        {
            IsAttacking = false;//动画停止了，也就停止了攻击
            Effect.SetActive(false);//动画停止后，禁用角色效果动画机（有的角色可能无需禁用）
            ChangeGravity(4);//这样才能以比较正常的速度下降


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
            if (tr.rotation.w == 1)
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
            if (tr.rotation.w == 1)
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
                ChangeGravity(4);//这样才能以比较正常的速度下降
                rigidbody2D.velocity = Vector2.zero;

                //修正停止冲刺后动作异常的Bug
                BanStandWalk = false;
                if (IsHanging)
                {
                    atlasAnimation.ChangeAnimation(DropAnimId);
                }

            }

        }
    }

    public override void PlayerUpX()
    {
        if (IsUpX)
        {
            ChangeGravity(0);
            atlasAnimation.ChangeAnimation(UpXAnimId);

            if(Time.timeSinceLevelLoad - UpXTimer <= 0.5f)
            {
                //不到点，冲
                if(tr.rotation.w == 1)
                {
                    //向右
                    rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(0.3f, 0.5f) *0.2f);

                }
                else
                {
                    rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(-0.3f, 0.5f) * 0.2f);
                }
            }
            else
            {
                //到点了，掉下来
                PlayerJiangZhi(0.2f);
                IsUpX = false;
                IsGreatAttacking = false;//解决攻击动画中断的问题
                IsPreparingAttacking = false;
                BanGreatAttack = false;
                BanAnimFlip = false;
                AllowRay = true;
                ChangeGravity(4);

            }
        }
    }
}



