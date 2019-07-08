using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections.Generic;
/// <summary>
/// 游戏流程控制消息处理
/// </summary>

namespace GameSystem
{
    /// <summary>
    /// 游戏消息枚举
    /// </summary>
    public enum GameMessage
    {
        Start,
        Next,
        Exit,
        //...
    }
    public class TheMatrix : MonoBehaviour
    {
        //组件实体
        private static TheMatrix instance;
        private static TheMatrix Instance
        {
            get
            {
                if (instance == null)
                    Debug.Log("error");
                return instance;
            }
        }

        //unity事件
        [System.Serializable]
        public class FloatEvent : UnityEvent<float> { }
        [System.Serializable]
        public class IntEvent : UnityEvent<int> { }


        

        //流程控制
        private IEnumerator InitGame(int level)
        {
            SceneManager.LoadScene("level" + level.ToString());
            while (true)
            {
                yield return 0;
                if (GetGameMessage(GameMessage.Next))
                {
                    StartCoroutine(InitGame(level + 1));
                    yield break;
                }
                if (GetGameMessage(GameMessage.Exit))
                {
                    Application.Quit();
                    yield break;
                }
                //...
            }
        }
        private IEnumerator StartGame()
        {
           yield return InitGame(1);
        }

        //游戏消息控制
        private static bool[] gameMessageArray = new bool[System.Enum.GetValues(typeof(GameMessage)).Length];
        /// <summary>
        /// 检查游戏信息，如果收到信息则返回true
        /// </summary>
        /// <param name="message"></param>
        /// <param name="reset"></param>
        /// <returns></returns>
        public static bool GetGameMessage(GameMessage message,bool reset = true)
        {
            if (gameMessageArray[(int)message])
            {
                if (reset)
                {
                    gameMessageArray[(int)message] = false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 发送游戏信息
        /// </summary>
        /// <param name="message"></param>
        public static void SendGameMessage(GameMessage message)
        {
            if (!gameMessageArray[(int)message])
                gameMessageArray[(int)message] = true;
        }
        /// <summary>
        /// 重置游戏信息
        /// </summary>
        public static void ResetGameMessage()
        {
            gameMessageArray.Initialize();
        }

        //协程控制
        private static Dictionary<System.Type, LinkedList<Coroutine>> routineDic = new Dictionary<System.Type, LinkedList<Coroutine>>();
        /// <summary>
        /// 启用一段协程，并加入协程链中，结束后移除
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static LinkedListNode<Coroutine> StartCoroutine(IEnumerator routine, System.Type key)
        {
            LinkedList<Coroutine> linkedlist;
            if (routineDic.ContainsKey(key))
            {
                linkedlist = routineDic[key];
            }
            else
            {
                linkedlist = new LinkedList<Coroutine>();
                routineDic.Add(key, linkedlist);
            }
            LinkedListNode<Coroutine> node = new LinkedListNode<Coroutine>(null);
            node.Value = Instance.StartCoroutine(SubCoroutine(routine, node));
            linkedlist.AddLast(node);
            return node;
        }
        /// <summary>
        /// 停止协程
        /// </summary>
        /// <param name="node"></param>
        public static void StopCoroutine(LinkedListNode<Coroutine> node)
        {
            if (node == null || node.List == null)
                return;
            Instance.StopCoroutine(node.Value);
            node.List.Remove(node);
        }
        public static void StopAllCoroutine(System.Type key)
        {
            if (!routineDic.ContainsKey(key))
                return;
            LinkedList<Coroutine> list = routineDic[key];
            foreach(Coroutine c in list)
            {
                Instance.StopCoroutine(c);
            }
            list.Clear();
        }
        /// <summary>
        /// 协程结束后，将该协程从协程链中移除
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static IEnumerator SubCoroutine(IEnumerator routine,LinkedListNode<Coroutine> node)
        {
            yield return routine;
            node.List.Remove(node);
            //Debug.Log("remove!");
        
        }

        //存档管理



        void Awake()
        {
            instance = this;
        }
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            StartCoroutine(StartGame());
        }
        private void Update()
        {
            //print(routineDic[typeof(TestSystemSetting)].Count);
        }

    }
}

