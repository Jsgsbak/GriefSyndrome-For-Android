using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;
using MEC;
using System;

[DisallowMultipleComponent]
public abstract class APlayerCtrlNoPhyX : MonoBehaviour
{
    /// <summary>
    /// 所选的魔法少女
    /// </summary>
    [Header("所选的魔法少女")]
    public Variable.PlayerFaceType SelectedMahoshaojo;

    [Header("玩家移动")]
    public float Speed = 10f;
    #region 跳跃
    public float JumpForce = 3000f;
    public float JumpSpeed = 20f;
    #endregion
    /// <summary>
    /// 向左看
    /// </summary>
    [Header("动画机")]
    readonly Quaternion LookLeft = new Quaternion(0f, 1f, 0f, 0f);
    public AtlasAnimation atlasAnimation;
    public int StandAnimId = 0;
    public int MoveAnimId = 1;

    #region 状态
    /// <summary>
    /// 悬空
    /// </summary>
    bool IsHanging = false;
    #endregion

    #region 自带组件
    Transform tr;
    SpriteRenderer spriteRenderer;
    #endregion

    private void Awake()
    {
        #region 初始化组件
        tr = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        #endregion

        //注册事件
        UpdateManager.FastUpdate.AddListener(FastUpdate);
    }


    public virtual void FastUpdate()
    {
        #region 移动
        //行走
        //rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(RebindableInput.GetAxis("Horizontal"), 0f) * 0.1f * Speed);
        tr.Translate(new Vector2(RebindableInput.GetAxis("Horizontal"), 0) * Time.deltaTime * Speed, Space.World);

        //跳跃
        if (RebindableInput.GetKeyDown("Jump"))
        {
           // rigidbody2D.AddForce(new Vector2(0, JumpForce));   //给刚体一个向上的力
        }
        #endregion

        #region 动作
        //先转向
        if (RebindableInput.GetAxis("Horizontal") > 0) spriteRenderer.flipX = true;
        else if (RebindableInput.GetAxis("Horizontal") < 0) spriteRenderer.flipX = false;
        //行走walk
        if (RebindableInput.GetAxis("Horizontal") != 0) atlasAnimation.ChangeAnimation(MoveAnimId);
        else { atlasAnimation.ChangeAnimation(StandAnimId); }
        //!!!!跳跃动作放在了Jump()中
        #endregion
    }



    /// <summary>
    /// 跳跃。目前是常规起跳
    /// </summary>
    /// <returns></returns>
    #region 内部方法

    /// <summary>
    /// 模拟重力
    /// </summary>
    void Drop()
    {
        if (IsHanging)
        {

        }
    }
    #endregion
}
