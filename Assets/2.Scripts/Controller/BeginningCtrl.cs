using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BeginningCtrl : MonoBehaviour
{
    /*
     * 弄这个场景和这个脚本算是有点妥协的意思了。。。
     * 屑DontDestroyOnLoad毛病太多
     */

    private void Start()
    {
        //在这里启动游戏说明是要完整测试，所以全部初始化gss以防意外的调试参数造成不必要的麻烦
        GameScoreSettingsIO gss = (GameScoreSettingsIO)Resources.Load("GameScoreAndSettings");
            
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;

#endif

#if UNITY_EDITOR
        //初始化
        gss.AllInitial();
#endif

        //使刚进入游戏一定显示标题画面，顺便初始化（title那里）
        gss.MajoSceneToTitle = false;

        //计算角色动画hash值（防止每次模仿动画都计算）
        int ii = 0;
        foreach (var item in Enum.GetNames(typeof(Variable.PlayerStatus)))
        {
            GameScoreSettingsIO.AnimationHash[ii] = Animator.StringToHash(item);
            ii++;
        }

        LoadingCtrl.LoadScene(1, true);
    }
/*
        StartCoroutine(CheckUpdate());
    }

    //检查是否存在更新包
    IEnumerator CheckUpdatePack()
    {
#if UNITY_ANDROID
        //新的差异包位置
        string NewPatchPackage = Application.persistentDataPath + "patch.apk";
       //现在安装的APK
        string NowInstalledApk = Application.dataPath;
        //新的完整安装包地址
        string NewPackage = Application.temporaryCachePath + "new.apk";

        //合并差异包
        [DllImport("bspatch")]
        static extern void StartPatch(string NowInstalledApk, string NewPackage, string NewPatchPackage);
#endif
    }*/
}
