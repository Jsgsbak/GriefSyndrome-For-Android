using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 单独拿出一个玩家信息控制，方便维护
/// </summary>
public class PlayerInfCtrl : MonoBehaviour
{
    public int PlayerId = 1;

    public TMP_Text Score;
    public TMP_Text Level;

    public Image Health;
    public Image Damaged;
    public Image Magia;
    public Image Rebirth;

    [ContextMenu("更新分数")]
public void UpdateScore()
    {
            Score.text = string.Format("{0}p Score {1}  {2}", (PlayerId).ToString(), StageCtrl.gameScoreSettings.Score[PlayerId - 1], TitleCtrl.PlayerFaceToRichText(StageCtrl.gameScoreSettings.SelectedGirlInGame)[PlayerId - 1]);
    }

    public void UpdateLevel()
    {
       // Level = 
    }
}
