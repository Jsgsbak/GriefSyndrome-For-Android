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
    public int HurtAnimId;
    public Sprite ForBodyDie;
    [Header("平A n段攻击")]
    public int[] zAttackAnimId;
    [Space(20)]
    [Header("角色效果动画机")]
    public AtlasAnimation EffectAnimation;
    public SpriteRenderer EffectRenderer;
    /// <summary>
    /// 显示攻击效果的物体
    /// </summary>
  [HideInInspector] public GameObject Effect;
    [Header("EffectAnimation中Z的效果动画ID")]
    public int[] zAttackEffectId;

    #region 玩家信息
    /// <summary>
    /// 所选的魔法少女
    /// </summary>
    [Header("所选的魔法少女")]
    public Variable.PlayerFaceType SelectedMahoshaojo;
    /// <summary>
    /// 人物等级
    /// </summary>
    [HideInInspector] public int Level = 1;
    /// <summary>
    /// 灵魂值
    /// </summary>
    [HideInInspector] public int SoulLimit;
    /// <summary>
    /// HP
    /// </summary>
    [HideInInspector] public int Vit;
    /// <summary>
    /// 攻击力
    /// </summary>
   [HideInInspector] public int Pow;
    /// <summary>
    /// 回复消耗的Soul Limit关于损失Vit的倍数
    /// </summary>
    [HideInInspector] public int Recovery = 18;
    /// <summary>
    /// 复活消耗的Soul Limit关于损失Vit最大值的倍数
    /// </summary>
    [HideInInspector] public int Rebirth = 30;
    /// <summary>
    /// 发动时Maiga消耗Vit数
    /// </summary>
    [HideInInspector] public int MaigaVit = 45;

    #endregion


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
    /// <summary>
    /// 被攻击
    /// </summary>
    public bool IsHurt = false;
    /// <summary>
    /// 玩家身体死亡
    /// </summary>
    public bool IsBodyDie = false;


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

    private void Start()
    {
        //根据已有数据获取玩家信息
    }


    public  void FastUpdate()
    {
        if (IsBodyDie || IsHurt)
        {
            //如果玩家死亡，直接返回，不接受后续处理
            return;
        }



        //注意，所有的攻击（Z X A）以及其衍生版本（比如在天上白给）都用抽象方法来实现
        if (RebindableInput.GetKeyDown("Attack") && ZattackCanGoOn)//ZattackCanGoOn:用于阻止玩家在动画结束前再次攻击 
        {

            IsZattacking = true;
            //防止意外出现站立动作
            BanStandWalkAnim = true;
            //Z攻击
            PlayerAttackZ();
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



        #region 动画
        //先转向（这样子弄是为了允许不动的时候保持原朝向）
        if (RebindableInput.GetAxis("Horizontal") > 0) { spriteRenderer.flipX = true; EffectRenderer.flipX = true; }
        else if (RebindableInput.GetAxis("Horizontal") < 0) { spriteRenderer.flipX = false; EffectRenderer.flipX = false; }

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

            //如果身体死了，并且接触到了地面，换上死亡贴图
           if(IsBodyDie) spriteRenderer.sprite = ForBodyDie;
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

    /// <summary>
    /// 收上
    /// </summary>
    /// <param name="collision"></param>
    [ContextMenu("受伤")]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            IsHurt = true;
            atlasAnimation.ChangeAnimation(HurtAnimId, true);

            //!!!!!!测试用
            rigidbody2D.AddForce(new Vector2(1f, 1f) * 40f);
            BodyDie();

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        IsHurt = false;
      
        

    }

    /// <summary>
    /// 身体挂了
    /// </summary>
    public void BodyDie()
    {
        IsBodyDie = true;
        //动画在射线那里

    }

    #region 内部方法

    public void GetExperience(int exp)
    {

    }
    public void LevelUp()
    {

    }


    /// <summary>
    /// 玩家僵直
    /// </summary>
    /// <param name="Time">僵直事件</param>
    public IEnumerator<float> JiangZhi(float Time)
    {
        EffectAnimation.PauseAnimation();
        atlasAnimation.PauseAnimation();
        yield return Timing.WaitForSeconds(Time);
        EffectAnimation.ContinueAnimation();
        atlasAnimation.ContinueAnimation();

    }

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

