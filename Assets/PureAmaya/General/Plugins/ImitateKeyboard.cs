using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

//https://blog.csdn.net/liujunjie612/article/details/73571803 代码来源，并基于此进行了修改



/// <summary>
/// 模拟键盘输入
/// </summary>
public class ImitateKeyboard : MonoBehaviour
{
    /// <summary>
    /// 模拟按键  按键对应表：http://www.doc88.com/p-895906443391.html
    /// </summary>
    /// <param name="bvk">虚拟键值 ESC键对应的是27</param>
    /// <param name="bScan">0</param>
    /// <param name="dwFlags">0为按下，1按住，2释放</param>
    /// <param name="dwExtraInfo">0</param>
    [DllImport("user32.dll", EntryPoint = "keybd_event")]
    public static extern void Keybd_event(byte bvk, byte bScan, int dwFlags, int dwExtraInfo);

}
