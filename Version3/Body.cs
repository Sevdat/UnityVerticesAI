using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3Int[] chest = new Vector3Int[]{
         new Vector3Int(4,4,7),new Vector3Int(4,4,4)
         };
    public static Vector3Int move = new Vector3Int(0,0,1); 
    public static Vector3Int[] tempChest;     
    void Start(){
    }

    // Update is called once per frame
    float time = 0;
    int i = 0;
    Vector3Int rotate(
        int angle,
        Vector3Int point, Vector3Int origin
         )
         {
        float radius = MathF.Sqrt(
            Mathf.Pow(origin.x-point.x,2)+
            Mathf.Pow(origin.y-point.y,2)+
            Mathf.Pow(origin.z-point.z,2)
        );
        float sin = radius*Mathf.Sin(angle*Mathf.PI/180);
        float cos = radius*Mathf.Cos(angle*Mathf.PI/180);
        int x = (sin>0)? (int)(sin +0.5f):(int)(sin -0.5f);
        int y = (cos>0)? (int)(cos +0.5f):(int)(cos -0.5f);
        print($"{x}:{y}:{radius}");
        return new Vector3Int(x,y,0);
        
    }
    void Update(){
        time += Time.deltaTime;
        if (time > 0.1f){

            WorldBuilder.createOrDelete(
                WorldBuilder.setVectorInBoundry(
                    chest[0], rotate(i,chest[0],chest[1])
                    ),true
                );
            time = 0;
            i++;
        }
    }
}
