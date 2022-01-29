using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PureAmaya.General
{
    public class ExMath
    {
        /// <summary>
        /// ����xy��Ϊ�����Ķ�ά����
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
        /// �����������͵ľ���ֵ
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
        /// ���ص����ȸ���ľ���ֵ
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
        /// �������Խ���ֵ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value1">����1ֵ</param>
        /// <param name="Value2">����2ֵ</param>
        /// <param name="Receiver1">��������1ֵ</param>
        /// <param name="Receiver2">��������2ֵ</param>
        public static T[] Exchange<T>(T Value1, T Value2)
        {
            T[] temp = { Value2, Value1 };
            return temp;
        }

    }
}

