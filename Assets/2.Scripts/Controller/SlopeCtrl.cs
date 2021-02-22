using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 计算斜坡的角度，并将该物体名字改成角度
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class SlopeCtrl : MonoBehaviour
{

    private void Awake()
    {
        if (!CompareTag("Slope"))
        {
            Destroy(this);
            //Destroy下一帧才会清除这个脚本，而且貌似比immediate更好一点，所以这样子写了
            return;
        }


      float y =  Mathf.Sin(transform.rotation.eulerAngles.z * (3.14159f * 2 / 360));
        float x = Mathf.Cos(transform.rotation.eulerAngles.z * (3.14159f * 2 / 360));
        

        name = string.Format("{0},{1}", x.ToString(), y.ToString());
    }


}
