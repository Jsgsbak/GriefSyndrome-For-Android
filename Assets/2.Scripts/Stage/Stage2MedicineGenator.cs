using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Stage2MedicineGenator : MonoBehaviour
{
    //场景2的图集不能包含药品
    public SpriteAtlas MedicineAtlas;
    /// <summary>
    /// 生成数量
    /// </summary>
    public int ObejctNumber = 30;

    /// <summary>
    /// 0左上 1右下，定位用锚点
    /// </summary>
    public Transform[] AnchorPoint;

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < ObejctNumber; i++)
        {
            //生成用于搭载图像 的物体
            GameObject Medicine = new GameObject(string.Format("Medicine_{0}", i.ToString()), typeof(SpriteRenderer));
            //缓存组件
            Transform tr = Medicine.transform;
            SpriteRenderer sp = Medicine.GetComponent<SpriteRenderer>();
            //随机位置（在两个锚点的范围内）
            tr.position = new Vector3(Random.Range(AnchorPoint[0].position.x, AnchorPoint[1].position.x), Random.Range(AnchorPoint[1].position.y, AnchorPoint[0].position.y),4f);
            tr.SetParent(this.transform);
            //随机药瓶大小
            float scale = Random.Range(1f, 2f);
            tr.localScale = Vector2.one * scale;
            //随机翻转
            sp.flipX = Random.value >= 0.5f; sp.flipY = Random.value >= 0.5f;
            //随机层内顺序
            sp.sortingOrder = Random.Range(-99, -90);
            //随机药瓶图片
            sp.sprite = MedicineAtlas.GetSprite(string.Format("medicine_{0}", Random.Range(0,9)));
          
            
            Medicine.isStatic = true;
        }
    }

    // Update is called once per frame
}
