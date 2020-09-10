using System.Collections;
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
        LoadingCtrl.LoadScene(1, false);
    }

}
