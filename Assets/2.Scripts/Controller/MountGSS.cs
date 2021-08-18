using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 专门提供一个GSS来让大家用
/// </summary>
public class MountGSS : MonoBehaviour
{
    public static GameScoreSettingsIO gameScoreSettings;//尽在这里弄一个单利

    // Start is called before the first frame update
    void Awake()
    {
        gameScoreSettings = (GameScoreSettingsIO)Resources.Load("GameScoreAndSettings");
    }
}
