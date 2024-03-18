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
                    WorldBuilder.vecToVecInt(chest[1],false), new Vector3Int(0,0,0)
                    ),true
                );
        WorldBuilder.createOrDelete(
                WorldBuilder.setVectorInBoundry(
                    WorldBuilder.vecToVecInt(chest[0],false), new Vector3Int(0,0,0)
                    ),true
                );
    }

    // Update is called once per frame
        float time = 0;
        int i = 0;
    
    int l = 0;
    void Update(){
        time += Time.deltaTime;
        if (i<360){
            float[] direc = WorldBuilder.vectorDirections(chest[0],chest[1]);
            WorldBuilder.createOrDelete(
                WorldBuilder.moveVector(
                    WorldBuilder.vecToVecInt(chest[0],false), 
                    WorldBuilder.vecToVecInt(
                        WorldBuilder.rotate(0,i,WorldBuilder.vectorRadius(direc),direc,WorldBuilder.rotateX),true
                        )
                    ),true
                );
                i++;
        }
    }
}
