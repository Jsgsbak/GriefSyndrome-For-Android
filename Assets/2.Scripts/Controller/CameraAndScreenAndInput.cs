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

    private void Start()
    {
        //从游戏设置中获取按键位置和大小
        Button = new bool[StageCtrl.gameScoreSettings.KeyPosScale.Length];
       CorrectScreenInput();

        //缓存一下长度
         ButtonLength = StageCtrl.gameScoreSettings.KeyPosScale.Length;

        

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
        if(Time.timeScale == 0)
        {
            return;
        }



        for (int i = 0; i < ButtonLength; i++)
        {
            if(i == 0 | i == 1 | i == 2 | i == 3 & StageCtrl.gameScoreSettings.UseScreenInput == 2)
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
                //不允许长按的键
                Button[i] = GUI.Button(StageCtrl.gameScoreSettings.KeyPosScale[i].PositionInUse, StageCtrl.gameScoreSettings.KeyPosScale[i].UIName);

            }

            //单独为移动写的
            if (Button[i])
            {
                switch (i)
                {
                    //左移
                    case 0:
                        StageCtrl.gameScoreSettings.Horizontal = -1;
                        break;
                        //右移
                    case 1:
                        StageCtrl.gameScoreSettings.Horizontal = 1;
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0:
                        StageCtrl.gameScoreSettings.Horizontal = 0;
                        break;
                    case 1:
                        StageCtrl.gameScoreSettings.Horizontal = 0;
                        break;
                }

            }



        }

        //布尔值得按键输入
        StageCtrl.gameScoreSettings.Up = Button[2];
        StageCtrl.gameScoreSettings.Down = Button[3];
        StageCtrl.gameScoreSettings.Jump = Button[4];
        StageCtrl.gameScoreSettings.Zattack = Button[5];
        StageCtrl.gameScoreSettings.Xattack = Button[6];
        StageCtrl.gameScoreSettings.Magia = Button[7];
        StageCtrl.gameScoreSettings.Pause = Button[8];
        StageCtrl.gameScoreSettings.CleanSoul = Button[9];
        StageCtrl.gameScoreSettings.CleanVit = Button[10];
        StageCtrl.gameScoreSettings.HurtMyself = Button[11];







    }


    /// <summary>
    /// /修正虚拟按键位置
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
}
