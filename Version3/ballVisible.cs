using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballVisible : MonoBehaviour
{
    void Start(){
        Program.Main();
    }

public static class QuaternionClass
{
    public static Vector3 FromAxisAngle(
        float angle,Vector3 rotationAxis,Vector3 rotatingPoint
        ){
        float length = Mathf.Sqrt(
            Mathf.Pow(rotationAxis.x,2.0f) + 
            Mathf.Pow(rotationAxis.y,2.0f) + 
            Mathf.Pow(rotationAxis.z,2.0f)
            );
        if (length > 0)
        {
            rotationAxis.x /= length;
            rotationAxis.y /= length;
            rotationAxis.z /= length;
        }
        float halfAngle = angle * 0.5f * (Mathf.PI/180.0f); // Convert degrees to radians
        float sinHalfAngle = Mathf.Sin(halfAngle);
        float w = Mathf.Cos(halfAngle);
        float x = rotationAxis.x * sinHalfAngle;
        float y = rotationAxis.y * sinHalfAngle;
        float z = rotationAxis.z * sinHalfAngle;

        Vector4 quat = new Vector4(x,y,z,w);
        Vector4 axisQuat = new Vector4(rotatingPoint.x, rotatingPoint.y, rotatingPoint.z,0);
        Vector4 inverseQuat = new Vector4(-x,-y,-z,w);
        Vector4 rotatedQuaternion = quatMul(quatMul(quat,axisQuat), inverseQuat);
        return new Vector3(rotatedQuaternion.x,rotatedQuaternion.y,rotatedQuaternion.z);
    }
    public static Vector4 quatMul(Vector4 q1, Vector4 q2)
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
        Vector3 point = new Vector3(1, 0, 0); 
        Vector3 rotationAxis = new Vector3(0, 0, 1);
        Vector3 rotation = QuaternionClass.FromAxisAngle(10,rotationAxis,point);

        print($"Quaternion: ({rotation.x}, {rotation.y}, {rotation.z})");
    }

}

}
