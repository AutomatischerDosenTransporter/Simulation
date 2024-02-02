using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public string topicName = "/cmd_vel";
    public Mapping vertical;
    public Mapping horizontal;


    public float motorTorque = 2000;
    public float brakeTorque = 2000;
    public float maxSpeed = 20;
    public float steeringRange = 30;
    public float steeringRangeAtMaxSpeed = 10;
    public float centreOfGravityOffset = -1f;

    public WheelCollider leftWheel;
    public WheelCollider rightWheel;


    Rigidbody rigidBody;
    ROSConnection ros;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;


        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<TwistMsg>(topicName, OnTwistMsg);
    }

    private float vInput = 0f;
    private float hInput = 0f;

    void OnTwistMsg(TwistMsg twist)
    {
        switch (vertical)
        {
            case Mapping.LinearX: vInput = (float) twist.linear.x; break;
            case Mapping.LinearY: vInput = (float) twist.linear.y; break;
            case Mapping.LinearZ: vInput = (float) twist.linear.z; break;
            case Mapping.AngularX: vInput = (float) twist.angular.x; break;
            case Mapping.AngularY: vInput = (float) twist.angular.y; break;
            case Mapping.AngularZ: vInput = (float) twist.angular.z; break;
        }

        switch (horizontal)
        {
            case Mapping.LinearX: hInput = (float)twist.linear.x; break;
            case Mapping.LinearY: hInput = (float)twist.linear.y; break;
            case Mapping.LinearZ: hInput = (float)twist.linear.z; break;
            case Mapping.AngularX: hInput = (float)twist.angular.x; break;
            case Mapping.AngularY: hInput = (float)twist.angular.y; break;
            case Mapping.AngularZ: hInput = (float)twist.angular.z; break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // Calculate current speed in relation to the forward direction of the car
        // (this returns a negative number when traveling backwards)
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);


        // Calculate how close the car is to top speed
        // as a number from zero to one
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);

        // Use that to calculate how much torque is available 
        // (zero torque at top speed)
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        // …and to calculate how much to steer 
        // (the car steers more gently at top speed)
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // Check whether the user input is in the same direction 
        // as the car's velocity
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        if (isAccelerating)
        {
            leftWheel.motorTorque = vInput * currentMotorTorque * ((hInput + 1) / 2);
            rightWheel.motorTorque = vInput * currentMotorTorque * (1 - (hInput + 1) / 2);

            leftWheel.brakeTorque = 0;
            rightWheel.brakeTorque = 0;
        } else
        {
            leftWheel.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
            rightWheel.brakeTorque = Mathf.Abs(vInput) * brakeTorque;

            leftWheel.motorTorque = 0;
            rightWheel.motorTorque = 0;
        }
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
