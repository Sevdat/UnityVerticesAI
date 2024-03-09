using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3[] chest = new Vector3[]{
         new Vector3(4,4,5),new Vector3(4,4,4)
         };
    public static Vector3 move = new Vector3(0,0,1); 
    public static Vector3[] tempChest;     
    void Start(){
            tempChest = chest;
    }

    // Update is called once per frame
    float time = 0;
    void Update(){
        time += Time.deltaTime;
        if (time > 0.4f){
            WorldBuilder.createFromVector(chest[0],false,move);
            tempChest[0] += move;
            print(tempChest[0]);
            chest = tempChest;
            WorldBuilder.createFromVector(chest[0],true,move);
            time = 0;
        }
    }
}
