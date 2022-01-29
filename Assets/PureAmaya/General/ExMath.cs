using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PureAmaya.General
{
    public class ExMath
    {
        /// <summary>
        /// 返回xy均为正数的二维向量
        /// </summary>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static Vector2 Abs(Vector2 vector2)
        {
            float x; float y;
            if (vector2.x < 0)
            {
                x = -vector2.x;
            }
            else
            {
                x = vector2.x;
            }

            if (vector2.y < 0)
            {
                y = -vector2.y;
            }
            else
            {
                y = vector2.y;
            }

            return new Vector2(x, y);
        }

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
        /// 两个属性交换值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value1">属性1值</param>
        /// <param name="Value2">属性2值</param>
        /// <param name="Receiver1">接受属性1值</param>
        /// <param name="Receiver2">接受属性2值</param>
        public static T[] Exchange<T>(T Value1, T Value2)
        {
            T[] temp = { Value2, Value1 };
            return temp;
        }

    }
}

