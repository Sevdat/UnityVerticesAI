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
        public Vector3 x = new Vector3(3,0,0);
        public Vector3 y = new Vector3(0,3,0);
        public Vector3 z = new Vector3(0,0,3);
        public rotateLimb hip;
        public rotateLimb knee;
        public rotateLimb foot;
        public Vector3 globalAngles;
        public static Vector3[] globalBody = new Vector3[]{
         new Vector3(20f,18f,20f),
         new Vector3(20f,12f,20f),
         new Vector3(20f,4f,20f),
         new Vector3(20f,2f,20f),
         new Vector3(20f,2f,25f),
        };
        public class rotateLimb {
            public Vector3[] localCross;
            public int[] globalIndex;
            public void moveLimb(float alphaAngles, Vector3 localRotationAxis){
                int index = VectorManipulator.localCrossIndex(localRotationAxis);
                Vector3 rotationAxis = localCross[index];
                globalBody = BodyCreator.rotatePart(alphaAngles,globalIndex,rotationAxis,globalBody);
                localCross = BodyCreator.rotateAxis(
                    alphaAngles,localCross,rotationAxis,index,globalBody
                    );
            }
            public void draw(bool drawOrDelete){
                Vector3[] body = BodyCreator.loadParts(globalIndex,globalBody);
                BitArrayManipulator.createOrDeleteObject(body, drawOrDelete);
            }
            public void drawAxis(bool drawOrDelete){
                Vector3 origin = globalBody[globalIndex[0]];
                Vector3[] addedOrigin = VectorManipulator.addToArray(localCross,origin);
                BitArrayManipulator.createOrDeleteObject(addedOrigin, drawOrDelete);
            }
        }
        public void initBody(){
            hip = new rotateLimb(){
                localCross = new Vector3[]{x,y,z},
                globalIndex = new int[]{0,1,2,3,4}
            };
            knee = new rotateLimb(){
                localCross = new Vector3[]{x,y,z},
                globalIndex = new int[]{1,2,3,4}
            };
            foot = new rotateLimb(){
                localCross = new Vector3[]{x,y,z},
                globalIndex = new int[]{2,3,4}
            };
        }
    }  
    void Start(){
        joints = new bodyStructure();
        joints.initBody();
        joints.hip.moveLimb(-50f,joints.x);
        joints.hip.draw(true);
    }

    float angle = 0;
    float time = 0;
    void Update(){
        time += Time.deltaTime;
        if (time >0.01f){
            angle = 1f;
            joints.hip.draw(false);
            joints.hip.drawAxis(false);

            joints.hip.moveLimb(angle,joints.z);

            joints.hip.draw(true);
            joints.hip.drawAxis(true);
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
