using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    bodyStructure joints;
    public class bodyStructure : WorldBuilder{
        public Vector3 globalAngles;
        public Vector3[] globalBody = new Vector3[]{
         new Vector3(20f,18f,20f),
         new Vector3(20f,12f,20f),
         new Vector3(20f,4f,20f),
         new Vector3(20f,2f,20f),
         new Vector3(20f,2f,25f)
        };
        public Vector3 localHipAngle;
        public int[] hip = new int[]{0,1,2,3,4};
        public Vector3 localKneeAngle;
        public int[] knee = new int[]{1,2,3,4};
        public Vector3 localFootAngle;
        public int[] foot = new int[]{2,3,4};

        public void moveHip(float alphaAngles,Vector3 rotationAxis){
            movePart(alphaAngles,hip,rotationAxis);
            localHipAngle = getAngles(globalBody[hip[0]],globalBody[hip[1]]);
        }
        public void moveKnee(float alphaAngles,Vector3 rotationAxis){
            movePart(alphaAngles,knee,rotationAxis);
            localKneeAngle = getAngles(globalBody[knee[0]],globalBody[knee[1]]);
        }
        public void moveFoot(float alphaAngles,Vector3 rotationAxis){
            movePart(alphaAngles,foot,rotationAxis);
            localFootAngle = getAngles(globalBody[foot[0]],globalBody[foot[1]]);
        }
        // public Vector3 rotateBody(Vector3 rot){
        //     float zx = rot.x*angleToRadian;
        //     float zy = rot.y*angleToRadian;
        //     float zz = rot.z*angleToRadian;
        //     float x = zx*Mathf.Cos()*MathF.Sin(globalAngles.x);
        //     float y = zy*Mathf.Cos()*MathF.Sin(globalAngles.y);
        //     float z = zz*MathF.Sin(globalAngles.z);
        //     return new Vector3();
        // }
        public Vector3[] loadParts(int[] bodyPart){
            int size = bodyPart.Length;
            Vector3[] vec = new Vector3[size];
            for (int i = 0; i < size; i++){
                vec[i] = globalBody[bodyPart[i]];
            }
            return vec;
        }
        public void movePart(float angles, int[] bodyPart, Vector3 rotationAxis){
            Vector3[] bodyVec = loadParts(bodyPart);
            Vector3 bodyOrigin = bodyVec[0];
            Vector3[] rotatedVec = rotateObject(angles,bodyOrigin,bodyVec,rotationAxis);
            for (int i = 0; i< bodyVec.Length; i++){
                globalBody[bodyPart[i]] = rotatedVec[i];
            }
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
        public void initBody(){
            globalAngles = new Vector3(0,0,0);
            localHipAngle = getAngles(globalBody[hip[0]],globalBody[hip[1]]);
            localKneeAngle = getAngles(globalBody[knee[0]],globalBody[knee[1]]);
            localFootAngle = getAngles(globalBody[foot[0]],globalBody[foot[1]]);
        }
        public void drawBody(){
            createOrDeleteObject(globalBody, true);
        }
    }  
    void Start(){
        // joints = new bodyStructure(){ 
        // };
        // joints.initBody();
        // joints.moveFoot(new Vector3(0f,0,0f));
        // joints.moveHip(new Vector3(0f,0f,zAngle));
        // joints.drawBody();
        int index = 8;
        axis = new Vector3[6*index];
        for(int i = 0; i<index;i++){
            int loc = i*6;
            axis[0+loc] = origin + new Vector3(i,0,0);
            axis[1+loc] = origin + new Vector3(-i,0,0);

            axis[2+loc] = origin + new Vector3(0,i,0);
            axis[3+loc] = origin + new Vector3(0,-i,0);

            axis[4+loc] = origin + new Vector3(0,0,i);
            axis[5+loc] = origin + new Vector3(0,0,-i);
        }
        WorldBuilder.createOrDeleteObject(axis,true);
    }
    // Update is called once per frame
    float time = 0;
    Vector3 origin = new Vector3(15,15,15);
    // Vector3 point = new Vector3(20,17.887f,15);
    Vector3 point = new Vector3(15,20,20);
    Vector3 rotationAxis = new Vector3(0,0,0);
    Vector3 point2;
    float rotate;
    Vector3[] draw;
    Vector3[] axis;
    void Update(){
        time += Time.deltaTime;
        if (time >0.1f){
        rotate +=1f;
        print(rotate);
        point2 = WorldBuilder.rotate(rotate,origin,point,rotationAxis);
        print(point2);
        draw = new Vector3[]{origin,point2};
        WorldBuilder.createOrDeleteObject(draw,true);
        time = 0f;
        // print(WorldBuilder.getAngles(origin,point));
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

    //     WorldBuilder.createOrDeleteObject(joints.globalBody, false);
    // print(joints.localKneeAngle);
    // joints.moveKnee(xyMove(xAngle,zAngle));
    // joints.tempArray(joints.globalBody,0.1f);
    // joints.drawBody();
