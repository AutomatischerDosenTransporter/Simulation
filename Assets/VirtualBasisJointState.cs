using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class VirtualBasisJointState : MonoBehaviour
{
    public string topicName = "/joint";
    public string jointName = "joint";

    public Transform tf;
    public Rigidbody rigidbody;
    public RotationAxis rotationAxis;

    ROSConnection ros;


    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointStateMsg>(topicName);
    }


    private void FixedUpdate()
    {

        string[] name = new string[1];
        name[0] = jointName;

        double[] position = new double[1];
        switch (rotationAxis)
        {
            case RotationAxis.X: position[0] = tf.localPosition.x; break;
            case RotationAxis.Y: position[0] = tf.localPosition.y; break;
            case RotationAxis.Z: position[0] = tf.localPosition.z; break;
        }


        double[] velocity = new double[1];
        switch (rotationAxis)
        {
            case RotationAxis.X: velocity[0] = rigidbody.velocity.x; break;
            case RotationAxis.Y: velocity[0] = rigidbody.velocity.y; break;
            case RotationAxis.Z: velocity[0] = rigidbody.velocity.z; break;
        }

        double[] effort = new double[1];
        effort[0] = 0;


        TimeMsg timeMsg = new TimeMsg((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), (uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000000);
        HeaderMsg headerMsg = new HeaderMsg(timeMsg, jointName);
        JointStateMsg jointStateMsg = new JointStateMsg(headerMsg, name, position, velocity, effort);

        ros.Publish(topicName, jointStateMsg);
    }

    public enum RotationAxis { X, Y, Z }

}
