using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1ChangeLayer : MonoBehaviour
{
    /// <summary>
    /// ÇÅµÄ±àºÅ
    /// </summary>
    public int Index;
    public void ChangeLayer(int value)
    {
        if(Index == value)
        {
            gameObject.layer = 13;
        }
    }
}
