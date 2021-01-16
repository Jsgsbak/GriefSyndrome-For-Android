using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;
using MEC;
using System;

[DisallowMultipleComponent]
public abstract class APlayerCtrl:MonoBehaviour
{
    #region  基础属性
    [Header("基础属性")]
    public int id = 0;
    public bool BanGravity = false;
    /// <summary>
    /// 禁用重力射线。用于穿透地板
    /// </summary>
    public bool BanGravityRay = false;
    /// <summary>
    /// 禁用跳跃
    /// </summary>
    public bool BanJump = false;
    public bool IsGround = true;

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
    Transform tr;
    Animator animator;
    SpriteRenderer spriteRenderer;
    #endregion

    #region 私有状态机（不保存到GSS中）
    /// <summary>
    /// 重力射线
    /// </summary>
    Ray2D[] rays = new Ray2D[2];
    int JumpCount = 0;
    /// <summary>
    /// 跳跃间隔计时器
    /// </summary>
    float JumpInteralTimer = 0f;
    /// <summary>
    /// 正在跳跃（专指上升阶段）
    /// </summary>
    bool IsJumping = false;
    /// <summary>
    /// 站在平台上
    /// </summary>
  public   bool StandOnPlatform = false;
    /// <summary>
    /// 正在穿过平台
    /// </summary>
    bool GoThroughPlatform = false;
    /// <summary>
    /// 穿墙瞬间的游戏时间，用于防止穿墙途中停止落体
    /// </summary>
    float PlatformTime = 0f;
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

    public virtual void FastUpdate()
    {
        #region  基础控制器
         RayCtrl();
        if(!BanGravity)Gravity();
        Move();
        AnimationCtrl();
        JumpAndFall();
        #endregion


        #region 攻击方法
        OrdinaryZ();HorizontalZ();VerticalZ();
        OrdinaryX();VerticalX();HorizontalX();
        Magia();
        #endregion
    }

    #region  基础控制器
    /// <summary>
    /// 动画控制器
    /// </summary>
    public void AnimationCtrl()
    {
        //左右翻转
        if (StageCtrl.gameScoreSettings.Horizontal == -1) tr.rotation = Quaternion.Euler(0f, 180f, 0f); //
        else if (StageCtrl.gameScoreSettings.Horizontal == 1) { tr.rotation = Quaternion.Euler(0f, 0f, 0f); }//spriteRenderer.flipX = StageCtrl.gameScoreSettings.Horizontal == -1;//

        //最低优先级
        animator.SetBool("Walk", StageCtrl.gameScoreSettings.Horizontal != 0);
        //跳跃动作（专指上升阶段）
        if (IsJumping)
        {
            animator.SetBool("Jump", true);
            animator.SetBool("Fall", false);
        }
        //下落动作
        if (!IsGround && !BanGravity && !IsJumping)
        {
            animator.SetBool("Jump", false);
            animator.SetBool("Fall",true);
        }
        //地面待机
        if(IsGround && BanGravity && !IsJumping)
        {
            animator.SetBool("Fall", false);
            animator.SetBool("Jump", false);
        }

    }

    /// <summary>
    /// 跳跃和下降（落体或者从地板掉下来，地板的Layer是Wall）
    /// </summary>
    public void JumpAndFall()
    {
        if (BanJump) return;

        //按下跳跃键
        if (!StageCtrl.gameScoreSettings.UseScreenInput)
        {
            StageCtrl.gameScoreSettings.Jump = RebindableInput.GetKeyDown("Jump");
        }
        //跳跃触发
        if(StageCtrl.gameScoreSettings.Jump && Mathf.Abs( JumpInteralTimer - Time.timeSinceLevelLoad) > 0.2F && JumpCount != 1)
        {
            JumpInteralTimer = Time.timeSinceLevelLoad;
            IsJumping = true;
            JumpCount++;
            BanGravity = true;
        }
        //跳跃状态
        if (IsJumping)
        {
            //上升
            if(Time.timeSinceLevelLoad - JumpInteralTimer <= 0.2f)
            {
                tr.Translate(Vector3.up * 15f * Time.deltaTime  * JumpInteralTimer/Time.timeSinceLevelLoad);
            }
            //下降（其实就是取消跳跃状态）
            else
            {
                BanGravity = false ;
                IsJumping = false;
            }
        }

        //跳跃计数器更新
        if (IsGround) JumpCount = 0;

        //穿过平台
        if (!StageCtrl.gameScoreSettings.UseScreenInput)
        {
            StageCtrl.gameScoreSettings.Down = RebindableInput.GetKeyDown("Down");
        }
        if(StageCtrl.gameScoreSettings.Down && StandOnPlatform)
        {
            BanGravity = false;
            //禁用重力射线，防止中途停止落体
            BanGravityRay = true;
            IsJumping = false;
            GoThroughPlatform = true;
            StandOnPlatform = false;//取消，防止多次执行
            PlatformTime = Time.timeSinceLevelLoad;
        }
        if (GoThroughPlatform && Time.timeSinceLevelLoad - PlatformTime >= 0.1f)
        {
            //一定时间之后启用重力射线判定，防止穿墙途中停止落体
            GoThroughPlatform = false;
            BanGravityRay = false;
            BanGravity = false;
        }

    }

    /// <summary>
    /// 射线控制器
    /// </summary>
    public void RayCtrl()
    {
        //重力射线
        if (!BanGravityRay)
        {
            rays[0] = new Ray2D(GavityRayPos[0].position, Vector2.down * 0.05f);
            rays[1] = new Ray2D(GavityRayPos[1].position, Vector2.down * 0.05f);
            RaycastHit2D infoLeft = Physics2D.Raycast(rays[1].origin, rays[1].direction, 0.05f);
            RaycastHit2D infoRight = Physics2D.Raycast(rays[0].origin, rays[0].direction, 0.05f);

            Debug.DrawRay(rays[0].origin, rays[0].direction, Color.blue);
            Debug.DrawRay(rays[1].origin, rays[1].direction, Color.blue);


            //在地上
            if (infoLeft.collider != null)// || infoRight.collider != null)
            {
                Debug.Log(infoLeft.collider.tag);

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
            else if(!IsJumping)//去除跳跃的情况
            {
                BanGravity = false;
                IsGround = false;
                StandOnPlatform = false;
            }

        }
    }
    
    public virtual void Move()
    {
        if (!StageCtrl.gameScoreSettings.UseScreenInput)
        {
            StageCtrl.gameScoreSettings.Horizontal = RebindableInput.GetAxis("Horizontal");
        }
        tr.Translate(StageCtrl.gameScoreSettings.Horizontal * Vector2.right * StageCtrl.gameScoreSettings.mahouShoujos[id].MoveSpeed * Time.deltaTime,Space.World);

        

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
    /// <summary>
    /// 下落重力
    /// </summary>
    public void Gravity()
    {
        if (!BanGravity)
        {
            tr.Translate( Vector2.down * 9.8f * Time.deltaTime,Space.World);

        }
    }
    #endregion



    #region 攻击方法
    public abstract void OrdinaryZ();
    public abstract void HorizontalZ();
    public abstract void VerticalZ();

    public abstract void OrdinaryX();
    public abstract void HorizontalX();
    public abstract void VerticalX();

    public abstract void Magia();
    #endregion
}

