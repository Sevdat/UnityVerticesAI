using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3[] chest = new Vector3[]{
         new Vector3(5,5,5),new Vector3(8,8,8)
         };
    public static Vector3 move = new Vector3(0,0,1); 
    public static Vector3[] tempChest;     
    void Start(){
        WorldBuilder.createOrDelete(
                WorldBuilder.setVectorInBoundry(
                    convert(chest[1],false), new Vector3Int(0,0,0)
                    ),true
                );
        WorldBuilder.createOrDelete(
                WorldBuilder.setVectorInBoundry(
                    convert(chest[0],false), new Vector3Int(0,0,0)
                    ),true
                );
    }

    // Update is called once per frame
        float time = 0;
        int i = 0;
    public static float angleToRadian = Mathf.PI/180;
    public const int rotateX = 0;
    public const int rotateY = 1;
    public const int rotateZ = 2;
    float[] vectorDirections(Vector3 origin, Vector3 point){
        float lineX = point.x-origin.x;
        float lineY = point.y-origin.y;
        float lineZ = point.z-origin.z;
        return new float[]{lineX,lineY,lineZ};
        }
    float vectorRadius(float[] vectorDirections){
        float radius = MathF.Sqrt(
            Mathf.Pow(vectorDirections[0],2)+
            Mathf.Pow(vectorDirections[1],2)+
            Mathf.Pow(vectorDirections[2],2)
        );
        return radius;
    }
    Vector3 rotate(
        float theta, float alpha,
        float radius, float[] vectorDirections,
        int direction
         ){
            float lineX = vectorDirections[0];
            float lineY = vectorDirections[1];
            float lineZ = vectorDirections[2];
            float currentTheta,adjacent,currentAlpha;
            float x = 0;
            float y = 0;
            float z = 0;
            switch(direction){
                case rotateX:
                    currentTheta = Mathf.Asin(lineZ/radius);
                    adjacent = radius*Mathf.Cos(currentTheta);
                    currentAlpha = Mathf.Acos(lineY/adjacent);
                    alpha = alpha*angleToRadian + Mathf.Sign(lineX)*currentAlpha;
                    theta = theta*angleToRadian + currentTheta;
                    x = adjacent*Mathf.Sin(alpha);
                    y = adjacent*Mathf.Cos(alpha);
                    z = radius*Mathf.Sin(theta);
                break;
                case rotateY:
                    currentTheta = Mathf.Asin(lineX/radius);
                    adjacent = radius*Mathf.Cos(currentTheta);
                    currentAlpha = Mathf.Acos(lineY/adjacent);
                    alpha = alpha*angleToRadian + Mathf.Sign(lineZ)*currentAlpha;
                    theta = theta*angleToRadian + currentTheta;
                    z = adjacent*Mathf.Sin(alpha);
                    y = adjacent*Mathf.Cos(alpha);
                    x = radius*Mathf.Sin(theta);
                break;
                case rotateZ:
                    currentTheta = Mathf.Asin(lineY/radius);
                    adjacent = radius*Mathf.Cos(currentTheta);
                    currentAlpha = Mathf.Acos(lineZ/adjacent);
                    alpha = alpha*angleToRadian + Mathf.Sign(lineX)*currentAlpha;
                    theta = theta*angleToRadian + currentTheta;
                    x = adjacent*Mathf.Sin(alpha);
                    z = adjacent*Mathf.Cos(alpha);
                    y = radius*Mathf.Sin(theta);
                break;
            }
            return new Vector3(x,y,z);
         }
         Vector3Int convert(Vector3 vec, bool rotation){
            float x = vec.x;
            float y = vec.y;
            float z = vec.z;
            if (rotation){
            x = (x>0)? x +0.5f:x -0.5f;
            y = (y>0)? y +0.5f:y -0.5f;
            z = (z>0)? z +0.5f:z -0.5f;
            }
            return new Vector3Int((int)x,(int)y,(int)z);
         }
    
    int l = 0;
    void Update(){
        time += Time.deltaTime;
        if (i<360){
            float[] direc = vectorDirections(chest[0],chest[1]);
            WorldBuilder.createOrDelete(
                WorldBuilder.moveVector(
                    convert(chest[0],false), 
                    convert(
                        rotate(0,i,vectorRadius(direc),direc,rotateZ),true
                        )
                    ),true
                );
                i++;
        }
    }
}
