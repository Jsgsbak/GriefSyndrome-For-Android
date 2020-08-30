using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;
using MEC;
using System;


//还需要限制一下地板防止出界

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class APlayerCtrl : MonoBehaviour
{
    public int Gravity = 25;

    [Header("玩家移动")]
    public float Speed = 10f;
    [Header("允许悬停吗(true=空中停止)")]
    public bool AllowHanging = false; //这个放在Jump那里，因为重力由Jump控制
    /// <summary>
    /// 不允许悬停时滞空的重力值（攻击用）
    /// </summary>
    [Header("不允许悬停时滞空的重力值（攻击用）")]
    public int GravityForAttack = 10;
    [Header("角色移动动画机")]
    public AtlasAnimation atlasAnimation;
    //这些动画ID如果取值-1则直接无视该动画
    public int StandAnimId = 0;
    public int MoveAnimId = 1;
    public int JumpAnimId = 19;
    public int DropAnimId;
    public int HurtAnimId;
    public int UpXAnimId;
    public int DownXAnimId;
    public int[] MagiaAnimId;//有的角色magia有蓄力之类的动作
    public int DieAnimId;
    public Sprite BodyDieImage;
    public Sprite SoulBall;
    public Color SoulBallColor;
    [Header("平A n段攻击")]
    public int[] zAttackAnimId;
    [Header("X攻击N段攻击")]
    public int[] GreatAttackAnimId;
    [Header("角色效果动画机")]
    public AtlasAnimation EffectAnimation;
    public SpriteRenderer EffectRenderer;
    /// <summary>
    /// 显示攻击效果的物体
    /// </summary>
    [HideInInspector] public GameObject Effect;
    [Header("EffectAnimation中Z的效果动画ID")]
    public int[] zAttackEffectId;
    public int[] GreatAttackEffectId;

    #region 玩家信息
    /// <summary>
    /// 所选的魔法少女
    /// </summary>
    [Header("所选的魔法少女")]
    public Variable.PlayerFaceType SelectedMahoshaojo;
    [Header("玩家信息")]
   public  int playerId = 0;
    /// <summary>
    /// 人物等级
    /// </summary>
     public int Level = 1;
    /// <summary>
    /// 当前经验（升级所需经验：12+2 * (level - 1)）  未验证，猜测
    /// </summary>
    public int Experience = 0;
    /// <summary>
    /// 灵魂值
    /// </summary>
     public int SoulLimit;
    /// <summary>
    /// 最大灵魂值
    /// </summary>
    public int MaxSoulLimit;
    /// <summary>
    /// HP
    /// </summary>
     public int Vit;
    /// <summary>
    /// 最大HP
    /// </summary>
    public int MaxVit;
    /// <summary>
    /// 攻击力
    /// </summary>
    public int Pow;
    /// <summary>
    /// 回复消耗的Soul Limit关于损失Vit的倍数
    /// </summary>
     public int RecoverySoul = 18;
    /// <summary>
    /// 复活消耗的Soul Limit关于损失Vit最大值的倍数
    /// </summary>
    public int RebirthSoul = 30;
    /// <summary>
    /// 发动时Maiga消耗Vit数
    /// </summary>
     public int MaigaVit = 45;

    #endregion


    #region 状态
    /// <summary>
    /// 不允许执行站立/行走
    /// </summary>
    [Header("玩家状态")]
    public bool BanStandWalk = false;
    /// <summary>
    /// 玩家僵直？
    /// </summary>
    public bool IsJiangZhi = false;
    /// <summary>
    /// 悬空
    /// </summary>
    public bool IsHanging = false;

    /// <summary>
    /// 禁止跳跃
    /// </summary>
    public bool BanJump = false;

    /// <summary>
    /// 正在跳跃
    /// </summary>
    public bool IsJumping = false;
    /// <summary>
    /// 跳跃次数
    /// </summary>
    public int JumpCount = 0;
    /// <summary>
    /// 跳跃用计时器
    /// </summary>
    public float JumpTimer = 0f;

    /// <summary>
    /// 禁止任何攻击
    /// </summary>
    public bool BanAnyAttack = false;

    /// <summary>
    /// Z平A连段次数
    /// </summary>
    public int ZattackCount = 0;
    /// <summary>
    /// 正在用Z攻击
    /// </summary>
    public bool IsAttacking = false;

    /// <summary>
    /// 禁止Z攻击
    /// </summary>
    public bool BanAttacking = false;
    /// <summary>
    /// X计时器
    /// </summary>
    public float GreatAttackTimer = 0f;

    /// <summary>
    /// 正在X蓄力
    /// </summary>
    public bool IsPreparingAttacking = false;
    /// <summary>
    /// 正在X攻击
    /// </summary>
    public bool IsGreatAttacking = false;
    /// <summary>
    /// 禁止X攻击
    /// </summary>
    public bool BanGreatAttack = false;
    /// <summary>
    /// X攻击阶段
    /// </summary>
    public int GteatAttackPart = 0;
    /// <summary>
    /// 正在UP+x攻击
    /// </summary>
    public bool IsUpX = false;
    /// <summary>
    /// Up+X攻击的计时器
    /// </summary>
    public float UpXTimer = 0f;
    /// <summary>
    /// 正在down+x攻击
    /// </summary>
    public bool IsDownX = false;
    /// <summary>
    /// DOWN+X计时器
    /// </summary>
   public float DownXTimer = 0f;
    /// <summary>
    /// 在用魔法吗
    /// </summary>
    public bool IsMagia = false;

    /// <summary>
    /// Magia计时器
    /// </summary>
    public float MagiaTimer = 0f;
    /// <summary>
    /// 被攻击
    /// </summary>
    public bool IsHurt = false;
    /// <summary>
    /// 玩家身体死亡
    /// </summary>
    public bool IsBodyDie = false;
    /// <summary>
    /// 是灵魂球的状态
    /// </summary>
    public bool IsSoulBall = false;
    /// <summary>
    /// 无敌了吗
    /// </summary>
    public bool IsWuDi = false;
    /// <summary>
    /// 禁用动画翻转
    /// </summary>
    public bool BanAnimFlip = false;
    /// <summary>
    /// 允许脚底的那个射线吗
    /// </summary>
    public bool AllowRay = true;
    #endregion



    #region 自带组件
    [HideInInspector] public Rigidbody2D rigidbody2D;
    [HideInInspector] public Transform tr;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    //[HideInInspector] public Collider2D collider2D;
    [HideInInspector] public BoxCollider2D collider2D;
    #endregion

    private void Awake()
    {
        #region 初始化组件
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
        tr = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Effect = EffectAnimation.gameObject;//显示攻击效果的物体
        collider2D = GetComponent<BoxCollider2D>();//碰撞箱
        #endregion

        #region 注册事件
        UpdateManager.FastUpdate.AddListener(FastUpdate);
        UpdateManager.FakeLateUpdate.AddListener(RayGround);
        UpdateManager.FastUpdate.AddListener(Jump);
        UpdateManager.FakeLateUpdate.AddListener(SimulatedGravityAndMove);
        UpdateManager.FastUpdate.AddListener(PlayerGreatAttack);
        atlasAnimation.AnimStop.AddListener(CheckAnimStop);
        #endregion

        #region 获取playerId，并将所选魔法少女的id信息录入gss中
        for (int i = 0; i < 3; i++)
        {
            //注意：SelectedGirlInGame是在标题界面的魔法少女选择part决定的，在Majo场景不会被修改，所以debug的时候要手动修改
            if (SelectedMahoshaojo == StageCtrl.gameScoreSettings.SelectedGirlInGame[i])
            {
                playerId = i+1;
                Debug.Log("ID " + playerId);
                break;
            }

        }
        #endregion

        #region 注册受伤事件以及设置tag
        tag = string.Format("{0}{1}", "Player", playerId.ToString());
        if(playerId == 1)
        {
            StageCtrl.Player1Hurt.AddListener(GetHurted);
        }
        else if(playerId == 2)
        {
            StageCtrl.Player2Hurt.AddListener(GetHurted);
        }
        else
        {
            StageCtrl.Player3Hurt.AddListener(GetHurted);

        }
        #endregion


        //根据已有数据获取玩家信息
        UpdatePlayerInformation();
    }


    public void FastUpdate()
    {
        //对玩家状态的判断与修改尽量往这里放
        if (IsSoulBall)
        {
            //灵魂球，允许自由移动
            rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(RebindableInput.GetAxis("Horizontal"), RebindableInput.GetAxis("Vertical")) * 0.1f * Speed);

        }


        //如果玩家死亡，直接返回，不接受后续处理
        if (IsBodyDie || IsHurt || IsJiangZhi)
        {
            return;
        }

        //尽量对状态的修改拿到这里
        if (!BanAnimFlip && RebindableInput.GetAxis("Horizontal") != 0) { AnimFlip(); }

        //常规的站立与行走（包含下降），跳跃时候的移动不归他管理
        if (!BanStandWalk)
        {
            WalkDropAndStand();
        }

        //跳跃
        if (!BanJump && RebindableInput.GetKeyDown("Jump") && JumpCount <= 1) 
        {

            //跳跃计数
            JumpCount++;
            //正在跳跃状态，允许跳跃
            IsJumping = true;
            //重置计时器，用于跳跃
            JumpTimer = Time.timeSinceLevelLoad;
            //迫不得已把动作判断给拿出来了
            atlasAnimation.ChangeAnimation(JumpAnimId, true);

            rigidbody2D.velocity = Vector2.zero;
     
        }

        //Z
        if (!BanAnyAttack && RebindableInput.GetKeyDown("Attack") && !BanAttacking) 
        {
            if (!AllowHanging && IsHanging)
            { 
                //如果不允许滞空，但是在空中，则缓降
                ChangeGravity(GravityForAttack); 
            }
            else if(AllowHanging && IsHanging)
            {
                //如果允许滞空，则呆着
                ChangeGravity(0);
            }

                PlayerAttack(); 
            BanStandWalk = true; 
            IsAttacking = true; 
            Effect.SetActive(true); 
        }
        else if (!BanAnyAttack && RebindableInput.GetKeyUp("Attack") && !BanAttacking)
        {
            IsAttacking = false;
        }

        //解决跳跃攻击后落地不会回复站立的bug
        else if (RebindableInput.GetKeyUp("Attack")) { IsAttacking = false; }

        //X蓄力 人物动作，特效另做处理
        else if (!BanAnyAttack && RebindableInput.GetKeyDown("GreatAttack") && !IsGreatAttacking && !BanGreatAttack)
        { 
            Effect.SetActive(true); 
            BanStandWalk = true;
      
            IsPreparingAttacking = true;
            GreatAttackTimer = Time.timeSinceLevelLoad;
            BanAnimFlip = true;
          //  AllowRay = false;
        }

        //X攻击 
        else if (!BanAnyAttack && RebindableInput.GetKeyUp("GreatAttack") && IsPreparingAttacking && !BanGreatAttack) 
        {
            IsGreatAttacking = true; 
            IsPreparingAttacking = false;
            BanAnimFlip = true;
        //    AllowRay = false;
            GreatAttackTimer = Time.timeSinceLevelLoad; 
        }

        //X + Up 攻击 
        else if(!BanAnyAttack && RebindableInput.GetKey("GreatAttack") && RebindableInput.GetAxis("Vertical") >= 1 &&!IsUpX && GteatAttackPart <= 0)
        {
            GteatAttackPart++;
            IsGreatAttacking = false;//解决攻击动画中断的问题
            IsPreparingAttacking = false;
            IsUpX = true;
            BanGreatAttack = true;
            BanStandWalk = true;
            BanAnimFlip = true;
      //      AllowRay = false;
            atlasAnimation.ChangeAnimation(UpXAnimId);
            UpXTimer = Time.timeSinceLevelLoad;
        }

        //X + Down 攻击 
        else if (!IsDownX &&!BanAnyAttack && RebindableInput.GetKey("GreatAttack") && RebindableInput.GetAxis("Vertical") <= -1 && GteatAttackPart == 0)
        {
            GteatAttackPart = 0;
            IsDownX = true;
            IsGreatAttacking = false;//解决攻击动画中断的问题
            IsPreparingAttacking = false;
            BanGreatAttack = true;
            BanStandWalk = true;
            BanAnimFlip = true;
            atlasAnimation.ChangeAnimation(DownXAnimId);
            DownXTimer = Time.timeSinceLevelLoad;
        }

        //这里是简化的处理，具体要在每个人的脚本里写
        if(!BanAnyAttack && !IsMagia && RebindableInput.GetKeyDown("Magia"))
        {
            MagiaTimer = Time.timeSinceLevelLoad;
            Magia(0);
        }
        else if (!BanAnyAttack && !IsMagia && RebindableInput.GetKey("Magia"))
        {
            Magia(1);
            IsMagia = true;
        }
        else if (IsMagia && RebindableInput.GetKeyUp("Magia"))
        {
            Magia(2);
           // IsMagia = false; 有的角色不能在这里取消状态
        }

        /*
        //没有操控输入的时候（除了移动），回复状态
        if(!RebindableInput.GetKey("Attack") && !RebindableInput.GetKey("Magia") && !RebindableInput.GetKey("GreatAttack") && !RebindableInput.GetKeyDown("Jump"))
        {
            BanStandWalk = false;
            AllowRay = true;
            IsPreparingAttacking = false;
            BanAnyAttack = false;
            BanJump = false;
        }*/
    }

    /// <summary>
    /// 使用射线判断是否在地上
    /// </summary>
    public void RayGround()
    {
        if (!AllowRay)
        {
            return;
        }
#if UNITY_EDITOR
        Debug.DrawRay(new Vector2(tr.position.x - 0.8f, tr.position.y), Vector2.down, Color.red);
        Debug.DrawRay(new Vector2(tr.position.x + 0.8f, tr.position.y), Vector2.down, Color.red);
#endif
        //仅对10（Ground）碰撞检测
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(tr.position.x - 0.8f, tr.position.y), Vector2.down, 1f, 1 << 10);//10:Ground层ID
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(tr.position.x + 0.8f, tr.position.y), Vector2.down, 1f, 1 << 10);//10:Ground层ID

        if (SoulLimit > 0)
        {

            if (Gravity > 0)//大于零：防止在上升的时候碰到平台意外初始化
            {
                //在地上，初始化
                if (hitLeft.collider != null || hitRight.collider != null)
                {

                    IsHanging = false;
                    JumpCount = 0;
                    ChangeGravity(25, false);
                    if (IsJumping) BanStandWalk = false;
                    IsUpX = false;
                    IsJumping = false;//没有刚好能跳上去的平台

                    //如果受伤了，并且接触到了地面，恢复被禁用的输入
                    if (IsHurt)
                    {
                        BanStandWalk = false;
                        BanJump = false;
                        BanAnyAttack = false;
                        IsHurt = false;
                    }

                    //如果身体死了，并且接触到了地面，换上死亡贴图
                 //   if (IsBodyDie) spriteRenderer.sprite = BodyDieImage;
                }

            }

            //这个就不怕受到平台的影响了
            if (hitLeft.collider == null && hitRight.collider == null)
            {
                IsHanging = true;//其他没有接触到地面的情况
            }

        }


        else
        {
            //魔女化
            //在地上，取消重力，取消刚体，结束游戏*
            if (hitLeft.collider != null || hitRight.collider != null)
            {
                Gravity = 0;
                rigidbody2D.bodyType = RigidbodyType2D.Static;
                rigidbody2D.Sleep();
                collider2D.enabled = false;
                //死亡动画
                atlasAnimation.ChangeAnimation(DieAnimId);
                //射线自我了解
                AllowRay = false;

                UpdateManager.FakeLateUpdate.RemoveAllListeners();

            }

        }
    }

    /// <summary>
    /// 不循环的动画结束后调用这个来进行检查
    /// </summary>
    /// <param name="AnimName"></param>
    public abstract void CheckAnimStop(string AnimName);
    

    /// <summary>
    /// Z键
    /// </summary>
    public abstract void PlayerAttack();
/// <summary>
/// x键攻击
/// </summary>
/// <param name="i">阶段数，从1开始</param>
    public abstract void PlayerGreatAttack();

    /// <summary>
    ///↑+X
    /// </summary>
    public abstract void PlayerUpX();

    public abstract void PlayerDownX();

    public abstract void Magia(int index);
    /// <summary>
    /// 受伤
    /// </summary>
    [ContextMenu("受伤")]
    private void GetHurted(int damage)
    {

        if (!IsWuDi && !IsBodyDie && !IsSoulBall && SoulLimit >= 0)
        {
            Vit -= damage;

            IsHurt = true;
            atlasAnimation.ChangeAnimation(HurtAnimId, true);
            BanStandWalk = true;
            BanJump = true;
            BanAnyAttack = true;
            //弹开
            rigidbody2D.velocity = Vector2.zero;
            if (tr.rotation.w == 1)
            {
                rigidbody2D.AddForce(new Vector2(2f, 1f) * 4f, ForceMode2D.Impulse);//先放着，找个时间用曲线来代替力
            }
            else
            {
                rigidbody2D.AddForce(new Vector2(-2f, 1f) * 4f, ForceMode2D.Impulse);//先放着，找个时间用曲线来代替力
            }

            PlayerJiangZhi(0.2f);

            if (SoulLimit <= 0)//有的时候vit（hp）还有但是没灵魂了，变成魔女
            {
                BecomeWitch();
                return;
            }


            if (Vit <= 0)
            {
                BodyDie();
            }
            else
            {
                PlayerWuDi();//防止死亡时无敌

            }

        }
    }

    /// <summary>
    /// 身体挂了
    /// </summary>
    public void BodyDie()  //先放着，找个时间用曲线来代替力
    {
        IsBodyDie = true;
        BanStandWalk = true;
        BanJump = true;
        BanAnyAttack = true;
        if (SoulLimit <= 0)
        {
            //魔女化
            BecomeWitch();
        }
        else
        {
            //普通死亡，复活

            PlayerReBirth();
        }

        //（额外的）弹开
        rigidbody2D.velocity = Vector2.zero;
        if (tr.rotation.w == 1)
        {
            rigidbody2D.AddForce(new Vector2(1f, 2f) * 4f, ForceMode2D.Impulse);//先放着，找个时间用曲线来代替力
        }
        else
        {
            rigidbody2D.AddForce(new Vector2(-1f, 2f) * 4f, ForceMode2D.Impulse);//先放着，找个时间用曲线来代替力
        }

        spriteRenderer.sprite = BodyDieImage;
        //动画在射线那里

    }

    #region 内部方法





    /// <summary>
    /// 转生为魔女
    /// </summary>
    public void BecomeWitch()
    {
        //死亡动画在射线里
        //状态修改
        BanGreatAttack = true;
        BanStandWalk = true;
        BanJump = true;
        BanAnyAttack = true;
        IsBodyDie = true;

        UpdateManager.FastUpdate.RemoveListener(Jump);
        UpdateManager.FakeLateUpdate.RemoveListener(SimulatedGravityAndMove);
        UpdateManager.FastUpdate.RemoveListener(PlayerGreatAttack);

        rigidbody2D.velocity = Vector2.zero;
        //（额外的）弹开
        if (tr.rotation.w == 1)
        {
            rigidbody2D.AddForce(new Vector2(1f, 2f) * 4f, ForceMode2D.Impulse);//先放着，找个时间用曲线来代替力
        }
        else
        {
            rigidbody2D.AddForce(new Vector2(-1f, 2f) * 4f, ForceMode2D.Impulse);//先放着，找个时间用曲线来代替力
        }
        Gravity = 0;
        rigidbody2D.gravityScale = 5;

        //黑烟

    }


    public void GetExperience(int exp)
    {
        Experience = Experience + exp;

        if(Experience >= 12 + 2*(Level - 1))
        {
            LevelUp();
            Experience = 0;
        }
    }


#if UNITY_EDITOR
    [ContextMenu("初始化gss")]
    public void Initial()
    {
        StageCtrl.gameScoreSettings.Initial();
    }
#endif

    [ContextMenu("升级")]
    public void LevelUp()
    {
        Level++;
        UpdatePlayerInformation();
    }

    #region 玩家无敌
    public void PlayerWuDi()
    {
        IsWuDi = true;
        CancelInvoke("WuDi");
        InvokeRepeating("WuDi", 0f, 0.1f);

    }
    int wudiCount = 0;
    void WuDi()
    {
        if(wudiCount % 2 == 0)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        }
        else
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        }
        if(wudiCount == 20)
        {
            IsWuDi = false; 
            CancelInvoke("WuDi");
            wudiCount = 0;
        }
        else
        {
            wudiCount++;
        }
    }
    #endregion

    #region 玩家复活
    /// <summary>
    /// 玩家僵直调用
    /// </summary>
    /// <param name="Time"></param>
    public void PlayerReBirth()
    {
        //为了无敌忽闪忽闪的效果
        PlayerWuDi();

        //取消滞留
        CancelInvoke("PlayerToSoul");

        //前半段处理
        BanStandWalk = true;
        BanJump = true;
        BanAnyAttack = true;
        AllowRay = false;
        Effect.SetActive(false);
        atlasAnimation.PauseAnimation();


        //后半段恢复处理
        Invoke("PlayerToSoul", 2f);
    }

    /// <summary>
    /// 玩家变成灵魂
    /// </summary>
    void PlayerToSoul()
    {
       

        //恢复soul，恢复vit
        //代码还没写

        PlayerWuDi();
        BanStandWalk = false;
        BanJump = true;
        BanAnyAttack = true;
        AllowRay = false;
        //灵魂变回人
        //先变成灵魂
        atlasAnimation.StopAnimation();//停止动画，防止意外占用SoulBall
        spriteRenderer.color = SoulBallColor;//变色
        spriteRenderer.sprite = SoulBall;//变成灵魂
        IsSoulBall = true;//允许FastUpdate中的自由移动
                          //    collider2D.enabled = false;//禁用物理引擎
                          //  rigidbody2D.bodyType = RigidbodyType2D.Kinematic;//禁用动力学，以运动学取代
        ChangeGravity(0);//0重力
       Invoke("SoulToPlayer",3f);//3秒复活
    }
    /// <summary>
    /// 灵魂变回人
    /// </summary>
    void SoulToPlayer()
    {
        IsBodyDie = false;
        BanStandWalk = false;
        BanJump = false;
        BanAnyAttack = false;
        AllowRay = true;
        IsSoulBall = false;
        atlasAnimation.ChangeAnimation(DropAnimId);//防止不恢复动作的bug出现
        ChangeGravity(25);//重力

        //2秒无敌
        PlayerWuDi();

        //变色
        spriteRenderer.color = Color.white;//变色

    }
    #endregion


    #region 玩家僵直
    /// <summary>
    /// 玩家僵直调用
    /// </summary>
    /// <param name="Time"></param>
    public void PlayerJiangZhi(float Time)
    {
        //取消滞留的僵直
        CancelInvoke("JiangZhiRecovery");

        //前半段僵直处理
        IsJiangZhi = true;
        BanStandWalk = true;
        BanJump = true;
        BanAnyAttack = true;
        AllowRay = false;
        BanAnimFlip = true;

        //重力
        ChangeGravity(25);



        Effect.SetActive(false);
        atlasAnimation.PauseAnimation();


        //后半段恢复处理
        Invoke("JiangZhiRecovery", Time);


    }

    /// <summary>
    /// 玩家僵直恢复处理
    /// </summary>
    /// <param name="Time">僵直事件</param>
    void JiangZhiRecovery()
    {
        BanStandWalk = false;
        BanJump = false;
        BanAnyAttack = false;
        AllowRay = true;
        BanAnimFlip = false;
        IsJiangZhi = false;//修复意外移动的bug
    }
    #endregion

    /// <summary>
    /// 改变重力
    /// </summary>
    /// <param name="value">新的重力值</param>
    /// <param name="TryToHang">false：用于跳跃</param>
    public void ChangeGravity(int value,bool TryToHang = true)
    {
        if (TryToHang)
        {
            IsJumping = false;
        }
        rigidbody2D.velocity = Vector2.zero;
        Gravity = value;
      //  rigidbody2D.gravityScale = value;
        
    }

    /// <summary>
    /// 因为这个有点特殊，在Update中调用，所以比较特别
    /// </summary>
    void Jump()
    {
        //正在跳跃的时候才跳
        if (IsJumping)
        {   
            //防止意外出现站立动作
            BanStandWalk = true;

            //跳跃时动作（移动）
            //rigidbody2D.MovePosition(rigidbody2D.position + );
            Move(new Vector2(RebindableInput.GetAxis("Horizontal"), 0f) * 0.1f * Speed);

            //起飞
            ChangeGravity(-15,false);

            /*
            if (Time.timeSinceLevelLoad - JumpTimer   > 0.25f)
            {
                //时间到，滞空
                rigidbody2D.gravityScale = 0f;
                JumpTimer = Time.timeSinceLevelLoad;
            }*/

            if (Time.timeSinceLevelLoad - JumpTimer > 0.3f)
            {
                //时间到，下降
                ChangeGravity(25,false);

            }


        }

    }


    void WalkDropAndStand()
    {
        //为啥要加上Drop：下降的时候没有禁用这个方法，并且如果这个方法能执行的话，说明没有干别的，比较适合做下落动作

        //动作
        // rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(RebindableInput.GetAxis("Horizontal"), 0f) * Speed);
        Move(new Vector2(RebindableInput.GetAxis("Horizontal"), 0f) * 0.1F *Speed);

        //动画
        if (RebindableInput.GetAxis("Horizontal") == 0 && !IsHanging) atlasAnimation.ChangeAnimation(StandAnimId);
        else if (RebindableInput.GetAxis("Horizontal") != 0 && !IsHanging) atlasAnimation.ChangeAnimation(MoveAnimId);
        //下落动画
        if (IsHanging && Gravity > 0) { atlasAnimation.ChangeAnimation(DropAnimId);}

    }

    #region     动画翻转

    readonly Quaternion LookAtLeft = new Quaternion(0f, 1f, 0f, 0f);
    readonly Quaternion LookAtRight = new Quaternion(0f, 0f, 0f, 1f);
    /// <summary>
    /// 动画翻转
    /// </summary>
    void AnimFlip()
    {
        if(RebindableInput.GetAxis("Horizontal") > 0)
        {
            // spriteRenderer.flipX = true;
            //  EffectRenderer.flipX = true;
            tr.rotation = LookAtRight;
        }
        else if(RebindableInput.GetAxis("Horizontal") < 0)
        {
            //  spriteRenderer.flipX = false;
            //   EffectRenderer.flipX = false;
            tr.rotation = LookAtLeft;

        }
    }

    #endregion 

    /// <summary>
    /// 模拟重力 执行移动
    /// </summary>
    public void SimulatedGravityAndMove()
    {
        if (IsHanging || IsJumping)//仅允许跳跃/悬空的时候有重力加成，解决抖动的问题
        {
            Move(new Vector2(0f, -Gravity * 0.01f));
        }
        Move(Vector2.zero, true);//移动
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="vector2"></param>
    /// <param name="StartMove">开始移动吗，否的话就计算移动的位置（用FakeLateUpdate调用true）</param>
    /// <returns></returns>
    Vector2 vector = Vector2.zero;
    Vector2 vecAlways = Vector2.zero;
    public void Move(Vector2 vector2, bool StartMove = false)
    {
        if (StartMove) 
        {
            rigidbody2D.position = rigidbody2D.position + vector * Time.deltaTime * 35f;
            vector = Vector2.zero;
        }
        else
        {
            vector += vector2;
        }
    }

    /// <summary>
    /// 更新玩家属性
    /// </summary>
    public void UpdatePlayerInformation()
    {
        Level = StageCtrl.gameScoreSettings.Level[playerId - 1];

        //Pow
        Pow = Mathf.Clamp(StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].BasicPow + StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].PowGrowth * (Level - 1), StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].BasicPow, StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].MaxPow);

        //Max SoulLimit
        MaxSoulLimit = Mathf.Clamp(StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].BasicSoulLimit + StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].SoulGrowth * (Level - 1), StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].BasicSoulLimit, StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].MaxSoul);

        //MAX vit(hp)
        for (int i = 0; i < StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].VitGrowth.Length; i++)
        {
            if (Level <= StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].VitGrowth[i])
            {
                MaxVit = Mathf.Clamp(StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].BasicVit + StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].VitGrowth[i] * (Level - 1), StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].BasicVit, StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].MaxVit);
                break;
            }
        }

        RebirthSoul = StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].Rebirth;
        RecoverySoul = StageCtrl.gameScoreSettings.mahouShoujos[(int)SelectedMahoshaojo].Recovery;

    }

    /// <summary>
    /// 向gss保存玩家信息（等级）
    /// </summary>
    public void SavePlayerInformation()
    {
        StageCtrl.gameScoreSettings.Level[playerId - 1] = Level;
    }


}
#endregion

