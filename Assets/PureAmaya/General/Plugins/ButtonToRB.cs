using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PureAmaya.General
{
    //取代RebindableInput

    /// <summary>
    /// 将按钮“整合”入RB的输入中（用这个的话就不允许在游戏中修改RBAxis了）
    /// </summary>
    [Obsolete]
    public class ButtonToRB : MonoBehaviour
    {
		//先借助RB的一些代码获取RB的有关信息
		static RebindableData rebindableManager;
		static List<RebindableKey> keyDatabase;

		//储存那个键被按下了
		static bool[] WhichKeyIsDown;

		// Use this for initialization
		void Start()
		{
			//获取RB的信息
			rebindableManager = RebindableData.GetRebindableManager();
			List<RebindableKey> keyDatabase = rebindableManager.GetCurrentKeys();
			//长度赋值
			WhichKeyIsDown = new bool[keyDatabase.Count];
		}

        public static bool GetKey(string inputName)
        {


            foreach (RebindableKey key in keyDatabase)
            {
                if (key.inputName == inputName)
                {
                    return Input.GetKey(key.input) || WhichKeyIsDown[keyDatabase.IndexOf(key)];
                }
            }

            throw new RebindableNotFoundException("The rebindable key '" + inputName + "' was not found.\nBe sure you have created it and haven't misspelled it.");


        }


        public static bool GetAxis(string inputName)
        {


            foreach (RebindableKey key in keyDatabase)
            {
                if (key.inputName == inputName)
                {
                    return Input.GetKey(key.input) || WhichKeyIsDown[keyDatabase.IndexOf(key)];
                }
            }

            throw new RebindableNotFoundException("The rebindable key '" + inputName + "' was not found.\nBe sure you have created it and haven't misspelled it.");


        }


        /// <summary>
        /// 按钮输入（只要按下就是true了）
        /// </summary>
        /// <param name="inputName"></param>
        public void GetButton(string inputName)
        {

			foreach (RebindableKey key in keyDatabase)
			{
				if (key.inputName == inputName)
				{
					WhichKeyIsDown[keyDatabase.IndexOf(key)] = true;
				}
			}

		}

        private void LateUpdate()
        {
            for (int i = 0; i < WhichKeyIsDown.Length; i++)
            {
				WhichKeyIsDown[i] = false;

			}
        }
    }
}