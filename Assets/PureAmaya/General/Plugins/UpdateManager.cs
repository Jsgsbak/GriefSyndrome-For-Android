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
        /*老式的

                /// <summary>
                /// 统一调用Update的接口
                /// </summary>
                public interface IUpdate
                {
                    /// <summary>
                    /// 更快的Update
                    /// </summary>
                    void FastUpdate();
                }

                /// <summary>
                /// 假的LateUpdate（所有FastUpdate执行后再执行）
                /// </summary>
                public interface IFakeLateUpdate
                {
                    void FakeLateUpdate();
                }

                public static List<IUpdate> UpdateInvoke = new List<IUpdate>();
                public static List<IFakeLateUpdate> FakeLateUpdateInvoke = new List<IFakeLateUpdate>();

                /// <summary>
                /// 注册一个新的Update
                /// </summary>
                /// <param name="update"></param>
                public static void AddUpdate(IUpdate update)
                {
                    if (!UpdateInvoke.Contains(update))
                    {
                        UpdateInvoke.Add(update);
                    }
                }
                /// <summary>
                /// 注册一个新的FakeLateUpdate
                /// </summary>
                /// <param name="update"></param>
                public static void AddUpdate(IFakeLateUpdate fakeLateUpdate)
                {
                    if (!FakeLateUpdateInvoke.Contains(fakeLateUpdate))
                    {
                        FakeLateUpdateInvoke.Add(fakeLateUpdate);
                    }
                }


                /// <summary>
                /// 移除一个已有的Update
                /// </summary>
                /// <param name="update"></param>
                public static void RemoveUpdate(IUpdate update)
                {
                    if (UpdateInvoke.Contains(update))
                    {
                        UpdateInvoke.Remove(update);
                    }
                }
                /// <summary>
                /// 移除一个已有的FakeLateUpdate
                /// </summary>
                /// <param name="update"></param>
                public static void RemoveUpdate(IFakeLateUpdate FakeLateUpdate)
                {
                    if (FakeLateUpdateInvoke.Contains(FakeLateUpdate))
                    {
                        FakeLateUpdateInvoke.Remove(FakeLateUpdate);
                    }
                }

                private void Update()
                {

                    for (int i = 0; i < UpdateInvoke.Count; i++)
                    {
                        UpdateInvoke[i].FastUpdate();

                    }


                }        */
   
    


        //*新的事件版
        public class UpdateEventClass : UnityEvent { }
        public static UpdateEventClass FastUpdate = new UpdateEventClass();
        /// <summary>
        /// 假的LateUpdate（所有FastUpdate执行后，且LateUpdate执行前执行）
        /// </summary>
        public static UpdateEventClass FakeLateUpdate = new UpdateEventClass();

        private void Update()
        {
            FastUpdate.Invoke();

            FakeLateUpdate.Invoke();
        }
    }

}