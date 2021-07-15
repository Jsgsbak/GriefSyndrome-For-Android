using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public float XMargin;
    public float YMargin;
    public float XSmooth = 8.0f;
    public float YSmooth = 8.0f;
    public Vector3 MaxXAndY;
    public Vector3 MinXAndY;

    public Transform Target;

    bool CheckXMargin()
    {
        return Mathf.Abs(transform.position.x - Target.position.x) > XMargin;
    }
    bool CheckYMargin()
    {
        return Mathf.Abs(transform.position.y - Target.position.y) < YMargin + 89.0f;
    }
    void LateUpdate()
    {
        FollowTarget();
    }
    void FollowTarget()
    {
        float targetX = transform.position.x;
        float targetY = transform.position.y;
        if (CheckXMargin())
        {
            targetX = Mathf.Lerp(transform.position.x, Target.position.x, XSmooth * Time.deltaTime);
        }
        if (CheckYMargin())
        {
            targetY = Mathf.Lerp(transform.position.y, Target.position.y + targetY, YSmooth * Time.deltaTime);
        }
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
    }
}
/*
————————————————
版权声明：本文为CSDN博主「sunsetv8」的原创文章，遵循CC 4.0 BY - SA版权协议，转载请附上原文出处链接及本声明。
原文链接：https://blog.csdn.net/sunsetv8/article/details/46497887
*/
