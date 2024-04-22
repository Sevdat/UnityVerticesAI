using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    bodyStructure joints;
    public class bodyStructure : WorldBuilder{
        public Vector3[] globalBody;
        public bodyPart[] limbArray;
    }  
    public class bodyPart: WorldBuilder {
        public int index;
        public int[] connected;
    }
    public bodyStructure initBody(Vector3[][] body){
        Vector3 x = new Vector3(3,0,0);
        Vector3 y = new Vector3(0,3,0);
        Vector3 z = new Vector3(0,0,3);
        int size = 0;
        for (int i = 0; i < body.Length; i++){
            size += body[i].Length;
        }
        Vector3[] vec = new Vector3[size+size*3];
        int count = 0;
        for (int i = 0; i<body.Length;i++){
            Vector3[] bodyVec = body[i];
            for (int e = 0; e< bodyVec.Length;e++){
                vec[count*4] = bodyVec[e];
                vec[count*4+1] = x;
                vec[count*4+2] = y;
                vec[count*4+3] = z;
                count+=1;
            }
        }
        bodyStructure createBody = new bodyStructure(){
            globalBody = vec,
            limbArray = new bodyPart[size]
        };
        return createBody;
    }
    public Vector3[] createLines(Vector3 startPoint, int[] substract){
        int size = substract.Length;
        Vector3[] verticalLine = new Vector3[substract.Length];
        for (int i = 0;i<size;i++){
            startPoint -= new Vector3(0,substract[i],0);
            verticalLine[i] = startPoint;
        }
        return verticalLine;
    }
    public void hierarchy(bodyStructure joints, int index, int[] connected){
        for (int i = 0; i<connected.Length;i++){
            connected[i] = connected[i]*4;
        }
        bodyPart move = new bodyPart(){
            index = index,
            connected = connected
        };
        joints.limbArray[index] = move;
    }

    void Start(){
        Vector3[] eye = createLines(
            new Vector3(20,46,15),
            new int[]{0}
            );
        Vector3[] head1 = createLines(
            new Vector3(22,48,15),
            new int[]{0,6}
            );
        Vector3[] head2 = createLines(
            new Vector3(18,48,15),
            new int[]{0,6}
            );
        
        Vector3[] neck = createLines(
            new Vector3(20,39,15),
            new int[]{0,2,2}
            );

        Vector3[] spine = createLines(
            new Vector3(20,30,15),
            new int[]{0,6,6,4}
            );

        Vector3[] rightArm = createLines(
            new Vector3(35,42,15),
            new int[]{0,6,6,6}
            );
        Vector3[] rightFinger1 = createLines(
            new Vector3(37,20,15),
            new int[]{0,2,2}
            );
        Vector3[] rightFinger2 = createLines(
            new Vector3(35,20,15),
            new int[]{0,2,2}
            );
        Vector3[] rightFinger3 = createLines(
            new Vector3(33,20,15),
            new int[]{0,2,2}
            );
        Vector3[] rightFinger4 = createLines(
            new Vector3(34,13,15),
            new int[]{0,2,2}
            );
        Vector3[] rightFinger5 = createLines(
            new Vector3(36,13,15),
            new int[]{0,2,2}
            );

        Vector3[] leftArm = createLines(
            new Vector3(5,42,15),
            new int[]{0,6,6,6}
            );
        Vector3[] leftFinger1 = createLines(
            new Vector3(7,20,15),
            new int[]{0,2,2}
            );
        Vector3[] leftFinger2 = createLines(
            new Vector3(5,20,15),
            new int[]{0,2,2}
            );
        Vector3[] leftFinger3 = createLines(
            new Vector3(3,20,15),
            new int[]{0,2,2}
            );
        Vector3[] leftFinger4 = createLines(
            new Vector3(4,13,15),
            new int[]{0,2,2}
            );
        Vector3[] leftFinger5 = createLines(
            new Vector3(6,13,15),
            new int[]{0,2,2}
            );

        Vector3[] rightLeg = createLines(
            new Vector3(12,30,15),
            new int[]{0,6,8,8,6}
            );
        Vector3[] leftLeg = createLines(
            new Vector3(28,30,15),
            new int[]{0,6,8,8,6}
            );

        Vector3[][] body = new Vector3[][]{
            eye,head1,head2,neck,
            spine,
            rightArm, rightFinger1, rightFinger2, rightFinger3, rightFinger4,rightFinger5,
            leftArm, leftFinger1, leftFinger2, leftFinger3, leftFinger4, leftFinger5,
            rightLeg,
            leftLeg,
        };

        joints = initBody(
            body
        );
        hierarchy(joints,5,new int[]{0,1,2,3,4,5});
        print(joints.limbArray[5].connected[1]);
    }
    public void rotate(bodyStructure joints,float angle, int index,int rotationAxis){
        int originIndex = index*4;
        Vector3 origin = joints.globalBody[originIndex];
        int rotationIndex = originIndex+rotationAxis;
        int[] connected = joints.limbArray[index].connected;
        print(connected[3]);
        int size = connected.Length;  
        Vector4 quat = WorldBuilder.QuaternionClass.angledAxis(angle,joints.globalBody[rotationIndex]);

        for (int i = 0; i<size;i++){
            int indexForGlobal = connected[i];
           joints.globalBody[indexForGlobal]= WorldBuilder.QuaternionClass.rotate(
                origin,joints.globalBody[indexForGlobal],quat
            );
            if (indexForGlobal != originIndex && rotationAxis !=1)
            joints.globalBody[indexForGlobal+1]= WorldBuilder.QuaternionClass.rotate(
                origin,joints.globalBody[indexForGlobal+1],quat
            );
            if (indexForGlobal != originIndex && rotationAxis !=2)
            joints.globalBody[indexForGlobal+2]= WorldBuilder.QuaternionClass.rotate(
                origin,joints.globalBody[indexForGlobal+2],quat
            );
            if (indexForGlobal != originIndex && rotationAxis !=3)
            joints.globalBody[indexForGlobal+3]= WorldBuilder.QuaternionClass.rotate(
                origin,joints.globalBody[indexForGlobal+3],quat
            );
        }

    }

    float time = 0;
    Vector3[] bod = new Vector3[60];
    void Update(){
        time += Time.deltaTime;
        if (time >0.1f){
                    for(int i = 0;i<60;i++){
            bod[i] = joints.globalBody[i*4];
        }
            WorldBuilder.BitArrayManipulator.createOrDeleteObject(bod,false);
            rotate(joints,10f,5,1);
                    for(int i = 0;i<60;i++){
            bod[i] = joints.globalBody[i*4];
        }
            WorldBuilder.BitArrayManipulator.createOrDeleteObject(bod,true);
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

    //     public class bodyStructure : WorldBuilder{
    //     public Vector3 x = new Vector3(3,0,0);
    //     public Vector3 y = new Vector3(0,3,0);
    //     public Vector3 z = new Vector3(0,0,3);
    //     public static Vector3[] globalBody = new Vector3[]{
    //         new Vector3(20f,18f,20f),
    //         new Vector3(20f,12f,20f),
    //         new Vector3(20f,4f,20f),
    //         new Vector3(20f,2f,20f),
    //         new Vector3(20f,2f,25f),
    //     };
    //     public global globalRotation;
    //     public static local[] limbArray;
    //     public class global {
    //         public Vector3[] globalCross;
    //         public int[] globalIndex;
    //         public void moveLimb(float alphaAngles, Vector3 localRotationAxis){
    //             int index = VectorManipulator.localCrossIndex(localRotationAxis);
    //             Vector3 rotationAxis = globalCross[index];
    //             globalBody = BodyCreator.rotatePart(alphaAngles,globalIndex,rotationAxis,globalBody);

    //             for (int i =0; i<limbArray.Length;i++){
    //                 local rotateLocal = limbArray[i];
    //                 globalCross = BodyCreator.rotateAxis(
    //                     alphaAngles,rotateLocal.localCross,
    //                     rotationAxis,index,globalBody
    //                     );
    //             }
    //         }
    //         public void draw(bool drawOrDelete){
    //             Vector3[] body = BodyCreator.loadParts(globalIndex,globalBody);
    //             BitArrayManipulator.createOrDeleteObject(body, drawOrDelete);
    //         }
    //     }
    //     public class local {
    //         public Vector3[] localCross;
    //         public int[] globalIndex;
    //         public void moveLimb(float angle, Vector3 localRotationAxis){
    //             int index = VectorManipulator.localCrossIndex(localRotationAxis);
    //             Vector3 rotationAxis = localCross[index];
    //             globalBody = BodyCreator.rotatePart(angle,globalIndex,rotationAxis,globalBody);
    //         }
    //         public void moveAxis(float alphaAngles, Vector3 localRotationAxis){
    //             int index = VectorManipulator.localCrossIndex(localRotationAxis);
    //             Vector3 rotationAxis = localCross[index];
    //             localCross = BodyCreator.rotateAxis(
    //                 alphaAngles,localCross,rotationAxis,index,globalBody
    //                 );
    //         }
    //         public void draw(bool drawOrDelete){
    //             Vector3[] body = BodyCreator.loadParts(globalIndex,globalBody);
    //             BitArrayManipulator.createOrDeleteObject(body, drawOrDelete);
    //         }
    //         public void drawAxis(bool drawOrDelete){
    //             Vector3 origin = globalBody[globalIndex[0]];
    //             Vector3[] addedOrigin = VectorManipulator.addToArray(localCross,origin);
    //             BitArrayManipulator.createOrDeleteObject(addedOrigin, drawOrDelete);
    //         }
    //     }
    //     public void rotateLimb(int index, float angle, Vector3 axis, bool drawOrDelete){
    //         local limb = limbArray[index];
    //         limb.moveLimb(angle,axis);
    //         limb.draw(drawOrDelete);
    //     }
    //     public void rotateLimbAxis(int index, float angle, Vector3 axis, bool drawOrDelete){
    //         local limbAxis = limbArray[index];
    //         limbAxis.moveAxis(angle,axis);
    //         limbAxis.drawAxis(drawOrDelete);
    //     }
    //     public void rotateGlobally(float angle, Vector3 axis, bool drawOrDelete){
    //         globalRotation.moveLimb(angle,axis);
    //         globalRotation.draw(drawOrDelete);
    //     }
    //     public void initBody(){
    //         globalRotation = new global(){
    //                 globalCross = new Vector3[]{x,y,z},
    //                 globalIndex = new int[]{0,1,2,3,4}
    //             };
    //         limbArray = new local[]{
    //             new local(){
    //                 localCross = new Vector3[]{x,y,z},
    //                 globalIndex = new int[]{0,1,2,3,4}
    //             },
    //             new local(){
    //                 localCross = new Vector3[]{x,y,z},
    //                 globalIndex = new int[]{1,2,3,4}
    //             },
    //             new local(){
    //                 localCross = new Vector3[]{x,y,z},
    //                 globalIndex = new int[]{2,3,4}
    //             },
    //         };
    //     }
    // }  
