using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// 打包的时候执行的脚本
/// </summary>
public class GSPreprocessBuild : Editor, IPreprocessBuildWithReport, IPostprocessBuildWithReport
{

    public int callbackOrder { get { return 0; } }

    /// <summary>
    /// 打包完成之后的处理
    /// </summary>
    /// <param name="report"></param>
    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log(report.files);
    }

    /// <summary>
    /// 打包之前
    /// </summary>
    /// <param name="report"></param>
    public void OnPreprocessBuild(BuildReport report)
    {
        //发包前全部初始化
        GameScoreSettingsIO gss = (GameScoreSettingsIO)Resources.Load("GameScoreAndSettings");
        gss.AllInitial();
        Debug.Log("初始化完成");
        //自动增加内部版本号，防止遗忘
        PlayerSettings.Android.bundleVersionCode++;


    }
}
