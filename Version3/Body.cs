using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    bodyStructure joints;
    public class bodyStructure : WorldBuilder{
        public Vector3 globalAngles = new Vector3(0,0,0);
        public Vector3[] globalBody = new Vector3[]{
         new Vector3(15f,12f,13f),
         new Vector3(15f,12f,10f),
         new Vector3(15f,2f,13f),
         new Vector3(15f,2f,16f)
        };
        public Vector3 localHipAngle = new Vector3(0,0,0);
        public int[] hip = new int[]{0,1,2,3};
        public Vector3 localKneeAngles = new Vector3(0,0,0);
        public int[] knee = new int[]{1,2,3};
        public Vector3 localFootAngles = new Vector3(0,0,0);
        public int[] foot = new int[]{2,3};

        public Vector3[] loadParts(int[] bodyPart){
            int size = bodyPart.Length;
            Vector3[] vec = new Vector3[size];
            for (int i = 0; i < size; i++){
                vec[i] = globalBody[bodyPart[i]];
            }
            return vec;
        }
        public Vector3 movePart(Vector3 angles, int[] bodyPart){
            Vector3[] bodyVec = loadParts(bodyPart);
            Vector3[] angleAndVec = 
                rotateObject(angles,bodyVec[0],bodyVec);
            for (int i = 0; i< bodyVec.Length; i++){
                globalBody[bodyPart[i]] = angleAndVec[i+1];
            }
            return angleAndVec[0];
        }
        public Vector3[] temp = new Vector3[]{new Vector3(0,0,0)};
        public void tempArray(Vector3[] globalBody, float step){
            int size = globalBody.Length-1;
            int stepSize = (int)(1/step);
            createOrDeleteObject(temp,false);
            temp = new Vector3[(int)(1/step)*size];
            for (int i = 0; i < globalBody.Length-1; i++){
                Vector3[] t = diagonal(
                    new Vector3[]{globalBody[i],globalBody[i+1]},
                    step);
                for(int e = 0; e < t.Length; e++){
                    temp[e + i*stepSize] = t[e];
                }
            }
        }
        public void drawBody(){
            createOrDeleteObject(globalBody, true);
        }
    }  
    void Start(){
        joints = new bodyStructure(){ 
        };
        WorldBuilder.createOrDeleteObject(joints.globalBody, false);
        joints.localHipAngle = joints.movePart(new Vector3(x,y,z),joints.hip);
        print(joints.localHipAngle);
        joints.tempArray(joints.globalBody,0.1f);
        joints.drawBody();
    }
    // Update is called once per frame
    float time = 10;
    static float zx = 0.0f*WorldBuilder.angleToRadian;
    static float zy = 0.0f*WorldBuilder.angleToRadian;
    static float zz = 0.0f*WorldBuilder.angleToRadian;
    float x = 10.0f*MathF.Sin(zx);
    float y = 10.0f*MathF.Sin(zy);
    float z = 10.0f*MathF.Sin(zz);

    void Update(){
        time += Time.deltaTime;
        if (time >1f){
            // WorldBuilder.createOrDeleteObject(joints.globalBody, false);
            // joints.localHipAngle = joints.movePart(new Vector3(x,y,z),joints.hip);
            // print(joints.localHipAngle);
            // joints.tempArray(joints.globalBody,0.1f);
            // joints.drawBody();
        time = 0f;
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
