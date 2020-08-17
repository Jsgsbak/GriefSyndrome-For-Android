using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;
using MEC;

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
    #region 跳跃
    bool m_isOnGround;
    bool is_jump;
    bool m_jump;
    public float JumpForce;
    #endregion
    /// <summary>
    /// 向左看
    /// </summary>
    [Header("动画机")]
    readonly Quaternion LookLeft = new Quaternion(0f, 1f, 0f, 0f);
    public AtlasAnimation atlasAnimation;
    public int StandAnimId = 0;
    public int MoveAnimId = 1;

    #region 自带组件
    Rigidbody2D rigidbody2D;
    Transform tr;
    SpriteRenderer spriteRenderer;
    #endregion

    private void Awake()
    {
        #region 初始化组件
        rigidbody2D = GetComponent<Rigidbody2D>();
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
         rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(RebindableInput.GetAxis("Horizontal"),0) * 0.1f * Speed);
        //tr.Translate(new Vector2(RebindableInput.GetAxis("Horizontal"), 0) * Time.deltaTime * Speed, Space.World);
       
        //跳跃
        if (RebindableInput.GetKeyDown("Jump"))
        {
       //    Jump();
        }
        #endregion

        #region 动作
        //先转向
        if (RebindableInput.GetAxis("Horizontal") > 0) spriteRenderer.flipX = true ;
        else if (RebindableInput.GetAxis("Horizontal") < 0) spriteRenderer.flipX = false;
        //行走walk
        if (RebindableInput.GetAxis("Horizontal") != 0) atlasAnimation.ChangeAnimation(MoveAnimId);
        else { atlasAnimation.ChangeAnimation(StandAnimId); }
        #endregion
    }
}
