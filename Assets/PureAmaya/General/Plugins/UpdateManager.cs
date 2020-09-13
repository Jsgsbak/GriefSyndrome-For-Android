using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.Events;

namespace PureAmaya.General
{
    //awake中会删除切换场景后残留的action。因此注册action的时候海清在start中。另外不推荐将该物体设置为DontDestroyOnLoad

    /// <summary>
    /// 提高Update效率
    /// </summary>
    [DisallowMultipleComponent]
    public class UpdateManager : MonoBehaviour
    {

        public static UpdateManager updateManager;

        //*新的事件版
        public class UpdateEventClass : UnityEvent { }
        public  UpdateEventClass FastUpdate = new UpdateEventClass();
        /// <summary>
        /// 假的LateUpdate（所有FastUpdate执行后，且LateUpdate执行前执行）
        /// </summary>
        public  UpdateEventClass FakeLateUpdate = new UpdateEventClass();
        /// <summary>
        /// 依赖于TIM的SlowUpdate
        /// </summary>
        public  UpdateEventClass SlowUpdate = new UpdateEventClass();

        private void Awake()
        {
            updateManager = this;
          
            FastUpdate.RemoveAllListeners();
            FakeLateUpdate.RemoveAllListeners();
            SlowUpdate.RemoveAllListeners();
        }
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