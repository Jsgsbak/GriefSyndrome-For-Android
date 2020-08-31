using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 单独拿出一个玩家信息更新，方便维护
/// </summary>
public class PlayerInfUpdate : MonoBehaviour
{


  [HideInInspector]  public int PlayerId = 1;


    //图片
    public TMP_Text Score;
    public TMP_Text Level;
    public TMP_Text SoulLimit;
    public TMP_Text PlayerName;

    public Image Health;
    public Image Damaged;
    public Image Magia;
    public Image Rebirth;
    public Image SoulGem;

    public Sprite[] SoulGems;


    public void RegEvent()
    {
        UICtrl.UpdateInf.AddListener(UpdateLevel);
        UICtrl.UpdateInf.AddListener(UpdateScore);
        UICtrl.UpdateInf.AddListener(UpdateSoulLimit);

    }



    [ContextMenu("更新分数")]
public void UpdateScore()
    {
            Score.text = string.Format("{0}p Score {1}  {2}", (PlayerId).ToString(), StageCtrl.gameScoreSettings.Score[PlayerId - 1], TitleCtrl.PlayerFaceToRichText(StageCtrl.gameScoreSettings.SelectedGirlInGame)[PlayerId - 1]);
    }

    [ContextMenu("更新等级")]
    public void UpdateLevel()
    {
        Level.text = string.Format("Lv. {0}", StageCtrl.gameScoreSettings.Level[PlayerId - 1].ToString());
    }

    [ContextMenu("更新灵魂值")]
    public void UpdateSoulLimit()
    {
        //Soul  limit <size=25> 19926</size>
        SoulLimit.text = string.Format("Soul limit  <size=25>{0}</size>", StageCtrl.gameScoreSettings.SoulLimitInGame[PlayerId - 1]);
    }

    [ContextMenu("设置名称，灵魂宝石图片 顺便剔除qb")]
    public void SetNameAndSG(string Name)
    {
        if(Name == string.Empty)
        {
            Name = "Miki Sayaka";
            SoulGem.sprite = SoulGems[4];
        }
        else if (Name.Equals("Homura") | Name.Equals("Homura_m"))
        {
            Name = "Akemi Homura";
            SoulGem.sprite = SoulGems[0];
        }
        else if(Name.Equals("Kyoko"))
        {
            Name = "Sakura Koyko";
            SoulGem.sprite = SoulGems[1];
        }
        else if (Name.Equals("Madoka"))
        {
            Name = "Kaname Madoka";
            SoulGem.sprite = SoulGems[2];
        }
        else if (Name.Equals("Mami"))
        {
            Name = "Tomoe Mami";
            SoulGem.sprite = SoulGems[3];
        }
        else if (Name == "Sayaka")
        {
            Name = "Miki Sayaka";
            SoulGem.sprite = SoulGems[4];
        }
        else
        {
            //以防万一，剔除qb
            Destroy(gameObject);
        }


        PlayerName.text = Name;
    }
}
