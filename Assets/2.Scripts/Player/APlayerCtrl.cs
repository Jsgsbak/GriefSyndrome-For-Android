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

    [Space]
    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidbody2D;

    /// <summary>
    /// 重力射线位置
    /// </summary>
    [Header("重力射线位置")]
    public Vector2[] GavityRayPos = { Vector2.zero, Vector2.zero };
   /// <summary>
   /// 射线显示
   /// </summary>
    Ray[] GravityRaysShow = new Ray[2];
    #endregion


    #region 组件
    [HideInInspector] Transform tr;
    #endregion
    private void Awake()
    {
        #region 获取组件
        tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
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
        if(!BanGravity) RayCtrl();
        Gravity();
        Move();
        AnimationCtrl();
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
        animator.SetBool("Walk", StageCtrl.gameScoreSettings.Horizontal != 0);
       if(StageCtrl.gameScoreSettings.Horizontal != 0) spriteRenderer.flipX = StageCtrl.gameScoreSettings.Horizontal == -1 ;
    }

    /// <summary>
    /// 射线控制器
    /// </summary>
    public void RayCtrl()
    {
        //重力射线
        if (!BanGravityRay)
        {
            GravityRaysShow[0] = new Ray(tr.InverseTransformPoint(GavityRayPos[0]), Vector2.down);
            GravityRaysShow[1] = new Ray(tr.InverseTransformPoint(GavityRayPos[0]), Vector2.down);
            Debug.DrawRay(tr.InverseTransformPoint(GavityRayPos[0]), Vector2.down, Color.red, 19f);

            BanGravity = (Physics.Raycast(GravityRaysShow[0], 0.1f, 13) && Physics.Raycast(GravityRaysShow[1], 0.1f, 13));
        }
    }
    
    public virtual void Move()
    {
        if (!StageCtrl.gameScoreSettings.UseScreenInput)
        {
            StageCtrl.gameScoreSettings.Horizontal = RebindableInput.GetAxis("Horizontal");
        }
        tr.Translate(StageCtrl.gameScoreSettings.Horizontal * Vector2.right * StageCtrl.gameScoreSettings.mahouShoujos[id].MoveSpeed * Time.deltaTime);

        /*
        if (rigidbody2D.)
        {

        }*/

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

