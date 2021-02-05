using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主相机/屏幕（显示）/虚拟按键之类的控制
/// </summary>
public class CameraAndScreenAndInput : MonoBehaviour
{
    bool[] Button;

    private void Start()
    {
        //从游戏设置中获取按键位置和大小
        Button = new bool[StageCtrl.gameScoreSettings.KeyPosScale.Length];
       CorrectScreenInput();
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

        //缓存一下
        int ButtonLength = StageCtrl.gameScoreSettings.KeyPosScale.Length;
        for (int i = 0; i < ButtonLength; i++)
        {
            //按键名字设置（UI上显示的）   预先弄一个布尔值是为了让那些按钮一直显示（并且方便后续处理）
            //除了部分按键外（跳跃键，暂停键，测试用soul清零键，测试用hp清零键，测试用通关键），所有按键都使用RepeatButton，需要得到“按下”属性的在脚本中自己限制
            Button[i] = GUI.RepeatButton(StageCtrl.gameScoreSettings.KeyPosScale[i].PositionInUse, StageCtrl.gameScoreSettings.KeyPosScale[i].UIName);
           
            //每个按钮的事件在这里写
            if (Button[i])
            {
                switch (i)
                {
                    case 0:
                        StageCtrl.gameScoreSettings.Horizontal = -1;
                        break;
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
