using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.U2D;

public class PausePlayerInf : MonoBehaviour
{
    public int PlayerId = -1;
    public TMP_Text text;
    public Image PlayerImage;

    private void OnEnable()
    {
        //放置一开游戏就调用这个函数而引发错误
        if(PlayerId != -1)
        {
            text.text = string.Format("{0}\n\nLevel:{2}\n\nVit:{3}\n\nSoul:{4}\n\nPow:{5}\n",
                                        name,PlayerId.ToString(),StageCtrl.gameScoreSettings.Level[PlayerId - 1].ToString(),
                                       StageCtrl.gameScoreSettings.VitInGame[PlayerId -1].ToString(),StageCtrl.gameScoreSettings.SoulLimitInGame[PlayerId -1].ToString(),StageCtrl.gameScoreSettings.PowInGame[PlayerId  -1].ToString() );
        }
    }


    [ContextMenu("设置名称")]
    public void SetNameAndImage(string Name,SpriteAtlas spriteAtlas)
    {
      PlayerImage.sprite = spriteAtlas.GetSprite(Name);

        if (Name == string.Empty)
        {
            Name = "Miki Sayaka";
        }
        else if (Name.Equals("Homura") | Name.Equals("Homura_m"))
        {
            Name = "Akemi Homura";
        }
        else if (Name.Equals("Kyoko"))
        {
            Name = "Sakura Koyko";
        }
        else if (Name.Equals("Madoka"))
        {
            Name = "Kaname Madoka";
        }
        else if (Name.Equals("Mami"))
        {
            Name = "Tomoe Mami";
        }
        else if (Name == "Sayaka")
        {
            Name = "Miki Sayaka";
        }
        else
        {
            //以防万一，剔除qb
            Destroy(gameObject);
        }

        name = Name;
    }

}
