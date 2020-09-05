﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// （隶属于UICtrl）单独拿出一个玩家信息更新，方便维护
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
    public Image Magia;//魔法放在前面
    public Image Rebirth;
    public Image SoulGem;

    public Sprite[] SoulGems;

    private void Awake()
    {
        UICtrl[] go = FindObjectsOfType<UICtrl>();
        if(go.Length != 1)
        {
            Debug.LogError("PlayerInfUpdate can't work,because the number of UICtrl isn't proper");
            this.enabled = false;
        }
    }

    public void RegEvent()
    {
        UICtrl.UpdateInf.AddListener(UpdateInf);

    }
    public void UpdateInf()
    {
        UpdateScore();
        UpdateLevel();
        UpdateHPBar();
        UpdateSoulLimit();        
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


    public void UpdateHPBar()
    {
        if(!StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId - 1])
        {

            if (StageCtrl.gameScoreSettings.GetHurtInGame[PlayerId - 1])
            {
                Magia.fillAmount = 0;
                Damaged.fillAmount = 1;
                StageCtrl.gameScoreSettings.GetHurtInGame[PlayerId - 1] = false;
            }
            else if (StageCtrl.gameScoreSettings.MagiaKeyDown[PlayerId - 1] && Magia.fillAmount < Health.fillAmount)
            {
                StageCtrl.gameScoreSettings.MagiaKeyDown[PlayerId - 1] = false;
                Magia.fillAmount = Health.fillAmount;
            }

            //放在这里的话Magia就能变得正常了
            Health.fillAmount = (float)StageCtrl.gameScoreSettings.VitInGame[PlayerId - 1] / (float)StageCtrl.gameScoreSettings.MaxVitInGame[PlayerId - 1];

        }
        else if(!StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId - 1] && StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId - 1])
        {
            Magia.fillAmount = 0;
            Damaged.fillAmount = 0;
            Health.fillAmount = 0; 
            Rebirth.fillAmount = 0;

        }
        //复活处理
       else if (StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId - 1] && StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId - 1])
        {
            //  Rebirth.fillAmount = (float)StageCtrl.gameScoreSettings.VitInGame[PlayerId - 1] / (float)StageCtrl.gameScoreSettings.MaxVitInGame[PlayerId - 1];
            //一秒七次，3秒回满血，恢复21次
            Rebirth.fillAmount = Rebirth.fillAmount + 1f / 20f;

        }
        /*旧版
        //魔法hp条减少
        if (StageCtrl.gameScoreSettings.MagiaVitInGame[PlayerId - 1] > 0 && MagiaForward.fillAmount - Health.fillAmount < -0.01f)
        {
            MagiaForward.fillAmount = Health.fillAmount + (float)StageCtrl.gameScoreSettings.MagiaVitInGame[PlayerId - 1] / (float)StageCtrl.gameScoreSettings.MaxVitInGame[PlayerId - 1];
            MagiaBack.fillAmount = Health.fillAmount + (float)StageCtrl.gameScoreSettings.MagiaVitInGame[PlayerId - 1] / (float)StageCtrl.gameScoreSettings.MaxVitInGame[PlayerId - 1];

        }
        //魔法 hp变长（二者几乎相等时一起变长）
        else if (Mathf.Abs(MagiaForward.fillAmount - Health.fillAmount) <= 0.01f)
        {
            MagiaForward.fillAmount = Health.fillAmount + (float)StageCtrl.gameScoreSettings.MagiaVitInGame[PlayerId - 1] / (float)StageCtrl.gameScoreSettings.MaxVitInGame[PlayerId - 1];
            MagiaBack.fillAmount = Health.fillAmount + (float)StageCtrl.gameScoreSettings.MagiaVitInGame[PlayerId - 1] / (float)StageCtrl.gameScoreSettings.MaxVitInGame[PlayerId - 1];

        }


        //受伤hp条减少
        if (StageCtrl.gameScoreSettings.HurtVitInGame[PlayerId - 1] > 0 && Damaged.fillAmount - Health.fillAmount < -0.01f)
        {
            Damaged.fillAmount = Health.fillAmount + (float)StageCtrl.gameScoreSettings.HurtVitInGame[PlayerId - 1] / (float)StageCtrl.gameScoreSettings.MaxVitInGame[PlayerId - 1];
        }
        //魔法hp变长（二者几乎相等时一起变长）
        else if (Mathf.Abs(MagiaForward.fillAmount - Health.fillAmount) <= 0.01f)
        {
            MagiaForward.fillAmount = Health.fillAmount;
        }


        //根据长度，显示先后修正
        MagiaForward.enabled = MagiaForward.fillAmount < Damaged.fillAmount;
        MagiaBack.enabled = MagiaForward.fillAmount > Damaged.fillAmount;
        */

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
