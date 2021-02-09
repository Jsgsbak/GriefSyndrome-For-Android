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

    public Toggle[] InputToggle;

    public TMP_InputField[] Rect;

   public static int EditingButton;

    public GameObject InputSetting;
    public GameObject MainTitle;
    public GameObject Input;

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
        //从游戏设置中获取按键位置和大小
        Button = new bool[TitleCtrl.gameScoreSettingsIO.KeyPosScale.Length];
       
        //缓存一下长度
        ButtonLength = TitleCtrl.gameScoreSettingsIO.KeyPosScale.Length;

        //更新开关状态
        InputToggle[TitleCtrl.gameScoreSettingsIO.UseScreenInput].isOn = true;

        //更新是否显示虚拟按键
        Input.SetActive(TitleCtrl.gameScoreSettingsIO.UseScreenInput == 2);

    }

    private void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (InputToggle[i].isOn)
            {
                TitleCtrl.gameScoreSettingsIO.UseScreenInput = i;

                Input.SetActive(i == 2);

                break;
            }
        }

    }
    /// <summary>
    /// 编辑框显示数据
    /// </summary>
    void EditorShow(int index)
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
}
