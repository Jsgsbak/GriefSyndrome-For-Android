using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PureAmaya.General;


public class UICtrl : MonoBehaviour
{
    public TMP_Text[] PlayerScore;



    private void Awake()
    {
        #region 初始化UI界面
        for (int i = 0; i < 3; i++)
        {
      //      PlayerScore[i]  = string.Format("{0} {1}  {2} {3} {4}", "Score", 0,TitleCtrl. PlayerFaceToRichText(TitleCtrl.)[0], PlayerFaceToRichText(PlayerFaces)[1], PlayerFaceToRichText(PlayerFaces)[2]);

        }
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
