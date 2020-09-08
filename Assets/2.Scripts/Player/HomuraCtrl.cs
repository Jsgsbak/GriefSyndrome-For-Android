using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//先占个位置。。。
public class HomuraCtrl : APlayerCtrl
{
    [Space(20)]
    [Header("还依靠别人吗")]
    public bool DontRelyOnOthers = true;

    public override void CheckAnimStop(string AnimName)
    {
    }

    public override void Magia(int index)
    {
    }

    public override void PlayerAttack()
    {
    }

    public override void PlayerDownX()
    {
    }

    public override void PlayerGreatAttack()
    {
    }

    public override void PlayerUpX()
    {
    }
}
