using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3[] chest = new Vector3[]{
         new Vector3(8f,2f,8f),new Vector3(5f,10f,2f)
         };
    public static Vector3 move = new Vector3(6f,6f,6f); 
    public static Vector3[] tempChest = new Vector3[]{
        new Vector3(0f,0f,0f)
        };     
    void Start(){
        WorldBuilder.createOrDeleteObject(chest,true);
        tempChest = WorldBuilder.diagonal(chest,tempChest,0.05f);
    }

    // Update is called once per frame
    float time = 0;
    void Update(){
        time += Time.deltaTime;
        if (time >0.05f){
        chest = WorldBuilder.moveObject(
            new Vector3(0f,0f,-1f),chest
        );
        chest = WorldBuilder.rotateObject(
            0,1,WorldBuilder.rotateZ,chest[0],chest
        );
        tempChest = WorldBuilder.diagonal(chest,tempChest,0.05f);
        time = 0;
        }
    }
}
        // chest = WorldBuilder.moveObject(
        //     new Vector3(0f,0f,-1f),chest
        // );
        // chest = WorldBuilder.rotateObject(
        //     0,1,WorldBuilder.rotateZ,move,chest
        // );
