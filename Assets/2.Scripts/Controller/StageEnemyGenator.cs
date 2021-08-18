using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

//BOSS���䵥��Ūһ����������BOSS������������л���أ�0.2.0�汾δװ����
/// <summary>
/// ���ɹ���ÿһ�صĹ���
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(StopPoint))]
public class StageEnemyGenator : MonoBehaviour
{
    StopPoint stopPoint;
    /// <summary>
    /// �Ǹ�ֹͣ��Ĺֱ�����ɾ���
    /// </summary>
   [HideInInspector] public int ClearedPoint = -1;
    /// <summary>
    /// �л�Ԥ��
    /// </summary>
    [InspectorName("�л��������ڳ����У�")]
    public GameObject[] Enemies;

    /// <summary>
    /// ����ĵл�������
    /// </summary>
   [HideInInspector]public int EnabledEnemyNumber = -1;

    // Start is called before the first frame update

    private void Start()
    {
        stopPoint = GetComponent<StopPoint>();

        UpdateManager.updateManager.FastUpdate.AddListener(FastUpdate);

        for (int i = 0; i < Enemies.Length; i++)
        {
            Enemies[i].SetActive(false);
        }
    }

    void FastUpdate()
    {
        //��ҵ�ֹͣ�㣬��ˢ����
        if(stopPoint.UsedPointIndex == ClearedPoint + 1)
        {
            Enemies[ClearedPoint + 1].SetActive(true);
            ClearedPoint = ClearedPoint + int.Parse(Enemies[ClearedPoint + 1].name);
        }
    }

}
