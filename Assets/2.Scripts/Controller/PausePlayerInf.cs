using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.U2D;

public class PausePlayerInf : MonoBehaviour
{
    int MahouShoujoId = -1;
    public TMP_Text text;
    public Image PlayerImage;

    private void OnEnable()
    {
        //放置一开游戏就调用这个函数而引发错误
        if(MahouShoujoId != -1)
        {
            text.text = string.Format("{0}\n\n\nLevel:{1}\n\n\nVit:{2}\n\n\nSoul:{3}\n\n\nPow:{4}\n",
                                        name,StageCtrl.gameScoreSettings.GirlsLevel[MahouShoujoId].ToString(),
                                       StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId].ToString(),StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId].ToString(),StageCtrl.gameScoreSettings.GirlsPow[MahouShoujoId].ToString() );
        }
    }


    [ContextMenu("设置名称")]
    public void SetNameAndImage(string Name,SpriteAtlas spriteAtlas)
    {
      PlayerImage.sprite = spriteAtlas.GetSprite(Name);

        if (Name == string.Empty)
        {
            MahouShoujoId = 4;
            Name = "Miki Sayaka";
        }
        else if (Name.Equals("Homura") | Name.Equals("Homura_m"))
        {
            MahouShoujoId = 0;
            Name = "Akemi Homura";
        }
        else if (Name.Equals("Kyoko"))
        {
            MahouShoujoId = 1;
            Name = "Sakura Koyko";
        }
        else if (Name.Equals("Madoka"))
        {
            MahouShoujoId = 2;
            Name = "Kaname Madoka";
        }
        else if (Name.Equals("Mami"))
        {
            MahouShoujoId = 3;
            Name = "Tomoe Mami";
        }
        else if (Name == "Sayaka")
        {
            MahouShoujoId = 4;
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
