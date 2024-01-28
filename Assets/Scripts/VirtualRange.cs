using RosMessageTypes.Adt;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class VirtualRange : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/sensor";
    public string transformName = "sensor";

    public float minRange = 00.0f;
    public float maxRange = 10.0f;
    public float fieldOfView = 30.0f;
    public RadiationType radiationType = RadiationType.ULTRASOUND;
    public int scanPoints = 400;
    public float alpha = 2;

    public bool showRaycastsMissed = false;
    public bool showRaycastsHit = false;
    public bool showHitPoints = true;

    public Transform sensor;


    int layerMask = ~(1 << 8);
    float correction;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RangeMsg>(topicName);
    }

    private void calcCorrection()
    {
        correction = maxRange / Mathf.Tan((90f - fieldOfView / 2) * Mathf.Deg2Rad);
    }

    private void FixedUpdate()
    {
        Vector2[] scanPointsTarget = new Vector2[scanPoints];

        int boundaryPoints = Mathf.RoundToInt(alpha * Mathf.Sqrt(scanPoints));
        float phi = (Mathf.Sqrt(5) + 1) / 2;
        for(int i = 0; i < scanPoints; i++)
        {
            float r = 1;
            if(i<scanPoints-boundaryPoints) r = Mathf.Sqrt(i-1/2)/Mathf.Sqrt(scanPoints-(boundaryPoints+1)/2);

            float theta = 2 * Mathf.PI * i / (phi * phi);

            float x = r * Mathf.Cos(theta);
            float y = r * Mathf.Sin(theta);

            scanPointsTarget[i] = new Vector2(x, y);
        }



        Vector3[] scanPointsDirs = new Vector3[scanPoints];
        calcCorrection();
        for (int i = 0; i < scanPoints; i++)
        {
            Vector3 forward = sensor.transform.forward * maxRange;
            Vector3 horizontal = sensor.transform.up * scanPointsTarget[i].y * correction;
            Vector3 vertical = sensor.transform.right * scanPointsTarget[i].x * correction;

            scanPointsDirs[i] = Vector3.Normalize(forward+horizontal+vertical);
        }


        float closestHit = maxRange;
        foreach(Vector3 dir in scanPointsDirs)
        {
            RaycastHit hit;
            if (Physics.Raycast(sensor.transform.position, dir, out hit, maxRange, layerMask))
            {
                if (showRaycastsHit) Debug.DrawLine(sensor.transform.position + dir * minRange, sensor.transform.position + dir * hit.distance, Color.yellow);
                if (showHitPoints) Debug.DrawLine(hit.point, hit.point + sensor.transform.forward * -0.02f, Color.green);
                if (closestHit > hit.distance) closestHit = hit.distance;
            }
            else
            {
                if (showRaycastsMissed) Debug.DrawLine(sensor.transform.position + dir * minRange, sensor.transform.position + dir * maxRange, Color.white);
            }
        }


        TimeMsg timeMsg = new TimeMsg((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), (uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000000);
        HeaderMsg headerMsg = new HeaderMsg(timeMsg, transformName);

        RangeMsg rangeMsg = new RangeMsg(headerMsg, (byte) radiationType, fieldOfView,minRange,maxRange, closestHit);

        ros.Publish(topicName, rangeMsg);
    }

    public enum RadiationType
    {
        ULTRASOUND = 0b0,
        INFRARED = 0b1
    }


    [ExecuteAlways]
    private void OnDrawGizmosSelected()
    {
        calcCorrection();

        Gizmos.color = Color.blue;

        Vector3 postion = sensor.transform.position;
        Vector3 forward = sensor.transform.forward * maxRange;
        Vector3 horizontal = sensor.transform.up * correction;
        Vector3 vertical = sensor.transform.right * correction;


        Vector3 up__dir = Vector3.Normalize(forward + horizontal);
        Gizmos.DrawLine(postion + up__dir * minRange, postion + up__dir * maxRange);

        Vector3 down_dir = Vector3.Normalize(forward - horizontal);
        Gizmos.DrawLine(postion + down_dir * minRange, postion + down_dir * maxRange);

        Vector3 left_dir = Vector3.Normalize(forward - vertical);
        Gizmos.DrawLine(postion + left_dir * minRange, postion + left_dir * maxRange);

        Vector3 right_dir = Vector3.Normalize(forward + vertical);
        Gizmos.DrawLine(postion + right_dir * minRange, postion + right_dir * maxRange);

        //Gizmos.DrawWireSphere(postion + forward, correction);
    }
}
