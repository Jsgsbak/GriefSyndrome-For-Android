using MEC;
using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class SayakaCtrl : APlayerCtrl
{
    private bool EnableZ3Rush = false;
    private bool EnableMagiaRush = false;


    private void Start()
    {
        //注册私有事件
        UpdateManager.FastUpdate.AddListener(PlayerUpX);
        UpdateManager.FastUpdate.AddListener(PlayerDownX);
        UpdateManager.FastUpdate.AddListener(FastUpdateForSayaka);//MD毛病真多

    }

    //注意：该方法注册在玩家身上的角色移动动画机，即根据玩家动作动画来进行判断
    public override void CheckAnimStop(string AnimName)
    {


        //平A 1 2 3 段连段判断，用于阻止玩家在动画结束前再次攻击
        if (AnimName.Equals("AttackA") || AnimName.Equals("Rush") || AnimName.Equals("Rush_fin"))
        {
            IsAttacking = false;//动画停止了，也就停止了攻击
            Effect.SetActive(false);//动画停止后，禁用角色效果动画机（有的角色可能无需禁用）
            ChangeGravity(25);

            //最后一击僵直与移动
            if (AnimName.Equals("Rush_fin"))
            {
                //僵直
                PlayerJiangZhi(0.2f);
                //禁用移动
                EnableZ3Rush = false;
            }
            else
            {
                PlayerJiangZhi(0.05f);
            }
        }


    }

    public void FastUpdateForSayaka()
    {
        if (EnableZ3Rush)
        {
            //以防万一，加个限制
            if (tr.rotation.w == 1)
            {
                Move(new Vector2(0.05f, 0f));
            }
            else
            {
                Move(new Vector2(-0.05f, 0f));
            }

        }

        if (IsMagia)
        {
            Magia(1);
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
            Move(new Vector2(RebindableInput.GetAxis("Horizontal"), 0f) * 0.01f * Speed);
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
            Move(new Vector2(RebindableInput.GetAxis("Horizontal"), 0f) * 0.01f * Speed);

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

            //僵直(结束动画调用)，移动调用
            if (atlasAnimation.PlayingSpriteId == 4)
            {
                UpdateManager.FastUpdate.AddListener(FastUpdateForSayaka);
                EnableZ3Rush = true;
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
            if (Time.timeSinceLevelLoad - GreatAttackTimer > 0.8f && GteatAttackPart <= 2)//0 1 2 3段 分别代表4个不同阶段
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
                Move(new Vector2(0.3f, 0f) * ((GteatAttackPart / 10) + 0.9f));
            }
            else
            {
                Move(new Vector2(-0.3f, 0f) * ((GteatAttackPart / 10) + 0.9f));
            }

            if (Time.timeSinceLevelLoad - GreatAttackTimer >= 0.2f)
            {
                //停止冲刺
                PlayerJiangZhi(0.1f);
                IsGreatAttacking = false;
                GteatAttackPart = 0;
                ChangeGravity(25);
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

            if (Time.timeSinceLevelLoad - UpXTimer <= 0.5f)
            {
                //不到点，冲
                if (tr.rotation.w == 1)
                {
                    //向右
                    Move(new Vector2(0.3f, 0.5f) * 0.25f);

                }
                else
                {
                    Move(new Vector2(-0.3f, 0.5f) * 0.25f);
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
                BanStandWalk = false;
                AllowRay = true;
                ChangeGravity(25);

            }
        }
    }
    
    //会突刺两次emmmm就当特性好了）
    public override void PlayerDownX()
    {
        if (IsDownX)
        {
            if (Time.timeSinceLevelLoad - DownXTimer <= 0.2f && GteatAttackPart == 0)
            {

                ChangeGravity(0);
                //不到点，冲
                if (tr.rotation.w == 1)
                {
                    //向右
                    Move(new Vector2(-0.5f, 0.5f) * 0.4f);

                }
                else
                {
                    Move(new Vector2(0.5f, 0.5f) * 0.4f);
                }
            }
            else if(GteatAttackPart == 0 && IsDownX)
            {
                GteatAttackPart = 2;
                IsHanging = true;//解决迷之悬空的bug
                DownXTimer = Time.timeSinceLevelLoad;
            }

            //>= 悬空一会
            if (Time.timeSinceLevelLoad - DownXTimer >= 0.1f && GteatAttackPart == 2)
            {
                ChangeGravity(1);//解决异常滞空
                Debug.Log("sda");

                //天上挂一会，然后冲下来
                if (tr.rotation.w == 1)
                {
                    //向右
                    Move(new Vector2(-0.5f, 0.5f) * -0.4f);

                }
                else
                {
                    Move(new Vector2(0.5f, 0.5f) * -0.4f);
                }

            }

            if (GteatAttackPart == 2 && !IsHanging && IsDownX)//IsDownX解决意外跳跃的Bug
            {
                Debug.Log("fff");
                GteatAttackPart = 3;
                DownXTimer = Time.timeSinceLevelLoad;
            }

            //反弹下
            if (Time.timeSinceLevelLoad - DownXTimer <= 0.2f && !IsHanging && GteatAttackPart == 3)
            {
                Debug.Log("dffd");
                //不到点，冲
                if (tr.rotation.w == 1)
                {
                    //向右
                    Move(new Vector2(-5f, 5f) * 0.2f);

                }
                else
                {
                    Move(new Vector2(5f, 5f) * 0.2f);
                }

            }
            else if(Time.timeSinceLevelLoad - DownXTimer >= 0.2f && !IsHanging && GteatAttackPart == 3)
            {
                //结束了
                ChangeGravity(25);
                IsDownX = false;
                IsGreatAttacking = false;//解决攻击动画中断的问题
                IsPreparingAttacking = false;
                BanGreatAttack = false;
                BanStandWalk = false;
                BanAnimFlip = false;
                AllowRay = true;
                GteatAttackPart = 0;
                PlayerJiangZhi(0.1f);
            }

        }
    }

    public override void Magia(int index)
    {
        //准备
        if (index == 0)
        {
            //状态设定
            BanAnimFlip = true;
            BanStandWalk = true;
            BanJump = true;
            AllowRay = false;
            atlasAnimation.ChangeAnimation(MagiaAnimId[0]);

            ChangeGravity(0);
        }
        //向冲刺缓冲
        if(atlasAnimation.PlayingSpriteId == 4 && !EnableMagiaRush)
        {
            EnableMagiaRush = true;
            MagiaTimer = Time.timeSinceLevelLoad;
        }

        //冲刺
         if(EnableMagiaRush)
        {
            if(Time.timeSinceLevelLoad - MagiaTimer <= 0.5f)
            {
                ChangeGravity(0);//防止意外出现重力


                //冲刺中按下了X，带有攻击性
                if (RebindableInput.GetKeyDown("GreatAttack"))
                {
                    atlasAnimation.ChangeAnimation(GreatAttackAnimId[1]);
                }
                else
                {
                    atlasAnimation.ChangeAnimation(MagiaAnimId[1]);
                }


                //魔法冲刺
                if (tr.rotation.w == 1)
                {
                    Move(new Vector2(0.2f, 0f));
                }
                else
                {
                    Move(new Vector2(-0.2f, 0f));
                }
            }

            else
            {
                //停止
                IsMagia = false;
                EnableMagiaRush = false;
                atlasAnimation.ChangeAnimation(StandAnimId);
                PlayerJiangZhi(0.1f);
                BanAnimFlip = false;
                BanStandWalk = false;
                BanJump = false;
                AllowRay = true;

                //解决冲刺后不会恢复状态的bug
                if (IsHanging)
                {
                    atlasAnimation.ChangeAnimation(DropAnimId);
                }
                else
                {
                    atlasAnimation.ChangeAnimation(StandAnimId);
                }

            }


        }


    }


}



