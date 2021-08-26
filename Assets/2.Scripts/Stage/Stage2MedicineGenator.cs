using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Stage2MedicineGenator : MonoBehaviour
{
    //����2��ͼ�����ܰ���ҩƷ
    public SpriteAtlas MedicineAtlas;
    /// <summary>
    /// ��������
    /// </summary>
    public int ObejctNumber = 30;

    /// <summary>
    /// 0���� 1���£���λ��ê��
    /// </summary>
    public Transform[] AnchorPoint;

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < ObejctNumber; i++)
        {
            //�������ڴ���ͼ�� ������
            GameObject Medicine = new GameObject(string.Format("Medicine_{0}", i.ToString()), typeof(SpriteRenderer));
            //�������
            Transform tr = Medicine.transform;
            SpriteRenderer sp = Medicine.GetComponent<SpriteRenderer>();
            //���λ�ã�������ê��ķ�Χ�ڣ�
            tr.position = new Vector3(Random.Range(AnchorPoint[0].position.x, AnchorPoint[1].position.x), Random.Range(AnchorPoint[1].position.y, AnchorPoint[0].position.y),4f);
            tr.SetParent(this.transform);
            //���ҩƿ��С
            float scale = Random.Range(1f, 2f);
            tr.localScale = Vector2.one * scale;
            //�����ת
            sp.flipX = Random.value >= 0.5f; sp.flipY = Random.value >= 0.5f;
            //�������˳��
            sp.sortingOrder = Random.Range(-99, -90);
            //���ҩƿͼƬ
            sp.sprite = MedicineAtlas.GetSprite(string.Format("medicine_{0}", Random.Range(0,9)));
          
            
            Medicine.isStatic = true;
        }
    }

    // Update is called once per frame
}
