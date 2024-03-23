using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    float[] radius = new float[]{6,5,4};
    public struct bodyStructure{
        public Vector3[] hip;
        public Vector3[] knee;
        public Vector3[] foot;
        public void movehip(
            Vector3[] hip
            ){
                knee = new Vector3[]{hip[1],hip[2],hip[3]};
                foot = new Vector3[]{knee[1],knee[2]};
        }
    }
    Vector3[] chest = new Vector3[]{
         new Vector3(10f,12f,10f),
         new Vector3(10f,7f,10f),
         new Vector3(10f,2f,10f),
         new Vector3(10f,2f,13f)
         };
    public static Vector3 move = new Vector3(6f,6f,6f); 
    public static Vector3[] tempChest = new Vector3[]{
        new Vector3(0f,0f,0f)
        };     
    bodyStructure lol;
    void Start(){
        lol = new bodyStructure(){
         hip = chest   
        };
        WorldBuilder.createOrDeleteObject(lol.hip,true);
        lol.hip = WorldBuilder.rotateObject(
            0,50,WorldBuilder.rotateY,lol.hip[0],lol.hip
        );
    }

    // Update is called once per frame
    float time = 0;
    void Update(){
        time += Time.deltaTime;
        if (time >1f){
        lol.movehip(lol.hip);
        lol.hip = WorldBuilder.rotateObject(
            0,50,WorldBuilder.rotateZ,lol.hip[0],lol.hip
        );
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
