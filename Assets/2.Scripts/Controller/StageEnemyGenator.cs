using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PureAmaya.General;

//BOSS房间单独弄一个生成器，BOSS房间的生成器有缓冲池（0.2.0版本未装备）
/// <summary>
/// 生成管理每一关的怪物
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(StopPoint))]
public class StageEnemyGenator : MonoBehaviour
{
    StopPoint stopPoint;
    /// <summary>
    /// 那个停止点的怪被清理干净了
    /// </summary>
   [HideInInspector] public int ClearedPoint = -1;
    /// <summary>
    /// 敌机预设
    /// </summary>
    [InspectorName("敌机父对象（在场景中）")]
    public GameObject[] Enemies;

    /// <summary>
    /// 激活的敌机的数量
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
        //玩家到停止点，该刷怪了
        if(stopPoint.UsedPointIndex == ClearedPoint + 1)
        {
            Enemies[ClearedPoint + 1].SetActive(true);
            ClearedPoint = ClearedPoint + int.Parse(Enemies[ClearedPoint + 1].name);
        }
    }

}
