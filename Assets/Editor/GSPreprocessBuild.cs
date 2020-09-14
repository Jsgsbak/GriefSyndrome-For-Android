using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

/// <summary>
/// 打包的时候执行的脚本
/// </summary>
public class GSPreprocessBuild : IPostprocessBuild
{
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildTarget target, string path)
    {
        //发包前全部初始化
        GameScoreSettingsIO gss = (GameScoreSettingsIO)Resources.Load("GameScoreAndSettings");
        gss.AllInitial();
    }
}
