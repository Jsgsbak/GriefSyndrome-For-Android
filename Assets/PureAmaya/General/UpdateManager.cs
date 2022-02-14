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

        public static UpdateManager updateManager;

        public class UpdateEventClass : UnityEvent { }

        /// <summary>
        /// Start之后,Update之前运行一次
        /// </summary>
        public UpdateEventClass LateStart = new();
        bool LateStartRun = false;

        public  UpdateEventClass FastUpdate = new();
        /// <summary>
        /// 假的LateUpdate（所有FastUpdate执行后，且LateUpdate执行前执行）
        /// </summary>
        public  UpdateEventClass FakeLateUpdate = new();
        /// <summary>
        /// 依赖于MEC的低速Update
        /// </summary>
        public  UpdateEventClass SlowUpdate = new();
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

            LateStartRun = true;
        }

        private void Update()
        {
            //多了个if判定，比起后续的代码来说对性能的损失应该不会太大
            if (LateStartRun)
            {
                LateStart.Invoke();
                LateStartRun = false;
            }

            FastUpdate.Invoke();

            FakeLateUpdate.Invoke();
        }

        private IEnumerator<float> SlowUpdatea()
        {
            while (true)
            {
                SlowUpdate.Invoke();
                yield return 0f;
            }
        }
    }

}