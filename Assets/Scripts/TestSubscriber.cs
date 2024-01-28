using RosMessageTypes.Std;
using System;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class TestSubscriber : MonoBehaviour
{

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<StringMsg>("chatter", onChatMessage);
        Debug.Log("Init Chatter");
    }

    void onChatMessage(StringMsg chatMsg)
    {
        Debug.Log(chatMsg.data);
    }

}
