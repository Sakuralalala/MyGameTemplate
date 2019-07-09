using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSystem
{
    public class SceneSystem : MySystem<SceneSystemSetting>
    {
        //场景栈
        private static Stack<string> sceneStack = new Stack<string>();
        //异步加载场景
        private static AsyncOperation Async;
        
        /// <summary>
        /// 加载进度事件
        /// </summary>
        public static event Action<float> LoadingProssgress;
        /// <summary>
        /// 场景结束加载事件
        /// </summary>
        public static event Func<float> OnLoad;
        /// <summary>
        /// 场景卸载事件
        /// </summary>
        public static event Func<float> OnUnload;

        /// <summary>
        /// 返回委托链中的最大返回值
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private static float GetMaxReturn(Func<float> func)
        {
            if (func == null)
                return 0;
            Delegate[] list = func.GetInvocationList();
            float max = 0;
            foreach(Delegate d in list)
            {
                Func<float> f = d as Func<float>;
                float temp = f();
                if (temp > max)
                    max = temp;
            }
            return max;
        }

        private static IEnumerator YieldPushScene(string sceneName,bool loadLoadingScene)
        {
            Async = SceneManager.LoadSceneAsync(sceneName, sceneStack.Count == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);

            if (loadLoadingScene)
                SceneManager.LoadScene(Setting.loadingScene, LoadSceneMode.Additive);

            //加载进度效果
            if (!Async.isDone)
            {
                LoadingProssgress?.Invoke(Async.progress);
                yield return 0;
            }

            //加载完成后延迟时间
            yield return new WaitForSeconds(GetMaxReturn(OnLoad));

            if (loadLoadingScene)
                SceneManager.UnloadSceneAsync(Setting.loadingScene);
            //新场景入栈
            sceneStack.Push(sceneName);
        }
        private static IEnumerator YieldPopScene(float delay)
        {
            yield return new WaitForSeconds(delay);
            string toPop = sceneStack.Pop();
            if (sceneStack.Count > 0)
                SceneManager.UnloadSceneAsync(toPop);

        }
        private static IEnumerator YieldPopAndPushScene(float delay,string sceneName,bool loadLoadingScene)
        {
            yield return YieldPopScene(delay);
            yield return YieldPushScene(sceneName, loadLoadingScene);
        }



        /// <summary>
        /// 加载新场景，入栈
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="loadLoadingScene">是否加载loadingScene</param>
        public static void PushScene(string sceneName,bool loadLoadingScene)
        {
            StartCoroutine(YieldPushScene(sceneName, loadLoadingScene));
        }
        /// <summary>
        /// 场景出栈
        /// </summary>
        /// <returns></returns>
        public static float PopScene()
        {
            if (sceneStack.Count == 0)
                return 0;
            float delay = GetMaxReturn(OnUnload);
            StartCoroutine(YieldPopScene(delay));
            return delay;
        }
        /// <summary>
        /// 出栈并且入栈
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loadLoadingScene"></param>
        /// <returns></returns>
        public static float PopAndPushScene(string sceneName,bool loadLoadingScene)
        {
            if (sceneStack.Count == 0)
                return 0;
            float delay = GetMaxReturn(OnUnload);
            StartCoroutine(YieldPopAndPushScene(delay, sceneName, loadLoadingScene));
            return delay;
        }
    }
}
