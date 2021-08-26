using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AdjustInputBox : MonoBehaviour
{
    public TMP_InputField inputField;
    public bool Plus = true;
    Button button;

    public int InputFieldId = 0;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        Debug.Log(inputField.text);

        if (Plus)
        {
            inputField.text = (int.Parse(inputField.text) + 1).ToString();
        }
        else
        {
            inputField.text = (int.Parse(inputField.text) - 1).ToString();
        }

        //更新gss
        if(TitleInputView.EditingButton != -1)
        {
            switch (InputFieldId)
            {
                case 0:
                    TitleCtrl.gameScoreSettingsIO.KeyPosScale[TitleInputView.EditingButton].EditPosition.x = float.Parse(inputField.text);
                    break;
                case 1:
                    TitleCtrl.gameScoreSettingsIO.KeyPosScale[TitleInputView.EditingButton].EditPosition.y = float.Parse(inputField.text);
                    break;
                case 2:
                    TitleCtrl.gameScoreSettingsIO.KeyPosScale[TitleInputView.EditingButton].EditPosition.width = float.Parse(inputField.text);
                    break;
            }
        }
    }
   
}
