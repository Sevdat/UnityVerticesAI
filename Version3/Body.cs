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

        public Vector3[] moveHipY(){
            hip = rotateObject(new Vector3(0,10,0),hip[0],hip);
            updateBody();
            return hip;
        }
        public Vector3[] moveHipZ(){
            hip = rotateObject(new Vector3(0,0,10),hip[0],hip);
            updateBody();
            return hip;
        }
        public void drawBody(){
            createOrDeleteObject(hip, true);
        }
        public void updateBody(
            ){
                knee = new Vector3[]{hip[1],hip[2],hip[3]};
                foot = new Vector3[]{knee[1],knee[2]};
                hip = new Vector3[]{hip[0],knee[0],foot[0],foot[1]};
        }
    }
    Vector3[] bodyComponents = new Vector3[]{
         new Vector3(10f,12f,13f),
         new Vector3(10f,7f,13f),
         new Vector3(10f,2f,13f),
         new Vector3(10f,2f,16f)
         };    
    void Start(){
        joints = new bodyStructure(){
        hip = bodyComponents   
        };
    }
    // Update is called once per frame
    float time = 0;
    void Update(){
        time += Time.deltaTime;
        if (time >1f){
            joints.moveHipY();
            joints.moveHipZ();
            joints.drawBody();
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
        // WorldBuilder.createOrDeleteObject(joints.hip,true);

    //         IEnumerator Lol(){
    //     yield return joints.moveHipY();
    //     yield return joints.moveHipZ();
    // }
