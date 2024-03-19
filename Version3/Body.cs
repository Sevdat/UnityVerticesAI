using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
     Vector3[] chest = new Vector3[]{
         new Vector3(8,8,8)
         };
    public static Vector3 move = new Vector3(5,5,4); 
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
        if (time > 0.1){
            chest = WorldBuilder.rotateObject(
                0,1,WorldBuilder.rotateX,move,chest
            );
            WorldBuilder.createOrDeleteObject(chest, true);
            time = 0;
        }
    }
}
