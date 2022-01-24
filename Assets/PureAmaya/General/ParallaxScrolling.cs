using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PureAmaya.General
{
    public class ParallaxScrolling : MonoBehaviour
    {
        [Header("lag=1与相机同步移动")]
        [Header("越远动的越慢lag越大")]
        [Space(10F)]
        public Transform Camera;

        [Header("深度一组")]
        public Transform[] DepthOnes;
        [Header("深度一组的滞后值")]
        [Range(0f,1f)]
        [SerializeField]
         float LagForOne = 0.1f;
        [Header("深度二组")]
        public Transform[] DepthTwos;
        [Header("深度二组的滞后值")]
        [Range(0f, 1f)]
        [SerializeField]
         float LagForTwo = 0.2f;

        [Header("深度三组")]
        public Transform[] DepthThrees;
        [Header("深度三组的滞后值")]
        [Range(0f, 1f)]
        [SerializeField]
        float LagForThree = 0.3f;
        /// <summary>
        /// 上一帧相机的位置
        /// </summary>
        Vector3 CameraLastFramePos = Vector3.zero;
        /// <summary>
        /// 相机与上一帧的位置差值
        /// </summary>
        Vector3 Difference;
        // Start is called before the first frame update


        SpriteRenderer[] One; SpriteRenderer[] Two; SpriteRenderer[] Three;

        //初始化场景的时候一次性加载完好了
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
                //相机与上一帧的位置差值
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
            //第一帧，保存一下位置之后才进入正常的视差滚动
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

