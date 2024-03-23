using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    bodyStructure joints;
    public class bodyStructure : WorldBuilder{
        public Vector3[] hip;
        public Vector3[] knee;
        public Vector3[] foot;

        public void moveHip(){
            hip = rotateObject(
                0,50,rotateY,hip[0],hip
            );
            hip = rotateObject(
                0,50,rotateZ,hip[0],hip
            );
            updateBody();
        }
        public void updateBody(
            ){
                knee = new Vector3[]{hip[1],hip[2],hip[3]};
                foot = new Vector3[]{knee[1],knee[2]};
                hip = new Vector3[]{hip[0],knee[0],foot[0],foot[1]};
        }
    }
    Vector3[] bodyComponents = new Vector3[]{
         new Vector3(10f,12f,10f),
         new Vector3(10f,7f,10f),
         new Vector3(10f,2f,10f),
         new Vector3(10f,2f,13f)
         };    
    void Start(){
        joints = new bodyStructure(){
        hip = bodyComponents   
        };
        WorldBuilder.createOrDeleteObject(joints.hip,true);
    }

    // Update is called once per frame
    float time = 0;
    void Update(){
        time += Time.deltaTime;
        if (time >1f){
            joints.moveHip();
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
