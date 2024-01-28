using RosMessageTypes.Adt;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.Tf2;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

public class VirtualTransform : MonoBehaviour
{

    ROSConnection ros;

    public string topicName = "/tf";
    public string parrentTransformName = "odom";
    public string childTransformName = "TF";

    public Transform transform;
    private Vector3 startPos;
    private Quaternion startRot;


    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TFMessageMsg>(topicName);
    
        startPos = transform.position;
        startRot = transform.rotation;
    }

    private void FixedUpdate()
    {

        TimeMsg timeMsg = new TimeMsg((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), (uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000000);


        var rotation = transform.rotation.To<FLU>();
        var position = transform.position.To<FLU>();

        TFMessageMsg transformMsg = new TFMessageMsg(
                new TransformStampedMsg[]
                {
                    new TransformStampedMsg(
                        new RosMessageTypes.Std.HeaderMsg(timeMsg, parrentTransformName),
                        childTransformName,
                        new TransformMsg(
                                new Vector3Msg(
                                    position.x,
                                    position.y,
                                    position.z
                                    ),
                                new QuaternionMsg(
                                    rotation.x,
                                    rotation.y,
                                    rotation.z,
                                    rotation.w
                                    )
                            )
                        )
                }
            );




        // Finally send the message to server_endpoint.py running in ROS

        ros.Publish(topicName, transformMsg);
    }


    private void OnApplicationQuit()
    {
        transform.SetPositionAndRotation( transform.position, transform.rotation );
        FixedUpdate();
    }

}
