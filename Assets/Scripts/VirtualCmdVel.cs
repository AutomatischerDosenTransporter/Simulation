using RosMessageTypes.Geometry;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class VirtualCmdVel : MonoBehaviour
{
    public string topicName = "/joint";
    public Mapping vertical = Mapping.None;
    public Mapping horizontal = Mapping.None;
    public float timeout = 0;

    ROSConnection ros;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistMsg>(topicName);
    }

    float inputless = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        float vInput = Input.GetAxis("Vertical");
        float hInput = Input.GetAxis("Horizontal");

        Vector3Msg linearMsg = new Vector3Msg();
        Vector3Msg angularMsg = new Vector3Msg();

        if (vInput == 0 && hInput == 0 && timeout > 0)
        {
            inputless += Time.fixedDeltaTime;
            if (inputless > timeout) return;
        } else
        {
            inputless = 0;
        }

        switch(vertical)
        {
            case Mapping.LinearX: linearMsg.x = vInput; break;
            case Mapping.LinearY: linearMsg.y = vInput; break;
            case Mapping.LinearZ: linearMsg.z = vInput; break;
            case Mapping.AngularX: angularMsg.x = vInput; break;
            case Mapping.AngularY: angularMsg.y = vInput; break;
            case Mapping.AngularZ: angularMsg.z = vInput; break;
        }

        switch (horizontal)
        {
            case Mapping.LinearX: linearMsg.x = hInput; break;
            case Mapping.LinearY: linearMsg.y = hInput; break;
            case Mapping.LinearZ: linearMsg.z = hInput; break;
            case Mapping.AngularX: angularMsg.x = hInput; break;
            case Mapping.AngularY: angularMsg.y = hInput; break;
            case Mapping.AngularZ: angularMsg.z = hInput; break;
        }

        TwistMsg twistMsg = new TwistMsg(
                linearMsg,
                angularMsg
            );

        ros.Publish(topicName, twistMsg);
    }

    public enum Mapping
    {
        None,
        LinearX,
        LinearY,
        LinearZ,
        AngularX,
        AngularY,
        AngularZ
    }
}
