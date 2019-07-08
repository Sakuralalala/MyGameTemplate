using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class TestSystem : MySystem<TestSystemSetting>
    {
        private static IEnumerator Test()
        {
            Debug.Log(Setting.ToString());
            yield return 0;
          
        }
        public static void  StartTeat()
        {
            StartCoroutine(Test());
        }
    }
}
