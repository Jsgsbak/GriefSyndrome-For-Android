using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//先暂时不继承
public class SayakaCtrl : APlayerCtrl
{
    /// <summary>
    /// 适用于X攻击蓄力的魔法阵
    /// </summary>
    [Space]
 //   public Animator MagicRing;
   public int ZattackCount = 0;
    /// <summary>
    /// z攻击这一阶段完成了
    /// </summary>
    bool ThisFregmentDone = false;
    bool AllowZcontinue;

    bool XordinaryDash = false;
    /// <summary>
    /// 普通X攻击计时器，用于记录普通X准备用时与设定普通X攻击冲刺速度还有普通X冲刺完之后间隔0.3s才能再充一次
    /// </summary>
    float OrdinaryXTimer = 0f;

    /// <summary>
    /// Down X攻击状态 -1向下 1向上 0没发动攻击 2反弹上升段
    /// </summary>
     int DownAttackMovingUpward = 0;
    /// <summary>
    /// Up X攻击移动
    /// </summary>
    bool UpAttackMove = false;
    
    /// <summary>
    /// 已经飞着发动了X攻击
    /// </summary>
    bool HasXattackFlying = false;

    int UpAttackCount = 0;
    bool MagiaDash = false;
    /// <summary>
    /// 魔法按了X键
    /// </summary>
    float MagiaDashSpeedRatio = 1f;



    public override void VariableInitialization()
    {
        base.VariableInitialization();
       
        //接着上面，自己的变量初始化
        ZattackCount = 0;
        XordinaryDash = false;
        OrdinaryXTimer = 0f;
        DownAttackMovingUpward = 0;
        UpAttackMove = false;
        UpAttackCount = 0;
         MagiaDash = false;
        MagiaDashSpeedRatio = 1f;
        //攻击动画/状态消除
        for (int i = 0; i < 3; i++)
        {
            IsAttack[i] = false;
        }
    }



    public override void Magia()
    {     
        //带有攻击的magia，并且防止多次运行
        if (MagiaDashSpeedRatio == 1f && MountGSS.gameScoreSettings.Xattack && IsAttack[2])
        {
            MagiaDashSpeedRatio = 1.5f;
            playerStatus = Variable.PlayerStatus.Magia_2;
        }

        //初始发动magia
        if(!IsAttack[2] && MountGSS.gameScoreSettings.Magia)
        {
            IsAttack[2] = true;
            CancelJump();
            BanWalk = true;
            BanTurnAround = true;
            SetGravityRatio(0f);
            //  BanGravityRay = true;

            playerStatus = Variable.PlayerStatus.Magia_1;

            MountGSS.gameScoreSettings.GirlsVit[MahouShoujoId] = MountGSS.gameScoreSettings.GirlsVit[MahouShoujoId] - MountGSS.gameScoreSettings.mahouShoujos[MahouShoujoId].MaigaVit;

            //用于UI中HP血条信息更新
            MountGSS.gameScoreSettings.MagiaKeyDown[PlayerId] = true;

        }

        //冲刺
        if (MagiaDash)
        {
            //掉落Bug修复
            SetGravityRatio(0f);

            if (DoLookRight)
            {
                Move(8f * MagiaDashSpeedRatio, true,  Vector2.right);
            }
            else
            {
                Move(8f * MagiaDashSpeedRatio, true, Vector2.left);
            }
        }
    }

    public override void HorizontalX()
    {
        if (MountGSS.gameScoreSettings.Horizontal != 0 && !IsAttack[1]  && MountGSS.gameScoreSettings.Xattack)
        {
          
            CancelJump();//直接中断跳跃并且不恢复
            SetGravityRatio(0f);
            MountGSS.gameScoreSettings.BanInput = true;
            IsAttack[1] = true;
            StopAttacking = false;
            playerStatus = Variable.PlayerStatus.HorizontalStrong_1;//移动
        }

        if (playerStatus == Variable.PlayerStatus.HorizontalStrong_1)
        {
             MountGSS.gameScoreSettings.BanInput = true; //BUG修复

            //移动
            if (DoLookRight)
            {
                Move(8f, true, Vector3.right);
            }
            else
            {
                Move(8f, true, Vector3.left);
            }
        }
    }


    public override void OrdinaryX()
    {
        if (IsGround && HasXattackFlying)
        {
            SetGravityRatio(0f);
            HasXattackFlying = false;
        }

        //从通常状态进入到X攻击准备状态
        if (!HasXattackFlying && MountGSS.gameScoreSettings.Horizontal == 0 && !IsAttack[1] && !MountGSS.gameScoreSettings.Up && !MountGSS.gameScoreSettings.Down  && MountGSS.gameScoreSettings.XattackPressed  && !XordinaryDash)
        {
            playerStatus = Variable.PlayerStatus.Strong_1;
            CancelJump();//直接中断跳跃并且不恢复
            IsAttack[1] = true;
            BanWalk = true;
            BanTurnAround = true;
            //只允许在空中发动一次
            if (!IsGround)
            {
                HasXattackFlying = true;
            }
            //保存一下时间，用于得到蓄力的效果
            OrdinaryXTimer = Time.timeSinceLevelLoad;
            SetGravityRatio(0.1f);
        }
        //松开X键，但仍然处于X攻击状态，所以能往前冲
        else if (!MountGSS.gameScoreSettings.XattackPressed && IsAttack[1]&& playerStatus == Variable.PlayerStatus.Strong_1 && !XordinaryDash)
        {
            playerStatus = Variable.PlayerStatus.Strong_2;
            XordinaryDash = true;
            //冲刺时不受重力影响
            SetGravityRatio(0f);
        }

        //冲刺移动（放在这里是为了移动流畅）
        else if (XordinaryDash)
        {
            //使用正负号的不同来防止多次计算
            if (OrdinaryXTimer >= 0F)
            {
                //从开始到蓄力完成有1.5s
                OrdinaryXTimer = -Mathf.Clamp01((Time.timeSinceLevelLoad - OrdinaryXTimer) / 1.5F);
            }

            if (DoLookRight)
            {
                Move(6F - OrdinaryXTimer * 4F, true, Vector2.right);
            }
            else
            {
                Move(6F - OrdinaryXTimer * 4F, true, Vector2.left);
            }
        }
    }
    public override void DownX()
    {
        if (MountGSS.gameScoreSettings.Horizontal == 0 && !IsAttack[1] && MountGSS.gameScoreSettings.Xattack && MountGSS.gameScoreSettings.Down )
        {

            CancelJump();//直接中断跳跃并且不恢复
            IsAttack[1] = true;
            playerStatus = Variable.PlayerStatus.DownStrong_1;//上升动作
            MountGSS.gameScoreSettings.BanInput = true;//在这一套攻击里，就靠取消僵直来把这个设置为false了
            SetGravityRatio(0f);
            DownAttackMovingUpward = 1;
        }

        //上升
        if (DownAttackMovingUpward == 1)
        {
            if (IsGround)
            {
                Move(10f, true, Vector2.up);
            }
            else
            {
                Move(5f, true, Vector2.up);
            }
        }
        //下降
        else if (DownAttackMovingUpward == -1)
        {
            if (DoLookRight)
            {
                Move(13f, true,  Vector2.right);
            }
            else
            {
                Move(13f, true,  Vector2.left);
            }

            //碰到地了（仅执行一次）
            if (IsGround && playerStatus != Variable.PlayerStatus.DownStrong_3)
            {
                //反弹
              Timing.RunCoroutine(XattackBound());

                DownAttackMovingUpward = 2;
            }
        }

        //触地反弹
        else if (DownAttackMovingUpward == 2)
        {
            if (DoLookRight)
            {
                Move(5f, true, new Vector2(-1f, 1f));
            }
            else
            {
                Move(5f, true,  new Vector2(1f, 1f));
            }
        }

    }

    public override void UpX()
    {


        if (IsGround) { UpAttackCount = 0; }

        if (MountGSS.gameScoreSettings.Horizontal == 0  && UpAttackCount < 2  && MountGSS.gameScoreSettings.Xattack && MountGSS.gameScoreSettings.Up)
        {
            CancelJump();//直接中断跳跃并且不恢复
            UpAttackCount++;
            UpAttackMove = true;
            IsAttack[1] = true;
            BanWalk = true;
            BanTurnAround = true;
            SetGravityRatio(0f);
            BanJump = true;

            playerStatus = Variable.PlayerStatus.UpStrong_1;
        }

        if (UpAttackMove)
        {
            if (DoLookRight)
            {
                Move(4f, true, new Vector2(1f, 3f));
            }
            else
            {
                Move(4f, true,  new Vector2(-1f, 3f));
            }
        }
    }

    public override void OrdinaryZ()
    {

        //时间足够长，第一阶段攻击
        if( MountGSS.gameScoreSettings.Zattack && !IsAttack[0])
        {
            ZattackCount = 1;
            playerStatus = Variable.PlayerStatus.Weak_1;
            IsAttack[0] = true;
            AllowZcontinue = false;
            ThisFregmentDone = false;
        }
        //下面用 && MountGSS.gameScoreSettings.ZattackPressed是为了能够判断那一帧的确按Z了
        else if ( ZattackCount == 1 && ThisFregmentDone && AllowZcontinue)
        {
            ZattackCount = 2;
            playerStatus = Variable.PlayerStatus.Weak_2;
            ThisFregmentDone = false;
            AllowZcontinue = false;
        }
        else if ( ZattackCount == 2 && ThisFregmentDone && AllowZcontinue)
        {
            ZattackCount = 3;
            playerStatus = Variable.PlayerStatus.Weak_1;
            ThisFregmentDone = false;
            AllowZcontinue = false;
        }
        else if (ZattackCount == 3 && ThisFregmentDone && AllowZcontinue)
        {

            ZattackCount = 4;
            playerStatus = Variable.PlayerStatus.Weak_2;
            ThisFregmentDone = false;
            AllowZcontinue = false;

        }
        else if ( ZattackCount == 4 && ThisFregmentDone && AllowZcontinue)
        {
            if (!IsGround)
            {
                IsAttack[0] = false;
                return;
            }
            MoveSpeedRatio = 1f;

            ZattackCount = 0;
            playerStatus = Variable.PlayerStatus.Weak_3;

            AllowZcontinue = false;
            ThisFregmentDone = false;
        }
        
        if(ThisFregmentDone &&  !AllowZcontinue)
        {

            AllowZcontinue = false;

            ZattackCount = 0;
            IsAttack[0] = false;
            ThisFregmentDone = false;
            if (IsGround)
            {
                SetGravityRatio(0f);
            }
            else
            {
                FallInteralTimer = Time.timeSinceLevelLoad;
                SetGravityRatio(1f);
            }
            MoveSpeedRatio = 1f;
            VariableInitialization();
        }



    }



    //Haruhi Suzumiya

    /// <summary>
    /// 攻击用动画逻辑
    /// </summary>
    /// <param name="AnimationName"></param>
    public override void  ZattackAnimationEvent(string AnimationName)
    {
        //虽然跳跃的那个也会用一次这个方法，但是没太大影响
        ZattackMove();

        switch (AnimationName)
        {
            //Z攻击的动画正处于攻击状态，不能中断
            case "ZattackDoing":
                MoveSpeedRatio = 0.1f;
                StopAttacking = false;
                BanTurnAround = true;//攻击状态不能转身
                CancelJump();
                if (IsGround)
                {
                    SetGravityRatio(0f);
                }
                else
                {
                    SetGravityRatio(0.2f);
                }
                break;


            case "Check":
                //检查要不要继续攻击
                Debug.Log("check");
                if (MountGSS.gameScoreSettings.ZattackPressed)
                {
                    AllowZcontinue = true;
                }
                break;

             

            //Z攻击某一阶段完成了
            case "ZattackFregmentDone":
                ThisFregmentDone = true;
                StopAttacking = true;
                BanTurnAround = false;//攻击完了可以
                                      //这里再检查一下
                Debug.Log("check");

                if (MountGSS.gameScoreSettings.ZattackPressed)
                {
                    AllowZcontinue = true;
                }
                break;

           

            //Z攻击最后一阶段向前跳
            case "ZattackFinJump":
                MountGSS.gameScoreSettings.BanInput = true;//防止出现任何操作
                BanTurnAround = true;//向前跳的时候不能转身
                StopAttacking = false;//不可以中断攻击
                SetGravityRatio(0.7f);
                ThisFregmentDone = false;
                //向前移动
                if (DoLookRight)
                {
                    Move(0.4f, false, Vector2.right);
                }
                else
                {
                    Move(0.4f, false, Vector2.left);
                }
                break;

            //Z攻击最后阶段结束
            case "ZattackFinDone":
                StopAttacking = true;
                MountGSS.gameScoreSettings.BanInput = true;
                //修改计数器重新循环动画
                ZattackCount = 0;
                IsAttack[0] = false;//连接处不属于攻击阶段，可以切换到其他动画和状态
                ThisFregmentDone = true;
                //尝试用回复变量的方法来解决bug
                VariableInitialization();
                
                //僵直
                Stiff(0.02f);

                //因为这里不会产生动画未结束松开Z导致动画结束的情况，所以不修改IsZattacking
                break;
        }
    }
    
    /// <summary>
    /// 普通重工结束
    /// </summary>
    /// <param name="AnimationName"></param>
    public override void XattackAnimationEvent(string AnimationName)
    {        
                //结束
                BanTurnAround = false;
                MountGSS.gameScoreSettings.BanInput = false;
                XordinaryDash = false;
                IsAttack[1] = false;

        SetGravityRatio(1f);

                VariableInitialization();
                //僵直
                Stiff(0.05f);
              


    }

    public override void HorizontalXattackAnimationEvent(string AnimationName)
    {
        //结束
        MountGSS.gameScoreSettings.BanInput = true;
        StopAttacking = true;
        IsAttack[1] = false;
        //尝试用回复变量的方法来解决bug
        VariableInitialization();
        Stiff(0.05f);

    }

    public override void DownXattackAnimationEvent(string AnimationName)
    {
        switch (AnimationName)
        {
            //到达顶峰，停顿一下
            case "PeakArrival":
                DownAttackMovingUpward = 0;
                break;

                //蹬了一脚，准备下移
            case "StartMovingDownward":
                playerStatus = Variable.PlayerStatus.DownStrong_2;//下移动作

                break;

                //下移
            case "Doing-Down":
                SetGravityRatio(1.5f);
                DownAttackMovingUpward = -1;
                break;

                //着陆
            case "Land":
              Timing.RunCoroutine(XattackBound());
                break;
        }
    }

    /// <summary>
    /// 上X攻击结束
    /// </summary>
    /// <param name="AnimationName"></param>
    public override void UpXattackAnimationEvent(string AnimationName)
    {
        UpAttackMove = false;
        IsAttack[1] = false;
        SetGravityRatio(0f);//为了悬空效果，僵直结束之后重力恢复
        Stiff(0.02f);

    }

    public override void MagiaAnimationEvent(string AnimationName)
    {
        switch (AnimationName)
        {
            case "Finish":
                MagiaDash = false;
                IsAttack[2] = false;
                MagiaDashSpeedRatio = 1f;

                //尝试用回复变量的方法来解决bug
                VariableInitialization();

                SetGravityRatio(1f);
                Stiff(0.02f);

                break;

            case "Dash":
                MagiaDash = true;
                break;
        }
    }

    /// <summary>
    /// 这个用于所有Z动画之中（仅限沙耶加），为了那种一个动画前进一次的效果
    /// </summary>
    void ZattackMove()
    {
        if (MountGSS.gameScoreSettings.Horizontal != 0 && IsGround)
        {
            if (DoLookRight)
            {
                Move(0.02f, false, Vector2.right);
            }
            else
            {
                Move(0.02f, false, Vector2.left);
            }
        }

    }


    /// <summary>
    /// DownX攻击触地之后的反弹
    /// </summary>
    /// <returns></returns>
    IEnumerator<float> XattackBound()
    {
        playerStatus = Variable.PlayerStatus.DownStrong_3;//反弹动作

        SetGravityRatio(0f);
        DownAttackMovingUpward = 2;
        //  IsStiff = true;这个也不能要，不然不会触发反弹效果
        yield return Timing.WaitForSeconds(0.2f);
        DownAttackMovingUpward = 0;
        //   Stiff(0.1f); 自带僵直效果了
        //  IsStiff = false;
        SetGravityRatio(1f);
        MountGSS.gameScoreSettings.BanInput = false;
       IsAttack[1] = false;

        //尝试用回复变量的方法来解决bug
        VariableInitialization();
    }



    public override void VerticalZ()
    {

    }
    public override void HorizontalZ()
    {

    }


}



//WWWWWW$E#RRKKKKKKKKKKKKK&K&&&&&&&&&&&&&&&&&&&FFFFFFFFFFFFXI*IIIIIIIIIIIIIIIIIIIIIIIYXXFXI1"';''''''!+]*ll**YFXFFXYI*IIIIIIIIIIIIII*IIIIIIIIIIIIIIII
//WWW$E##RRRRRRKRKKKKKKKKKKKKKK&&&&&&&&&&&&&&&&&&&FFFFFFFFXIIIIIIYYYYYYYYYYIIIIIIIIIIIIIYXFFXli';''''''~+]***YFFFFFFFYIIIIIIIIIIIIIIIIII***lllll**lll
//E##RRRRRRRRRRRRRKKKKKKKKKKKKKKKK&&&&&&&&&&&&&&&&&FFFFFFXIIIIIIYYYYYYYYYYYYYYYYYIIIIIIIIIIYX&&I+~;;''';;!/l*IFFFFFFFXIIYIIIIIIIIYIl}//+iiiiiiiiiiiii
//RRRRRRRRRRRRRRRRRRRKKKKKKKKKKKKKKKK &&&&&&&&&&&&&&&&&&&FYYYYYYYXXXXXXXXXXXYYYYYYYYYIYIIIIIIIYYF&Y1!;;'''''!/*FFFFFFFFYYYYIIIIII*1>!~!">>iii+iiiiiiii
//##RRRRRRRRRRRRRRRRRRKKRKKKKKKKKKKKKKK&&&&&&&&&&&&XIl]1+iiiiiiii++//1}]l**IYYXXXXYYYYYYYIIIIIIIYX&X}";;''';'ilF&FFFFFYYYYIIYYl/!;;'~~!">i+++++++++++
//###R##RRRRRRRRRRRRRRRRRRRRRRKKKKKKKKKKKKK&&&&&&&]~';;;;;;''''''''';''''~~!">i/1]lIYXF&&&&FFXXYYIIYFF]!;'''~""+*&&F&FYYYYYY*/
//##RRRRRRKKKKKK&&&&&&&&&&&&&&&&&&&&&&&&&KK&&&&&&&}'''''''''''''''';;''''''''''''''~!">+/}lIYF&&KKK&FF&F}!;;~>>""/Y&&FYYYYI1"~';;;;''~~">+/111111111/
//FXXXXYYYYYIIIIIIIIIII* *II * ****IIIIIIIIIIIYYYYYYYY * *lllll * **ll]}1 / +ii"!~''';;;;;'''''''';'''~!>+/}*YXFKR&*"'">>>">1XKXXI1>!!!~''''''~!"i+1111111//1}
//FFFFF && FFFFFFFFFXXXXXYYYYYYYYYYIIIIIIIIIIIIIYYYYXXXFFFF &&&&&&&&&&&& KK && FXY *]1 / i > "!~'';;''''''';;;''~!>/]IY]+>>>>>" > *X}> !!""!~''''''~!> +/ 11}]**IIYYX
//RRKKKKKKKKKKKKKKKKKKKKK &&&& FFFFXXXXXXXXXXXXXXXXFFFFFFFFFFFFFFFFFF &&&& &KKRRR#####RK&XY*]1+>"~''';;''''';;;~+/i>>>>>"">"""""!'''''';''!"i1]*IYXXXXXXX
//F && &KKKRRRRRRRRRKKKKKRRRRRRRRRRRRKK && &FFFFFFFFFFF &&&&&&&&&&&&&&&&&&&&&&&& FFF &&&& &KKR######RKFX*]1i"!'';;;;!>iiiii>>>>>>"""';;;;''!>+}lIYXXXXXXXXXXX
//****IIYYYXF && KKRRR##RRRRRRRRRRRRRRRRRRRRRRKK&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&FF&&&KKR##EEEE#KFY*}/ii+>>>>>>>>>>>+}}11}]l*IIYXXXXXXXXXXXXXXXX
//III********** IIYYXF&KRRR####RRRRRRRRRRRRR#####RRRKKKKKKKKKKKKKKKKKKKKKKKKKKK&&&&&&&&&&&&&&&&&&&&&KR##E$$EE#R&Y*]1+i""""+XFFFFFFXXXXXXXXXXXXXFFFXXXX
//IIIIIIIIIIIIII*****IIIYXF&KRR###########RR###########RRRKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK&&&&&&&&&&&&&&&&&&KRR#EE$$E#RFYl1}&&FFXXXXXFFFFFFFFFXXXYII**l
//YYYYYIIIIIIIIIIIIIIIII**IIYYXF&KR##################EWWW$EE#RRRRRRRRRRRRKRKKKKKKKKKKKKKKKKKKKKK&&&&&&&&&&&&&&&&&KKR#EEEEEEE##K&FXYYYYXXXYYI*********
//XXXXYYYYYYYYYYXXXXFF &&&&KKKRRRRR##################EW%%%%%%WW$EE#RRRRRRRRRRRRRRRKRKKKKKKKKKKKKKKKKKKK&&&&&&&&&&&&&FFFF&KKR######R&YIIIIIIIIIIIIIIIYY
//FFFF &&&&KKRRR##EEEEEEEEEEEEEEEEEEEEEEEEEE##EEEEEE$%%%%%%%%%%%%%%W$E##RRRRRRRRRRRRRRRRRRRRRRKKKKKKKKKKKKKKK&&&&&&&&&&FFFFFFFFFFXXXYYYYYYYYYYYYXXXXXX
//##EE$$$$$$$$$$$$$$$EEEEEEEEEEEEEEEEEEEEEEEEEEEEE$%%%%%%%%%%%%%%%%%%%W$$E##RRRRRRRRRRRRRRRRRRRRRRRRKKKKKKKKKKKKKK&&&&&&&&FFFFFFFXXXXXXXXXXXXFFFFFFFF
//WWWWWWW$$$$$$$$$$$$$$$$$$$$$EEEEEEEEEEEEEEEEEEEW %%%%%%%%%%%%%%%%%%%%%%%%WW$EE###RRRRRRRRRRRRRRRRRRRRRRRRRRKKKKKKKKKKK&&&&&&&&&FFFFFFFFFFFFFFFFFFF&&
//WWWWWWWWWWWWWWWWWW$$$$$$$$$$$$$$$$$$$$EEEEEE$$WN%%%N%%%%%%%%N%%%%%%%N%%%%%%%%W$$E###RRRRRRRRRRRRRRRRRRRRRRRRRRRRKKKKKKKKKKKKK&&&&&&&&&&&&&&&&&&&&&&
//WWWWWWWWWWWWWWWWWWWWWWWWWW$$$$$$$$$$$$$$$$$$$W %NNNNNN%NNN%%%N%%%%%%NK$%%%%%%%%%%%W$$E####RRRRR#RRRRRRRRRRRRRRRRRRRRRRRRKKKKKKKKKKKKKKKKKKKKKKKKKKKK
//%%%%%%WWWWWWWWWWWWWWWWWWWWWWWWWWWW$$$$$$$$$$$W$$W%%%NNNNNNNNN%%%%%NK/EN$%%%%%%%%%%%%%%W$E#E###R####R#RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR
//%%%%%%%%%%%%%%%WWWWWWWWWWWWWWWWWWWWWWWW$W$$$W$$$$W$$$WW%%%NNNN%%%N#/+&@K$%%%%%%%%%%%%N%NX]IXK##################RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR
//%%%%%%%%%%%%%%%%%%%%W%WWWWWWWWWWWWWWWWW%WW$W$$WWW$$$$$$$$$WWW%%%NE1++l@&&N%%%%%%%%%%N$$%}> i +/ l#%$$$EEE#############################################
//E$W %%%%%%%%%%%%%%%%%%%%%%%% WWWWWWW %%% NN % WWW$$W % W$$W$$$$$$$W$EEEWE1++iiR#]%%%%%%%%%%%NX#W]*Il]1/FNN%%%%WW$$$EE######################################
//R##E$W%%%%%%%%%%%%%%%%%WW%%%%%%%NNN@@@%WWWWEW%%$$WW$$$$$$$%$$EW#**Il}*FNlKN%N%%%%%%%%I$#I*}///++*$N%%NNNNN%%%WW$$EEEE##################EEEEEEEEEEEE
//RRRRR#E$W%%%%%%%%%%%%%%%%NNN@@@@N@@@N%%%W%E$%%%$$%WW$$$$$W%$$$&/+i!!i/l&K1E$WWW%%%NNX1$l++++////+/F%NN%N%%NNNNNNN%%%%WW$$$EEEEEEEEEEEEEEEEEEE$$WW%%
//W$EE####EE$W%%NNNNNN@@@@@@@N%$E$%@@N%%%%%WE%%%%E$%WWWW$$$WNW$I/1+!!!~''~]]}E&K$EE##&//l]lIFKKRKK#K}1F%@NNNNNNNNNNNNNNNNNNN%%%%WWW$$$$$$$$WWW%%%NNNN
//NN %%%%%%%%%%%N@@@@NN%W$E#RK&K#%@@N%%%%%%%$$%%%%EW%WWWWW$$%N&}}&RF$$E#RR&}i!1Fl&I}/+i>~/F#%#RWWE}1IRR*}&WNNNNNNNNNNNNN%%%%%NNNNNNNNNN%%%%NNNNNNNN%NN
//&KRRR########RRRRKK&&&FFFFR$NM@N%%%%%%%%%$%%%%%#W%WWWWW$$$l/Y#*+i]X&#$W#l";'i>""!~'''~>!!"]&#KWI;!>YNFi&@%NNNNNNNNNNN%$$$WWW%%%NNNNNNNNNN%%%%%%%%%%
//FFFFFFFFFFFFFFFFFFFFFF & KE%@M@N%%%%%%%%%%%$N%%%%EW%WWWWW$WK/$W1>!+i+RWKK#!;'''''''''''';;1}1&I]I],';1&}INNNNNNNNNNNNNN%W$$$$$$$WWWW%%%%%%%NNNNN%%%%%
//FFFFFFFFFF & FFFFFFFF&&REWW%%WWW%%%%%%%%%%WWN%%%%EW%%%WWWWW#X$K+~,i$X]l/+*!;''''''''''''';+K"'!1*~'~'"!>F*RNNNNNNNNNNNN%W$$$$$$$$$$E$$W%NNN$&FK#$W%NN
//FFFFFFFFFFFFFFFFFFF#WWW$W$E###E$$WW%%%%%W%N%%N%$WN%%%WWW$%]'>i'';>X+;']1;'''''';'''''''';>"!"//~!!~''~>i&%%%NNNNNNNNN%$$$$EE$$$W%%%%$#F*/!';;'!i1]*
//FFFFFFFFFFFFFFFFFFFF & KRE$WWWWWW$$$WWWW%%WNN%%N%%$@%N%%WWWWF!'~~!"~"i!!~'''''''~"'''''''''''~~~~!!~~~'i11&W$$WW%%NNNNN%WW%%%%%$#KX*1i!';;''~'';,,;,,
//XFXXFXXFFFFFFFXXXXXXXXXXFF & KR#E$$WWWWW%NWNNNN@%NW@NN%%%WW%K]!!!"!"!~~'''''''''!>'''''''''~~~~!!!!~~~!i//&W$$$$$W%W%%%%ER&X*1i"~;,,,,;'~~~~~''';;';;
//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXFFFF &&#W%R&&EN%%%@NN%%%WW$Y1~!!!!~~~''''''''''''''''''''''~~~~~~~!/1/+"YW$$$$WWW$WW#+!';,,,,,;;;''~~~~~~~~~';;;;;;
//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXYYYY &$%&YYF%%N%@@@@%%%W%&}i ~~~~~''''''''''''''''''''''''''''';'~~''! + RW$$$W$W %$WWY ''''''';;;;;'~~~~!!~~~''; ; ;,; ;
//YYYXYXYXXXXYXXXYXYYYYXXYXXYYXYXXXYXXXXXXE$$XYYRN % NN@@@@%%% WWl / !''''''''''''''''''''''''''''~~~~'1R*}lF$@N$WWWW$%%W$$1~!~~~'';;;;'''~~~!!!~~''; ;,,; ;
//YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY & EW$&FYENNN@@@@@N %%%$FIi!!!"""~'''''''''''~''''''''"""!i* RE$%NNN%WWWW$WN@F&#"!!!!~~'''~~~~~~!!!!!~~~~'';;;;
//YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYIFRE$RYY#NW%@@M@NWE#RR&Y}/""""~''''''''''''''''''';'">*K&RRKK&&&&&R#$WN@K>KI">"""!!!!!!""""""""!!!!!!~~''''
//YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYIIYYIIYIRE#E##RRR##EE$$$E*>ii;,;''''''''''''''';'"**1E$$$$WWW$E#R&XXXFK11Fi>>>>>>>>"""">>>>>>""""""!~~~~~'
//IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIYIIIIYYYX & KRE$W%%%%%%WW$WE1*WKli~; ''''''''''''!i &% @E}K$$$$$$$$$WWW$ER & YIl}/ +iii >>>>>>>>>>>>>>>>> """!~~''''
//IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIYXFK#$W%%%%%%%W%WWWWW$WXR@@@%$l"!~''''''~"i+l@@NNFR$$$$$$$$$$$$$$$$$ERFI]1>iiiiiii>>>>>>>>>>>>>"!~''';;
//IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII* IIIIEW%%%%%%%%%%%%%%%WWWWW#%@NNN%&//+i>"">i///+l#$NNK#$$$$$$$$$$$$$$$EEEEE##I>iiiiiiiii>>>>>>>>>>>"~'''';;
//*********************************************K%W%%%%%%%%%%%%%%WWWW%#%@@@EKY////////////+]X&E@&KW$$$$$$$$$$$$$$$EEE###/>iiiiiiii>>>>>>>>>>""~';;;;;;
//********************************************lX%W%%%%%%%%%%%%%%WWWW%#%@@WK&&1+////////++/*&FX#FKW$$$$$$$$$$$$$$$EEE#$*i++++++iii>>>>>>>>>"!';;;;;;;'
//**********************************************$%%%%%%%%%%%%%%%%WWW%R$@%RFXFYl1/////+/}*YXYYXFXRW$WWW$$$$$$$$$$EEE#EX///++++++iiii>>iiii>!'';;;'''''
//lllll* llllllllllllllllllllllllllllllllllllll*l&%W%%%%%%%%%%%%%%W%W%K#WKFXXXXFFX1///lFFXXXYXYFY#W$WWWW%%NNN%%%%WW$$K111//////++iiiiii++i>!''''~~~~~~
//llllllllllllllllllllllllllllllllllllllllllllll* EN%%%%%%%%%%%%NNN%W%K&KXXXXXFKF&]+/1XFKFXXXYX&XEWWWW%N@@@@@@@NN%W$WRl]}1 / 111/////111////i>!~'~~~!!!"
//]l]lll]llllllllllllllll]llllllllllllll]ll]l]ll]lFEW %%%% NN@@@@@@@NW %#X#&FXXFKRXFl//1XX&&XXXXK&F$WWW%@@NNN%%%WW$$$$EE#RKXl11}1111lYIl]}}11/i"!!!!"">>
//]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]}]*I &$NNNN@@@@@@@@@@%% EXKRFFF &&&& XY//}XXX&&XF&&XKWWWW%%WWNNN%W$$$$EE####R#R&l}]]}]**]]]]]]]}}/+iiiii++
//]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]}]X#$%%%%%%%%%%%%NNN@N%W$F&EKK&FFFKXFl>]FXXX&&KF&XEWWWWW$WW%%WW$$$$EEEE###RRRRRI}]]]l]]]]]]]]]}}1///////
//]]]]]]]]]]]]]]]]]]]]]]]]]]]]}]}}]]]]]]]]]}}XEWWWWWWWW %% WWWWWWWWWWWWW & X$R & XXXX && XX/*XXXXXFXFFFWWWWWWWWW$$$$$$$$$EEEE###RRRRRY}]]}]]]]]]]]}11111/////
//}}}}}}}}}}}}}}}}}}}}}}}}}}}}1 /11}}}}1}}]1*#W$$$WWWWWW%WWWWWWWWWWWWW%#Y#$FFXXXXK&F&&XXXXXXX&FRWWWWWWWWW$$$$$$$$$$EEEEE###RRR#Y}]]]]}}}}}}}111111////
//}}}}}}}}}}}}}}}}}}}}}}1}}}}1+i++111/i+1/Y$$$$WWWWWWWWWWWWWWWWWWWWWWWWFF%RFXXXXFKFX&&XXXXXX&XEWWWWWWWWW$$$$$$$$$$$$EEEE###RRR#*}]}}}}}}}}}1111111///
//}}}}}}}1}}}}}}}}}}}}1+ii+11/iii+++>!~~!Y$$$$WWWWWWWWWWWWWWWWWWWWWWWWW#YE%&FXXXX&&F&&XXXXXFFFWWWWWW$WWWW$$$$$$$$$$$$EEEEE###RRRl}}}}}}}}}}}111111///
//111111111111111111111 />"">i+i>>>>!''';l$E$$$WWWWWWWWWWWWWWWWWWWWWWWW$$FF%EFXXXX&&FKFXXXXX&XRWWWWWW$WWWWW$$$$$$$$$$$$$EEEE###R#K}111}}1}1}}1111111//
/////1/11111111111111111/i"!~~~~~'';;',/EE$$WWWWWWWWWWWWWWWWWWWWWWWWW$$WKXRNKFXXX&&FKFFXXXXFF#WWWWWW$$W$W$$$$$$$$$$$$$$$EEEE##RR#X11111111111111///++
//">i+///////////////////+i!'''';'~';;!REE$%N%WWWWWWWWWWWWWWWWWWWWWW$$W%EX&%W&XF&KFFXIYXXXKKFEWWWWWWWWW$$$$$$$$$$$WW$$$$$EEE###RR#l/111111/////+++i>>
//~!"">i+++++///////////+i"~';;;~1}~;;IEEWN@%WWWWWWWWWWWWWWWWWWWWWWW$EWW$KF$@EF*++]F}!~+XRN$FWWWWW$$WWWW$$$$$$$$WWW%NNN%$$EEE##RRR&////+i>"!!!!""""!~
//''~''~!!!!">ii+++++++i"!~';;;'!1/!;"#EWNNWWWWWWWWWWWWWWWWWWWWWWWWW$EWW%#X$@%*;;"lX*/',F@@$&$WWWW$$WWWWW$$$$$$W##RKR#$N@NW$EE##RKR*ii>!~'''''''''';;
//''''; ; ; ''~~!">ii+++i>!~'';;;;;'''',/E$%WWWWWWWWWWWWWWWWWWWWWWWWWWWEEWWWWFR@%&]~>lXY}"XEN @W&$WWWW$$WWWWWWW$$$$#E$EIl*RKW@@N%W$#RRRF"~~''''''''';;;;;
//; ; ;;;;;;'''~~!>>iii>!~';;;;;;;;;;,`l$$$$$WW%%WWWWWWWWWWWWWWWWWWWWWEEWWWW&KNN%#Fl*l]*#%$%@%&E%WWWW$WWWWWWWWWW$IR##YlIK*Y%%N@@N%$#KK+'''';;;;;;;;;;;;
//; ; ;;;;;;;;''''~~!"""~'';;;;;;;;;,"*#EE$WW%NN%%WWWWWWWWWWWWWWWWWWWWEEWWW%K&N@WW$RKYRWW$W%@NKR%WWWW$WWWWWWWWW$W&lIIFFXI*#$$$WNNNN%$E];';;;;;;;;;;;;;;
//; ; ;;;;;;;;;;;;;'''~~'';;;;;;;;;']REE$W%N@@N%%%WWWWWWWWWWWWWWWWWWWW$EWWW%R&N@%WWE#%W$WWW%NNE&WWWWW$$WWWWWWWWW$WEKK&&KR$$$$$$$$W%%%%&~',;;;;;;;;;;;,,
//; ; ;;;;;;;;;;;;;;;;;;;';;;;;;;;;YEE$W%N@@N%WWWWWWWWWWWWWWWWWWWWWWWWE$WWW%KK%%NN%%%WWWW$WN%NWF#%WWWW$WWWWWWWW%WW%%WWWWW$$$$$$$EE####RF*/';;;;;;,;;;;,
//; ; ;;;;;;;;;;;;;;;;;;;;;;;;;;;,>E$%NNN@N%WW%WWWWWWWWWWWWWWWWWWWWWWWE$WW%$F#N%%NNNWW$E##WW%%NK&WWWWW$WWWWWWWERFYl*YFK#W$$$$$$$EE##RK&&FF",;;;;;;;;;;;
//; ; ;;;;;;;;;;;;;;;;;;;;;;;;;;;;!E%%NNN%WWWWWWWWWWWWWWWWWWWWWWWWWWW$#$WW%RFWNW$$WNWRFXXREEW%NEFE%WWW$WWWWWEX]}}1Yl/}/1I#$$$$$$$EE##RKK&K>,;;;;;;;;;;;
//; ; ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;FN%NN%WWWWWWWW%%%WWWWWWWWWWWWWWWWWE#$WW$FKN%$E#E$&YIIY#EE$%NWFK%WWW$$WW%#}1Il}IFXI1**i1R$$$$$$$WWW$$$$E",;;;;;;;;;;;
//; ; ;;;;;;;;;;;;;;;;;;;;;;'''''',>%%WWWWWWW%%%%%%%%%WWWWWWWWWWWWWWWE#$W%RX$NWEE#E&YIIIX##EE%N%KF$WWW$$WWW*]Y**RW$WWKl]X}]E$$$$$$$W%%%%WR~;;;;;;;;;;;;
//; ; ;;;;;'''';;;;;;';;;''''''~';>Y$$$WWWW%W%%%%%%%%%WWWWWWWWWWWWWWW$#$%$FKN%$E#ERYI*IYRE#EEW%N#X#%WWW$WW$]]X}l#%WW%&}]*//E$$$$$$EEEE$$%R',;;;;;;;;;;;
//; ; ;;'''''''';;;;''''''~~~~~~~*E$$$WWWWW%%%%%%%%%%%%WWWWWWWWWWWWWWWWW%RF$NWEEE#XYIIXRE#EEE$%NWF&WWWWWWW%F++YI1*XXl1*I/1XW$$$$$$$EE####RY]";'';;;;;;;
//; ''''''''''''';;;'~~~~~~~~!'*$E$$WWWWW%%%%%%%%%%%%%%%WWWWWWWWWWWWWW%$F&N%$E#E&YIYXRE#EEEE$%NN&XE%WWWWWWW##EE$*l*l$$$EK%WWW$$$$$EE###RKK&F";;;;;;;;;

//来自https://github.com/Rainbow-Dreamer/ascii-converter（图片视频转ASCII字符）
//另一个魔女（不过毫不相干
