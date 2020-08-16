using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EasyBGMCtrl : MonoBehaviour
{
    public AudioClip[] BGM;
    public AudioClip[] SE;

    public static EasyBGMCtrl easyBGMCtrl;

     AudioSource BGMPlayer;
     AudioSource SEPlayer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        #region 组件初始化
        easyBGMCtrl = this;
        AudioSource[] AS= GetComponents<AudioSource>();
        BGMPlayer = AS[0];
        SEPlayer = AS[1];
        #endregion
    }

    /// <summary>
    /// 音量调整
    /// </summary>
    /// <param name="BGM">是否为BGM，否为SE</param>
    /// <param name="volume">音量</param>
    public void Volume(bool BGM,float volume)
    {
        //限制大小
        volume = Mathf.Clamp01(volume);
        if (BGM)
        {
            BGMPlayer.volume = volume;
        }
        else
        {
            SEPlayer.volume = volume;
        }
    }



  public void PlayBGM(int index)
    {

        if (index < 0)
        {
            BGMPlayer.Stop();
        }
        else
        {
            BGMPlayer.Stop();
            BGMPlayer.clip = BGM[index];
            BGMPlayer.Play();

        }
    }

    
}
