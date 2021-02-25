using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;
using MEC;
using System;

//多人游戏玩家需要同步的信息：PlayerStatus， 输入，
[DisallowMultipleComponent]
public abstract class APlayerCtrl : MonoBehaviour, IMove
{
    #region  基础属性
    [Header("基础属性")]
    public int PlayerId = 1;
    public int MahouShoujoId = 0;
    /// <summary>
    /// 玩家所在斜坡的单位圆坐标（角度/三角函数那些）
    /// </summary>
    public Vector2 PlayerSlope = Vector2.right;

    [Space]
    //为了方便调试，这些Ban和Is先暂时放在这里
    public bool BanGravity = false;
    /// <summary>
    /// 禁用重力射线。用于穿透地板（仅AplayerCtrl可以修改）
    /// </summary>
   [SerializeField] bool BanGravityRay = false;
    /// <summary>
    /// 禁用跳跃
    /// </summary>
    public bool BanJump = false;
    public bool BanWalk = false;
    public bool IsGround = true;
    /// <summary>
    /// 站在地板上
    /// </summary>
    public bool StandOnFloor = false;
    /// <summary>
    /// 禁用转身
    /// </summary>
    public bool BanTurnAround = false;
    /// <summary>
    /// 禁止输入，仅适用于按一下按键动画执行到底且确实不需要外部输入的攻击
    /// </summary>
    public bool BanInput = false;
    /// <summary>
    /// 无敌状态
    /// </summary>
    public bool IsInvincible = false;
    /// <summary>
    /// 重力射线位置
    /// </summary>
    [Header("重力射线位置")]
    public Transform[] GavityRayPos = new Transform[2];
    #endregion

    /// <summary>
    /// 玩家死亡。注意：联机的时候只有自己控制的角色才调用
    /// </summary>

    public static Variable.OrdinaryEvent PlayerGemBroken = new Variable.OrdinaryEvent();


    #region 组件
    public Transform tr;
   [HideInInspector]  public Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    Material Material;
    #endregion

    #region 私有状态机（不保存到GSS中）
    /// <summary>
    /// 玩家状态（仅应用于动画机）
    /// </summary>
    public Variable.PlayerStatus playerStatus;
    /// <summary>
    /// 重力射线
    /// </summary>
    readonly Ray2D[] rays = new Ray2D[3];
    public RaycastHit2D infoLeft;
    public RaycastHit2D infoRight;
    public RaycastHit2D infoHor;

    public float GravityRatio = 1f;

    [HideInInspector] public float MoveSpeedRatio = 1f;

    /// <summary>
    /// 禁止左右移动（-1左 1右）
    /// </summary>
    public int BanLeftOrRight = 0;

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
    public bool IsJumping = false;

    /// <summary>
    /// 正在向前跳（仅对有向前跳动画的角色有用）
    /// </summary>
    public bool IsJumpingForward = false;

    /// <summary>
    /// 站在平台上
    /// </summary>
    public bool StandOnPlatform = false;

    [HideInInspector] public int SlopeInstanceId = 0;

    /// <summary>
    /// 正在穿过平台
    /// </summary>
    [HideInInspector] public bool GoThroughPlatform = false;
    /// <summary>
    /// 穿墙瞬间的游戏时间，用于防止穿墙途中停止落体
    /// </summary>
    float PlatformTime = 0f;
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

    #endregion

    private void Awake()
    {
        #region 获取组件
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Material = spriteRenderer.material;
        #endregion
    }

    private void Start()
    {
        #region 注册事件
        UpdateManager.updateManager.FastUpdate.AddListener(FastUpdate);
        #endregion


        //影子魔女的话开启黑色描边效果
        if (StageCtrl.gameScoreSettings.BattlingMajo == Variable.Majo.ElsaMaria)
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
        InvokeRepeating("PerSecondChange", 1f, 1f);

        //保存本地玩家选择的魔法少女的魔法少女id
        StageCtrl.gameScoreSettings.PlayerSelectedGirlId = MahouShoujoId;

        //获取PlayerId
        for (int i = 0; i < 3; i++)
        {
            if (StageCtrl.gameScoreSettings.SelectedGirlInGame[i].ToString().Equals(name))
            {
                PlayerId = i;
                break;
            }
        }
        //注册受伤事件
        switch (PlayerId)
        {
            case 0:
                StageCtrl.stageCtrl.Player1Hurt.AddListener(GetHurt);
                break;
            case 1:
                StageCtrl.stageCtrl.Player2Hurt.AddListener(GetHurt);
                break;
            case 2:
                StageCtrl.stageCtrl.Player3Hurt.AddListener(GetHurt);
                break;
        }
        //设置tag
        tag = string.Format("Player{0}", (PlayerId+1).ToString());
        //修正玩家层
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
        //安卓是直接作用于StageCtrl.gameScoreSettings.Horizontal
        //为了防止在不同的帧运行，所以放到了这里
        //屏幕输入的按钮放在了主相机脚本里
        //这个是通常版本，有的角色（比如沙耶加）可能重写了
        if (StageCtrl.gameScoreSettings.UseScreenInput == 0)
        {
            StageCtrl.gameScoreSettings.Horizontal = RebindableInput.GetAxis("Horizontal");
            StageCtrl.gameScoreSettings.Jump = RebindableInput.GetKeyDown("Jump");
            StageCtrl.gameScoreSettings.Down = RebindableInput.GetKey("Down");
            StageCtrl.gameScoreSettings.Up = RebindableInput.GetKey("Up");
            StageCtrl.gameScoreSettings.Zattack =RebindableInput.GetKeyDown("Zattack");
            StageCtrl.gameScoreSettings.ZattackPressed = RebindableInput.GetKey("Zattack");
            StageCtrl.gameScoreSettings.Xattack = RebindableInput.GetKeyDown("Xattack");
            StageCtrl.gameScoreSettings.XattackPressed = RebindableInput.GetKey("Xattack");
            StageCtrl.gameScoreSettings.Magia = RebindableInput.GetKeyDown("Magia");
            StageCtrl.gameScoreSettings.MagiaPressed = RebindableInput.GetKey("Magia");

            StageCtrl.gameScoreSettings.Pause = RebindableInput.GetKeyDown("Pause");

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


    void FastUpdate()
    {
        if (!BanGravity) Gravity();
       

        RayCtrl();

        if (StageCtrl.gameScoreSettings.LocalIsStiff)
        {
            return;
        }

        //还是以最高优先级执行输入代理
        InputAgent();

        #region  基础控制器
        JumpAndFall();

        if (!BanWalk) Walk();
        SetStatus();
        if(!StageCtrl.gameScoreSettings.LocalIsStiff) AnimationCtrl();
        #endregion


        #region 攻击方法
        //防止死亡状态、按下跳跃的瞬间发动攻击
        if (StageCtrl.gameScoreSettings.Jump || StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] || IsInvincible)
        {
            //修复攻击过程中跳跃仍然显示攻击动画的bug
            return;
        }


        //前面的!IsAttack[1]是为了防止做这个攻击的时候意外发动其他的攻击
        //这里加限制条件/修改状态要三思，主要是在抽象的方法里更改和限制
        //对于玩家来说，除了跳跃键，其他的都是能够接受长时间按住的
        if (!IsAttack[1] && !IsAttack[2] && !StageCtrl.gameScoreSettings.Xattack &&!StageCtrl.gameScoreSettings.XattackPressed) { OrdinaryZ(); HorizontalZ(); VerticalZ(); }
        if (!IsAttack[0] && !IsAttack[2] && !StageCtrl.gameScoreSettings.Zattack && !StageCtrl.gameScoreSettings.ZattackPressed ) { OrdinaryX(); HorizontalX(); UpX(); DownX(); }
        //magia对VIT/血条的处理在各自的脚本里  限制vit有bug   松开魔法键之后仍然会执行魔法
        if (!IsAttack[0] && !IsAttack[1] && /*StageCtrl.gameScoreSettings.Magia &&*/ StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] > StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].MaigaVit | IsAttack[2]) { Magia(); }

        BanWalk = IsAttack[0] || IsAttack[1] || IsAttack[2] || StageCtrl.gameScoreSettings.Zattack || StageCtrl.gameScoreSettings.Magia || StageCtrl.gameScoreSettings.Xattack;//在这里统一弄一个，直接在这里禁用移动，不再在各种攻击方法和动画事件中禁用了
        BanJump = IsAttack[0] || IsAttack[1] || IsAttack[2] || StageCtrl.gameScoreSettings.Zattack || StageCtrl.gameScoreSettings.Magia || StageCtrl.gameScoreSettings.Xattack;//在这里统一弄一个，直接在这里禁用移动，不再在各种攻击方法和动画事件中禁用了

        #endregion


    }


    #region  基础控制器

    /// <summary>
    /// 设置玩家状态
    /// </summary>
    public  void SetStatus()
    {


        if (IsAttack[0] || IsAttack[1] || IsAttack[2])
        {
            return;
        }

        //通过对一些状态变量的判断，得出当前玩家的状态，并应用于动画

        //基础的在这里写，攻击的在各自玩家脚本中重写
        if (StageCtrl.gameScoreSettings.Horizontal == 0 && IsGround && !IsJumping && !StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] && !StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId] && !StageCtrl.gameScoreSettings.GetHurtInGame[PlayerId])
        {
            playerStatus = Variable.PlayerStatus.Idle;
        }
        else if (StageCtrl.gameScoreSettings.Horizontal != 0 && IsGround && !IsJumping && !StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] && !StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId] && !StageCtrl.gameScoreSettings.GetHurtInGame[PlayerId])
        {
            playerStatus = Variable.PlayerStatus.Walk;
        }
        else if (IsJumping && !IsJumpingForward && !StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] && !StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId] && !StageCtrl.gameScoreSettings.GetHurtInGame[PlayerId])
        {
            playerStatus = Variable.PlayerStatus.Jump;
        }
        else if (IsJumping && IsJumpingForward && !StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] && !StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId] && !StageCtrl.gameScoreSettings.GetHurtInGame[PlayerId])
        {
            playerStatus = Variable.PlayerStatus.JumpForward;

        }
        else if (!IsGround && !IsJumping && !StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] && !StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId] && !StageCtrl.gameScoreSettings.GetHurtInGame[PlayerId])
        {
            playerStatus = Variable.PlayerStatus.Fall;
        }
        else if (StageCtrl.gameScoreSettings.GetHurtInGame[PlayerId] && StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId] >= 0)
        {
            playerStatus = Variable.PlayerStatus.GetHurt;
        }
        else if (StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId])
        {
            playerStatus = Variable.PlayerStatus.PlayerSoul;
        }
        else if (StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] && !StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId] && !PlayPlayerDie2)
        {
            playerStatus = Variable.PlayerStatus.PlayerDie_1;
        }
        else if (PlayPlayerDie2)
        {
            playerStatus = Variable.PlayerStatus.PlayerDie_2;
        }


    }



    /// <summary>
    /// 动画控制器（攻击用动画与攻击逻辑放在了一起）
    /// </summary>
    public void AnimationCtrl()
    {
        //未停止攻击/受伤动画正在播放的时候不能切换到其他任何形态
        if (!StopAttacking && playerStatus == Variable.PlayerStatus.GetHurt)
        {
            return;
        }

        //左右翻转
        if (!BanTurnAround)
        {
            if (StageCtrl.gameScoreSettings.Horizontal == -1) { tr.rotation = Quaternion.Euler(0f, 180f, 0f); DoLookRight = false; } //
            else if (StageCtrl.gameScoreSettings.Horizontal == 1) { tr.rotation = Quaternion.Euler(0f, 0f, 0f); DoLookRight = true; }//spriteRenderer.flipX = StageCtrl.gameScoreSettings.Horizontal == -1;//
        }

        animator.Play(GameScoreSettingsIO.AnimationHash[(int)playerStatus]);
       
    }

    /// <summary>
    /// 跳跃和穿过平台（落体或者从地板掉下来，地板的Layer是Wall）
    /// </summary>
    public void JumpAndFall()
    {
        //除了禁用跳跃的情况，攻击的时候也不能跳（单独写出来，不然太麻烦了）
        if (BanJump || StageCtrl.gameScoreSettings.Zattack || StageCtrl.gameScoreSettings.Xattack || StageCtrl.gameScoreSettings.Magia) return;

        //跳跃触发
        if (!StageCtrl.gameScoreSettings.Down && StageCtrl.gameScoreSettings.Jump && JumpCount != 2)
        {
            MoveSpeedRatio = 1f;
            JumpInteralTimer = Time.timeSinceLevelLoad;
            IsJumping = true;
            JumpCount++;
            BanGravity = true;
            BanGravityRay = true;

            IsJumpingForward = StageCtrl.gameScoreSettings.Horizontal != 0;
        }
        //跳跃状态
        if (IsJumping)
        {
            //上升
            if (Time.timeSinceLevelLoad - JumpInteralTimer <= 0.35f)
            {
                //解决一个很奇怪的BUG
                IsGround = false;
                tr.Translate(Vector3.up * (GameScoreSettingsIO.JumpSpeed - (Time.timeSinceLevelLoad - JumpInteralTimer) * GameScoreSettingsIO.JumpSpeed / 0.3F) * Time.deltaTime);
            }
            //下降（其实就是取消跳跃状态）
            else
            {
                BanGravityRay = false;
                BanGravity = false;
                IsJumping = false;
                IsJumpingForward = false;

            }
        }

        //跳跃计数器更新
        if (IsGround) { JumpCount = 0; }



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

    /// <summary>
    /// （千万不要多次重复执行！！！）对于正在跳跃过程中发动魔法/攻击的情况，直接取消跳跃状态  每个角色的每个攻击都要有
    /// </summary>
    public void CancelJump()
    {
        if (!IsJumping)
        {
            return;
        }
        //既然要取消，那肯定是跳起来了
        BanGravity = false;
        BanGravityRay = false;//有向上移动的攻击时，才能禁用这个
        IsGround = false;
        IsJumping = false;
        IsJumpingForward = false;

    }

    /// <summary>
    /// 射线控制器
    /// </summary>
    public void RayCtrl()
    {

        //这里的判断不能被禁用
        rays[0] = new Ray2D(GavityRayPos[0].position, Vector2.down * 0.1f);
        rays[1] = new Ray2D(GavityRayPos[1].position, Vector2.down * 0.1f);
        infoLeft = Physics2D.Raycast(rays[1].origin, rays[1].direction, 0.01f);
        infoRight = Physics2D.Raycast(rays[0].origin, rays[0].direction, 0.01f);

        Debug.DrawRay(rays[0].origin, rays[0].direction * 0.1f, Color.blue);
        Debug.DrawRay(rays[1].origin, rays[1].direction * 0.1f, Color.blue);


        //重力射线
        if (!BanGravityRay)
        {
            //在地上
            if (infoLeft.collider != null)// || infoRight.collider != null)
            {
                StandOnPlatform = infoLeft.collider.CompareTag("Platform");
                StandOnFloor = infoLeft.collider.CompareTag("Floor") || infoLeft.collider.CompareTag("Wall") || infoLeft.collider.CompareTag("Slope");
               
                //斜坡
                if (infoLeft.collider.CompareTag("Slope") && SlopeInstanceId != infoLeft.collider.GetInstanceID())
                {
                    string[] vec = infoLeft.collider.name.Split(',');
                    PlayerSlope = new Vector2(float.Parse(vec[0]), float.Parse(vec[1]));
                    //这串代码感觉效率挺低的，所以加了一个ID判断
                    SlopeInstanceId = infoLeft.collider.GetInstanceID();
                }
                else if (infoLeft.collider.CompareTag("Floor") || infoLeft.collider.CompareTag("Wall") || infoLeft.collider.CompareTag("Platform"))
                {
                    PlayerSlope = Vector2.right;
                    SlopeInstanceId = 0;
                }
            }
            else if (infoRight.collider != null)// || infoRight.collider != null)
            {
                StandOnPlatform = infoRight.collider.CompareTag("Platform");
                StandOnFloor = infoRight.collider.CompareTag("Floor") || infoRight.collider.CompareTag("Wall")|| infoRight.collider.CompareTag("Slope");
                //斜坡
                if (infoRight.collider.CompareTag("Slope") && SlopeInstanceId != infoRight.collider.GetInstanceID())
                {
                    string[] vec = infoRight.collider.name.Split(',');
                    PlayerSlope = new Vector2(float.Parse(vec[0]), float.Parse(vec[1]));
                    //这串代码感觉效率挺低的，所以加了一个ID判断
                    SlopeInstanceId = infoRight.collider.GetInstanceID();
                }
                else if (infoRight.collider.CompareTag("Floor") || infoRight.collider.CompareTag("Wall") || infoRight.collider.CompareTag("Platform"))
                {
                    PlayerSlope = Vector2.right;
                    SlopeInstanceId = 0;

                }


            }
            //啥也没才上，腾空
            else if (!IsJumping)//去除跳跃的情况
            {
                //非攻击状态
                if (!IsAttack[0] && !IsAttack[1] && !IsAttack[2])
                {
                    BanGravity = IsGround;
                }
                //攻击状态
                else
                {
                    //
                    if (IsGround) BanGravity = true;
                }


                StandOnPlatform = false;
                StandOnFloor = false;
                SlopeInstanceId = 0;
                PlayerSlope = Vector2.right;

            }

            //有斜坡参数，说明肯定在地上
            if (PlayerSlope != Vector2.right) IsGround = true;

            IsGround = StandOnFloor || StandOnPlatform;
          //非攻击状态
           if(!IsAttack[0] && !IsAttack[1] && !IsAttack[2])
            {
                BanGravity = IsGround;
            }
           //攻击状态
            else
            {
                //
                if (IsGround) BanGravity = true;
            }

            //直接在这里强制转化成true好了）
            if (StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId])
            {
                IsGround = false;
                BanGravity = true;
            }

        }

        //水平移动防止穿墙射线
        if (DoLookRight)
        {
            rays[2] = new Ray2D(GavityRayPos[0].position + Vector3.up * 0.2f, Vector2.right * 0.8f);
            //  rays[1] = new Ray2D(GavityRayPos[1].position + Vector3.up * 0.5f, Vector2.left * 10f);
        }
        else//
        {
            rays[2] = new Ray2D(GavityRayPos[0].position + Vector3.up * 0.2f, -Vector2.right * 0.8f);
            //   rays[1] = new Ray2D(GavityRayPos[1].position + Vector3.up * 0.5f, -Vector2.left * 10f);
        }
        //  infoLeft = Physics2D.Raycast(rays[1].origin, rays[1].direction,10f);
        infoHor = Physics2D.Raycast(rays[2].origin, rays[2].direction, 0.8f);
        // Debug.DrawRay(rays[1].origin, rays[1].direction * 1f, Color.red);
     
        //撞墙限制移动
        BanLeftOrRight = 0;

        if (infoHor.collider != null)// || infoRight.collider != null)
        {
            Debug.Log(infoHor.collider.name); 



            //撞墙限制移动
            if ( infoHor.collider.CompareTag("Wall"))
            {
                if (DoLookRight)
                {
                    BanLeftOrRight = 1;
                }
                else
                {
                    BanLeftOrRight = -1;

                }
            }

        }

        //脚插地修复
        if (infoLeft.collider != null && infoHor.collider != null &&  IsGround && !StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId] && PlayerSlope == Vector2.right)
        {
            //如果水平射线与脚底射线得到的东西一致，那么说明脚插在地里（不对地板进行处理）
            if ( infoLeft.collider.GetInstanceID().Equals(infoHor.collider.GetInstanceID()))
            {
                Debug.Log("YUKI.N> 紧急修正程序已启动");

                tr.Translate(Vector3.up * 0.1f, Space.World);
                return;
            }
        }
        else if (infoRight.collider != null && infoHor.collider != null && IsGround && !StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId] && PlayerSlope == Vector2.right)
        {
            //如果水平射线与脚底射线得到的东西一致，那么说明脚插在地里（不对地板进行处理）
            if ( infoRight.collider.GetInstanceID().Equals(infoHor.collider.GetInstanceID()))
            {
                Debug.Log("YUKI.N> 紧急修正程序已启动");

                tr.Translate(Vector3.up * 0.1f, Space.World);
            }
        }

    }

    /// <summary>
    /// 普通的行走用
    /// </summary>
    void Walk()
    {
        /*
        if (StageCtrl.gameScoreSettings.UseScreenInput == 2)
        {
            switch (StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId])
            {
                //是那个球，直接无视平台
                case true:
                    Move(GameScoreSettingsIO.MoveSpeed, true, StageCtrl.gameScoreSettings.joystick);
                    break;
                //还没死呢
                case false:
                    Move(GameScoreSettingsIO.MoveSpeed, true, Vector2.right * StageCtrl.gameScoreSettings.Horizontal);
                    break;
            }
        }*/
            switch (StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId])
            {
                //是那个球，直接无视平台
                case true:
                    if (StageCtrl.gameScoreSettings.Up)
                    {
                        Move(GameScoreSettingsIO.MoveSpeed, true, Vector2.up);
                    }
                    else if (StageCtrl.gameScoreSettings.Down)
                    {
                        Move(GameScoreSettingsIO.MoveSpeed, true, Vector2.down);
                    }
                    break;
        }
        //不管是否死亡都用同一个左右移动
        Move(GameScoreSettingsIO.MoveSpeed, true, Vector2.right * StageCtrl.gameScoreSettings.Horizontal);

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

    /// <summary>
    /// 自带joystick（Space为World，自行处理Direction的正负）
    /// </summary>
    /// <param name="Speed"></param>
    /// <param name="UseTimeDelta"></param>
    /// <param name="Direction"></param>
    /// <param name="space"></param>
    public void Move(float Speed, bool UseTimeDelta, Vector2 Direction)
    {
        //应用斜坡
        if(Direction == Vector2.right && DoLookRight)
        {
            Direction = PlayerSlope;
        }
        else if (Direction == Vector2.left && !DoLookRight && Direction != -PlayerSlope)
        {
            Direction = -PlayerSlope;

        }




        float x = Direction.x; float y = Direction.y;
        bool Border = false;

        //站在地板上，剔除向下移动方向（为球球做的）
        if (StandOnFloor && StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId] && Direction.y < 0)
        {
            //  Direction = new Vector2(Direction.x, 0f);
            y = 0f;
            Border = true;
        }

        //到天花板了，剔除向上移动方向（仅适用于灵魂球）
        else if (tr.localPosition.y >= 4.1f && Direction.y > 0 && StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId])
        {
            y = 0f;
            Border = true;
        }

        //左右的空气墙
        if (tr.localPosition.x >= 8.2f && Direction.x > 0)
        {
            x = 0f;
            Border = true;
        }
        else if (tr.localPosition.x <= -8.2f && Direction.x < 0)
        {
            x = 0f;
            Border = true;
        }
        //左右碰到平台（找个时间和上面的额合并一下）
        if (BanLeftOrRight == -1 && Direction.x < 0)
        {
            x = 0f;
            Border = true;
        }
        else if (BanLeftOrRight == 1 && Direction.x > 0)
        {
            x = 0f;
            Border = true;
        }
        if (Border)
        {
            //减少一个new
            Direction = new Vector2(x, y);
        }

        //缺少：PlayerSlope计算（判断），左右版边限制移动

        if (UseTimeDelta)
        {
            tr.Translate(Direction * Speed * MoveSpeedRatio * Time.deltaTime, Space.World);
        }
        else
        {
            tr.Translate(Direction * Speed * MoveSpeedRatio, Space.World);
        }
    }

    /// <summary>
    /// 下落重力
    /// </summary>
    public void Gravity()
    {
        /*
        if(!IsGround | !IsJumping && !StartFall)
        {
            FallTime = Time.timeSinceLevelLoad;
            StartFall = true;
        }
        else
        {
            StartFall = false;
        }


        //加速度
        if (StartFall)
        {
            GravityRatio = GravityRatio + 0.5f * Time.deltaTime;
        }*/

        tr.Translate(Vector2.down * 9.8f * GravityRatio * Time.deltaTime, Space.World);

    }



    #endregion

    #region 僵直
    /// <summary>
    /// 设置僵直
    /// </summary>
    /// <param name="Time">僵直事件</param>
    public void Stiff(float Time)
    {
        //取消以前的僵直（仅仅是换成另一个僵直，并不是取消将至）
     //   Timing.KillCoroutines("PlayerStiff");
        //僵直状态
        StopAttacking = false;
        BanGravity = IsGround;
        GravityRatio = 1F;
        MoveSpeedRatio = 1F;
        BanGravityRay = false;
        animator.enabled = !true;
        BanInput = !false;//这一个就够了
        StageCtrl.gameScoreSettings.LocalIsStiff = !false;

        //启用新的僵直
        StartCoroutine("PlayerStiff", Time);
        Timing.RunCoroutine(PlayerStiff(Time), "PlayerStiff");

    }
    /// <summary>
    /// 这里经常初BUG
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
     IEnumerator<float> PlayerStiff(float d)
    {

        yield return Timing.WaitForSeconds(d);

        //状态恢复
        BanWalk = false;
        StopAttacking = !false;
        BanGravity = IsGround;
        GravityRatio = 1F;
        MoveSpeedRatio = 1F;
        BanGravityRay = false;
        BanTurnAround = false;
        BanJump = false;
        animator.enabled = true;
        StageCtrl.gameScoreSettings.LocalIsStiff = false;
        //第二帧才解除禁用
        yield return Timing.WaitForOneFrame;
        BanInput = false;


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
   

    #region 时间变化 信息更新与升级

    /// <summary>
    /// 随着时间流逝，灵魂宝石变黑
    /// </summary>
    void PerSecondChange()
    {
        if (!StageCtrl.gameScoreSettings.DoesMajoOrShoujoDie && Time.timeScale != 0 && !IsInvincible && StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] != 0 && StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId] != 0 && !StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] && !StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId])
        {
            StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId]--;

            //soul随着时间扣没了
            if (StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId] <= 0)
            {
                Die(1);
            }

            if (!StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] && StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] < StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].BasicVit + Grow(StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].VitGrowth, StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].VitGrowthLevelLimit, true))
            {
                StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] = StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] + 7;
                StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] = Mathf.Clamp(StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId], 0, StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].MaxVit);
            }
        }
    }


    void LevelUp()
    {
        StageCtrl.gameScoreSettings.GirlsLevel[MahouShoujoId]++;
        UpdateInf(false);
    }

    /// <summary>
    /// 更新玩家信息（游戏一开始/升级，根据等级获取
    /// </summary>
    /// <param name="StartGame">是否刚开游戏或者复活完成</param>
    void UpdateInf(bool StartGameOrRebirth)
    {
        GameScoreSettingsIO gss = StageCtrl.gameScoreSettings;
        if (StartGameOrRebirth)
        {
            //累计值，直接回复到最大值
            gss.GirlsVit[MahouShoujoId] = gss.mahouShoujos[MahouShoujoId].BasicVit + Grow(gss.mahouShoujos[MahouShoujoId].VitGrowth, gss.mahouShoujos[MahouShoujoId].VitGrowthLevelLimit, true);
            gss.GirlsVit[MahouShoujoId] = Mathf.Clamp(gss.GirlsVit[MahouShoujoId], 0, gss.mahouShoujos[MahouShoujoId].MaxVit);
            gss.GirlSoulLimit[MahouShoujoId] = gss.mahouShoujos[MahouShoujoId].BasicSoulLimit + gss.mahouShoujos[MahouShoujoId].SoulGrowth * (StageCtrl.gameScoreSettings.GirlsLevel[MahouShoujoId] - 1);
            gss.GirlsVit[MahouShoujoId] = Mathf.Clamp(gss.GirlsVit[MahouShoujoId], 0, gss.mahouShoujos[MahouShoujoId].MaxSoul);
            gss.GirlsPow[MahouShoujoId] = gss.mahouShoujos[MahouShoujoId].BasicPow + gss.mahouShoujos[MahouShoujoId].PowGrowth * (StageCtrl.gameScoreSettings.GirlsLevel[MahouShoujoId] - 1);
            gss.GirlsVit[MahouShoujoId] = Mathf.Clamp(gss.GirlsVit[MahouShoujoId], 0, gss.mahouShoujos[MahouShoujoId].MaxPow);

        }
        //因为每次升级都要调用，所以无需乘以等级
        else
        {
            gss.GirlsVit[MahouShoujoId] = gss.GirlsVit[MahouShoujoId] + Grow(gss.mahouShoujos[MahouShoujoId].VitGrowth, gss.mahouShoujos[MahouShoujoId].VitGrowthLevelLimit, false);
            gss.GirlsVit[MahouShoujoId] = Mathf.Clamp(gss.GirlsVit[MahouShoujoId], 0, gss.mahouShoujos[MahouShoujoId].MaxVit);
            gss.GirlSoulLimit[MahouShoujoId] = gss.GirlSoulLimit[MahouShoujoId] + gss.mahouShoujos[MahouShoujoId].SoulGrowth;
            gss.GirlsVit[MahouShoujoId] = Mathf.Clamp(gss.GirlsVit[MahouShoujoId], 0, gss.mahouShoujos[MahouShoujoId].MaxSoul);
            gss.GirlsPow[MahouShoujoId] = gss.GirlsPow[MahouShoujoId] + gss.mahouShoujos[MahouShoujoId].PowGrowth;
            gss.GirlsVit[MahouShoujoId] = Mathf.Clamp(gss.GirlsVit[MahouShoujoId], 0, gss.mahouShoujos[MahouShoujoId].MaxPow);
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
                if (StageCtrl.gameScoreSettings.GirlsLevel[MahouShoujoId] < LevelLimit[i])
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
                if (StageCtrl.gameScoreSettings.GirlsLevel[MahouShoujoId] < LevelLimit[i])
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

#if UNITY_EDITOR
    [ContextMenu("Hurt")]
    public void HurtMyself()
    {
        GetHurt(20);
    }
    [ContextMenu("CleanSoul")]
    public void CleanSoul()
    {
        if (!StageCtrl.gameScoreSettings.DoesMajoOrShoujoDie)
        {
            GetHurt(56756756);
        }

    }
    [ContextMenu("CleanVit")]
    public void CleanVit()
    {
        GetHurt(StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId]);

    }

    [ContextMenu("Succeed")]
    public void Succeed()
    {
        StageCtrl.gameScoreSettings.Succeed = true;
    }
#endif


    /// <summary>
    /// 受伤（调试版）
    /// </summary>
    public void GetHurt(int damage)
    {
        //无敌、死亡、灵魂球不执行后续操作
        if (IsInvincible || StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] || StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId])
        {
            return;
        }


        //清除状态
        VariableInitialization();
        BanInput = true;
     
        IsAttack[0] = false;
        IsAttack[1] = false;
        if(MahouShoujoId != 4)
        {
            //沙耶加magia受击不中断攻击
            IsAttack[2] = false;
        }
        

        //如果承受不住这个攻击，宝石直接碎了
        if (StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId] - damage * StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].Recovery <= 0)
        {
            StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId] = 0;
            Die(1);
            return;
        }

        //扣个血完事
        if (StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] > damage)
        {
            //扣除hp（vit)
            StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] = StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] - damage;
            //扣除soullimit
            StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId] = StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId] - damage * StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].Recovery;


            //   StageCtrl.gameScoreSettings.HurtGirlsVit[MahouShoujoId] = damage;
            StageCtrl.gameScoreSettings.GetHurtInGame[PlayerId] = true;


            //无敌状态
            StartCoroutine("Invincible");
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
        StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId] = StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId] - damage * StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].Rebirth;

        StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] = true;

        BanInput = true;

    }

    /// <summary>
    /// 判断：复活或者宝石黑掉了(PlayerDie_1动画最后一帧调用）
    /// </summary>
    public void RebirthOrGemBroken()
    {

        //宝石黑掉了
        if (StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId] <= 0)
        {
            PlayPlayerDie2 = true;
        }
        //复活
        else
        {
            StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId] = true;


            //光球效果
            MoveSpeedRatio = 1.2f;

            BanInput = false;
            BanGravity = true;
            // BanGravityRay = true;

        }
    }

    /// <summary>
    /// 受伤动画结束后的事件
    /// </summary>
    public void HurtAnimationEndEvent()
    {
        BanInput = false;
        StageCtrl.gameScoreSettings.GetHurtInGame[PlayerId] = false;
    }

    /// <summary>
    /// 无敌状态调用
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator Invincible()
    {
        IsInvincible = true;

        for (int i = 0; i < 15; i++)
        {
            yield return new WaitForSeconds(0.1f);
            animator.enabled = !animator.enabled;//我屈服了，dnmdBUG
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }

        //防止bug，启用一次
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
        BanInput = false;
        BanWalk = false;
        GravityRatio = 1F;
        MoveSpeedRatio = 1F;
        BanJump = false;
        BanGravity = IsGround;
        BanGravityRay = false;
        BanTurnAround = false;
        StageCtrl.gameScoreSettings.GetHurtInGame[PlayerId] = false;
        StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] = false;
        StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId] = false;
        StageCtrl.gameScoreSettings.LocalIsStiff = false;
    }

    /// <summary>
    /// 复活完成（光球动画最后一帧调用）
    /// </summary>
    public void RebirthDone()
    {
        MoveSpeedRatio = 1f;
        // BanGravity = false;
        StartCoroutine("Invincible");
        StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId] = false;
        StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] = false;

        //在这里恢复VIT，为了得到血条恢复的效果
        int MaxVit = StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].BasicVit + Grow(StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].VitGrowth, StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].VitGrowthLevelLimit, true);
        MaxVit = Mathf.Clamp(MaxVit, 0, StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].MaxVit);
        StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] = MaxVit;

    }

    /// <summary>
    /// 宝石碎了，玩家倒地，变成便服的动画调用
    /// </summary>
    /// <returns></returns>
    public void GemBroken()
    {
        //应用影子魔女的效果，尝试修复移动设备下不产生fade效果的bug
        Material.EnableKeyword("OUTBASE_ON");
        Material.EnableKeyword("GREYSCALE_ON");

        //玩家变黑，然后消失
        Material.EnableKeyword("GREYSCALE_ON");//变黑
        //消失准备
        Material.EnableKeyword("FADE_ON");//开始消失的shader特征

        //更换动画机，表演消失动画
        animator.runtimeAnimatorController = StageCtrl.gameScoreSettings.GemBrokenFadeAnimator;
        spriteRenderer.sprite = StageCtrl.gameScoreSettings.PlayerDieImage[MahouShoujoId];

        //设置死亡状态
        StageCtrl.gameScoreSettings.MagicalGirlsDie[MahouShoujoId] = true;

        PlayerGemBroken.Invoke();

        //StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId] = false 不能设置，因为宝石碎了的前提就是身子挂了
    }

    /// <summary>
    /// Fade效果结束，已经看不见玩家了（动画调用），判断是否三个魔法少女都死了
    /// </summary>
    public void DestroyPoorGirl()
    {
        //场上有一个玩家死了
        StageCtrl.stageCtrl.PlayerDie();
        //删除物体
        Destroy(gameObject);
    }
    #endregion
}

