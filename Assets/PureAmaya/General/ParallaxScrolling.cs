using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PureAmaya.General
{
    public class ParallaxScrolling : MonoBehaviour
    {
        [Header("lag=1�����ͬ���ƶ�")]
        [Header("ԽԶ����Խ��lagԽ��")]
        [Space(10F)]
        public Transform Camera;

        [Header("���һ��")]
        public Transform[] DepthOnes;
        [Header("���һ����ͺ�ֵ")]
        [Range(0f,1f)]
        [SerializeField]
         float LagForOne = 0.1f;
        [Header("��ȶ���")]
        public Transform[] DepthTwos;
        [Header("��ȶ�����ͺ�ֵ")]
        [Range(0f, 1f)]
        [SerializeField]
         float LagForTwo = 0.2f;

        [Header("�������")]
        public Transform[] DepthThrees;
        [Header("���������ͺ�ֵ")]
        [Range(0f, 1f)]
        [SerializeField]
        float LagForThree = 0.3f;
        /// <summary>
        /// ��һ֡�����λ��
        /// </summary>
        Vector3 CameraLastFramePos = Vector3.zero;
        /// <summary>
        /// �������һ֡��λ�ò�ֵ
        /// </summary>
        Vector3 Difference;
        // Start is called before the first frame update


        SpriteRenderer[] One; SpriteRenderer[] Two; SpriteRenderer[] Three;

        //��ʼ��������ʱ��һ���Լ��������
        private void Awake()
        {
            One = new SpriteRenderer[DepthOnes.Length];
            Two = new SpriteRenderer[DepthTwos.Length];
            Three = new SpriteRenderer[DepthThrees.Length];

            for (int i = 0; i < One.Length; i++)
            {
                One[i] = DepthOnes[i].GetComponent<SpriteRenderer>();
            }
            for (int i = 0; i < DepthTwos.Length; i++)
            {
                Two[i] = DepthTwos[i].GetComponent<SpriteRenderer>();
            }
            for (int i = 0; i < Three.Length; i++)
            {
                Three[i] = DepthThrees[i].GetComponent<SpriteRenderer>();
            }
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if(CameraLastFramePos != Vector3.zero)
            {
                //�������һ֡��λ�ò�ֵ
                Difference = Camera.position - CameraLastFramePos;
               
                for (int i = 0; i < DepthOnes.Length; i++)
                {
                    if (One[i].isVisible)
                    {
                        Change(DepthOnes, i, LagForOne);
                    }
                }
                for (int i = 0; i < DepthTwos.Length; i++)
                {
                    if (Two[i].isVisible)
                    {
                        Change(DepthTwos, i, LagForTwo);
                    }
                }
                for (int i = 0; i < DepthThrees.Length; i++)
                {
                    if (Three[i].isVisible)
                    {
                        Change(DepthThrees, i, LagForThree);
                    }
                }
                CameraLastFramePos = Camera.position;

            }
            //��һ֡������һ��λ��֮��Ž����������Ӳ����
            else
            {
                CameraLastFramePos = Camera.position;
            }
        }

        void Change(Transform[] Groups, int index,float Lag)
        {
            Groups[index].SetPositionAndRotation(Groups[index].position + Difference * Lag, Groups[index].rotation);
        }
    }

    

}

