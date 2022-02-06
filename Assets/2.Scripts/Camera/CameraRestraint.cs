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

    public int InitialPosition;
    /// <summary>
    /// ����ı任���
    /// </summary>
    Transform tr;
    /// <summary>
    /// ���������λ�ã���ֹ����NEW��
    /// </summary>
    Vector2 vector2;
    /// <summary>
    /// �ո�·����һ���㣿
    /// </summary>
    int PassedIndex;
    /// <summary>
    /// ȷ��������ܻ����ŵĵ㣬�����л��ߴ���
    /// </summary>
    int NextPointDrawingLine;
    /// <summary>
    /// ��һ������������
    /// </summary>
    bool NextPointRight = false;

    public Transform Target;
    /// <summary>
    /// ��ʼ��
    /// </summary>
    public void Initialize(Transform Camera, Transform target)
    {
        Target = target;
        tr = Camera;
        //��ʼ��λ��
        tr.SetPositionAndRotation(new Vector3(CameraPoints[InitialPosition].Point.x, CameraPoints[InitialPosition].Point.y, -10f), Quaternion.identity);
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
        tr.SetPositionAndRotation(new Vector3(CameraPoints[index].Point.x, CameraPoints[index].Point.y, -10f), Quaternion.identity);
        PassedIndex = index;
        NextPointDrawingLine = index + 1;
    }

    public void UpdatePoint()
    {
        //������һ�ξ����ĵ�����            
        vector2 = tr.position;

        //����һ�����������ߵĵ��е�С�ĵ�ʱ����Ŵ������С�Ĳ������������¸վ����ĵ����
        if (CameraPoints[Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1)].AllowGoBack && !CameraPoints[PassedIndex].AllowGoBack)
        {

        }
        else
        {
            //ĳ����������λ������㹻��������Ϊ��������µġ��վ����ĵ㡱
            if ((vector2 - CameraPoints[Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1)].Point).sqrMagnitude <= 0.1f)
            {
                PassedIndex = Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1);
            }

        }


        //ֱ��ȡ��һ����Ϊ��һ��Ҫ�����ĵ�
        NextPointDrawingLine = Mathf.Clamp(PassedIndex + 1, 0, CameraPoints.Length - 1);
        //ȷ����Ŵ�һ���ĵ������滹������
        NextPointRight = CameraPoints[NextPointDrawingLine].Point.x - CameraPoints[PassedIndex].Point.x > 0f;





    }


    /// <summary>
    /// ��������ƶ�����
    /// </summary>
    /// <param name="Raw">���������ƶ�����</param>
    /// <param name="Player">��ҵ�λ�ã�ȫ�֣�</param>
    public Vector2 RepairCameraMoveDirection(Vector2 Raw, Vector2 Player)
    {
        //�����ĳ��������
        if ((vector2 - CameraPoints[PassedIndex].Point).sqrMagnitude <= 0.1f)
        {
            //ȷ�����޸ģ�ˮƽ�ƶ�����
            if (Raw.x > 0 && !CameraPoints[PassedIndex].BanLeftRight[1])
            {
                return MoveCamera(Raw);
            }
            else if (Raw.x < 0 && !CameraPoints[PassedIndex].BanLeftRight[0])
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
            //��Щ���������ߵ��ߣ�ֱ�Ӱ����������ķ����ߺ��ˣ���Ҫ�������������һ���ľ���
            if (CameraPoints[NextPointDrawingLine].AllowGoBack)
            {
                switch (NextPointRight)
                {
                    //��һ�����ڸվ����ĵ������
                    case true:

                        //��������ߣ������������Ļ�Ҳ࣬�����������һ����ܽ�
                        if (Raw.x > 0f && Player.x > tr.position.x && (CameraPoints[NextPointDrawingLine].Point - vector2).sqrMagnitude <= 0.1F)
                        {
                            return Vector2.zero;
                        }
                        //��������ߣ������������Ļ��࣬��������ĸչ��ĵ�ܽ�
                        else if (Raw.x < 0f && Player.x < tr.position.x && (CameraPoints[PassedIndex].Point - vector2).sqrMagnitude <= 0.1F)
                        {
                            return Vector2.zero;
                        }
                        //��������ߣ������������Ļ��࣬��������ĸվ����ĵ��Զ
                        else if (Raw.x < 0f && Player.x < tr.position.x && (CameraPoints[PassedIndex].Point - vector2).sqrMagnitude > 0.1F)
                        {
                            return MoveCamera(Raw);
                        }
                        //��������ߣ������������Ļ�Ҳ࣬�����������һ�����Զ
                        else if (Raw.x > 0f && Player.x > tr.position.x && (CameraPoints[NextPointDrawingLine].Point - vector2).sqrMagnitude > 0.1F)
                        {
                            //��㶯��׷��ȥ
                            return MoveCamera(Raw * 1.2f);
                        }
                        else
                        {
                            return Vector2.zero;
                        }
                    //��һ�����ڸվ����ĵ������
                    case false:
                        //��������ߣ������������Ļ�Ҳ࣬��������ĸվ����ĵ�ܽ�
                        if (Raw.x > 0f && Player.x > tr.position.x && (CameraPoints[PassedIndex].Point - vector2).sqrMagnitude <= 0.1F)
                        {
                            Debug.Log("2");
                            return Vector2.zero;
                        }
                        //��������ߣ������������Ļ��࣬���������Ҫ�����ĵ�ܽ�
                        else if (Raw.x < 0f && Player.x < tr.position.x && (CameraPoints[NextPointDrawingLine].Point - vector2).sqrMagnitude <= 0.1F)
                        {
                            Debug.Log("3");
                            return Vector2.zero;
                        }
                        //��������ߣ������������Ļ��࣬�����������һ�����ĵ��Զ
                        else if (Raw.x < 0f && Player.x < tr.position.x && (CameraPoints[NextPointDrawingLine].Point - vector2).sqrMagnitude > 0.1F)
                        {
                            Debug.Log("5");
                            return MoveCamera(Raw);
                        }
                        //��������ߣ������������Ļ�Ҳ࣬��������ĸվ����ĵ��Զ
                        else if (Raw.x > 0f && Player.x > tr.position.x && (CameraPoints[PassedIndex].Point - vector2).sqrMagnitude > 0.1F)
                        {
                            Debug.Log("6");
                            return MoveCamera(Raw);
                        }
                        else
                        {
                            return Vector2.zero;
                        }
                }


            }

            //���ܻ�ͷ
            else
            {
                switch (NextPointRight)
                {
                    //��һ�����ڸվ����ĵ������
                    case true:
                        //��������ߣ������������Ļ�м�
                        if (Raw.x > 0f && Mathf.Abs(Player.x - tr.position.x) <= 0.1f)
                        {
                            //���˳���ƶ���ȥ
                            return MoveCamera(Raw);
                        }
                        //��������ߣ������������Ļ�Ҳ�
                        else if (Raw.x >= 0f && Player.x - tr.position.x > 0.1f)
                        {
                            //������ƶ�һ�㣬Ϊ�˸������
                            return MoveCamera(Raw * 1.2F);
                        }
                        else
                        {
                            return Vector2.zero;
                        }
                    //��һ�����ڸվ����ĵ������
                    case false:
                        //��������ߣ������������Ļ�м�
                        if (Raw.x < 0f && Mathf.Abs(Player.x - tr.position.x) <= 0.1f)
                        {
                            return MoveCamera(Raw);
                        }
                        //��������ߣ������������Ļ���
                        else if (Raw.x <= 0f && Player.x - tr.position.x < -0.1f)
                        {
                            //������ƶ�һ�㣬Ϊ�˸������
                            return MoveCamera(Raw * 1.2F);
                        }
                        else
                        {
                            return Vector2.zero;
                        }
                }
            }



        }


    }

    /// <summary>
    /// �ƶ�����������ִ�У�˵������������ܣ�
    /// </summary>
    /// <param name="Raw">���ԭʼ���루һ��Ҳ��ԭʼ����AplayerCtrl�ӹ����ˣ�</param>
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
        Gizmos.color = Color.green;

        for (int i = 0; i < CameraPoints.Length; i++)
        {
            //�����ﻭ���� ��������дλ��TM����ʾ
            Gizmos.DrawSphere(new Vector3(CameraPoints[i].Point.x, CameraPoints[i].Point.y, -3f), 0.6f);

            //׼��19��Ӧ�ù���  ���
            if (i <= 9)
            {

                Gizmos.DrawIcon(new Vector3(CameraPoints[i].Point.x, CameraPoints[i].Point.y + 1f, 1f), string.Format("{0}.psd", i.ToString()), true, Color.black);
            }
            else
            {
                Gizmos.DrawIcon(new Vector3(CameraPoints[i].Point.x, CameraPoints[i].Point.y + 1f, 1f), "1.psd", true, Color.black);
                Gizmos.DrawIcon(new Vector3(CameraPoints[i].Point.x, CameraPoints[i].Point.y + 1f, 1f), string.Format("{0}.psd", i.ToString()), true, Color.black);
            }



            foreach (var item in CameraPoints[i].ConnectPointIndex)
            {
                //�ҵ����п�������������ӵĵ㣬Ȼ����
                if (CameraPoints[i].AllowGoBack)
                {
                    //�����߻�ȥ��������
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(CameraPoints[i].Point, CameraPoints[item].Point);
                }
                else
                {
                    //�����߻�ȥ��������
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(CameraPoints[i].Point, CameraPoints[item].Point);
                }
                //��飺��������������ĵ�������ҷ��أ��򱨴�
                if (CameraPoints[item].AllowGoBack && CameraPoints[i].AllowGoBack)
                {
                    Debug.LogErrorFormat("���{0}��{1}��AllowGoBack�趨��ͻ�򲻱�Ҫ", item.ToString(), i.ToString());
                }

            }

        }

    }
#endif



    [System.Serializable]
    public class LinarPoint
    {
        [Header("ê��")]
        public Vector2 Point = new();
        [Header("�����ӵĵ�����")]
        public List<int> ConnectPointIndex = new();
        /// <summary>
        /// ��������ܷ��ƶ� 0��1��
        /// </summary>
        public bool[] BanLeftRight = { false, false };

        /// <summary>
        /// �Ƿ�������������ߣ�������ǰ��������űȽ�С�ĵط���
        /// </summary>
        public bool AllowGoBack = false;
    }


}
