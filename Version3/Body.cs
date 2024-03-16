using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3Int[] chest = new Vector3Int[]{
         new Vector3Int(5,3,5),new Vector3Int(3,5,10)
         };
    public static Vector3Int move = new Vector3Int(0,0,1); 
    public static Vector3Int[] tempChest;     
    void Start(){
        WorldBuilder.createOrDelete(
                WorldBuilder.setVectorInBoundry(
                    chest[1], new Vector3Int(0,0,0)
                    ),true
                );
        WorldBuilder.createOrDelete(
                WorldBuilder.setVectorInBoundry(
                    chest[0], new Vector3Int(0,0,0)
                    ),true
                );
    }

    // Update is called once per frame
    float time = 0;
    int i = 0;

    Vector3Int rotateZX(
        float theta, float alpha,
        Vector3Int origin,Vector3Int point
         ){
        float lineX = point.x-origin.x;
        float lineY = point.y-origin.y;
        float lineZ = point.z-origin.z;
        float radius = MathF.Sqrt(
            Mathf.Pow(lineX,2)+
            Mathf.Pow(lineY,2)+
            Mathf.Pow(lineZ,2)
        );
        float angleToRadian = Mathf.PI/180;
        float currentTheta = Mathf.Asin(lineY/radius);
        float adjacent = radius*Mathf.Cos(currentTheta);
        float currentAlpha = Mathf.Acos(lineZ/adjacent);

        alpha = alpha*angleToRadian + currentAlpha;
        theta = theta*angleToRadian + currentTheta;
        float x = adjacent*Mathf.Sin(alpha);
        float z = adjacent*Mathf.Cos(alpha);
        float y = radius*Mathf.Sin(theta);

        int x1 = (x>0)? (int)(x +0.5f):(int)(x -0.5f);
        int y1 = (y>0)? (int)(y +0.5f):(int)(y -0.5f);
        int z1 = (z>0)? (int)(z +0.5f):(int)(z -0.5f);
        return new Vector3Int(x1,y1,z1);
    } 

    Vector3Int rotateXY(
        float theta, float alpha,
        Vector3Int origin,Vector3Int point
         ){
        float lineX = point.x-origin.x;
        float lineY = point.y-origin.y;
        float lineZ = point.z-origin.z;
        float radius = MathF.Sqrt(
            Mathf.Pow(lineX,2)+
            Mathf.Pow(lineY,2)+
            Mathf.Pow(lineZ,2)
        );
        float angleToRadian = Mathf.PI/180;
        float currentTheta = Mathf.Asin(lineZ/radius);
        float adjacent = radius*Mathf.Cos(currentTheta);
        float currentAlpha = Mathf.Acos(lineX/adjacent);

        alpha = alpha*angleToRadian + currentAlpha;
        theta = theta*angleToRadian + currentTheta;
        float y = adjacent*Mathf.Sin(alpha);
        float x = adjacent*Mathf.Cos(alpha);
        float z = radius*Mathf.Sin(theta);

        int x1 = (x>0)? (int)(x +0.5f):(int)(x -0.5f);
        int y1 = (y>0)? (int)(y +0.5f):(int)(y -0.5f);
        int z1 = (z>0)? (int)(z +0.5f):(int)(z -0.5f);
        return new Vector3Int(x1,y1,z1);
    }

    Vector3Int rotateYZ(
        float theta, float alpha,
        Vector3Int origin,Vector3Int point
         ){
        float lineX = point.x-origin.x;
        float lineY = point.y-origin.y;
        float lineZ = point.z-origin.z;
        float radius = MathF.Sqrt(
            Mathf.Pow(lineX,2)+
            Mathf.Pow(lineY,2)+
            Mathf.Pow(lineZ,2)
        );
        float angleToRadian = Mathf.PI/180;
        float currentTheta = Mathf.Asin(lineX/radius);
        float adjacent = radius*Mathf.Cos(currentTheta);
        float currentAlpha = Mathf.Acos(lineZ/adjacent);

        alpha = alpha*angleToRadian + currentAlpha;
        theta = theta*angleToRadian + currentTheta;
        float y = adjacent*Mathf.Sin(alpha);
        float z = adjacent*Mathf.Cos(alpha);
        float x = radius*Mathf.Sin(theta);

        int x1 = (x>0)? (int)(x +0.5f):(int)(x -0.5f);
        int y1 = (y>0)? (int)(y +0.5f):(int)(y -0.5f);
        int z1 = (z>0)? (int)(z +0.5f):(int)(z -0.5f);
        return new Vector3Int(x1,y1,z1);
    }  
    
    int l = -90;
    void Update(){
        time += Time.deltaTime;
        if (i<360){
            WorldBuilder.createOrDelete(
                WorldBuilder.setVectorInBoundry(
                    chest[0], rotateYZ(0,0,chest[0],chest[1])
                    ),true
                );
                i++;
        }
    }
}
