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
        public static Vector3[] globalBody = new Vector3[]{
            new Vector3(20f,18f,20f),
            new Vector3(20f,12f,20f),
            new Vector3(20f,4f,20f),
            new Vector3(20f,2f,20f),
            new Vector3(20f,2f,25f),
        };
        public global globalRotation;
        public static local[] limbArray;
        public class global {
            public Vector3[] globalCross;
            public int[] globalIndex;
            public void moveLimb(float alphaAngles, Vector3 localRotationAxis){
                int index = VectorManipulator.localCrossIndex(localRotationAxis);
                Vector3 rotationAxis = globalCross[index];
                globalBody = BodyCreator.rotatePart(alphaAngles,globalIndex,rotationAxis,globalBody);

                for (int i =0; i<limbArray.Length;i++){
                    local rotateLocal = limbArray[i];
                    globalCross = BodyCreator.rotateAxis(
                        alphaAngles,rotateLocal.localCross,
                        rotationAxis,index,globalBody
                        );
                }
            }
            public void draw(bool drawOrDelete){
                Vector3[] body = BodyCreator.loadParts(globalIndex,globalBody);
                BitArrayManipulator.createOrDeleteObject(body, drawOrDelete);
            }
        }
        public class local {
            public Vector3[] localCross;
            public int[] globalIndex;
            public void moveLimb(float angle, Vector3 localRotationAxis){
                int index = VectorManipulator.localCrossIndex(localRotationAxis);
                Vector3 rotationAxis = localCross[index];
                globalBody = BodyCreator.rotatePart(angle,globalIndex,rotationAxis,globalBody);
            }
            public void moveAxis(float alphaAngles, Vector3 localRotationAxis){
                int index = VectorManipulator.localCrossIndex(localRotationAxis);
                Vector3 rotationAxis = localCross[index];
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
        public void rotateLimb(int index, float angle, Vector3 axis, bool drawOrDelete){
            local limb = limbArray[index];
            limb.moveLimb(angle,axis);
            limb.draw(drawOrDelete);
        }
        public void rotateLimbAxis(int index, float angle, Vector3 axis, bool drawOrDelete){
            local limbAxis = limbArray[index];
            limbAxis.moveAxis(angle,axis);
            limbAxis.drawAxis(drawOrDelete);
        }
        public void rotateGlobally(float angle, Vector3 axis, bool drawOrDelete){
            globalRotation.moveLimb(angle,axis);
            globalRotation.draw(drawOrDelete);
        }
        public void initBody(){
            globalRotation = new global(){
                    globalCross = new Vector3[]{x,y,z},
                    globalIndex = new int[]{0,1,2,3,4}
                };
            limbArray = new local[]{
                new local(){
                    localCross = new Vector3[]{x,y,z},
                    globalIndex = new int[]{0,1,2,3,4}
                },
                new local(){
                    localCross = new Vector3[]{x,y,z},
                    globalIndex = new int[]{1,2,3,4}
                },
                new local(){
                    localCross = new Vector3[]{x,y,z},
                    globalIndex = new int[]{2,3,4}
                },
            };
        }
    }  
    void Start(){
        joints = new bodyStructure();
        joints.initBody();
        joints.rotateGlobally(-50f,joints.z,true);
    }

    float angle = 0;
    float time = 0;
    void Update(){
        time += Time.deltaTime;
        if (time >0.01f){
            angle = 1f;
            joints.rotateLimb(1,0,joints.x,false);
            joints.rotateLimb(1,1,joints.x,true);
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
