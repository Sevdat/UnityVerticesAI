using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballVisible : MonoBehaviour
{
    void Start(){
        Program.Main();
    }

public class QuaternionClass
{

    // Method to calculate the quaternion from an axis and an angle
    public static Vector4 FromAxisAngle(Vector3 axis, float angle)
    {
        // Normalize the axis
        float length = Mathf.Sqrt(
            Mathf.Pow(axis.x,2.0f) + 
            Mathf.Pow(axis.y,2.0f) + 
            Mathf.Pow(axis.z,2.0f)
            );
        if (length > 0)
        {
            axis.x /= length;
            axis.y /= length;
            axis.z /= length;
        }

        // Convert the angle to radians
        float halfAngle = angle * 0.5f * (Mathf.PI / 180.0f); // Convert degrees to radians
        float sinHalfAngle = Mathf.Sin(halfAngle);

        // Calculate the quaternion components
        float w = Mathf.Cos(halfAngle);
        float x = axis.x * sinHalfAngle;
        float y = axis.y * sinHalfAngle;
        float z = axis.z * sinHalfAngle;

        return new Vector4(x, y, z, w);
    }
    public static Vector4 a(Vector4 q1, Vector4 q2)
    {
        float w = q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z;
        float x = q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y;
        float y = q1.w * q2.y - q1.x * q2.z + q1.y * q2.w + q1.z * q2.x;
        float z = q1.w * q2.z + q1.x * q2.y - q1.y * q2.x + q1.z * q2.w;
        return new Vector4(x, y, z, w);
    }
}

class Program
{
    public static void Main()
    {
        Vector3 B = new Vector3(1, 1, 1); 
        Vector3 rotationAxis = new Vector3(0, 1, 0);
        float angle = (float)Mathf.Atan2(B.y, B.x) * (180.0f / Mathf.PI);
        Vector4 rotation = QuaternionClass.FromAxisAngle(rotationAxis, angle);
        float magnitude = Mathf.Sqrt(
            Mathf.Pow(rotation.w,2.0f) + 
            Mathf.Pow(rotation.x,2.0f) + 
            Mathf.Pow(rotation.y,2.0f) + 
            Mathf.Pow(rotation.z,2.0f)
            );
        if (magnitude > 0)
        {
            rotation.w /= magnitude;
            rotation.x /= magnitude;
            rotation.y /= magnitude;
            rotation.z /= magnitude;
        }
        print($"Quaternion: ({rotation.w}, {rotation.x}, {rotation.y}, {rotation.z})");
        print($"Angle relative to origin: {angle} degrees");
        Vector3 vectorToRotate = new Vector3(0, 1, 0);
        Vector3 rotatedVector = RotateVector(vectorToRotate, rotation);
        print($"Rotated vector: ({rotatedVector.x}, {rotatedVector.y}, {rotatedVector.z})");
    }

    // Method to rotate a vector by a quaternion
    static Vector3 RotateVector(Vector3 vector, Vector4 quaternion)
    {
        // Convert the vector to a quaternion with a scalar part of 0
        Vector4 vectorQuaternion = new Vector4(vector.x, vector.y, vector.z,0);

        // Perform the quaternion multiplication
        Vector4 rotatedQuaternion = QuaternionClass.a(QuaternionClass.a(quaternion,vectorQuaternion), 
        new Vector4(-quaternion.x,-quaternion.y,-quaternion.z,quaternion.w)
        );

        // Extract the vector part of the rotated quaternion
        return new Vector3(rotatedQuaternion.x, rotatedQuaternion.y, rotatedQuaternion.z);
    }
}

}
