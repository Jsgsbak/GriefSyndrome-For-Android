using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EasyBGMCtrl : MonoBehaviour
{
    public AudioClip[] BGM;
    public AudioClip[] SE;

    public static EasyBGMCtrl easyBGMCtrl;

   [HideInInspector] public  AudioSource BGMPlayer;
    [HideInInspector] public  AudioSource SEPlayer;

    private void Awake()
    {
      //  DontDestroyOnLoad(gameObject);

        #region 组件初始化
        easyBGMCtrl = this;
        AudioSource[] AS= GetComponents<AudioSource>();
        BGMPlayer = AS[0];
        SEPlayer = AS[1];
        #endregion
    }


    /// <summary>
    /// 改变音量。一般UI控制这个函数
    /// </summary>
    /// <param name="vol"></param>
    /// <param name="IsBGM"></param>
    public void ChangeVol(float vol,bool IsBGM)
    {
        if (IsBGM)
        {
            BGMPlayer.volume = vol;
        }
        else
        {
            SEPlayer.volume = vol;
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
