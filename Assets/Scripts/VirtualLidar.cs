using NUnit.Framework;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Sensor;
using System;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;


public class VirtualLidar : MonoBehaviour

{

    ROSConnection ros;

    public string topicName = "/lidar";
    public string transformName = "odom";

    public float minRange = 00.1f;
    public float maxRange = 10.0f;
    public int scanPoints = 360;

    public bool showRaycastsMissed = false;
    public bool showRaycastsHit = false;
    public bool showHitPoints = true;
    public bool reverseData = true;

    public GameObject sensor;


    void Start()

    {

        // start the ROS connection

        ros = ROSConnection.GetOrCreateInstance();

        ros.RegisterPublisher<LaserScanMsg>(topicName);

    }


    int layerMask = ~(1 << 8);

    private void FixedUpdate()
    {
        float[] distance = new float[scanPoints];

        for (int i = 0; i < scanPoints; i++)
        {
            int index = i;
            if(reverseData) index = scanPoints-1 - index;
            sensor.transform.Rotate(new Vector3(0,360f/scanPoints, 0),Space.Self);

            RaycastHit hit;
            if (Physics.Raycast(sensor.transform.position, sensor.transform.forward, out hit, maxRange, layerMask))
            {
                if(showRaycastsHit)  Debug.DrawLine(sensor.transform.position + sensor.transform.forward * minRange, sensor.transform.position + sensor.transform.forward * hit.distance, Color.yellow);
                if (showHitPoints) Debug.DrawLine(hit.point, hit.point + sensor.transform.forward * -0.02f, Color.green);
                distance[index] = hit.distance;
            }
            else
            {
                if(showRaycastsMissed) Debug.DrawLine(sensor.transform.position + sensor.transform.forward * minRange, sensor.transform.position + sensor.transform.forward * maxRange, Color.white);
                distance[index] = -1;
            }
        }

        TimeMsg timeMsg = new TimeMsg((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), (uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()*1000000);

        LaserScanMsg laserScanMsg = new LaserScanMsg(
            new RosMessageTypes.Std.HeaderMsg(timeMsg,transformName),
            0 * Mathf.Deg2Rad,
            360 * Mathf.Deg2Rad,
            1 * Mathf.Deg2Rad,
            Time.fixedDeltaTime,
            Time.fixedTime,
            minRange,
            maxRange,
            distance,
            new float[] { });


        // Finally send the message to server_endpoint.py running in ROS

        ros.Publish(topicName, laserScanMsg);
    }

    [ExecuteAlways]
    private void OnDrawGizmosSelected()
    {
        Vector3 oldPos = Vector3.zero;
        for(int i = 0; i < scanPoints; i++)
        {
            sensor.transform.Rotate(new Vector3(0, 360f/ scanPoints, 0), Space.Self);

            Vector3 newPos = sensor.transform.forward;
            if(oldPos != Vector3.zero)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(sensor.transform.position + newPos * maxRange, sensor.transform.position + oldPos * maxRange);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(sensor.transform.position + newPos * minRange, sensor.transform.position + oldPos * minRange);
            }
            oldPos = newPos;
        }
    }


}

