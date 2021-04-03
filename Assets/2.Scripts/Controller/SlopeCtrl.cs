using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 计算斜坡的角度，并将该物体名字改成角度
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class SlopeCtrl : MonoBehaviour
{
    /// <summary>
    /// 左0 右1
    /// </summary>
    public Transform[] Borders = new Transform[2];

    private void Awake()
    {
        //非斜坡不允许
        if (!CompareTag("Slope"))
        {
            Destroy(this);
            return;
        }


      float y =  Mathf.Sin(transform.rotation.eulerAngles.z * (3.14159f * 2 / 360));
        float x = Mathf.Cos(transform.rotation.eulerAngles.z * (3.14159f * 2 / 360));

        Vector2 d = new Vector2(x, y);
        d = d.normalized;
        x = d.x;y = d.y;

        name = string.Format("{0},{1},{2},{3}", x.ToString(), y.ToString(),Borders[0].position.x, Borders[1].position.x);
    }


}
