using System;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public CarManager carManager;

    public float motorTorque = 2000;
    public float brakeTorque = 2000;
    public float maxSpeed = 20;
    public float steeringRange = 30;
    public float steeringRangeAtMaxSpeed = 10;
    public float centreOfGravityOffset = -1f;
    public bool ManualDrive = false;
    public bool all_wheels_on_track = true;
    public int LastCheckpoint = 0;


    WheelControl[] wheels;
    Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;

        // Find all child GameObjects that have the WheelControl script attached
        wheels = GetComponentsInChildren<WheelControl>();
    }

    public void resetPosition()
    {
        if (!carManager._carSetupReady) return;
        LastCheckpoint = Math.Max(LastCheckpoint - 3, 0);
        Vector3 startPosition = carManager.centerLine[LastCheckpoint];
        Vector3 direction = carManager.centerLine[LastCheckpoint + 1] - carManager.centerLine[LastCheckpoint];
        Rigidbody carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.Sleep();
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
        carRigidbody.position = startPosition + new Vector3(0, 2, 0);
        carRigidbody.rotation = Quaternion.LookRotation(direction);
        carRigidbody.WakeUp();
    }

    public RawState getRawState()
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

        RawState rawState = new RawState();
        rawState.x = gameObject.transform.position.x;
        rawState.y = gameObject.transform.position.z;
        rawState.speed = rigidBody.velocity.magnitude;
        rawState.steering_angle = wheels[0].WheelCollider.steerAngle;
        rawState.is_reversed = forwardSpeed < 0;
        rawState.all_wheels_on_track = all_wheels_on_track;
        LastCheckpoint = carManager.getClosestWaypoint(gameObject.transform.position);
        rawState.closest_waypoints = new int[] { LastCheckpoint, LastCheckpoint + 1 };
        rawState.progress = LastCheckpoint / carManager.centerLine.Count;
        return rawState;
    }

    public void move(float vInput,  float hInput)
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

        foreach (var wheel in wheels)
        {
            // Apply steering to Wheel colliders that have "Steerable" enabled
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
            }

            if (isAccelerating)
            {
                // Apply torque to Wheel colliders that have "Motorized" enabled
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
                }
                wheel.WheelCollider.brakeTorque = 0;
            }
            else
            {
                // If the user is trying to go in the opposite direction
                // apply brakes to all wheels
                wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
                wheel.WheelCollider.motorTorque = 0;
            }
        }

    }
}
