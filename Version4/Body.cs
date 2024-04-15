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
         new Vector3(20f,2f,25f),
        };
        public Vector3[] localHipAngle;
        public int[] hip = new int[]{0,1,2,3,4};
        public Vector3 localKneeAngle;
        public int[] knee = new int[]{1,2,3,4};
        public Vector3 localFootAngle;
        public int[] foot = new int[]{2,3,4};
        
        public Vector3 x = new Vector3(3,0,0);
        public Vector3 y = new Vector3(0,3,0);
        public Vector3 z = new Vector3(0,0,3);

        public void moveHip(float alphaAngles, Vector3 localRotationAxis){
            int index = 0;
            if (localRotationAxis == y) index = 1;
            if (localRotationAxis == z) index = 2;
            Vector3 rotationAxis = localHipAngle[index];
            globalBody = BodyCreator.rotatePart(alphaAngles,hip,rotationAxis,globalBody);
            localHipAngle = BodyCreator.rotateAxis(
                alphaAngles,localHipAngle,rotationAxis,index,globalBody
                );
        }
        public void moveKnee(float alphaAngles,Vector3 ax){
            globalBody = BodyCreator.rotatePart(alphaAngles,knee,ax,globalBody);
        }
        public void moveFoot(float alphaAngles,Vector3 ax){
            globalBody = BodyCreator.rotatePart(alphaAngles,foot,ax,globalBody);
        }
        public void initBody(){
            localHipAngle = new Vector3[]{x,y,z};
        }
        public void draw(Vector3[] body, bool drawOrDelete){
            BitArrayManipulator.createOrDeleteObject(body, drawOrDelete);
        }
        public void drawAxis(Vector3[] localAngle,Vector3 origin,bool drawOrDelete){
            Vector3[] addedOrigin = VectorManipulator.addToArray(localAngle,origin);
            BitArrayManipulator.createOrDeleteObject(addedOrigin, drawOrDelete);
        }
    }  
    void Start(){
        joints = new bodyStructure();
        joints.initBody();
         joints.moveHip(-50f,joints.x);
        joints.draw(joints.globalBody, true);
    }

    float angle = 0;
    float time = 0;
    void Update(){
        time += Time.deltaTime;
        if (time >0.01f){
            angle =1f;
            joints.draw(joints.globalBody,false);
            joints.drawAxis(joints.localHipAngle,joints.globalBody[0],false);

            joints.moveHip(angle,joints.z);

            joints.draw(joints.globalBody,true);
            joints.drawAxis(joints.localHipAngle,joints.globalBody[0],true);
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

    //     WorldBuilder.createOrDeleteObject(joints.globalBody, false);
    // print(joints.localKneeAngle);
    // joints.moveKnee(xyMove(xAngle,zAngle));
    // joints.tempArray(joints.globalBody,0.1f);
    // joints.drawBody();
