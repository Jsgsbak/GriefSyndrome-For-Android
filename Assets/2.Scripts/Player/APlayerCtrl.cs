using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;
using MEC;
using System;
//真 屎山代码 警告
//多人游戏玩家需要同步的信息：PlayerStatus， 输入
[DisallowMultipleComponent]
public abstract class APlayerCtrl : MonoBehaviour
{
    #region  基础属性
    /// <summary>
    /// 123玩家是哪一个
    /// </summary>
    public int PlayerId { private set; get; }

    /// <summary>
    /// 选的哪一个角色
    /// </summary>
    public Variable.PlayerFaceType MahouShoujoType;



    bool _banJump = false;
    /// <summary>
    /// 禁用跳跃
    /// </summary>
    public bool BanJump
    {
        set
        {
            //禁用输入的话，是一定禁止走动的
            if (MountGSS.gameScoreSettings.BanInput)
            {
                _banJump = true;
            }
            else
            {
                _banJump = value;
            }
        }
        get
        {
            //禁用输入的话，是一定禁止走动的
            if (MountGSS.gameScoreSettings.BanInput)
            {
                return true;
            }
            else
            {
                return _banJump;
            }
        }
    }



    bool _banWalk = false;
    public bool BanWalk
    {
        set
        {
            //禁用输入的话，是一定禁止走动的
            if (MountGSS.gameScoreSettings.BanInput)
            {
                _banWalk = true;
            }
            else
            {
                _banWalk = value;
            }
        }
        get
        {
            //禁用输入的话，是一定禁止走动的
            if (MountGSS.gameScoreSettings.BanInput)
            {
                return true;
            }
            else
            {
                return _banWalk;
            }
        }
    }


    bool _banTurnAround = false;
    /// <summary>
    /// 禁用转身
    /// </summary>
    public bool BanTurnAround
    {
        set
        {
            //禁用输入的话，是一定禁止走动的
            if (MountGSS.gameScoreSettings.BanInput)
            {
                _banTurnAround = true;
            }
            else
            {
                _banTurnAround = value;
            }
        }
        get
        {
            //禁用输入的话，是一定禁止走动的
            if (MountGSS.gameScoreSettings.BanInput)
            {
                return true;
            }
            else
            {
                return _banTurnAround;
            }
        }
    }

    /// <summary>
    /// 斜坡参数
    /// </summary>
    public float PlayerSlopeAngle;
    /// <summary>
    /// 脚底向下的射线
    /// </summary>
    RaycastHit2D FeetDown;

    /// <summary>
    /// 无敌状态
    /// </summary>
    [HideInInspector] public bool IsInvincible = false;

    [HideInInspector] public float MoveSpeedRatio = 1f;

    /// <summary>
    /// 是否在地面
    /// </summary>
    public bool IsGround { get { return StandOnFloor || StandOnPlatform; }}
    /// <summary>
    /// 脚下是啥
    /// </summary>
    public int WhatUnderFoot {  get; private set; }


    public Transform Feet;

    /// <summary>
    /// 相机顶，防止玩家向上移动超出视野
    /// </summary>
    public Transform Roof;
    #endregion



    #region 私有状态机（不保存到GSS中）


    /// <summary>
    /// 玩家状态（仅应用于动画机）
    /// </summary>
    public Variable.PlayerStatus PlayerStatus;

    bool _doLookRight = true;
    /// <summary>
    /// 向右看吗
    /// </summary>
    public bool DoLookRight
    {
        get { return _doLookRight; }
        private set { _doLookRight = value; }
    }




    /// <summary>
    /// 跳跃次数
    /// </summary>
    public int JumpCount { private set; get; }
    /// <summary>
    /// 正在竖直向上跳跃
    /// </summary>
    public bool IsJumping { private set; get; }

    float FallFromPlatformTime;

    /// <summary>
    /// 正在向前跳（仅对有向前跳动画的角色有用）
    /// </summary>
    public bool IsJumpingForward { private set; get; }

    [SerializeField] bool _standOnPlatform;
    /// <summary>
    /// 站在平台上
    /// </summary>
    public bool StandOnPlatform { get { return _standOnPlatform; } private set { _standOnPlatform = value; } }

    [SerializeField] bool _standOnFloor;
    /// <summary>
    /// 站在地板上
    /// </summary>
    public bool StandOnFloor { get { return _standOnFloor; } private set { _standOnFloor = value; } }

    /// <summary>
    /// 正在攻击，防止意外切换到其他攻击状态 0 z 1 x 2 Magia
    /// </summary>
    public bool[] IsAttack = new bool[3];

    /// <summary>
    /// 能否可以/已经停止攻击（中断攻击）
    /// </summary>
    [HideInInspector] public bool StopAttacking = true;

    /// <summary>
    /// 播放玩家死亡第二阶段动画
    /// </summary>
    public bool PlayPlayerDie2 = false;
    /// <summary>
    /// 播放玩家死亡第三阶段动画
    /// </summary>
    public bool PlayPlayerDie3 = false;

    #endregion

    #region 组件
    Transform tr;
    Animator animator;
    SpriteRenderer spriteRenderer;
    Material Material;
    Rigidbody2D rigidbody2D;
    GameObject go;
    #endregion


    private void Awake()
    {
        #region 获取组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Material = spriteRenderer.material;
        tr = transform;
        rigidbody2D = GetComponent<Rigidbody2D>();
        go = gameObject;
        #endregion

        //初始化组件
        rigidbody2D.drag = 0f;
        rigidbody2D.mass = 1f;
        SetGravityRatio(1f);
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody2D.simulated = true;
    }

    private void Start()
    {
        UpdateManager.updateManager.FastUpdate.AddListener(FastUpdate);

        //影子魔女的话开启黑色描边效果
        if (MountGSS.gameScoreSettings.BattlingMajo == Variable.Majo.ElsaMaria)
        {
            Material.EnableKeyword("OUTBASE_ON");
            Material.EnableKeyword("GREYSCALE_ON");
        }
        else
        {
            Material.DisableKeyword("OUTBASE_ON");
            Material.DisableKeyword("GREYSCALE_ON");
        }


        //更新玩家信息
        UpdateInf(true);

        //玩家属性每秒的变化
        InvokeRepeating(nameof(PerSecondChange), 1f, 1f);

        //保存本地玩家选择的魔法少女的魔法少女id
        MountGSS.gameScoreSettings.PlayerSelectedGirlId = (int)MahouShoujoType;

        //获取PlayerId（ 1 2 3 哪一个）
        for (int i = 0; i < 3; i++)
        {
            if (MountGSS.gameScoreSettings.SelectedGirlInGame[i] == MahouShoujoType)
            {
                PlayerId = i;
                break;
            }
        }
        //记录玩家初始化的位置
        MountGSS.gameScoreSettings.PlayersPosition[PlayerId] = tr.position;

        //设置tag
        tag = string.Format("Player{0}", (PlayerId + 1).ToString());

        //修正玩家层（设置为Player，通常玩家层）
        gameObject.layer = 8;

#if UNITY_EDITOR
        //方便调试罢了
        //计算动画hash值
        int ii = 0;
        foreach (var item in Enum.GetNames(typeof(Variable.PlayerStatus)))
        {
            GameScoreSettingsIO.AnimationHash[ii] = Animator.StringToHash(item);
            ii++;
        }
#endif



    }






    /// <summary>
    /// 输入代理（只有跳跃Jump是Down)
    /// </summary>
    void InputAgent()
    {
        //为了配合安卓/IOS用的RepeatButton，取消单机按钮的判定，如有需要在相应的攻击方法中加限制
        //安卓是直接作用于MountGSS.gameScoreSettings.Horizontal
        //为了防止在不同的帧运行，所以放到了这里
        //屏幕输入的按钮放在了主相机脚本里
        //这个是通常版本，有的角色（比如沙耶加）可能重写了
        if (MountGSS.gameScoreSettings.UseScreenInput == 0)
        {
            MountGSS.gameScoreSettings.Horizontal = RebindableInput.GetAxis("Horizontal");
            MountGSS.gameScoreSettings.Jump = RebindableInput.GetKeyDown("Jump");
            MountGSS.gameScoreSettings.Down = RebindableInput.GetKey("Down");
            MountGSS.gameScoreSettings.Up = RebindableInput.GetKey("Up");
            MountGSS.gameScoreSettings.Zattack = RebindableInput.GetKeyDown("Zattack");
            MountGSS.gameScoreSettings.ZattackPressed = RebindableInput.GetKey("Zattack");
            MountGSS.gameScoreSettings.Xattack = RebindableInput.GetKeyDown("Xattack");
            MountGSS.gameScoreSettings.XattackPressed = RebindableInput.GetKey("Xattack");
            MountGSS.gameScoreSettings.Magia = RebindableInput.GetKeyDown("Magia");
            MountGSS.gameScoreSettings.MagiaPressed = RebindableInput.GetKey("Magia");
            MountGSS.gameScoreSettings.Pause = RebindableInput.GetKeyDown("Pause");

        }

        //如果禁用了输入/时间挺停止
        if (MountGSS.gameScoreSettings.BanInput | Time.timeScale == 0)
        {
            MountGSS.gameScoreSettings.Horizontal = 0;
            MountGSS.gameScoreSettings.Jump = false;
            MountGSS.gameScoreSettings.Down = false;
            MountGSS.gameScoreSettings.Zattack = false;
            MountGSS.gameScoreSettings.Xattack = false;
            MountGSS.gameScoreSettings.Magia = false;
        }
    }


    void FastUpdate()
    {

        //这里的话，多人游戏要改改，总不能所有的远程玩家都不进行处理吧
        if (!MountGSS.gameScoreSettings.LocalIsStiff)
        {
            //以最高优先级执行输入代理
            InputAgent();


            //左右翻转
            if (!BanTurnAround)
            {
                if (MountGSS.gameScoreSettings.Horizontal == -1 && DoLookRight)
                {
                    tr.rotation = Quaternion.Euler(0f, 180f, 0f);
                    DoLookRight = false;

                }
                else if (MountGSS.gameScoreSettings.Horizontal == 1 && !DoLookRight)
                {
                    tr.rotation = Quaternion.Euler(0f, 0f, 0f);
                    DoLookRight = true;
                }
            }



            #region  基础控制器
            JumpAndFall();
            if (!BanWalk) Walk();
            if (!IsJumping && !IsJumpingForward && go.layer != 7) RayCtrl();
            SetStatus();
            if (!MountGSS.gameScoreSettings.LocalIsStiff) AnimationCtrl();

            #region 攻击方法
            //防止死亡状态、按下跳跃的瞬间发动攻击
            if (MountGSS.gameScoreSettings.Jump || MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] || IsInvincible)
            {
                //修复攻击过程中跳跃仍然显示攻击动画的bug
                return;
            }


            //前面的!IsAttack[1]是为了防止做这个攻击的时候意外发动其他的攻击
            //这里加限制条件/修改状态要三思，主要是在抽象的方法里更改和限制
            //对于玩家来说，除了跳跃键，其他的都是能够接受长时间按住的
            if (!IsAttack[1] && !IsAttack[2] && !MountGSS.gameScoreSettings.Xattack && !MountGSS.gameScoreSettings.XattackPressed) { OrdinaryZ(); HorizontalZ(); VerticalZ(); }
            if (!IsAttack[0] && !IsAttack[2] && !MountGSS.gameScoreSettings.Zattack && !MountGSS.gameScoreSettings.ZattackPressed) { OrdinaryX(); HorizontalX(); UpX(); DownX(); }
            //magia对VIT/血条的处理在各自的脚本里  限制vit有bug   松开魔法键之后仍然会执行魔法
            if (!IsAttack[0] && !IsAttack[1] && /*MountGSS.gameScoreSettings.Magia &&*/ MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] > MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].MaigaVit | IsAttack[2]) { Magia(); }

            BanWalk = IsAttack[0] || IsAttack[1] || IsAttack[2] || MountGSS.gameScoreSettings.Zattack || MountGSS.gameScoreSettings.Magia || MountGSS.gameScoreSettings.Xattack;//在这里统一弄一个，直接在这里禁用移动，不再在各种攻击方法和动画事件中禁用了
            BanJump = IsAttack[0] || IsAttack[1] || IsAttack[2] || MountGSS.gameScoreSettings.Zattack || MountGSS.gameScoreSettings.Magia || MountGSS.gameScoreSettings.Xattack;//在这里统一弄一个，直接在这里禁用移动，不再在各种攻击方法和动画事件中禁用了

            #endregion

        }

        #endregion




    }

    private void RayCtrl()
    {

        //脚底向下发射一个射线
        FeetDown = Physics2D.Raycast(Feet.position, Vector2.down,1F, (1 << 9) | (1 << 13) | (1 << 14));
        Collider2D collider2D = FeetDown.collider;

#if UNITY_EDITOR
        Debug.DrawRay(Feet.position, Vector2.down, Color.green);
#endif

        if(collider2D == null)
        {
            StandOnFloor = false;
            StandOnPlatform = false;
            return;
        }
        //脚底踩地面
        if (collider2D.CompareTag("Platform") || collider2D.CompareTag("Floor"))
        {
            StandOnFloor = collider2D.CompareTag("Floor"); 
            StandOnPlatform = collider2D.CompareTag("Platform");
            PlayerSlopeAngle =  ExMath.Deg2Rad(Vector2.Angle(Vector2.down, FeetDown.normal));
        }
        //不在地上
        else if (collider2D.CompareTag("Platform") && collider2D.CompareTag("Floor"))
        {
            StandOnFloor = false;
            StandOnPlatform = false;
        }



    }

    #region  基础控制器


    /// <summary>
    /// 设置玩家状态（状态机）
    /// </summary>
    public void SetStatus()
    {


        if (IsAttack[0] || IsAttack[1] || IsAttack[2])
        {
            return;
        }

        //通过对一些状态变量的判断，得出当前玩家的状态，并应用于动画

        //基础的在这里写，攻击的在各自玩家脚本中重写
        if (MountGSS.gameScoreSettings.Horizontal == 0 && IsGround && !IsJumping && !MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] && !MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId] && !MountGSS.gameScoreSettings.GetHurtInGame[PlayerId])
        {
            PlayerStatus = Variable.PlayerStatus.Idle;
        }
        else if (MountGSS.gameScoreSettings.Horizontal != 0 && IsGround && !IsJumping && !MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] && !MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId] && !MountGSS.gameScoreSettings.GetHurtInGame[PlayerId])
        {
            PlayerStatus = Variable.PlayerStatus.Walk;
        }
        else if (IsJumping && !IsJumpingForward && !MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] && !MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId] && !MountGSS.gameScoreSettings.GetHurtInGame[PlayerId])
        {
            PlayerStatus = Variable.PlayerStatus.Jump;
        }
        else if (IsJumping && IsJumpingForward && !MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] && !MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId] && !MountGSS.gameScoreSettings.GetHurtInGame[PlayerId])
        {
            PlayerStatus = Variable.PlayerStatus.JumpForward;

        }
        else if (!IsGround && !IsJumping && !MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] && !MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId] && !MountGSS.gameScoreSettings.GetHurtInGame[PlayerId])
        {
            PlayerStatus = Variable.PlayerStatus.Fall;
        }
        else if (MountGSS.gameScoreSettings.GetHurtInGame[PlayerId] && MountGSS.gameScoreSettings.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] >= 0)
        {
            PlayerStatus = Variable.PlayerStatus.GetHurt;
        }
        else if (MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId])
        {
            PlayerStatus = Variable.PlayerStatus.PlayerSoul;
        }
        else if (MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] && !MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId] && !PlayPlayerDie2 && !PlayPlayerDie3)
        {
            PlayerStatus = Variable.PlayerStatus.PlayerDie_1;
        }
        else if (PlayPlayerDie2 && !PlayPlayerDie3)
        {
            PlayerStatus = Variable.PlayerStatus.PlayerDie_2;
        }
        //消失状态
        else if (PlayPlayerDie3)
        {
            PlayerStatus = Variable.PlayerStatus.PlayerDie_3;
        }

    }



    /// <summary>
    /// 动画控制器（攻击用动画与攻击逻辑放在了一起）
    /// </summary>
    public void AnimationCtrl()
    {
        //动画根据时间流逝速度播放（暂停游戏时暂停动画）
        animator.speed = Time.timeScale;
        if (KaQiTuoLiTai && PlayerStatus == Variable.PlayerStatus.Walk)
        {
            animator.speed = Time.timeScale * MoveSpeedRatio;//卡其脱离太的话，动画也快点
        }
        //未停止攻击/受伤（含死亡）动画正在播放的时候不能切换到其他任何形态
        if (!StopAttacking && PlayerStatus == Variable.PlayerStatus.GetHurt)
        {
            return;
        }

        //播放预设动画
        animator.Play(GameScoreSettingsIO.AnimationHash[(int)PlayerStatus]);
        //跳跃动画播放的时候，如果再次进行了一次跳跃，则重新播放该动画
        if (PlayerStatus == Variable.PlayerStatus.Jump || PlayerStatus == Variable.PlayerStatus.JumpForward)
        {
            if (MountGSS.gameScoreSettings.Jump && JumpCount == 2)
            {
                Debug.Log("114514");
                animator.Play(GameScoreSettingsIO.AnimationHash[(int)PlayerStatus], -1, 0f);
            }
        }

    }

    /// <summary>
    /// 跳跃和穿过平台（落体或者从地板掉下来，地板的Layer是Platform）
    /// </summary>
    public void JumpAndFall()
    {
        //除了禁用跳跃的情况，攻击的时候也不能跳（单独写出来，不然太麻烦了）
        if (BanJump || MountGSS.gameScoreSettings.Zattack || MountGSS.gameScoreSettings.Xattack || MountGSS.gameScoreSettings.Magia || MountGSS.gameScoreSettings.LocalIsStiff) return;

        //跳跃触发
        if (!MountGSS.gameScoreSettings.Down && MountGSS.gameScoreSettings.Jump && JumpCount != 2)
        {
            IsJumping = true;
            JumpCount++;
            StandOnFloor = false;
            StandOnPlatform = false;
            IsJumpingForward = MountGSS.gameScoreSettings.Horizontal != 0;

            SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.PlayerJump);
            //恢复中立，并顺便消除垂直方向的速度
            SetGravityRatio(1f);
            //施加向上的力（大小待测试）
            rigidbody2D.AddForce(Vector2.up * 9.5f, ForceMode2D.Impulse);
        }

        else if (IsJumping || IsJumpingForward)
        {
            //垂直几乎没速度了
            if (rigidbody2D.velocity.y < 0f)
            {
                IsJumping = false;
                IsJumpingForward = false;
            }
        }
        //跳跃计数器更新
        else if (IsGround && !IsJumping && !IsJumpingForward) { JumpCount = 0; }



        //穿过平台，激活之后一直往下掉，直到碰到地板才停止
        //. Changing this property(rigidbody2D.simulated) is much more memory and processor-efficient than enabling or disabling individual Collider 2D and Joint 2D components.
        if (MountGSS.gameScoreSettings.Down && StandOnPlatform && MountGSS.gameScoreSettings.Jump && go.layer != 7)
        {
            //更改玩家层，使玩家具有下落传过平台的条件
            go.layer = 7;//7 与平台不碰撞的
            IsJumping = false;
            FallFromPlatformTime = Time.timeSinceLevelLoad;
        }

        //一定时间后，大概是穿过平台了，回复玩家层准备接受对平台的碰撞
        else if (Time.timeSinceLevelLoad - FallFromPlatformTime >= 1f && go.layer == 7)
        {
            go.layer = 8;//8 通常玩家层
        }

    }

    /// <summary>
    /// 对于正在跳跃过程中发动魔法/攻击的情况，直接取消跳跃状态  每个角色的每个攻击都要有
    /// </summary>
    public void CancelJump()
    {
        if (!IsJumping)
        {
            return;
        }
        //既然要取消，那肯定是跳起来了，不在地上，悬空，准备后面的处理
        SetGravityRatio(0f);
        IsJumping = false;
        IsJumpingForward = false;
    }

    /// <summary>
    /// 用这个设定重力速率（但凡用到这个，垂直速度都会调成0）
    /// </summary>
    /// <param name="number"></param>
    public void SetGravityRatio(float number, bool AllowCheck = true)
    {
#if UNITY_EDITOR
        if (number > 1f && AllowCheck) { Debug.Log("该重力比率大于1，如果是想使玩家快速下降，请将该方法的AllowCheck参数改为false"); }
#endif
        rigidbody2D.gravityScale = number * 2.3f;
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);
    }


    /// <summary>
    /// 普通的行走用
    /// </summary>
    void Walk()
    {
        //灵魂球的上下移动
        switch (MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId])
        {
            //是那个球，直接无视平台
            case true:
                if (MountGSS.gameScoreSettings.Up)
                {
                    Move(GameScoreSettingsIO.MoveSpeed, Vector2.up);
                }
                else if (MountGSS.gameScoreSettings.Down)
                {
                    Move(GameScoreSettingsIO.MoveSpeed, Vector2.down);
                }
                break;
        }
        //不管是否死亡都用同一个左右移动
        Move(GameScoreSettingsIO.MoveSpeed, Vector2.right * MountGSS.gameScoreSettings.Horizontal);

        // tr.Translate(MountGSS.gameScoreSettings.Horizontal * Vector2.right * MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].MoveSpeed * MoveSpeedRatio * Time.deltaTime, Space.World);

    }

    /// <summary>
    /// 移动（Space为World，自行处理方向正负）
    /// </summary>
    /// <param name="Speed"></param>
    /// <param name="UseTimeDelta"></param>
    /// <param name="Direction"></param>
    /// <param name="space"></param>
    public void Move(float Speed, Vector2 Direction)
    {
        //消除因为更换为物理引擎后造成的数值差异
        Speed /= 2.6f;

        Direction = new Vector2(Direction.x *-Mathf.Cos( PlayerSlopeAngle) * Speed, Direction.y * Mathf.Sin(PlayerSlopeAngle) * Speed);

        //水平方向速度为0，站在地上，消除竖直方向速度，防止玩家弹起或者下滑
        if(ExMath.Approximation(0.01F,0F, Direction.x) && IsGround )
        {

            rigidbody2D.velocity = new Vector2(MountGSS.gameScoreSettings.PlayerMove.x, 0f);//竖直方向：物理引擎碰撞引起的
        }
        else
        {
            rigidbody2D.velocity = new Vector2(MountGSS.gameScoreSettings.PlayerMove.x, rigidbody2D.velocity.y);//竖直方向：物理引擎碰撞引起的
        }
        MountGSS.gameScoreSettings.PlayerMove = Speed * MoveSpeedRatio * Direction;


        //更新玩家位置
        MountGSS.gameScoreSettings.PlayersPosition[PlayerId] = rigidbody2D.position; //相机抖动的话，注意一下这行代码

    }

    #endregion

    #region 僵直
    /// <summary>
    /// 设置僵直
    /// </summary>
    /// <param name="Time">僵直事件</param>
    public void Stiff(float Time)
    {
        //僵直状态
        StopAttacking = false;
        MoveSpeedRatio = 1F;
        animator.enabled = false;//暂停动画
        MountGSS.gameScoreSettings.BanInput = true;//这一个就够了
        MountGSS.gameScoreSettings.LocalIsStiff = true;
        SetGravityRatio(1f);//中断了所有的咏唱，牛顿把玩家从天上拉下来

        //启用新的僵直
        Timing.RunCoroutine(PlayerStiff(Time), "PlayerStiff");

    }
    /// <summary>
    /// 这里经常出BUG
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    IEnumerator<float> PlayerStiff(float d)
    {

        yield return Timing.WaitForSeconds(d);

        //状态恢复
        StopAttacking = true;
        MoveSpeedRatio = 1F;
        animator.enabled = true;
        MountGSS.gameScoreSettings.LocalIsStiff = false;
        MountGSS.gameScoreSettings.BanInput = false;
        yield return 0f;


    }
    #endregion
    public void PlayerSE(Variable.SoundEffect soundEffect)
    {
        SoundEffectCtrl.soundEffectCtrl.PlaySE(soundEffect);
    }


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


    #region 时间变化 信息更新与升级

    /// <summary>
    /// 随着时间流逝，灵魂宝石变黑
    /// </summary>
    void PerSecondChange()
    {
        if (!MountGSS.gameScoreSettings.DoesMajoOrShoujoDie && Time.timeScale != 0 && !IsInvincible && MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] != 0 && MountGSS.gameScoreSettings.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] != 0 && !MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] && !MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId])
        {
            MountGSS.gameScoreSettings.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId]--;

            //soul随着时间扣没了
            if (MountGSS.gameScoreSettings.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] <= 0)
            {
                Die(1);
            }

            if (!MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] && MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] < MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].BasicVit + Grow(MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].VitGrowth, MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].VitGrowthLevelLimit, true))
            {
                MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] + 7;
                MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = Mathf.Clamp(MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId], 0, MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].MaxVit);
            }
        }
    }


    public void LevelUp()
    {
        SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.LevelUp);

        MountGSS.gameScoreSettings.GirlsLevel[MountGSS.gameScoreSettings.PlayerSelectedGirlId]++;
        UpdateInf(false);
    }

    /// <summary>
    /// 更新玩家信息（游戏一开始/升级，根据等级获取
    /// </summary>
    /// <param name="StartGame">是否刚开游戏或者复活完成</param>
    void UpdateInf(bool StartGameOrRebirth)
    {
        GameScoreSettingsIO gss = MountGSS.gameScoreSettings;
        if (StartGameOrRebirth)
        {
            //累计值，直接回复到最大值
            gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].BasicVit + Grow(gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].VitGrowth, gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].VitGrowthLevelLimit, true);
            gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = Mathf.Clamp(gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId], 0, gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].MaxVit);
            gss.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].BasicSoulLimit + gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].SoulGrowth * (MountGSS.gameScoreSettings.GirlsLevel[MountGSS.gameScoreSettings.PlayerSelectedGirlId] - 1);
            gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = Mathf.Clamp(gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId], 0, gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].MaxSoul);
            gss.GirlsPow[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].BasicPow + gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].PowGrowth * (MountGSS.gameScoreSettings.GirlsLevel[MountGSS.gameScoreSettings.PlayerSelectedGirlId] - 1);
            gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = Mathf.Clamp(gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId], 0, gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].MaxPow);

        }
        //因为每次升级都要调用，所以无需乘以等级
        else
        {
            gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] + Grow(gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].VitGrowth, gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].VitGrowthLevelLimit, false);
            gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = Mathf.Clamp(gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId], 0, gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].MaxVit);
            gss.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = gss.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] + gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].SoulGrowth;
            gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = Mathf.Clamp(gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId], 0, gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].MaxSoul);
            gss.GirlsPow[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = gss.GirlsPow[MountGSS.gameScoreSettings.PlayerSelectedGirlId] + gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].PowGrowth;
            gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = Mathf.Clamp(gss.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId], 0, gss.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].MaxPow);
        }

    }

    /// <summary>
    /// 一些属性成长值的计算（返回可用的成长值）
    /// </summary>
    /// <param name="GrowparaSetting">适用于成长的参数的成长设置 e.g. VitGrowth </param>
    /// <param name="LevelLimit">成长参数的等级门槛  e.g. VitGrowthLevelLimit</param>
    /// <param name="Returntotal">返回累加值吗（升级用false）</param>
    /// <returns></returns>
    int Grow(int[] GrowparaSetting, int[] LevelLimit, bool Returntotal)
    {
        int j = 0;
        int length = LevelLimit.Length;

        if (Returntotal)
        {

            //先根据等级来判断采取多少级别的成长值
            for (int i = 0; i < length; i++)
            {
                //如果当前角色等级低于i阶等级限制的门槛
                if (MountGSS.gameScoreSettings.GirlsLevel[MountGSS.gameScoreSettings.PlayerSelectedGirlId] < LevelLimit[i])
                {
                    break;
                }
                else
                {
                    //则返回上一阶的成长值
                    if (i != 0)
                    {
                        j += GrowparaSetting[i - 1];
                    }
                }
            }

        }
        else
        {
            //先根据等级来判断采取多少级别的成长值
            for (int i = 0; i < length; i++)
            {
                //如果当前角色等级低于i阶等级限制的门槛
                if (MountGSS.gameScoreSettings.GirlsLevel[MountGSS.gameScoreSettings.PlayerSelectedGirlId] < LevelLimit[i])
                {
                    //则返回上一阶的成长值
                    if (i != 0)
                    {
                        j += GrowparaSetting[i - 1];
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        return j;

    }
    #endregion

    #region 受伤，死亡与无敌

    /// <summary>
    /// 清除当前灵魂值
    /// </summary>
    public void CleanSoul()
    {
        if (!MountGSS.gameScoreSettings.DoesMajoOrShoujoDie)
        {
            GetHurt(56756756);
        }

    }
    /// <summary>
    /// 清除当前血量
    /// </summary>
    public void CleanVit()
    {
        GetHurt(MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId]);

    }

    /// <summary>
    /// 一滴血一滴血！！！！
    /// </summary>
    public void OneBlood()
    {
        GetHurt(MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] - 1);

    }

    /// <summary>
    /// 调式模式按钮：修改移动速率 仅沙耶加的时候可用（因为按钮附加在 以后再适配其他角色吧。。。 
    /// </summary>
    bool KaQiTuoLiTai = false;
    /// <summary>
    /// 适用于卡其脱离太的速度设置（按钮事件）
    /// </summary>
    public void SpeedSet()
    {
        KaQiTuoLiTai = !KaQiTuoLiTai;

        ///通常移动速度
        float Common = 1f;
        ///球的移动速度
        float Ball = 1.2f;

        if (MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId])
        {
            if (KaQiTuoLiTai)
            {
                SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.KaQiTuoLiTai);
                MoveSpeedRatio = Ball * 4f;
            }
            else
            {
                MoveSpeedRatio = Ball;
            }
        }
        else
        {
            if (KaQiTuoLiTai)
            {
                SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.KaQiTuoLiTai);
                MoveSpeedRatio = Common * 4f;
            }
            else
            {
                MoveSpeedRatio = Common;
            }
        }
    }


    /// <summary>
    /// 玩家受伤（调试版）
    /// </summary>
    /// <param name="damage">伤害</param>
    /// <param name="AllowDeathAnimation">允许播放死亡动画（false=直接变成球）</param>
    public void GetHurt(int damage)
    {
        //无敌、死亡、灵魂球不执行后续操作
        if (IsInvincible || MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] || MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId])
        {
            return;
        }


        //清除状态
        VariableInitialization();
        MountGSS.gameScoreSettings.BanInput = true;

        IsAttack[0] = false;
        IsAttack[1] = false;
        if (MountGSS.gameScoreSettings.PlayerSelectedGirlId != 4)
        {
            //沙耶加magia受击不中断攻击
            IsAttack[2] = false;
        }


        //如果承受不住这个攻击，宝石直接碎了
        if (MountGSS.gameScoreSettings.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] - damage * MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].Recovery <= 0)
        {
            MountGSS.gameScoreSettings.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = 0;
            Die(1);
            return;
        }

        //扣个血完事
        if (MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] > damage)
        {
            //扣除hp（vit)
            MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] - damage;
            //扣除soullimit
            MountGSS.gameScoreSettings.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = MountGSS.gameScoreSettings.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] - damage * MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].Recovery;


            //   MountGSS.gameScoreSettings.HurtGirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = damage;
            MountGSS.gameScoreSettings.GetHurtInGame[PlayerId] = true;


            //无敌状态
            Timing.RunCoroutine(Invincible());
        }
        //挂了，但不至于宝石碎了
        else
        {

            //死亡
            Die(damage);
        }
    }



    /// <summary>
    /// 死亡
    /// </summary>
    public void Die(int damage)
    {

        //扣除soullimit
        MountGSS.gameScoreSettings.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = MountGSS.gameScoreSettings.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] - damage * MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].Rebirth;

        MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] = true;

        MountGSS.gameScoreSettings.BanInput = true;

    }

    /// <summary>
    /// 判断：复活或者宝石黑掉了(PlayerDie_1动画最后一帧调用）
    /// </summary>
    public void RebirthOrGemBroken()
    {
        IsInvincible = true;

        //宝石黑掉了
        if (MountGSS.gameScoreSettings.GirlSoulLimit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] <= 0)
        {
            SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.GemBreak);

            PlayPlayerDie2 = true;
        }
        //变成光球以便复活
        else
        {
            MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId] = true;

            SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.PlayerToBall);

            //光球效果
            MoveSpeedRatio = 1.2f;

            MountGSS.gameScoreSettings.BanInput = false;

            SetGravityRatio(0f); ;

        }
    }

    /// <summary>
    /// 受伤动画结束后的事件
    /// </summary>
    public void HurtAnimationEndEvent()
    {
        MountGSS.gameScoreSettings.BanInput = false;
        MountGSS.gameScoreSettings.GetHurtInGame[PlayerId] = false;
    }

    /// <summary>
    /// 无敌状态调用（1.5s版）
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator<float> Invincible()
    {
        IsInvincible = true;

        for (int i = 0; i < 15; i++)
        {
            yield return Timing.WaitForSeconds(0.1f);
            animator.enabled = !animator.enabled;//我屈服了，dnmdBUG
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }

        //防止bug，强调一次变量修改
        spriteRenderer.enabled = true;
        animator.enabled = true;
        IsInvincible = false;
    }

    /// <summary>
    /// 动画、状态变量初始化（所有变量，范围最广）
    /// </summary>
    public virtual void VariableInitialization()
    {
        //攻击状态+动画参数消除需要重写
        //这里是公用的，私用的重写
        MountGSS.gameScoreSettings.BanInput = false;
        BanWalk = false;
        BanJump = false;
        SetGravityRatio(1f);
        BanTurnAround = false;
        MountGSS.gameScoreSettings.GetHurtInGame[PlayerId] = false;
        MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] = false;
        MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId] = false;
        MountGSS.gameScoreSettings.LocalIsStiff = false;
    }

    /// <summary>
    /// 复活完成（光球动画最后一帧调用）
    /// </summary>
    public void RebirthDone()
    {
        SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.PlayerRebirth);


        MoveSpeedRatio = 1f;
        // BanGravity = false;
        StartCoroutine(nameof(Invincible));
        MountGSS.gameScoreSettings.IsSoulBallInGame[PlayerId] = false;
        MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] = false;

        //在这里恢复VIT，为了得到血条恢复的效果
        int MaxVit = MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].BasicVit + Grow(MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].VitGrowth, MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].VitGrowthLevelLimit, true);
        MaxVit = Mathf.Clamp(MaxVit, 0, MountGSS.gameScoreSettings.mahouShoujos[MountGSS.gameScoreSettings.PlayerSelectedGirlId].MaxVit);
        MountGSS.gameScoreSettings.GirlsVit[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = MaxVit;

    }

    /// <summary>
    /// 宝石碎了，玩家倒地，变成便服   动画调用
    /// </summary>
    /// <returns></returns>
    public void GemBroken()
    {
        // SoundEffectCtrl.soundEffectCtrl.PlaySE(Variable.SoundEffect.GemBreakFadeOut); 不要这个音效了，有点乱

        /*
        //应用影子魔女的效果，尝试修复移动设备下不产生fade效果的bug
        Material.EnableKeyword("OUTBASE_ON");

        //玩家变黑，然后消失
        Material.EnableKeyword("GREYSCALE_ON");//变黑
        //消失准备
        Material.EnableKeyword("FADE_ON");//开始消失的shader特征
        */
        //设置死亡状态
        MountGSS.gameScoreSettings.MagicalGirlsDie[MountGSS.gameScoreSettings.PlayerSelectedGirlId] = true;

        //切换为消失动画和状态
        PlayPlayerDie3 = true;
        PlayPlayerDie2 = false;

        //MountGSS.gameScoreSettings.IsBodyDieInGame[PlayerId] = false 不能设置，因为宝石碎了的前提就是身子挂了
    }

    /// <summary>
    /// Fade效果结束，已经看不见玩家了（动画调用），判断是否三个魔法少女都死了
    /// </summary>
    public void DestroyPoorGirl()
    {
        //场上有一个玩家死了
        MountGSS.gameScoreSettings.PlayerDie();
        //删除物体
        Destroy(gameObject);
    }
    #endregion

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //获取的有效碰撞体数
        int GetNumber = 0;

        for (int i = 0; i < collision.contactCount; i++)
        {
            //限制高度，使在高度之下的才能被识别为脚下踩的东西
            if (Feet.position.y >= collision.GetContact(i).point.y)
            {
                //对于触碰地面的处理
                if (collision.collider.CompareTag("Floor") || collision.collider.CompareTag("Platform")) //获取任务#1  尽量减少获取任务
                {
                    //修改落地状态
                    StandOnPlatform = collision.collider.CompareTag("Platform");
                    StandOnFloor = collision.collider.CompareTag("Floor");

                    GetNumber++;//每获取一次有效的碰撞数据，就增加一次，当总数达到预期目标之后，直接破坏这个for
                }

            }


            if (GetNumber == 1) //现在就一个获取任务
            {
                break;
            }
        }








    }

}


