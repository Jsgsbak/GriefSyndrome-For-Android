using MEC;
using PureAmaya.General;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

//先暂时不继承
public class SayakaCtrl : APlayerCtrl
{
    public override void HorizontalX()
    {

    }

    public override void HorizontalZ()
    {

    }

    public override void Magia()
    {

    }

    public override void OrdinaryX()
    {

    }

    public override void OrdinaryZ()
    {
            animator.SetBool("Zattack", StageCtrl.gameScoreSettings.Zattack);
        BanWalk = StageCtrl.gameScoreSettings.Zattack;
    }

    public override void VerticalX()
    {

    }

    public override void VerticalZ()
    {

    }
}




