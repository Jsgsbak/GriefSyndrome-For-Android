using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

public class BGMCtrl : MonoBehaviour
{
    public bool IsStaff = false;

    public AudioClip BedEnding;

    AudioSource audioSource;
    // Start is called before the first frame update

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (GameScoreSettingsIO.AllDie)
        {
            audioSource.clip = BedEnding;
        }

        UpdateVol();

    }

    void Start()
    {
        UpdateManager.updateManager.FastUpdate.AddListener(UpdateVol);
    }

    /// <summary>
    /// ¸üÐÂÒôÁ¿
    /// </summary>
    // Update is called once per frame
    void UpdateVol()
    {

audioSource.volume = GameScoreSettingsIO.BGMVol;
        
    }
}
