using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class APlayerCtrl : MonoBehaviour
{
    /// <summary>
    /// 所选的魔法少女
    /// </summary>
    [Header("所选的魔法少女")]
    public Variable.MahouShoujo mahouShoujo;

    [Header("玩家移动")]
    public float Speed = 10f;

    [Header("动画机")]
    readonly Vector3 Y180 = new Vector3(0f, 180f, 0f);
    public AtlasAnimation atlasAnimation;
    public int StandAnimId = 0;
    public int MoveAnimId = 1;

    #region 自带组件
    Rigidbody2D rigidbody2D;
    Transform tr;
    #endregion

    private void Awake()
    {
        #region 初始化组件
        rigidbody2D = GetComponent<Rigidbody2D>();
        tr = transform;
        #endregion

        //注册事件
        UpdateManager.FastUpdate.AddListener(FastUpdate);
    }


   public virtual void FastUpdate()
    {
        #region 移动
        rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(RebindableInput.GetAxis("Horizontal"),0) * Time.deltaTime * Speed);
        #endregion

        #region 动作
        //先转向
        if (RebindableInput.GetAxis("Horizontal") > 0) tr.rotation = Quaternion.identity;
        else if (RebindableInput.GetAxis("Horizontal") < 0) tr.rotation = new Quaternion(0f, 1f, 0f, 0f);

        //行走walk
        if (RebindableInput.GetAxis("Horizontal") != 0) atlasAnimation.ChangeAnimation(MoveAnimId);
        else { atlasAnimation.ChangeAnimation(StandAnimId); }
        #endregion
    }
}
