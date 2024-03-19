using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3[] chest = new Vector3[]{
         new Vector3(5.8f,5.2f,2.6f),new Vector3(2.6f,3.2f,8.8f)
         };
    public static Vector3 move = new Vector3(6f,6f,6f); 
    public static Vector3[] tempChest;     
    void Start(){
        WorldBuilder.createOrDeleteObject(chest,true);
    }

    // Update is called once per frame
        float time = 0;
        int i = 0;
    
    int l = 0;
    void Update(){
        time += Time.deltaTime;
        if (time >0.01f){
        chest = WorldBuilder.moveObject(
            new Vector3(0f,0f,1f),chest
        );
        chest = WorldBuilder.rotateObject(
            0,1,WorldBuilder.rotateZ,move,chest
        );
        print($"{chest[0]} {chest[1]}");
        time = 0;
        }
    }
}
