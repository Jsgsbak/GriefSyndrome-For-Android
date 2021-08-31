using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        //把完整的版本号改回来原来的简易版本号
        PlayerSettings.bundleVersion = PlayerSettings.bundleVersion.Split('_')[0];
    }

    public void OnPostprocessFailed(BuildReport report)
    {
        Debug.Log(report.files);

        //把完整的版本号改回来原来的简易版本号
        PlayerSettings.bundleVersion = PlayerSettings.bundleVersion.Split('_')[0];

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
        //自动增加内部版本号，防止遗忘
        PlayerSettings.Android.bundleVersionCode++;
        //修改成完整的版本号
        PlayerSettings.bundleVersion = string.Format("{0}_Build {1}", PlayerSettings.bundleVersion, PlayerSettings.Android.bundleVersionCode.ToString());
        //A.B.C  A:大换血 B:功能更新 C:功能调整数值调整各种调整 Build之后的内部版本号是修复bug的


    }
}
