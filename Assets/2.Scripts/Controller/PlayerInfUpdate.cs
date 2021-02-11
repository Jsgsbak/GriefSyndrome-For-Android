﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static APlayerCtrl;

/// <summary>
/// （隶属于UICtrl）单独拿出一个玩家信息更新，方便维护（显示在UI里）
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
    public Image SoulGemBlack;

    public Sprite[] SoulGems;

    int MahouShoujoId;

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
        UICtrl.uiCtrl.UpdateInf.AddListener(UpdateInf);

    }
    public void UpdateInf()
    {
        UpdateScore();
        UpdateLevel();
        UpdateHPBar();
        UpdateSoulLimit();
        UpdateSoulGem();
    }


    [ContextMenu("更新分数")]
public void UpdateScore()
    {
            Score.text = string.Format("{0}p Score {1}  {2}", (PlayerId).ToString(), StageCtrl.gameScoreSettings.Score[PlayerId - 1], TitleCtrl.PlayerFaceToRichText(StageCtrl.gameScoreSettings.SelectedGirlInGame)[PlayerId - 1]);
    }

    [ContextMenu("更新等级")]
    public void UpdateLevel()
    {
        Level.text = string.Format("Lv. {0}", StageCtrl.gameScoreSettings.GirlsLevel[MahouShoujoId].ToString());
    }

    [ContextMenu("更新灵魂值")]
    public void UpdateSoulLimit()
    {
        //Soul  limit <size=25> 19926</size>
        SoulLimit.text = string.Format("Soul limit  <size=25>{0}</size>",Mathf.Clamp( StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId],0,999999));
    }
    
    public void UpdateSoulGem()
    {
        float now = Mathf.Clamp(StageCtrl.gameScoreSettings.GirlSoulLimit[MahouShoujoId], 0, 999999);
        float max = StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].BasicSoulLimit + StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].SoulGrowth * (StageCtrl.gameScoreSettings.GirlsLevel[MahouShoujoId] - 1);
        SoulGemBlack.color = new Color(0f, 0f, 0f, 1 - now / max);
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

            //最后处理VIT血条
            Health.fillAmount = (float)StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] / (StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].BasicVit + Grow(StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].VitGrowth, StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].VitGrowthLevelLimit, true));

        }
        //挂了，即魔女化
        else if(!StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId - 1] && StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId - 1])
        {
            Magia.fillAmount = 0;
            Damaged.fillAmount = 0;
            Health.fillAmount = 0; 
            Rebirth.fillAmount = 0;

        }
        //复活处理
       else if (StageCtrl.gameScoreSettings.IsSoulBallInGame[PlayerId - 1] && StageCtrl.gameScoreSettings.IsBodyDieInGame[PlayerId - 1] && Time.timeScale != 0)
        {
            //  Rebirth.fillAmount = (float)StageCtrl.gameScoreSettings.GirlsVit[MahouShoujoId] / (float)StageCtrl.gameScoreSettings.StageCtrl.gameScoreSettings.mahouShoujos[MahouShoujoId].MaxVit;
            //一秒七次，3秒回满血，恢复21次
            Rebirth.fillAmount += 1f / 21f;
            
        }
    }

    [ContextMenu("设置名称，灵魂宝石图片 顺便剔除qb")]
    public void SetNameAndSG(string Name)
    {
        if(Name == string.Empty)
        {
            MahouShoujoId = 4;
            Name = "Miki Sayaka";
            SoulGem.sprite = SoulGems[4];
        }
        else if (Name.Equals("Homura") | Name.Equals("Homura_m"))
        {
            MahouShoujoId = 0;
            Name = "Akemi Homura";
            SoulGem.sprite = SoulGems[0];
        }
        else if(Name.Equals("Kyoko"))
        {
            MahouShoujoId = 1;
            Name = "Sakura Koyko";
            SoulGem.sprite = SoulGems[1];
        }
        else if (Name.Equals("Madoka"))
        {
            MahouShoujoId = 2;
            Name = "Kaname Madoka";
            SoulGem.sprite = SoulGems[2];
        }
        else if (Name.Equals("Mami"))
        {
            MahouShoujoId = 3;
            Name = "Tomoe Mami";
            SoulGem.sprite = SoulGems[3];
        }
        else if (Name == "Sayaka")
        {
            MahouShoujoId = 4;
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

    //我实在没办法了把他给从AplayerCtrl复制来了，一切以AplayerCtrl中的为准
    int Grow(int[] GrowparaSetting, int[] LevelLimit, bool Returntotal)
    {
        int j = 0;

        if (Returntotal)
        {

            //先根据等级来判断采取多少级别的成长值
            for (int i = 0; i < LevelLimit.Length; i++)
            {
                //如果当前角色等级低于i阶等级限制的门槛
                if (StageCtrl.gameScoreSettings.GirlsLevel[MahouShoujoId] < LevelLimit[i])
                {
                    break;
                }
                else
                {
                    //则返回上一阶的成长值
                    if (i != 0)
                    {
                        j += GrowparaSetting[i - 1];
                    }
                }
            }

        }
        else
        {
            //先根据等级来判断采取多少级别的成长值
            for (int i = 0; i < LevelLimit.Length; i++)
            {
                //如果当前角色等级低于i阶等级限制的门槛
                if (StageCtrl.gameScoreSettings.GirlsLevel[MahouShoujoId] < LevelLimit[i])
                {
                    //则返回上一阶的成长值
                    if (i != 0)
                    {
                        j += GrowparaSetting[i - 1];
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        return j;

    }
}
