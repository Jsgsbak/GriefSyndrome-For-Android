using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class SoundEffectCtrl : MonoBehaviour
{
    public AudioClip[] SE;

    public static SoundEffectCtrl soundEffectCtrl;

    [Header("在哪个场景里允许不被Destroy")]
    /// <summary>
    /// 在哪个场景里允许不被Destroy
    /// </summary>
    public string SceneNameDontDestroyOnLoad;

    /// <summary>
    /// 是否为克隆的。用于多余BGMCtrl的删除处理
    /// </summary>
    [HideInInspector] public bool IsClone = false;

    [HideInInspector] public AudioSource SEPlayer;



    private void Awake()
    {

        #region 组件初始化
        soundEffectCtrl = this;
        SEPlayer = GetComponent<AudioSource>();
        #endregion

    }

    private void Start()
    {
        //非克隆体才能DontDestroyOnLoad
        if (!IsClone && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == SceneNameDontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);

        }

    }

    /// <summary>
    /// 改变音效音量。一般UI控制这个函数
    /// </summary>
    /// <param name="vol"></param>
    /// <param name="IsBGM"></param>
    public void ChangeVol(float vol)
    {
            SEPlayer.volume = vol;
    }


    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySE(Variable.SoundEffect soundEffectName)
    {
        if ((int)soundEffectName < 0)
        {
            SEPlayer.Stop();
        }
        else
        {
            SEPlayer.PlayOneShot(SE[(int)soundEffectName]);

        }
    }



}


