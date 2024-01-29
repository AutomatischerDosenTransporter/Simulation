using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BasisRoboterTransformManager : MonoBehaviour
{
    public Transform marker;
    public Transform goal;
    public Vector3 difference = Vector3.zero;
    public Vector3 offset = Vector3.zero;

    public Transform x;
    public Axis xAxis = Axis.None;

    public Transform y;
    public Axis yAxis = Axis.None;

    public Transform z;
    public Axis zAxis = Axis.None;


    void Update()
    {
        if (marker == null || goal == null) return;
        if (x == null || y == null || z == null) return;
        if (xAxis == Axis.None || yAxis == Axis.None || zAxis == Axis.None) return;

        difference = marker.position - goal.position;

        Vector3 position;
        switch (xAxis)
        {
            case Axis.X: 
                position = x.position;
                position.x = marker.position.x - offset.x;
                x.position = position;
                break;
            case Axis.Y:
                position = x.position;
                position.y = marker.position.x - offset.x;
                x.position = position;
                break;
            case Axis.Z:
                position = x.position;
                position.z = marker.position.x - offset.x;
                x.position = position;
                break;
        }
        switch (yAxis)
        {
            case Axis.X:
                position = y.position;
                position.x = marker.position.y - offset.y;
                y.position = position;
                break;
            case Axis.Y:
                position = y.position;
                position.y = marker.position.y - offset.y;
                y.position = position;
                break;
            case Axis.Z:
                position = y.position;
                position.z = marker.position.y - offset.y;
                y.position = position;
                break;
        }
        switch (zAxis)
        {
            case Axis.X:
                position = z.position;
                position.x = marker.position.z - offset.z;
                z.position = position;
                break;
            case Axis.Y:
                position = z.position;
                position.y = marker.position.z - offset.z;
                z.position = position;
                break;
            case Axis.Z:
                position = z.position;
                position.z = marker.position.z - offset.z;
                z.position = position;
                break;
        }


    }


    public enum Axis
    {
        X, Y, Z, None
    }

}
