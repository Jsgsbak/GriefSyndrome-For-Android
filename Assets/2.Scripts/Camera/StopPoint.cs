using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.Events;

/// <summary>
/// ���ƴ�����ֹͣ�㣬������ͼ���˿���ǽ�����ֹͣ��
/// </summary>
public class StopPoint : MonoBehaviour
{
    /// <summary>
    /// ֹͣ��
    /// </summary>
    [Tooltip("ֹͣ��")]
    public List<Vector2> PointsEditor;//������������

#if UNITY_EDITOR
    [Header("��Ϸ��ʵ���õĵ����ɫ")]
    public Color GameUseColor = Color.red;
#endif

    /// <summary>
    /// �ո��ù���ֹͣ��
    /// </summary>
     int UsedPointIndex = -1;

    /// <summary>
    /// ֹͣ����ƶ�
    /// </summary>
    [HideInInspector]
    public bool StopCamera = false;

    public UnityEvent<int> CancelStopPoint;

    /// <summary>
    ///  ���ֹͣ��(�������꣩
    /// </summary>
    /// <param name="Camera"></param>
    /// <returns>true����ֹͣ���ϣ�ֹͣ�ƶ����</returns>
    public void CheckStopPoint(Vector2 Camera)
    {
        //���һ��ֹͣ�㼤��󣬲��ټ��ֹͣ��
        if(UsedPointIndex + 1 >= PointsEditor.Count)
        {
            return;
        }


        //������һ��ֹͣ�㣬ͣ�����
        if ((PointsEditor[UsedPointIndex +1] - Camera).sqrMagnitude <= 0.1f)
        {
            UsedPointIndex++;
            StopCamera = true;
        }
    }

    /// <summary>
    /// ֹֹͣͣ������������
    /// </summary>
    public void CancelStop()
    {
        StopCamera = false;
        CancelStopPoint.Invoke(UsedPointIndex);
    }

#if UNITY_EDITOR

    public void OnDrawGizmos()
    {
        Gizmos.color = GameUseColor;


        for (int i = 0; i < PointsEditor.Count; i++)
        {
            //�����ﻭ����
            Gizmos.DrawSphere(new Vector3(PointsEditor[i].x, PointsEditor[i].y,1f), 0.8f);


        }

    }





#endif
}
