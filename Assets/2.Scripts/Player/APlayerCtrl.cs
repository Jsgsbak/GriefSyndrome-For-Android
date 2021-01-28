using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;
using MEC;
using System;

[DisallowMultipleComponent]
public abstract class APlayerCtrl : MonoBehaviour, IMove
{
    #region  基础属性
    [Header("基础属性")]

    public int MahouShoujoId = 0;
    //为了便于调试先放在这里，以后应当移动到GSS中
    public float JumpSpeed = 15f;
    /// <summary>
    /// 玩家所在斜坡的单位圆坐标（角度/三角函数那些）
    /// </summary>
    public Vector2 PlayerSlope = Vector2.right;

    [Space]
    //为了方便调试，这些Ban和Is先暂时放在这里
    public bool BanGravity = false;
    /// <summary>
    /// 禁用重力射线。用于穿透地板
    /// </summary>
    public bool BanGravityRay = false;
    /// <summary>
    /// 禁用跳跃
    /// </summary>
    public bool BanJump = false;
    public bool BanWalk = false;
    public bool IsGround = true;
    /// <summary>
    /// 禁用转身
    /// </summary>
    public bool BanTurnAround = false;
    /// <summary>
    /// 禁止输入，仅适用于按一下按键动画执行到底且确实不需要外部输入的攻击
    /// </summary>
    public bool BanInput = false;

    /// <summary>
    /// 重力射线位置
    /// </summary>
    [Header("重力射线位置")]
    public Transform[] GavityRayPos = new Transform[2];
    /// <summary>
    /// 射线显示
    /// </summary>
    Ray[] GravityRaysShow = new Ray[2];
    #endregion


    #region 组件
    [HideInInspector] public Transform tr;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    #endregion

    #region 私有状态机（不保存到GSS中）
    /// <summary>
    /// 重力射线
    /// </summary>
    Ray2D[] rays = new Ray2D[2];
    public float GravityRatio = 1f;

    public bool IsStiff = false;

    [HideInInspector] public float MoveSpeedRatio = 1f;
    /// <summary>
    /// 向右看吗
    /// </summary>
    [HideInInspector] public bool DoLookRight = true;
    public int JumpCount = 0;
    /// <summary>
    /// 跳跃间隔计时器
    /// </summary>
    float JumpInteralTimer = 0f;
    /// <summary>
    /// 正在跳跃（专指上升阶段）
    /// </summary>
    [HideInInspector] public bool IsJumping = false;
    /// <summary>
    /// 站在平台上
    /// </summary>
    public bool StandOnPlatform = false;
    /// <summary>
    /// 正在穿过平台
    /// </summary>
    [HideInInspector] public bool GoThroughPlatform = false;
    /// <summary>
    /// 穿墙瞬间的游戏时间，用于防止穿墙途中停止落体
    /// </summary>
    float PlatformTime = 0f;
    [HideInInspector] public bool IsMoving = false;
    /// <summary>
    /// 连击攻击最小间隔
    /// </summary>
    [HideInInspector] public float PressAttackInteral = 1f;
    /// <summary>
    /// 正在攻击，防止意外切换到其他攻击状态 0 z 1 x 2 Magia
    /// </summary>
    public bool[] IsAttack = new bool[3];
    /// <summary>
    /// 能否可以/已经停止攻击（中断攻击）
    /// </summary>
    [HideInInspector] public bool StopAttacking = true;
    #endregion

    private void Awake()
    {
        #region 获取组件
        tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        #endregion
    }

    private void Start()
    {
        #region 注册事件
        UpdateManager.updateManager.FastUpdate.AddListener(FastUpdate);
        #endregion
    }

    /// <summary>
    /// 输入代理
    /// </summary>
    public virtual void InputAgent()
    {
        //为了配合安卓/IOS用的RepeatButton，取消单机按钮的判定，如有需要在相应的攻击方法中加限制

        //为了防止在不同的帧运行，所以放到了这里
        //屏幕输入的按钮放在了主相机脚本里
        //这个是通常版本，有的角色（比如沙耶加）可能重写了
        if (!StageCtrl.gameScoreSettings.UseScreenInput)
        {
            StageCtrl.gameScoreSettings.Horizontal = RebindableInput.GetAxis("Horizontal");
            StageCtrl.gameScoreSettings.Jump = RebindableInput.GetKeyDown("Jump");
            StageCtrl.gameScoreSettings.Down = /* RebindableInput.GetKeyDown("Down") ||*/ RebindableInput.GetKey("Down");
            StageCtrl.gameScoreSettings.Up = /*RebindableInput.GetKeyDown("Up")  || */ RebindableInput.GetKey("Up");
            //这个的话只要按下了攻击键/按住攻击键就算
            StageCtrl.gameScoreSettings.Zattack =/* RebindableInput.GetKeyDown("Zattack") || */ RebindableInput.GetKey("Zattack");
            StageCtrl.gameScoreSettings.Xattack = /* || RebindableInput.GetKeyDown("Xattack") */RebindableInput.GetKey("Xattack");
        }

        //如果禁用了输入
        if (BanInput)
        {
            StageCtrl.gameScoreSettings.Horizontal = 0;
            StageCtrl.gameScoreSettings.Jump = false;
            StageCtrl.gameScoreSettings.Down = false;
            //这个的话只要按下了攻击键/按住攻击键就算
            StageCtrl.gameScoreSettings.Zattack = false;
            StageCtrl.gameScoreSettings.Xattack = false;

        }
    }


    public void FastUpdate()
    {
        if (!BanGravity) Gravity();
        RayCtrl();

        if (IsStiff)
        {
            return;
        }

        //还是以最高优先级执行输入代理
        InputAgent();

        #region  基础控制器
        Walk();
        AnimationCtrl();
        JumpAndFall();
        #endregion


        #region 攻击方法
        //这里都是抽象的，在各自的脚本里重写（包括攻击逻辑与攻击动画）
        if (StageCtrl.gameScoreSettings.Jump)
        {
            //修复攻击过程中跳跃仍然显示攻击动画的bug
            return;
        }

        //前面的!IsAttack[1]是为了防止做这个攻击的时候意外发动其他的攻击
        //这里加限制条件/修改状态要三思，主要是在抽象的方法里更改和限制
        if (!IsAttack[1] && !IsAttack[2]) { OrdinaryZ(); HorizontalZ(); VerticalZ(); }
        if (!IsAttack[0] && !IsAttack[2]) { OrdinaryX(); HorizontalX(); UpX(); DownX(); }

        BanWalk = IsAttack[0] || IsAttack[1] || IsAttack[2] || StageCtrl.gameScoreSettings.Zattack || StageCtrl.gameScoreSettings.Magia || StageCtrl.gameScoreSettings.Xattack;//在这里统一弄一个，直接在这里禁用移动，不再在各种攻击方法和动画事件中禁用了
        BanJump = IsAttack[0] || IsAttack[1] || IsAttack[2] || StageCtrl.gameScoreSettings.Zattack || StageCtrl.gameScoreSettings.Magia || StageCtrl.gameScoreSettings.Xattack;//在这里统一弄一个，直接在这里禁用移动，不再在各种攻击方法和动画事件中禁用了

        Magia();
        #endregion

    }

    #region  基础控制器
    /// <summary>
    /// 动画控制器（攻击用动画与攻击逻辑放在了一起）
    /// </summary>
    public void AnimationCtrl()
    {
        if (!StopAttacking)
        {
            return;
        }

        //左右翻转
        if (!BanTurnAround)
        {
            if (StageCtrl.gameScoreSettings.Horizontal == -1) { tr.rotation = Quaternion.Euler(0f, 180f, 0f); DoLookRight = false; } //
            else if (StageCtrl.gameScoreSettings.Horizontal == 1) { tr.rotation = Quaternion.Euler(0f, 0f, 0f); DoLookRight = true; }//spriteRenderer.flipX = StageCtrl.gameScoreSettings.Horizontal == -1;//
        }

        //现在根据状态机启用对应动画
        animator.SetBool("Walk", StageCtrl.gameScoreSettings.Horizontal != 0);
        animator.SetBool("Jump", IsJumping);
        animator.SetBool("Fall", !IsGround && !BanGravity && !IsJumping);
    }

    /// <summary>
    /// 跳跃和下降（落体或者从地板掉下来，地板的Layer是Wall）
    /// </summary>
    public void JumpAndFall()
    {
        //除了禁用跳跃的情况，攻击的时候也不能跳（单独写出来，不然太麻烦了）
        if (BanJump || StageCtrl.gameScoreSettings.Zattack || StageCtrl.gameScoreSettings.Xattack) return;

        //跳跃触发
        if (!StageCtrl.gameScoreSettings.Down && StageCtrl.gameScoreSettings.Jump && Mathf.Abs(JumpInteralTimer - Time.timeSinceLevelLoad) > 0.2F && JumpCount != 2)
        {
            MoveSpeedRatio = 1f;
            JumpInteralTimer = Time.timeSinceLevelLoad;
            IsJumping = true;
            JumpCount++;
            BanGravity = true;
        }
        //跳跃状态
        if (IsJumping)
        {
            //上升
            if (Time.timeSinceLevelLoad - JumpInteralTimer <= 0.2f)
            {
                //解决一个很奇怪的BUG
                IsGround = false;
                tr.Translate(Vector3.up * JumpSpeed * Time.deltaTime * JumpInteralTimer / Time.timeSinceLevelLoad);
            }
            //下降（其实就是取消跳跃状态）
            else
            {
                BanGravity = false;
                IsJumping = false;
            }
        }

        //跳跃计数器更新
        if (IsGround) JumpCount = 0;

        //穿过平台
        if (StageCtrl.gameScoreSettings.Down && StandOnPlatform && StageCtrl.gameScoreSettings.Jump)
        {
            BanGravity = false;
            //禁用重力射线，防止中途停止落体
            BanGravityRay = true;
            IsJumping = false;
            IsGround = false;
            GoThroughPlatform = true;
            StandOnPlatform = false;//取消，防止多次执行
            PlatformTime = Time.timeSinceLevelLoad;
        }
        if (GoThroughPlatform && Time.timeSinceLevelLoad - PlatformTime >= 0.1f)//0.2s大约射线能穿过平台
        {
            //一定时间之后启用重力射线判定，防止穿墙途中停止落体
            GoThroughPlatform = false;
            BanGravityRay = false;
            BanGravity = false;
        }

    }

#if UNITY_EDITOR
    bool PreviousFrameHasCancelledJump = false;
    float Timerrrr = 0;
#endif

    /// <summary>
    /// （千万不要多次重复执行！！！）对于正在跳跃过程中发动魔法/攻击的情况，直接取消跳跃状态
    /// </summary>
    public void CancelJump()
    {
#if UNITY_EDITOR

        Debug.Log("取消跳跃");

        //如果上一帧执行过了
        if (PreviousFrameHasCancelledJump && Time.timeSinceLevelLoad - Timerrrr <= 0.1f)
        {
            Debug.LogError("CancelJump多次重复执行，你也不怕出bug");
        }
        else if (PreviousFrameHasCancelledJump && Time.timeSinceLevelLoad - Timerrrr > 0.1f)
        {
            PreviousFrameHasCancelledJump = false;
        }
        else if (!PreviousFrameHasCancelledJump)
        {
            Timerrrr = Time.timeSinceLevelLoad;
            PreviousFrameHasCancelledJump = true;
        }
#endif

        if (!IsJumping)
        {
            return;
        }
        //既然要取消，那肯定是跳起来了
        animator.SetBool("Jump", false);
        BanGravity = false;
        IsGround = false;
        IsJumping = false;
    }

    /// <summary>
    /// 射线控制器
    /// </summary>
    public void RayCtrl()
    {
        //重力射线
        if (!BanGravityRay)
        {
            rays[0] = new Ray2D(GavityRayPos[0].position, Vector2.down * 0.02f);
            rays[1] = new Ray2D(GavityRayPos[1].position, Vector2.down * 0.02f);
            RaycastHit2D infoLeft = Physics2D.Raycast(rays[1].origin, rays[1].direction, 0.02f);
            RaycastHit2D infoRight = Physics2D.Raycast(rays[0].origin, rays[0].direction, 0.02f);

            Debug.DrawRay(rays[0].origin, rays[0].direction * 0.02f, Color.blue);
            Debug.DrawRay(rays[1].origin, rays[1].direction * 0.02f, Color.blue);

            //在地上
            if (infoLeft.collider != null)// || infoRight.collider != null)
            {
                StandOnPlatform = infoLeft.collider.CompareTag("Platform");
                BanGravity = infoLeft.collider.CompareTag("Floor") || infoLeft.collider.CompareTag("Platform");
                IsGround = infoLeft.collider.CompareTag("Floor") || infoLeft.collider.CompareTag("Platform");
            }
            else if (infoRight.collider != null)// || infoRight.collider != null)
            {
                StandOnPlatform = infoRight.collider.CompareTag("Platform");
                BanGravity = infoRight.collider.CompareTag("Floor") || infoRight.collider.CompareTag("Platform");
                IsGround = infoRight.collider.CompareTag("Floor") || infoRight.collider.CompareTag("Platform");
            }
            //啥也没才上，腾空
            else if (!IsJumping)//去除跳跃的情况
            {
                BanGravity = false;
                IsGround = false;
                StandOnPlatform = false;
            }

        }
    }

    /// <summary>
    /// 普通的行走用
    /// </summary>
    void Walk()
    {
        if (BanWalk)
        {
            return;
        }

        IsMoving = StageCtrl.gameScoreSettings.Horizontal != 0;
        Move(StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].MoveSpeed, true, PlayerSlope.normalized, Vector2.right * StageCtrl.gameScoreSettings.Horizontal, Space.World);
        // tr.Translate(StageCtrl.gameScoreSettings.Horizontal * Vector2.right * StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].MoveSpeed * MoveSpeedRatio * Time.deltaTime, Space.World);


        /*MD我真服了，这个Bug令人恶心
        if (StageCtrl.gameScoreSettings.UseScreenInput)
        {
            tr.Translate(StageCtrl.gameScoreSettings.Horizontal * Vector2.right * StageCtrl.gameScoreSettings.mahouShoujos[id].MoveSpeed * Time.deltaTime);
        }
        else
        {
            tr.Translate(RebindableInput.GetAxis("Horizontal") * Vector2.right * StageCtrl.gameScoreSettings.mahouShoujos[id].MoveSpeed * Time.deltaTime);
        }*/
    }

    public void Move(float Speed, bool UseTimeDelta, Vector2 Slope, Vector2 Direction, Space space = Space.Self)
    {

        //缺少：PlayerSlope计算（判断），左右版边限制移动

        if (UseTimeDelta)
        {
            tr.Translate(Direction * Slope * Speed * MoveSpeedRatio * Time.deltaTime, space);
        }
        else
        {
            tr.Translate(Direction * Slope * Speed * MoveSpeedRatio, space);
        }
    }

    /// <summary>
    /// 下落重力
    /// </summary>
    public void Gravity()
    {
        if (!BanGravity)
        {
            tr.Translate(Vector2.down * 9.8f * GravityRatio * Time.deltaTime, Space.World);

        }
    }
    #endregion

    #region 僵直
    /// <summary>
    /// 设置僵直
    /// </summary>
    /// <param name="Time">僵直事件</param>
    public virtual void Stiff(float Time)
    {
        //取消以前的僵直（仅仅是换成另一个僵直，并不是取消将至）
        StopCoroutine("PlayerStiff");
        //僵直状态
        StopAttacking = false;
        BanGravity = IsGround;
        GravityRatio = 1F;
        MoveSpeedRatio = 1F;
        BanGravityRay = false;
        animator.enabled = !true;
        BanInput = !false;//这一个就够了
        IsStiff = !false;

        //启用新的僵直
        StartCoroutine("PlayerStiff", Time);

    }
    /// <summary>
    /// 这里经常初BUG
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
   public virtual IEnumerator PlayerStiff(float d)
    {

        yield return new WaitForSeconds(d);

        //状态恢复
        BanWalk = false;
        StopAttacking = !false;
        BanGravity = IsGround;
        GravityRatio = 1F;
        MoveSpeedRatio = 1F;
        BanGravityRay = false;
        BanTurnAround = false;
        BanJump = false;
        BanInput = false;
        animator.enabled = true;
        IsStiff = false;

    }
    #endregion


    #region 攻击方法
    /// <summary>
    /// 普通Z攻击，又名Zattack
    /// </summary>
    public abstract void OrdinaryZ();
    public abstract void HorizontalZ();
    public abstract void VerticalZ();

    public abstract void OrdinaryX();
    public abstract void HorizontalX();
    public abstract void UpX();
    public abstract void DownX();

    public abstract void Magia();

    /// <summary>
    /// Z攻击动画事件
    /// </summary>
    /// <param name="AnimationName"></param>
    public abstract void ZattackAnimationEvent(string AnimationName);
    /// <summary>
    /// 通常X攻击动画逻辑
    /// </summary>
    /// <param name="AnimationName"></param>
    public abstract void XattackAnimationEvent(string AnimationName);

    /// <summary>
    /// 水平X攻击动画逻辑
    /// </summary>
    /// <param name="AnimationName"></param>
    public abstract void HorizontalXattackAnimationEvent(string AnimationName);

    /// <summary>
    /// 上X攻击动画逻辑
    /// </summary>
    /// <param name="AnimationName"></param>
    public abstract void UpXattackAnimationEvent(string AnimationName);
    public abstract void DownXattackAnimationEvent(string AnimationName);

    public abstract void MagiaAnimationEvent(string AnimationName);
    #endregion
}

