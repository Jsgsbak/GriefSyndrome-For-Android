using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PureAmaya.General
{
    public class ExMath
    {
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
        /// �ڸ������ȷ�Χ�ڣ��ж������������Ƿ���ͬ
        /// </summary>
        /// <param name="precision">�жϾ���</param>
        /// <param name="AllowEqual">�Ƿ������ֵ���</param>
        /// <param name="Valve">Ҫ�Ƚϵ�ֵ</param>
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
        /// ָ���������㣨�������ܸߵ㣩
        /// </summary>
        /// <param name="exponent">ָ��</param>
        /// <param name="Base">����</param>
        /// <returns></returns>
        public static int ExponentialFunction(int exponent,int Base = 10)
        {
            for (int i = 0; i < exponent; i++)
            {
                Base *= Base;
            }

            return Base;

        }


        /// <summary>
        /// �Ƿ��ڸ����ķ�Χ��
        /// </summary>
        /// <param name="value">����ֵ</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool Range(float value,float min,float max)
        {
            if(min > max)
            {
                float S = min;
                min = max;
                max = S;
            }

            if(value >= min && value <= max)
            {
                return true;
            }
            else
            {
                return
                    false;
            }
        }
    }
}

