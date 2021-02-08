using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 主相机/屏幕（显示）/虚拟按键之类的控制
/// </summary>
public class TitleInputView : MonoBehaviour
{
    bool[] Button;
    int ButtonLength;
    public ETCJoystick Joystick;

    public Toggle[] InputToggle;

    public TMP_InputField[] Rect;

    int EditingButton;

    public GameObject InputSetting;
    public GameObject MainTitle;

    private void Start()
    {
        //注册事件，按钮一旦修改就保存到GSS中
        Rect[0].onValueChanged.AddListener(delegate (string c) {
            TitleCtrl.gameScoreSettingsIO.KeyPosScale[EditingButton].EditPosition.x = float.Parse(Rect[0].text);
        });
        Rect[1].onValueChanged.AddListener(delegate (string c) {
            TitleCtrl.gameScoreSettingsIO.KeyPosScale[EditingButton].EditPosition.y = float.Parse(Rect[1].text);
        });
        Rect[2].onValueChanged.AddListener(delegate (string c) {
            TitleCtrl.gameScoreSettingsIO.KeyPosScale[EditingButton].EditPosition.width = float.Parse(Rect[2].text);
        });
        Rect[3].onValueChanged.AddListener(delegate (string c) {
            TitleCtrl.gameScoreSettingsIO.KeyPosScale[EditingButton].EditPosition.height = float.Parse(Rect[3].text);
        });
    }
    private void OnEnable()
    {
        //所有的 -5 都是为了消除0.0.7版本中测试用的按钮

        //从游戏设置中获取按键位置和大小
        Button = new bool[TitleCtrl.gameScoreSettingsIO.KeyPosScale.Length - 4];
        CorrectScreenInput();
       
        //缓存一下长度
        ButtonLength = TitleCtrl.gameScoreSettingsIO.KeyPosScale.Length - 4;

        //更新开关状态
        InputToggle[TitleCtrl.gameScoreSettingsIO.UseScreenInput].isOn = true;
    }

    private void Update()
    {
        //开关状态应用 0.0.7 先凑合着用
        for (int i = 0; i < 3; i++)
        {
            if (InputToggle[i].isOn)
            {
                TitleCtrl.gameScoreSettingsIO.UseScreenInput = i;
                break;
            }
        }

#if UNITY_EDITOR
        CorrectScreenInput();
#endif
    }

    // Start is called before the first frame update
    void OnGUI()
    {
        if(TitleCtrl.gameScoreSettingsIO.UseScreenInput == 0)
        {
            Joystick.gameObject.SetActive(TitleCtrl.gameScoreSettingsIO.UseScreenInput == 2);
            return;
        }
        Joystick.gameObject.SetActive(TitleCtrl.gameScoreSettingsIO.UseScreenInput == 2);


        for (int i = 0; i < ButtonLength; i++)
        {
            if (i == 0 | i == 1 | i == 2 | i == 3 && TitleCtrl.gameScoreSettingsIO.UseScreenInput == 2)
            {
                //使用虚拟摇杆的时候，直接跳过箭头的绘制
                continue;
            }



            //按键名字设置（UI上显示的）   预先弄一个布尔值是为了让那些按钮一直显示（并且方便后续处理）
            //除了部分按键外（跳跃键，暂停键，测试用soul清零键，测试用hp清零键，测试用通关键），所有按键都使用RepeatButton，需要得到“按下”属性的在脚本中自己限制
            if (TitleCtrl.gameScoreSettingsIO.KeyPosScale[i].AllowPress)
            {
                //允许长按的键
                Button[i] = GUI.RepeatButton(TitleCtrl.gameScoreSettingsIO.KeyPosScale[i].PositionInUse, TitleCtrl.gameScoreSettingsIO.KeyPosScale[i].UIName);
            }
            else
            {
                Button[i] = GUI.Button(TitleCtrl.gameScoreSettingsIO.KeyPosScale[i].PositionInUse, TitleCtrl.gameScoreSettingsIO.KeyPosScale[i].UIName);
            }


            if (Button[i])
            {
                EditingButton = i;
                ShowRectEdit(i);
            }
        }


    }

    /// <summary>
    /// 显示编辑RECT的四个框，并同步RECT显示
    /// </summary>
    void ShowRectEdit(int index)
    {
        Rect[0].text = TitleCtrl.gameScoreSettingsIO.KeyPosScale[index].EditPosition.x.ToString();
        Rect[1].text = TitleCtrl.gameScoreSettingsIO.KeyPosScale[index].EditPosition.y.ToString();
        Rect[2].text = TitleCtrl.gameScoreSettingsIO.KeyPosScale[index].EditPosition.width.ToString();
        Rect[3].text = TitleCtrl.gameScoreSettingsIO.KeyPosScale[index].EditPosition.height.ToString();
    }


    /// <summary>
    /// 把当前单个按钮设置保存到GSS（检查视图注入）
    /// </summary>
    public void SaveToGSS(int index)
    {
        TitleCtrl.gameScoreSettingsIO.KeyPosScale[index].EditPosition.y = float.Parse(Rect[1].text);
        TitleCtrl.gameScoreSettingsIO.KeyPosScale[index].EditPosition.width = float.Parse(Rect[2].text);
        TitleCtrl.gameScoreSettingsIO.KeyPosScale[index].EditPosition.height = float.Parse(Rect[3].text);

    }

    /// <summary>
    /// 保存设置到存档
    /// </summary>
    void SaveToFile()
    {
        TitleCtrl.gameScoreSettingsIO.SaveInput();
    }

    /// <summary>
    /// 撤销所有变化
    /// </summary>
    public void RevokeAllChange()
    {
        TitleCtrl.gameScoreSettingsIO.RevokeInputChange();
    }

    public void BackToTitle()
    {
        SaveToFile();
        InputSetting.SetActive(false);
        MainTitle.SetActive(true);
    }

    /// <summary>
    /// /修正虚拟按键Rect（大小位置）
    /// </summary>
    void CorrectScreenInput()
    {
        float CorrectWidth = (float)Screen.width / 1010f;
        float CorrectHeight = (float)Screen.height / 568f;


        for (int i = 0; i < TitleCtrl.gameScoreSettingsIO.KeyPosScale.Length; i++)
        {
            TitleCtrl.gameScoreSettingsIO.KeyPosScale[i].PositionInUse = new Rect(TitleCtrl.gameScoreSettingsIO.KeyPosScale[i].EditPosition.x * CorrectWidth, TitleCtrl.gameScoreSettingsIO.KeyPosScale[i].EditPosition.y * CorrectHeight, TitleCtrl.gameScoreSettingsIO.KeyPosScale[i].EditPosition.width * CorrectWidth, TitleCtrl.gameScoreSettingsIO.KeyPosScale[i].EditPosition.height * CorrectHeight);
        }


    }
   
}
