using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主相机/屏幕（显示）/虚拟按键之类的控制
/// </summary>
public class CameraAndScreenAndInput : MonoBehaviour
{
    bool[] Button;
    int ButtonLength;
    public GameObject Joystick;
    /// <summary>
    /// 禁用绘制虚拟按键/摇杆
    /// </summary>
    bool BanDrawingInput = false;
    /// <summary>
    /// 游戏结束（3个魔法少女都死了或者魔女打死了）
    /// </summary>
    bool GameOver = false;

    private void Start()
    {
        //从游戏设置中获取按键位置和大小
        Button = new bool[StageCtrl.gameScoreSettings.KeyPosScale.Length];
       CorrectScreenInput();
        
        //根据需要卸载虚拟按键
        if(StageCtrl.gameScoreSettings.UseScreenInput == 2)
        {
            Joystick.SetActive(true);       
            //游戏一开始先运行一下，修复不能停止移动的bug
            MoveEnd();

        }
        else
        {
            DestroyImmediate(Joystick);
        }

        //缓存一下长度
         ButtonLength = StageCtrl.gameScoreSettings.KeyPosScale.Length;

        //注册玩家死亡事件（自己控制的玩家死亡）
        APlayerCtrl.PlayerGemBroken.AddListener(delegate () { BanDrawingInput = true; });
        //成功打完魔女
        StageCtrl.stageCtrl.MajoDefeated.AddListener(delegate () { GameOver = true; });
        //玩家全死了
        StageCtrl.stageCtrl.AllGirlsInGameDie.AddListener(delegate () { GameOver = true; });

    }

#if UNITY_EDITOR
    private void Update()
    {
        CorrectScreenInput();

    }
#endif

    // Start is called before the first frame update
    void OnGUI()
    {
        //游戏暂停时停止绘制
        if(Time.timeScale == 0 || GameOver)
        {
            if (StageCtrl.gameScoreSettings.UseScreenInput == 2) { Joystick.SetActive(false); }
            return;
        }

        //所选玩家死亡，不绘制按钮了，只绘制一个暂停按钮
        if (BanDrawingInput)
        {
            Button[8] = GUI.Button(StageCtrl.gameScoreSettings.KeyPosScale[8].PositionInUse, StageCtrl.gameScoreSettings.KeyPosScale[8].UIName);
            if (Button[8]) StageCtrl.gameScoreSettings.Pause = true;
            if (StageCtrl.gameScoreSettings.UseScreenInput == 2) { Joystick.SetActive(false); }
            return;
        }

        if (StageCtrl.gameScoreSettings.UseScreenInput == 2) { Joystick.SetActive(Time.timeScale != 0); }
       


        for (int i = 0; i < ButtonLength; i++)
        {
            if(i == 0 | i == 1 | i == 2 | i == 3 && StageCtrl.gameScoreSettings.UseScreenInput == 2)
            {
                //使用虚拟摇杆的时候，直接跳过箭头的绘制
                continue;
            }



            //按键名字设置（UI上显示的）   预先弄一个布尔值是为了让那些按钮一直显示（并且方便后续处理）
            //除了部分按键外（跳跃键，暂停键，测试用soul清零键，测试用hp清零键，测试用通关键），所有按键都使用RepeatButton，需要得到“按下”属性的在脚本中自己限制
             if(StageCtrl.gameScoreSettings.KeyPosScale[i].AllowPress)
            {
                //允许长按的键
                Button[i] = GUI.RepeatButton(StageCtrl.gameScoreSettings.KeyPosScale[i].PositionInUse, StageCtrl.gameScoreSettings.KeyPosScale[i].UIName);
            }
            else 
            {
                Button[i] = GUI.Button(StageCtrl.gameScoreSettings.KeyPosScale[i].PositionInUse, StageCtrl.gameScoreSettings.KeyPosScale[i].UIName);
            }
        }

        //单独为移动写的
        if (Button[0])
        {
            //左移
            StageCtrl.gameScoreSettings.Horizontal = -1;

        }
        else if (Button[1])
        {
            //右移
            StageCtrl.gameScoreSettings.Horizontal = 1;
        }
        //布尔值得按键输入
        StageCtrl.gameScoreSettings.Up = Button[2];
        StageCtrl.gameScoreSettings.Down = Button[3];
        StageCtrl.gameScoreSettings.Jump = Button[4];
        StageCtrl.gameScoreSettings.Zattack = Button[5];
        StageCtrl.gameScoreSettings.Xattack = Button[6];
        StageCtrl.gameScoreSettings.Magia = Button[7];
        if(Button[8]) StageCtrl.gameScoreSettings.Pause = true;
        //0.0.7版测试用
        StageCtrl.gameScoreSettings.CleanSoul = Button[9];
        StageCtrl.gameScoreSettings.CleanVit = Button[10];
        StageCtrl.gameScoreSettings.HurtMyself = Button[11];
        StageCtrl.gameScoreSettings.Succeed = Button[12];

    }


    /// <summary>
    /// /修正虚拟按键Rect（大小位置）
    /// </summary>
    void CorrectScreenInput()
    {
        float CorrectWidth = (float)Screen.width / 1010f;
        float CorrectHeight = (float)Screen.height / 568f;


        for (int i = 0; i < StageCtrl.gameScoreSettings.KeyPosScale.Length; i++)
        {
            StageCtrl.gameScoreSettings.KeyPosScale[i].PositionInUse = new Rect(StageCtrl.gameScoreSettings.KeyPosScale[i].EditPosition.x * CorrectWidth, StageCtrl.gameScoreSettings.KeyPosScale[i].EditPosition.y * CorrectHeight, StageCtrl.gameScoreSettings.KeyPosScale[i].EditPosition.width * CorrectWidth, StageCtrl.gameScoreSettings.KeyPosScale[i].EditPosition.height* CorrectHeight);
        }


    }

    /// <summary>
    /// 虚拟按键输入
    /// </summary>
    /// <param name="vector2"></param>
    public void JoystickInput(Vector2 vector2)
    {
        //这个是为了得到速度的方向以及兼容其他的输入方式
        if(vector2.x > 0.3f)
        {
            StageCtrl.gameScoreSettings.Horizontal = 1;
        }
        else if (vector2.x < -0.3f)
        {
            StageCtrl.gameScoreSettings.Horizontal = -1;
        }
        else if(vector2.x <= 0.3f || vector2.x >= -0.3f)
        {
            StageCtrl.gameScoreSettings.Horizontal = 0;
        }
        StageCtrl.gameScoreSettings.Up = vector2.y >= 0.8f;
        StageCtrl.gameScoreSettings.Down = vector2.y <= -0.8f;

        StageCtrl.gameScoreSettings.joystick = vector2;

    }

    public void MoveEnd()
    {
        StageCtrl.gameScoreSettings.Up = false;
        StageCtrl.gameScoreSettings.Down = false;
        StageCtrl.gameScoreSettings.Horizontal = 0;
        StageCtrl.gameScoreSettings.joystick = Vector2.zero; ;
    }
}
