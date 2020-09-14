using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class EasyBGMCtrl : MonoBehaviour
{
    public AudioClip[] BGM;
    public AudioClip[] SE;

    public static EasyBGMCtrl easyBGMCtrl;

    [Header("在哪个场景里允许不被Destroy")]
    /// <summary>
    /// 在哪个场景里允许不被Destroy
    /// </summary>
    public string SceneNameDontDestroyOnLoad;

    /// <summary>
    /// 是否为克隆的。用于多余BGMCtrl的删除处理
    /// </summary>
    [HideInInspector] public bool IsClone = false;

   [HideInInspector] public  AudioSource BGMPlayer;
    [HideInInspector] public  AudioSource SEPlayer;



    private void Awake()
    {

        #region 组件初始化
        easyBGMCtrl = this;
        AudioSource[] AS= GetComponents<AudioSource>();
        BGMPlayer = AS[0];
        SEPlayer = AS[1];
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

    /// <summary>
    /// 播放BGM
    /// </summary>
    /// <param name="index">小于0停止播放</param>
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

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="index">小于0停止播放</param>
    public void PlaySE(int index)
    {
        if (index < 0)
        {
            SEPlayer.Stop();
        }
        else
        {
            SEPlayer.PlayOneShot(SE[index]);

        }
    }

}
