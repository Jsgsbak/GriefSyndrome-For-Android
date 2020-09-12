using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.Events;

namespace PureAmaya.General
{

    /// <summary>
    /// 提高Update效率
    /// </summary>
    [DisallowMultipleComponent]
    public class UpdateManager : MonoBehaviour
    {
       
        //*新的事件版
        public class UpdateEventClass : UnityEvent { }
        public static UpdateEventClass FastUpdate = new UpdateEventClass();
        /// <summary>
        /// 假的LateUpdate（所有FastUpdate执行后，且LateUpdate执行前执行）
        /// </summary>
        public static UpdateEventClass FakeLateUpdate = new UpdateEventClass();
        /// <summary>
        /// 依赖于TIM的SlowUpdate
        /// </summary>
        public static UpdateEventClass SlowUpdate = new UpdateEventClass();


        private void Start()
        {
           // DontDestroyOnLoad(gameObject);
            Timing.RunCoroutine(SlowUpdatea(), Segment.SlowUpdate);
        }

        private void Update()
        {
            FastUpdate.Invoke();

            FakeLateUpdate.Invoke();
        }

        private IEnumerator<float> SlowUpdatea()
        {
            while(true)
            {
                SlowUpdate.Invoke();
                yield return 0f;
            }
        }
    }

}