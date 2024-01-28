using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

public class VirtualIMU : MonoBehaviour
{
    // https://docs.ros2.org/latest/api/sensor_msgs/msg/Imu.html
    ROSConnection ros;

    public string topicName = "/imu";
    public string transformName = "imu";

    public Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ImuMsg>(topicName);
    }

    private void FixedUpdate()
    {

        TimeMsg timeMsg = new TimeMsg((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), (uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000000);
        HeaderMsg headerMsg = new HeaderMsg(timeMsg, transformName);


        var rotation = transform.rotation.To<FLU>();
        var angularVelocity = rigidbody.angularVelocity.To<FLU>();
        var linearVelocity = rigidbody.velocity.To<FLU>();


        QuaternionMsg quaternionMsg  = new QuaternionMsg( rotation.x, rotation.y, rotation.z, rotation.w);
        Vector3Msg angularVelocityMsg = new Vector3Msg(angularVelocity.x, angularVelocity.y, angularVelocity.z);
        Vector3Msg linearVelocityMsg = new Vector3Msg(linearVelocity.x, linearVelocity.y, linearVelocity.z);

        double[] convergance = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        ImuMsg imuMsg = new ImuMsg(headerMsg,quaternionMsg,convergance,angularVelocityMsg,convergance, linearVelocityMsg, convergance);

        ros.Publish(topicName, imuMsg);
    }
}
