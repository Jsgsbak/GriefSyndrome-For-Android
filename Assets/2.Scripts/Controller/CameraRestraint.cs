using System.Collections.Generic;
using UnityEngine;

//�����Щ����ļ����Ҫ̫С����ҵ�Raw����ΪMove�������뵽�����ս��

/// <summary>
/// Լ������ĵ����ƶ��켣������Լ����
/// </summary>
public class CameraRestraint : MonoBehaviour
{
    /// <summary>
    /// ��������ƶ����������ê��
    /// </summary>
    [Header("��������ƶ����������ê��")]
    public LinarPoint[] CameraPoints;//��������鶼��ָ���
    [Header("Gizmos��ɫ")]
    public Color color = Color.green;

    public int InitialPosition;
    /// <summary>
    /// ����ı任���
    /// </summary>
    Transform  tr;
    /// <summary>
    /// ���������λ�ã���ֹ����NEW��
    /// </summary>
    Vector2 vector2;
    /// <summary>
    /// �ո�·����һ���㣿
    /// </summary>
    public int PassedIndex;
    /// <summary>
    /// ȷ��������ܻ����ŵĵ㣬�����л��ߴ���
    /// </summary>
    public int NextPointDrawingLine;
    /// <summary>
    /// ��һ������������
    /// </summary>
    bool NextPointRight = false;
    /// <summary>
    /// ��ʼ�����λ��
    /// </summary>
    public void Initialize(Transform Camera)
    {
        tr = Camera;
        //��ʼ��λ��
        tr.SetPositionAndRotation(CameraPoints[InitialPosition].Point,Quaternion.identity);
        PassedIndex = InitialPosition;

        for (int i = 0; i < CameraPoints.Length; i++)
        {
            //�޸���0��1��û��1��0������Ĳ����ƶ������⣨��ʵӦ��˫�����ƶ���
            foreach (var item in CameraPoints[i].ConnectPointIndex)
            {
                if (CameraPoints[item].ConnectPointIndex.Contains(i))
                {
                    continue;
                }
                CameraPoints[item].ConnectPointIndex.Add(i);
            }

        }
       
    }

    /// <summary>
    /// ֱ����Ծ��ĳ����
    /// </summary>
    public void JumpToPoint(int index)
    {
        tr.SetPositionAndRotation(CameraPoints[index].Point, Quaternion.identity);
    }

    /// <summary>
    /// ��������ƶ�����
    /// </summary>
    /// <param name="Raw">���������ƶ�����</param>
    /// <param name="Player">��ҵ�λ�ã�ȫ�֣�</param>
    public Vector2 RepairCameraMoveDirection(Vector2 Raw,Vector2 Player)
    {

        //������һ�ξ����ĵ�����
        //����һ�ξ����ĵ��ǰ�������Լ���ȡ1����
        for (int i = -1; i < 2; i++)
        {
            vector2 = tr.position;

            //ĳ����������λ������㹻��������Ϊ��������µġ��վ����ĵ㡱
            if ((vector2 - CameraPoints[Mathf.Clamp( PassedIndex+i,0,CameraPoints.Length - 1)].Point).sqrMagnitude <= 0.1f)
            {
                PassedIndex = Mathf.Clamp(PassedIndex + i, 0, CameraPoints.Length - 1);
            }
        }

        //���������������ĵ㣬�����������������
        //�����·���ĵ������������
        if (tr.position.x - CameraPoints[PassedIndex].Point.x < 0)
        {
            /*
            //��һ���ڸ�·���ĵ�����ĵ㣬�����ҵ����������վ����ĵ�֮��������
            if (CameraPoints[Mathf.Clamp(PassedIndex - 1, 0, CameraPoints.Length - 1)].Point.x - CameraPoints[PassedIndex].Point.x < 0 && CameraPoints[Mathf.Clamp(PassedIndex - 1, 0, CameraPoints.Length - 1)].ConnectPointIndex.Contains(PassedIndex))
            {
                NextPointDrawingLine = Mathf.Clamp(PassedIndex - 1, 0, CameraPoints.Length - 1);
            }
            else*/ if (CameraPoints[Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1)].Point.x - CameraPoints[PassedIndex].Point.x < 0 && CameraPoints[Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1)].ConnectPointIndex.Contains(PassedIndex))
            {
                NextPointRight = false;
                NextPointDrawingLine = Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1);
            }

        }
        //�����·���ĵ������������
        else
        {             //��һ���ڸ�·���ĵ�����ĵ㣬�����ҵ����������վ����ĵ�֮��������

            /*
            if (CameraPoints[Mathf.Clamp(PassedIndex - 1, 0, CameraPoints.Length - 1)].Point.x - CameraPoints[PassedIndex].Point.x > 0 && CameraPoints[Mathf.Clamp(PassedIndex - 1, 0, CameraPoints.Length - 1)].ConnectPointIndex.Contains(PassedIndex))
            {
                NextPointDrawingLine = Mathf.Clamp(PassedIndex - 1, 0, CameraPoints.Length - 1);
            }
            else */
            if (CameraPoints[Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1)].Point.x > CameraPoints[PassedIndex].Point.x  && CameraPoints[Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1)].ConnectPointIndex.Contains(PassedIndex))
            {
                NextPointRight = true;
                NextPointDrawingLine = Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1);
            }
        }


        //�����ĳ��������
        if ((vector2 - CameraPoints[PassedIndex].Point).sqrMagnitude <= 0.1f)
        {
            //ȷ�����޸ģ�ˮƽ�ƶ�����
            if (Raw.x > 0 && !CameraPoints[PassedIndex].BanLeftRight[1])
            {
                return MoveCamera(Raw);
            }
            else if(Raw.x < 0 && !CameraPoints[PassedIndex].BanLeftRight[0])
            {
                return MoveCamera(Raw);
            }
            else
            {
                //RAW�ƶ�������������ƶ������г�ͻ�����ƶ����
                return Vector2.zero;
            }
        }
        //��������ϣ���������㶯
        else
        {
            switch (NextPointRight)
            {
                //��һ�����ڸվ����ĵ������
                case true:
                    //��������ߣ������������Ļ�м�
                    if (Raw.x > 0 && Mathf.Abs(Player.x - tr.position.x) <= 0.1f)
                    {
                        //���˳���ƶ���ȥ
                        return MoveCamera(Raw);
                    }
                    //��������ߣ������������Ļ�Ҳ�
                    else if (Raw.x > 0 && Player.x - tr.position.x > 0.1f)
                    {
                        //������ƶ�һ�㣬Ϊ�˸������
                        return MoveCamera(Raw * 1.2f);
                    }
                    else
                    {
                        return Vector2.zero;
                    }
                //��һ�����ڸվ����ĵ������
                case false:
                    //��������ߣ������������Ļ�м�
                    if (Raw.x > 0 && Mathf.Abs(Player.x - tr.position.x) <= 0.1f)
                    {
                        return MoveCamera(Raw);
                    }                    
                    //��������ߣ������������Ļ���
                    else if (Raw.x > 0 && Player.x - tr.position.x < 0.1f)
                    {
                        //������ƶ�һ�㣬Ϊ�˸������
                        return MoveCamera(Raw * 1.2f);
                    }
                    else
                    {
                        return Vector2.zero;
                    }
            }
        }

      
    }

    /// <summary>
    /// �ƶ�����������ִ�У�˵������������ܣ�
    /// </summary>
    /// <param name="Raw"></param>
    /// <returns></returns>
    Vector2 MoveCamera(Vector2 Raw)
    {
       //ȷ���վ����ĵ����ȡ����һ����ķ������Ҳ�����
        Vector2 direction = (CameraPoints[NextPointDrawingLine].Point - CameraPoints[PassedIndex].Point).normalized;
        //�������������ƶ�����������direction�ĽǶ�
        float angle = Vector2.Angle(Raw, direction);
        //��������ֵ
        return Raw.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad) * direction;
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = color;


        for (int i = 0; i < CameraPoints.Length; i++)
        {
            //�����ﻭ����
            Gizmos.DrawSphere(CameraPoints[i].Point, 0.6f);
            Gizmos.DrawIcon(CameraPoints[i].Point, (i - 1).ToString(),true);

            //�ҵ����п�������������ӵĵ㣬Ȼ����
            foreach (var item in CameraPoints[i].ConnectPointIndex)
            {
                Gizmos.DrawLine(CameraPoints[i].Point, CameraPoints[item].Point);
            }

        }

    }
#endif



    [System.Serializable]
    public class LinarPoint
    {
        [Header("ê��")]
        public Vector2 Point;
        [Header("�����ӵĵ�����")]
        public List<int> ConnectPointIndex;
        /// <summary>
        /// ��������ܷ��ƶ� 0��1��
        /// </summary>
        public bool[] BanLeftRight = { false, false };

     
    }


}
