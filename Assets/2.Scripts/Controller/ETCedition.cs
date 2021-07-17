﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 这个脚本是挂在每一个ETC 控件上的，用于更新图片与响应操作012
/// </summary>
public class ETCedition : MonoBehaviour
{
    //每个角色不同的图标
    public Sprite[] ActionLogos = new Sprite[6];
    ETCButton ETCButton;
    RectTransform tr;
    /// <summary>
    /// 是否在标题编辑状态
    /// </summary>
    public bool TitleSettingMode = false;

    public enum ETCActions
    {
        Jump = 2,
        Weak = 3,
        Strong = 4,
        Magia = 5,
        Pause = 1,
        Joystick = 0,
    }

    /// <summary>
    /// 这个按钮是干啥的
    /// </summary>
    public ETCActions buttonActions;

    //一下为标题用
    public TMP_InputField[] Rect;

    float ClickTime = 0f;


    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<RectTransform>();
        //更新自身的Rect
        UpdateRect();

        //费标题编辑模式，是游戏模式
        if (!TitleSettingMode)
        {
            //获取组件
            if (buttonActions == ETCActions.Joystick || buttonActions == ETCActions.Pause)
            {
                return;
            }

            ETCButton = GetComponent<ETCButton>();

            //更新按钮图片（摇杆不支持）
            ETCButton.normalSprite = ActionLogos[MountGSS.gameScoreSettings.PlayerSelectedGirlId];
            ETCButton.pressedSprite = ActionLogos[MountGSS.gameScoreSettings.PlayerSelectedGirlId];


        }


    }

    /// <summary>
    /// 标题单击事件（用于编辑）
    /// </summary>
    public void OnClickForTitle()
    {
        //传递按钮ID，便于保存
        TitleInputView.EditingButton = (int)buttonActions;

        //点击之后在编辑栏中显示属性（大小和位置）
        Rect[0].text = TitleCtrl.gameScoreSettingsIO.KeyPosScale[(int)buttonActions].EditPosition.x.ToString();
        Rect[1].text = TitleCtrl.gameScoreSettingsIO.KeyPosScale[(int)buttonActions].EditPosition.y.ToString();
        Rect[2].text = TitleCtrl.gameScoreSettingsIO.KeyPosScale[(int)buttonActions].EditPosition.width.ToString();
        Rect[3].text = TitleCtrl.gameScoreSettingsIO.KeyPosScale[(int)buttonActions].EditPosition.height.ToString();
    }

    private void Update()
    {
        //仅在标题界面编辑的时候才能即时更新
        if (TitleSettingMode)
        {
            UpdateRect();
        }
    }


    /// <summary>
    /// 更新自身的Rect
    /// </summary>
    void UpdateRect()
    {
        GameScoreSettingsIO gss;

        //处于标题编辑模式
        if (TitleSettingMode)
        {
            gss = TitleCtrl.gameScoreSettingsIO;
        }
        //游戏模式
        else
        {
            gss = MountGSS.gameScoreSettings;
        }

        tr.anchoredPosition = new Vector2(gss.KeyPosScale[(int)buttonActions].EditPosition.x, gss.KeyPosScale[(int)buttonActions].EditPosition.y);
        tr.sizeDelta = new Vector2(gss.KeyPosScale[(int)buttonActions].EditPosition.width, gss.KeyPosScale[(int)buttonActions].EditPosition.height);
    }

    /// <summary>
    /// 虚拟按键输入
    /// </summary>
    /// <param name="vector2"></param>
    public void OnMove(Vector2 vector2)
    {


        //这个是为了得到速度的方向以及兼容其他的输入方式
        if (vector2.x > 0.3f)
        {
            MountGSS.gameScoreSettings.Horizontal = 1;
        }
        else if (vector2.x < -0.3f)
        {
            MountGSS.gameScoreSettings.Horizontal = -1;
        }
        else if (vector2.x <= 0.3f || vector2.x >= -0.3f)
        {
            MountGSS.gameScoreSettings.Horizontal = 0;
        }
        MountGSS.gameScoreSettings.Up = vector2.y >= 0.7f;
        MountGSS.gameScoreSettings.Down = vector2.y <= -0.7f;

        MountGSS.gameScoreSettings.joystick = vector2;

    }

    public void MoveEnd()
    {
        MountGSS.gameScoreSettings.Up = false;
        MountGSS.gameScoreSettings.Down = false;
        MountGSS.gameScoreSettings.Horizontal = 0;
        MountGSS.gameScoreSettings.joystick = Vector2.zero; ;
    }


    //无论是否支持长按，都在这里设置为true
    public void OnDown()
    {
        if (buttonActions == ETCActions.Pause)
        {
            MountGSS.gameScoreSettings.Pause = true;
        }

        //僵直状态仅允许暂停游戏
        if (MountGSS.gameScoreSettings.LocalIsStiff)
        {
            return;
        }

        //单点的按键有时间间隔
        if (Time.timeSinceLevelLoad - ClickTime >= 0.1f)
        {

            ClickTime = Time.timeSinceLevelLoad;

            switch (buttonActions)
            {
                case ETCActions.Jump:
                    MountGSS.gameScoreSettings.Jump = true;
                    break;
                case ETCActions.Weak:
                    MountGSS.gameScoreSettings.Zattack = true;
                    break;
                case ETCActions.Strong:
                    MountGSS.gameScoreSettings.Xattack = true;
                    break;
                case ETCActions.Magia:
                    MountGSS.gameScoreSettings.Magia = true;
                    break;
            }
        }


        switch (buttonActions)
        {
            case ETCActions.Weak:
                MountGSS.gameScoreSettings.ZattackPressed = true;
                break;
            case ETCActions.Strong:
                MountGSS.gameScoreSettings.XattackPressed = true;
                break;
            case ETCActions.Magia:
                MountGSS.gameScoreSettings.MagiaPressed = true;
                break;
        }


    }

    //不支持长按的按钮在这里设置为false
    public void OnPress()
    {
        switch (buttonActions)
        {
            case ETCActions.Jump:
                MountGSS.gameScoreSettings.Jump = false;
                break;
            case ETCActions.Weak:
                MountGSS.gameScoreSettings.Zattack = false;
                break;
            case ETCActions.Strong:
                MountGSS.gameScoreSettings.Xattack = false;
                break;
            case ETCActions.Magia:
                MountGSS.gameScoreSettings.Magia = false;
                break;
            case ETCActions.Pause:
                MountGSS.gameScoreSettings.Pause = false;
                break;
        }
    }

    //支持长按的按钮在这里设置为false
    public void OnUp()
    {
        switch (buttonActions)
        {
            case ETCActions.Weak:
                MountGSS.gameScoreSettings.ZattackPressed = false;
                break;
            case ETCActions.Strong:
                MountGSS.gameScoreSettings.XattackPressed = false;
                break;
            case ETCActions.Magia:
                MountGSS.gameScoreSettings.MagiaPressed = false;
                break;

        }
    }


}
