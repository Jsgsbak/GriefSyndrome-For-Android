using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SayakaCtrl : APlayerCtrl
{


    public override void PlayerAttackZ()
    {
        #region 动作
            if (ZattackCount == 0 || ZattackCount == 1)
            {
                atlasAnimation.ChangeAnimation(zAttackAnimId[ ZattackCount]);
                ZattackCount++;

            //按Z行走
            }
            else
            {
                atlasAnimation.ChangeAnimation(zAttackAnimId[ZattackCount]);
                ZattackCount = 0;
                //僵直，移动
            }
        }
        #endregion
    }



