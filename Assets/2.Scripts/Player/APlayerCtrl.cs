using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;
using MEC;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class APlayerCtrl : MonoBehaviour
{
    /// <summary>
    /// 所选的魔法少女
    /// </summary>
    [Header("所选的魔法少女")]
    public Variable.PlayerFaceType SelectedMahoshaojo;

    [Header("玩家移动")]
    public float Speed = 10f;

    /// <summary>
    /// 向左看
    /// </summary>
    readonly Quaternion LookLeft = new Quaternion(0f, 1f, 0f, 0f);
    [Header("角色移动动画机")]
    public AtlasAnimation atlasAnimation;
    //这些动画ID如果取值-1则直接无视该动画
    public int StandAnimId = 0;
    public int MoveAnimId = 1;
    public int JumpAnimId = 19;
    [Header("平A n段攻击")]
    public int[] zAttackAnimId;
    [Space(20)]
    [Header("角色效果动画机")]
    public AtlasAnimation EffectAnimation;
    /// <summary>
    /// 显示攻击效果的物体
    /// </summary>
    GameObject Effect;
    [Header("EffectAnimation中Z的效果动画ID")]
    public int[] zAttackEffectId;

    #region 状态
    /// <summary>
    /// 不允许执行站立/行走动作
    /// </summary>
    [Header("玩家状态")]
    public bool BanStandWalkAnim = false;

    /// <summary>
    /// 悬空
    /// </summary>
    public bool IsHanging = false;
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
    /// 可以二段跳
    /// </summary>
    public bool CanJumpTwice = true;

    /// <summary>
    /// Z平A连段次数
    /// </summary>
    public int ZattackCount = 0;
    /// <summary>
    /// 正在用Z攻击
    /// </summary>
   public bool IsZattacking = false;
    /// <summary>
    /// Z可以继续连段
    /// </summary>
    public bool ZattackCanGoOn = true;
    #endregion

    #region 自带组件
    [HideInInspector]public  Rigidbody2D rigidbody2D;
    [HideInInspector] public Transform tr;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    #endregion

    private void Awake()
    {
        #region 初始化组件
        rigidbody2D = GetComponent<Rigidbody2D>();
        tr = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Effect = EffectAnimation.gameObject;//显示攻击效果的物体
        #endregion

        //注册事件
        UpdateManager.FastUpdate.AddListener(FastUpdate);
        UpdateManager.FastUpdate.AddListener(RayGround);
        //允许 能够停止的动画 停止后 出现 站立动作
        atlasAnimation.AnimStop.AddListener(CheckAnimStop);

    }


    public  void FastUpdate()
    {
        //注意，所有的攻击（Z X A）以及其衍生版本（比如在天上白给）都用抽象方法来实现
        if (RebindableInput.GetKeyDown("Attack") && ZattackCanGoOn)//ZattackCanGoOn:用于阻止玩家在动画结束前再次攻击 
        {

            IsZattacking = true;
            //防止意外出现站立动作
            BanStandWalkAnim = true;
            //Z攻击
            PlayerAttackZ();
        }
        else
        {
            //不攻击的时候

}




        #region 移动
        //行走
        if (!IsZattacking)
        {
            //常规行走
            rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(RebindableInput.GetAxis("Horizontal"), 0f) * 0.1f * Speed);

            //关于按着Z键行走的说明：因为每个角色不一样，放在对应的角色脚本中写（PlayerAttackZ方法中）
        }
        //tr.Translate(new Vector2(RebindableInput.GetAxis("Horizontal"), 0) * Time.deltaTime * Speed, Space.World);

        //跳跃
        if (RebindableInput.GetKeyDown("Jump") && JumpCount < 1)//这里很迷emmm 1是二段跳 2就成三段了
        {
            CanJumpTwice = true;
            atlasAnimation.ChangeAnimation(JumpAnimId, true);//托管的动作在这里
            JumpCount++;
            JumpTimer = Time.timeSinceLevelLoad;
            IsJumping = true;
            //rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpSpeed);
        }
        else if(JumpCount >= 1)
        {
            CanJumpTwice = false;
        }
        Jump();

        #endregion



        #region 动作
        //先转向（这样子弄是为了允许不动的时候保持原朝向）
        if (RebindableInput.GetAxis("Horizontal") > 0) spriteRenderer.flipX = true;
        else if (RebindableInput.GetAxis("Horizontal") < 0) spriteRenderer.flipX = false;

        //不受是否挂在天上影响的动画

        //跳跃（放在上面跳跃移动那里）



        //没挂在天上时的动画
        if (!IsHanging)
        {

            //行走walk
            if (RebindableInput.GetAxis("Horizontal") != 0 && !BanStandWalkAnim) { atlasAnimation.ChangeAnimation(MoveAnimId); }
            else if(!BanStandWalkAnim) { atlasAnimation.ChangeAnimation(StandAnimId); }

        }

        //挂在天上用的动画
        else
        {

        }



        #endregion
    }

    /// <summary>
    /// 使用射线判断是否在地上
    /// </summary>
    public void RayGround()
    {
        Debug.DrawRay(transform.position, Vector2.down, Color.red);
        //仅对10（Ground）碰撞检测
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, 1 << 10);//10:Ground层ID


        if (hit.collider != null)
        {
            //在地上，初始化
            IsHanging = false;
            JumpCount = 0;
            CanJumpTwice = true;
        }
        else
        {
            IsHanging = true;
        }



    }

    /// <summary>
    /// 不循环的动画结束后调用这个来进行检查
    /// </summary>
    /// <param name="AnimName"></param>
    public abstract void CheckAnimStop(string AnimName);
    

    /// <summary>
    /// Z键平A
    /// </summary>
    public abstract void PlayerAttackZ();



    #region 内部方法
    void Jump()
    {
        //正在跳跃的时候才跳
        if (IsJumping)
        {
            //调整状态
            IsHanging = true;

            //防止意外出现站立动作
            BanStandWalkAnim = true;


            //起飞
            rigidbody2D.gravityScale = -40f;

            /*
            if (Time.timeSinceLevelLoad - JumpTimer   > 0.25f)
            {
                //时间到，滞空
                rigidbody2D.gravityScale = 0f;
                JumpTimer = Time.timeSinceLevelLoad;
            }*/

            if (Time.timeSinceLevelLoad - JumpTimer > 0.3f && rigidbody2D.gravityScale == -40f)
            {
                //时间到，下降
                rigidbody2D.gravityScale = 40f;
                IsJumping = false;
                //允许出现站立动作
                BanStandWalkAnim = false;


            }
            else { return; }


        }

    }


}
#endregion

