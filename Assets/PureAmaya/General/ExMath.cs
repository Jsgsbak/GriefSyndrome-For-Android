using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PureAmaya.General
{
    public class ExMath
    {
        /// <summary>
        /// 返回整数类型的绝对值
        /// </summary>
        /// <param name="Int"></param>
        /// <returns></returns>
        public static int Abs(int Int)
        {
            if (Int < 0)
            {
                return -Int;
            }
            else
            {
                return Int;
            }
        }

        /// <summary>
        /// 返回单精度浮点的绝对值
        /// </summary>
        /// <param name="float"></param>
        /// <returns></returns>
        public static float Abs(float Float)
        {
            if (Float < 0)
            {
                return -Float;
            }
            else
            {
                return Float;
            }
        }

        /// <summary>
        /// 在给定精度范围内，判断两个浮点数是否相同
        /// </summary>
        /// <param name="precision">判断精度</param>
        /// <param name="AllowEqual">是否允许差值相等</param>
        /// <param name="Valve">要比较的值</param>
        /// <returns></returns>
        public static bool Approximation(float precision, float Valve1,float Valve2, bool AllowEqual = true)
        {
            switch (AllowEqual)
            {
                case true:
                    return Abs(Valve1 - Valve2) <= precision;

                case false:
                    return Abs(Valve1 - Valve2) < precision;

            }

        }

        /// <summary>
        /// 指数函数计算（可能性能高点）
        /// </summary>
        /// <param name="exponent">指数</param>
        /// <param name="Base">底数</param>
        /// <returns></returns>
        public static int ExponentialFunction(int exponent,int Base = 10)
        {
            for (int i = 0; i < exponent; i++)
            {
                Base *= Base;
            }

            return Base;

        }






    }
}

