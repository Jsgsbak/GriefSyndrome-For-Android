using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主相机控制以及屏幕控制器UI
/// </summary>
public class ScreenInput : MonoBehaviour
{
    [Header("定位用安卓控制UI")]
    public Rect LeftButton;
    bool Left;
    public Rect RightButton;
    bool Right;


    string info;
    // Start is called before the first frame update
    #region 安卓和编辑器创建按钮虚拟按键
    void OnGUI()
    {
        //游戏暂停时停止绘制
        if(Time.timeScale == 0)
        {
            return;
        }

        //方向键
        Left = GUI.RepeatButton(LeftButton, "←"); Right = GUI.RepeatButton(RightButton, "→");
        if (Left)
        {
            StageCtrl.gameScoreSettings.Horizontal = -1;
        }
        else if (Right)
        {
            StageCtrl.gameScoreSettings.Horizontal = 1;
        }
        else
        {
            StageCtrl.gameScoreSettings.Horizontal = 0;
        }


    }
    #endregion

}
