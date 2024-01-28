using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.Tf2;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class VirtualWheelJointState : MonoBehaviour
{
    public string topicName = "/joint";
    public string jointName = "joint";

    public Transform wheelTransform;
    public WheelCollider wheelCollider;
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
            case RotationAxis.X: position[0] = wheelTransform.rotation.eulerAngles.x * Mathf.Deg2Rad; break;
            case RotationAxis.Y: position[0] = wheelTransform.rotation.eulerAngles.y * Mathf.Deg2Rad; break;
            case RotationAxis.Z: position[0] = wheelTransform.rotation.eulerAngles.z * Mathf.Deg2Rad; break;
        }


        double[] velocity = new double[1];
        velocity[0] = wheelCollider.rpm / 60f * 2f * Mathf.PI;

        double[] effort = new double[1];
        effort[0] = Math.Max(wheelCollider.motorTorque, wheelCollider.brakeTorque);
        

        TimeMsg timeMsg = new TimeMsg((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), (uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000000);
        HeaderMsg headerMsg = new HeaderMsg(timeMsg, jointName);
        JointStateMsg jointStateMsg = new JointStateMsg(headerMsg, name, position, velocity, effort);

        ros.Publish(topicName, jointStateMsg);
    }

    public enum RotationAxis { X, Y, Z }

}
