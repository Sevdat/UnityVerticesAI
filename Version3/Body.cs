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
        public Vector3[] fullBody = new Vector3[]{
         new Vector3(15f,12f,13f),
         new Vector3(15f,7f,13f),
         new Vector3(15f,2f,13f),
         new Vector3(15f,2f,16f)
         };
        public int[] hip = new int[]{0,1,2,3};
        public int[] knee = new int[]{1,2,3};
        public int[] foot = new int[]{2,3};

        public Vector3[] loadParts(int[] bodyPart){
            int size = bodyPart.Length;
            Vector3[] vec = new Vector3[size];
            for (int i = 0; i < size; i++){
                vec[i] = fullBody[bodyPart[i]];
            }
            return vec;
        }
        public void movePart(Vector3 angles, int[] bodyPart){
            Vector3[] bodyVec = loadParts(bodyPart);
            Vector3[] rotatedVec = 
                rotateObject(angles,bodyVec[0],bodyVec);
            for (int i = 0; i< bodyVec.Length; i++){
                fullBody[bodyPart[i]] = rotatedVec[i];
            }
        }
        public void drawBody(){
            createOrDeleteObject(fullBody, true);
        }
    }  
    void Start(){
        joints = new bodyStructure(){ 
        };
    }
    // Update is called once per frame
    float time = 0;
    void Update(){
        time += Time.deltaTime;
        if (time >2f){
            joints.movePart(new Vector3(0,10,10),joints.knee);
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
