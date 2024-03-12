using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3Int[] chest = new Vector3Int[]{
         new Vector3Int(3,3,5),new Vector3Int(3,3,2)
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
        int angleXY, int angleZ,
        Vector3Int point, Vector3Int origin
         ){
        float radius = MathF.Sqrt(
            Mathf.Pow(origin.x-point.x,2)+
            Mathf.Pow(origin.y-point.y,2)+
            Mathf.Pow(origin.z-point.z,2)
        );
        float faceXY =
            angleXY*Mathf.PI/180 + MathF.Asin((origin.y-point.y)/radius);
        float faceZ = 
            angleZ*Mathf.PI/180 + MathF.Atan((origin.x-point.x)/(origin.z-point.z));
        float sin = Mathf.Sin(faceXY);
        float cos = Mathf.Cos(faceXY);
   
        float sinZ = Mathf.Sin(faceZ);
        float cosZ = Mathf.Cos(faceZ);

        float x = radius*cos*sinZ;
        int x1 = (x>0)? (int)(x +0.5f):(int)(x -0.5f);

        float y = radius*sin*sinZ;
        int y1 = (y>0)? (int)(y +0.5f):(int)(y -0.5f);
        
        float z = radius*cosZ;
        int z1 = (z>0)? (int)(z +0.5f):(int)(z -0.5f);
        return new Vector3Int(x1,y1,z1);
    } 
    // yxz = right(positive goes forward)
    // zxy = front(positive goes forward)
    
    int l = -90;
    void Update(){
        time += Time.deltaTime;
        if (i<361){
            WorldBuilder.createOrDelete(
                WorldBuilder.setVectorInBoundry(
                    chest[0], rotate(0,30,chest[0],chest[1])
                    ),true
                );
                i++;
            if (i>=360 && l <90) {i =0;l+=10;};
        }
    }
}
