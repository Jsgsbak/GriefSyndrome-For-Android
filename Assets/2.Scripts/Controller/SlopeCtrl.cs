using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����б�µĽǶȣ��������������ָĳɽǶ�
/// </summary>
public class SlopeCtrl : MonoBehaviour
{

    private void Awake()
    {
        if (!CompareTag("Slope"))
        {
            Destroy(this);
            //Destroy��һ֡�Ż��������ű�������ò�Ʊ�immediate����һ�㣬����������д��
            return;
        }


      float y =  Mathf.Sin(transform.rotation.eulerAngles.z * (3.14159f * 2 / 360));
        float x = Mathf.Cos(transform.rotation.eulerAngles.z * (3.14159f * 2 / 360));
        

        name = string.Format("{0},{1}", x.ToString(), y.ToString());
    }


}
