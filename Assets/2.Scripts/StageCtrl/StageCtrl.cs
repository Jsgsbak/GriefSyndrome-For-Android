using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageCtrl : MonoBehaviour
{
    public static GameScoreSettingsIO gameScoreSettings;


    public class intEvent : UnityEvent<int> { }
    public static intEvent Player1Hurt = new intEvent();
    public static intEvent Player2Hurt = new intEvent();
    public static intEvent Player3Hurt = new intEvent();


    private void Awake()
    {
        gameScoreSettings = (GameScoreSettingsIO)Resources.Load("GameScoreAndSettings");
    }

    // Start is called before the first frame update
    void Start()
    {
       // Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public static void HurtPlayer(int damage, int PlayerId)
    {
        if (PlayerId == 1)
        {
            Player1Hurt.Invoke(damage);
        }
        else if (PlayerId == 2)
        {
            Player2Hurt.Invoke(damage);
        }
        else
        {
            Player3Hurt.Invoke(damage);
        }
    }

}
