using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3Int[] chest = new Vector3Int[]{
         new Vector3Int(3,3,5),new Vector3Int(4,5,2)
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
    //https://stackoverflow.com/questions/20313363/trigonometry-3d-rotation-around-center-point
    Vector3Int rotate(
        float angleY, float angleZ,
        Vector3Int origin,Vector3Int point
         ){
        float lineX = origin.x-point.x;
        float lineY = origin.y-point.y;
        float lineZ = origin.z-point.z;
        float radius = MathF.Sqrt(
            Mathf.Pow(lineX,2)+
            Mathf.Pow(lineY,2)+
            Mathf.Pow(lineZ,2)
        );
        
        float angleToRadian = Mathf.PI/180;
        float faceY = angleY*angleToRadian;
        float faceZ = angleZ*angleToRadian;
        float deg90 = 90*angleToRadian;
        float adjacent = radius*Mathf.Cos(faceY);
        faceZ +=  Mathf.Cos(adjacent/lineZ);
        faceY +=  Mathf.Cos(adjacent/radius);

        float x = adjacent*Mathf.Cos(deg90-faceZ);
        float y = radius*Mathf.Sin(faceY);
        float z = adjacent*Mathf.Cos(faceZ);

        int x1 = (x>0)? (int)(x +0.5f):(int)(x -0.5f);
        int y1 = (y>0)? (int)(y +0.5f):(int)(y -0.5f);
        int z1 = (z>0)? (int)(z +0.5f):(int)(z -0.5f);
        return new Vector3Int(x1,y1,z1);
    } 
    
    int l = -90;
    void Update(){
        time += Time.deltaTime;
        if (i<361){
            WorldBuilder.createOrDelete(
                WorldBuilder.setVectorInBoundry(
                    chest[0], rotate(0,0,chest[0],chest[1])
                    ),true
                );
                i++;
            if (i>=360 && l <90) {i =0;l+=10;};
        }
    }
}
