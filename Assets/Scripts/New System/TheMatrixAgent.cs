using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

/// <summary>
/// TheMatrix的代理，负责给TheMatrix发送消息
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("Tool/The Matrix Agent")]
public class TheMatrixAgent : MonoBehaviour
{
    public GameMessage message;
    public KeyCode KeyCode = KeyCode.J;

    public void SendGameMessage()
    {
        TheMatrix.SendGameMessage(message);
        print(message + "send!");

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode))
        {
            TestSystem.StartTeat();
            TheMatrix.SendGameMessage(message);
            print(message + "send!");
        }
    }

}
