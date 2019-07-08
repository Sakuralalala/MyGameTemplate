using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameSystem
{
    /// <summary>
    /// 所有子系统的父类
    /// </summary>
    /// <typeparam name="MySystemSetting">子系统设置类</typeparam>
    public abstract class MySystem<MySystemSetting> where MySystemSetting : ScriptableObject
    {
        private static MySystemSetting setting;
        public static MySystemSetting Setting
        {
            get
            {
                if(setting == null)
                {
                    setting = Resources.Load<MySystemSetting>("System/" + typeof(MySystemSetting).ToString().Substring(11));
                }
                return setting;
            }
        }

        /// <summary>
        /// 调用协程
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static LinkedListNode<Coroutine> StartCoroutine(IEnumerator enumerator)
        {
            return TheMatrix.StartCoroutine(enumerator, typeof(MySystemSetting));
        }
        /// <summary>
        /// 停止协程
        /// </summary>
        /// <param name="node"></param>
        public static void StopCoroutine(LinkedListNode<Coroutine> node)
        {
            TheMatrix.StopCoroutine(node);
        }
        /// <summary>
        /// 停止所有协程
        /// </summary>
        public static void StopAllCoroutine()
        {
            TheMatrix.StopAllCoroutine(typeof(MySystemSetting));
        }






       
    }

}