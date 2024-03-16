using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3Int[] chest = new Vector3Int[]{
         new Vector3Int(7,3,5),new Vector3Int(3,5,10)
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
        const int rotateX = 0;
        const int rotateY = 1;
        const int rotateZ = 2;
      Vector3Int rotate(
        float theta, float alpha,
        Vector3Int origin,Vector3Int point,
        int direction
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
            float currentTheta,adjacent,currentAlpha;
            float x = 0;
            float y = 0;
            float z = 0;
            switch(direction){
                case rotateX:
                    currentTheta = Mathf.Asin(lineZ/radius);
                    adjacent = radius*Mathf.Cos(currentTheta);
                    currentAlpha = Mathf.Acos(lineX/adjacent);
                    alpha = alpha*angleToRadian + Mathf.Sign(lineY)*currentAlpha;
                    theta = theta*angleToRadian + currentTheta;
                    y = adjacent*Mathf.Sin(alpha);
                    x = adjacent*Mathf.Cos(alpha);
                    z = radius*Mathf.Sin(theta);
                break;
                case rotateY:
                    currentTheta = Mathf.Asin(lineX/radius);
                    adjacent = radius*Mathf.Cos(currentTheta);
                    currentAlpha = Mathf.Acos(lineZ/adjacent);
                    alpha = alpha*angleToRadian + Mathf.Sign(lineZ)*currentAlpha;
                    theta = theta*angleToRadian + currentTheta;
                    y = adjacent*Mathf.Sin(alpha);
                    z = adjacent*Mathf.Cos(alpha);
                    x = radius*Mathf.Sin(theta);
                break;
                case rotateZ:
                    currentTheta = Mathf.Asin(lineY/radius);
                    adjacent = radius*Mathf.Cos(currentTheta);
                    currentAlpha = Mathf.Acos(lineZ/adjacent);
                    print(currentAlpha);
                    alpha = alpha*angleToRadian + Mathf.Sign(lineX)*currentAlpha;
                    theta = theta*angleToRadian + currentTheta;
                    x = adjacent*Mathf.Sin(alpha);
                    z = adjacent*Mathf.Cos(alpha);
                    y = radius*Mathf.Sin(theta);
                break;
            }
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
                    chest[0], rotate(0,0,chest[0],chest[1],0)
                    ),true
                );
                i++;
        }
    }
}
