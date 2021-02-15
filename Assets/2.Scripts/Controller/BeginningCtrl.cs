﻿using System.Collections;
using System.Collections.Generic;
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
        //使刚进入游戏一定显示标题画面，顺便初始化（title那里）
        gss.MajoSceneToTitle = false;

        LoadingCtrl.LoadScene(1, true);
    }

}
