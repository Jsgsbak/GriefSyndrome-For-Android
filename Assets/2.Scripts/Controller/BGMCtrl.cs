using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

public class BGMCtrl : MonoBehaviour
{
    AudioSource audioSource;
  public  GameScoreSettingsIO GSS;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateManager.updateManager.SlowUpdate.AddListener(UpdateVol);
    }

    /// <summary>
    /// ¸üÐÂÒôÁ¿
    /// </summary>
    // Update is called once per frame
    void UpdateVol()
    {
        if (GSS != null)
        {
            audioSource.volume = GSS.BGMVol;
        }
        else
        {
            audioSource.volume = MountGSS.gameScoreSettings.BGMVol;
        }
    }
}
