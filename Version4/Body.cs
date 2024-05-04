using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    bodyStructure joints;
    public class bodyStructure : WorldBuilder{
        public int[][][] bodyHierarchy;
        public Vector3[] localConnections;
    }
    public struct index {
        public int currentIndex;
        public indexConnections[] connections;
        public index(int currentIndex, indexConnections[] connections) {
            this.currentIndex = currentIndex;
            this.connections = connections;
        }
    }
    public struct indexConnections {
        public int connectedIndex;
        public float radius;
        public indexConnections(int connectedIndex, float radius) {
            this.connectedIndex = connectedIndex;
            this.radius = radius;
        }
    }
    public indexConnections connections(int connectedIndex, float radius){
        return new indexConnections(connectedIndex,radius);
    }
    public HashSet<int> createSet(int size){
        HashSet<int> set = new HashSet<int>();
        for(int i = 0; i < size; i++){
            set.Add(i);
        }
        return set;
    }
    void Start(){
        index index0 = new index(
                0, new indexConnections[]{
                    connections(1,2f)
                });
        index index1 = new index(
                1, new indexConnections[]{
                    connections(2,1f)
                });
        index index2 = new index(
                2, new indexConnections[]{
                    connections(3,1f)
                });
        index index3 = new index(
                3, new indexConnections[]{
                    connections(4,3f),
                    connections(33,0f),
                    connections(34,0f)
                });
        index index4 = new index(
                4, new indexConnections[]{
                    connections(5,3f)
                });
        index index5 = new index(
                5, new indexConnections[]{
                    connections(6,2f)
                });
        index index6 = new index(
                6, new indexConnections[]{
                    connections(35,0f),
                    connections(36,0f)
                });
        index index7 = new index(
                7, new indexConnections[]{
                    connections(9,3f)
                });
        index index8 = new index(
                8, new indexConnections[]{
                    connections(10,3f)
                });
        index index9 = new index(
                9, new indexConnections[]{
                    connections(11,3f)
                });
        index index10 = new index(
                10, new indexConnections[]{
                    connections(12,3f)
                });
        index index11 = new index(
                11, new indexConnections[]{
                    connections(13,3f)
                });
        index index12 = new index(
                12, new indexConnections[]{
                    connections(14,3f)
                });
        index index13 = new index(
                13, new indexConnections[]{
                });
        index index14 = new index(
                14, new indexConnections[]{}
                );
        index index15 = new index(
                15, new indexConnections[]{
                    connections(17,3f)
                });
        index index16 = new index(
                16, new indexConnections[]{
                    connections(18,3f)
                });
        index index17 = new index(
                17, new indexConnections[]{
                    connections(19,3f)
                });
        index index18 = new index(
                18, new indexConnections[]{
                    connections(20,3f)
                });
        index index19 = new index(
                19, new indexConnections[]{
                    connections(21,1f),
                    connections(23,1f),
                    connections(31,1f)
                });
        index index20 = new index(
                20, new indexConnections[]{
                    connections(24,1f),
                    connections(26,1f),
                    connections(28,1f)
                });
        index index21 = new index(
                21, new indexConnections[]{
                    connections(22,1f)
                });
        index index22 = new index(
                22, new indexConnections[]{}
                );  
        index index23 = new index(
                23, new indexConnections[]{
                    connections(30,1f)
                });
        index index24 = new index(
                24, new indexConnections[]{
                    connections(25,1f),
                });
        index index25 = new index(
                25, new indexConnections[]{}
                );
        index index26 = new index(
                26, new indexConnections[]{
                    connections(27,1f),
                });
        index index27 = new index(
                27, new indexConnections[]{}
                );
        index index28 = new index(
                28, new indexConnections[]{
                    connections(29,1f),
                });
        index index29 = new index(
                29, new indexConnections[]{
                });
        index index30 = new index(
                30, new indexConnections[]{}
                );
        index index31 = new index(
                31, new indexConnections[]{
                    connections(32,1f),
                });
        index index32 = new index(
                32, new indexConnections[]{}
                );
        index index33 = new index(
                33, new indexConnections[]{
                    connections(15,3f)
                });
        index index34 = new index(
                34, new indexConnections[]{
                    connections(16,3f)
                });
        index index35 = new index(
                35, new indexConnections[]{
                    connections(7,3f)
                });
        index index36 = new index(
                36, new indexConnections[]{
                    connections(8,3f)
                });

        List<index> jointList = new List<index>{
            index0,index1,index2,index3,index4,
            index5,index6,index7,index8,index9,
            index10,index11,index12,index13,index14,
            index15,index16,index17,index18,index19,
            index20,index21,index22,index23,index24,
            index25,index26,index27,index28,index29,
            index30,index31,index32,index33,index34,
            index35,index36
        };
        Vector3 startPoint = new Vector3(20,30,20);
        joints = jointHierarchy(startPoint,jointList);
        for(int i = 0; i <joints.localConnections.Length;i = i+4){
            Vector3 vec = joints.localConnections[i];
            print($"{i/4} : {vec}");
        }

        print(joints.localConnections[16*4]);
        rotate(joints,90f,33,3);
        rotate(joints,-90f,34,3);
        
    }
    public indexConnections[][] sortedConnections(List<index> jointList){
        int size = jointList.Count;
        indexConnections[][] sortedJointArray = new indexConnections[size][];
        for (int i = 0; i<size; i++){
            index joint = jointList[i];
            int index = joint.currentIndex;
            indexConnections[] connectedToIndex = joint.connections;
            sortedJointArray[index] = connectedToIndex;
        }
        return sortedJointArray;
    }
    public bodyStructure jointHierarchy(Vector3 startPoint, List<index> jointList){
        indexConnections[][] sortedJointArray = sortedConnections(jointList);
        int size = sortedJointArray.Length;
        HashSet<int> set = createSet(size);
        int[][][] indexParts = new int[size][][];
        for (int i = 0;i<size;i++){
            indexParts[i] = indexHierarchy(i, sortedJointArray, set);
        }
        Vector3[] vecWithAxis = createLine(startPoint,sortedJointArray,set);
        bodyStructure createBody = new bodyStructure(){
            bodyHierarchy = indexParts,
            localConnections = vecWithAxis
        };
        return createBody;
    }
    public int[][] indexHierarchy(int index,indexConnections[][] sortedJointArray,HashSet<int> search){
        HashSet<int> setClone = new HashSet<int>(search);
        List<indexConnections[]> activeConnections = new List<indexConnections[]>(){
            sortedJointArray[index]
        };
        int axisSize = 4;
        List<int> hierarchy = new List<int>(){index*axisSize};
        while (activeConnections.Count != 0){
            indexConnections[] connectedArray = activeConnections[0];
            if (connectedArray!= null){
                for (int i = 0;i<connectedArray.Length;i++){
                    indexConnections connection = connectedArray[i];
                    int searchIndex = connection.connectedIndex;
                    if (setClone.Contains(searchIndex)) {
                        hierarchy.Add(searchIndex*axisSize);
                        activeConnections.Add(sortedJointArray[searchIndex]);
                        setClone.Remove(searchIndex);
                    }
                }
            }
        activeConnections.RemoveAt(0);
        }
        int[] remainder = new int[setClone.Count];
        setClone.CopyTo(remainder);
        return new int[][]{hierarchy.ToArray(),remainder};
    }

    public Vector3[] createLine(Vector3 startPoint,indexConnections[][] sortedJointArray,HashSet<int> search){
        int size = sortedJointArray.Length;
        HashSet<int> setClone = new HashSet<int>(search);
        Vector3[] jointVectors = new Vector3[size*4];
        Vector3 x = new Vector3(3,0,0);
        Vector3 y = new Vector3(0,3,0);
        Vector3 z = new Vector3(0,0,3);
        jointVectors[0] = startPoint;
        jointVectors[1] = startPoint+x;
        jointVectors[2] = startPoint+y;
        jointVectors[3] = startPoint+z;
        List<int> indexList = new List<int>(){0};
        while (indexList.Count != 0){
            indexConnections[] connectionsArray = sortedJointArray[indexList[0]];
            if (connectionsArray!= null){
                for (int i = 0;i<connectionsArray.Length;i++){
                    indexConnections connection = connectionsArray[i];
                    int index = connection.connectedIndex;
                    if (setClone.Contains(index)) {
                        indexList.Add(index);
                        Vector3 vec = jointVectors[indexList[0]*4]-new Vector3(0f,connection.radius,0f);
                        setClone.Remove(index);
                        index*=4;
                        jointVectors[index] = vec;
                        jointVectors[index+1] = vec+x;
                        jointVectors[index+2] = vec+y;
                        jointVectors[index+3] = vec+z;
                    }
                }
            }
        indexList.RemoveAt(0);
        }

        return jointVectors;
    }
        public void rotate(bodyStructure joints,float angle, int index,int rotationAxis){
        int originIndex = index*4;
        Vector3[] bodyVec = joints.localConnections;
        Vector3 origin = bodyVec[originIndex];
        int rotationIndex = originIndex+rotationAxis;
        int[] connected = joints.bodyHierarchy[index][0];
        int size = connected.Length;  
        Vector4 quat = WorldBuilder.QuaternionClass.angledAxis(angle,bodyVec[rotationIndex]-origin);

        for (int i = 0; i<size;i++){
            int indexForGlobal = connected[i];
            bodyVec[indexForGlobal]= WorldBuilder.QuaternionClass.rotate(
                origin,bodyVec[indexForGlobal],quat
            );
            bodyVec[indexForGlobal+1]= WorldBuilder.QuaternionClass.rotate(
                origin,bodyVec[indexForGlobal+1],quat
            );
            bodyVec[indexForGlobal+2]= WorldBuilder.QuaternionClass.rotate(
                origin,bodyVec[indexForGlobal+2],quat
            );
            bodyVec[indexForGlobal+3]= WorldBuilder.QuaternionClass.rotate(
                origin,bodyVec[indexForGlobal+3],quat
            );
        }
    }
    float time = 0;
    Vector3[] bod = new Vector3[60];
    void Update(){
        time += Time.deltaTime;
        if (time >0.1f){
            draw(0);
            time = 0f;
        }
    }
    public void draw(int choice){

        if (choice == 0){
            WorldBuilder.BitArrayManipulator.createOrDeleteObject(joints.localConnections,false);
            WorldBuilder.BitArrayManipulator.createOrDeleteObject(joints.localConnections,true);
        }
        if (choice == 1){
            for(int i = 0; i <joints.localConnections.Length;i = i+4){
                bod[i/4] = joints.localConnections[i];
            }
            WorldBuilder.BitArrayManipulator.createOrDeleteObject(bod,false);
            for(int i = 0; i <joints.localConnections.Length;i = i+4){
                bod[i/4] = joints.localConnections[i];
            }
            // rotate(joints,1f,0,1);
            // rotate(joints,-1f,34,1);
            WorldBuilder.BitArrayManipulator.createOrDeleteObject(bod,true);
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

    //     public bodyStructure initBody(Vector3[][] body){
    //     Vector3 x = new Vector3(3,0,0);
    //     Vector3 y = new Vector3(0,3,0);
    //     Vector3 z = new Vector3(0,0,3);
    //     int size = 0;
    //     for (int i = 0; i < body.Length; i++){
    //         size += body[i].Length;
    //     }
    //     Vector3[] vec = new Vector3[size+size*3];
    //     int count = 0;
    //     for (int i = 0; i<body.Length;i++){
    //         Vector3[] bodyVec = body[i];
    //         for (int e = 0; e< bodyVec.Length;e++){
    //             vec[count*4] = bodyVec[e];
    //             vec[count*4+1] = x;
    //             vec[count*4+2] = y;
    //             vec[count*4+3] = z;
    //             count+=1;
    //         }
    //     }
    //     bodyStructure createBody = new bodyStructure(){
    //         globalBody = vec,
    //         limbArray = new bodyPart[size]
    //     };
    //     return createBody;
    // }
    // public Vector3[] createLines(Vector3 startPoint, int[] substract){
    //     int size = substract.Length;
    //     Vector3[] verticalLine = new Vector3[substract.Length];
    //     for (int i = 0;i<size;i++){
    //         startPoint -= new Vector3(0,substract[i],0);
    //         verticalLine[i] = startPoint;
    //     }
    //     return verticalLine;
    // }
    // public void hierarchy(bodyStructure joints, int index, int[] connected){
    //     for (int i = 0; i<connected.Length;i++){
    //         connected[i] = connected[i]*4;
    //     }
    //     bodyPart move = new bodyPart(){
    //         index = index,
    //         connected = connected
    //     };
    //     joints.limbArray[index] = move;
    // }

    //     public void rotate(bodyStructure joints,float angle, int index,int rotationAxis){
    //     int originIndex = index*4;
    //     Vector3 origin = joints.globalBody[originIndex];
    //     int rotationIndex = originIndex+rotationAxis;
    //     int[] connected = joints.limbArray[index].connected;
    //     int size = connected.Length;  
    //     Vector4 quat = WorldBuilder.QuaternionClass.angledAxis(angle,joints.globalBody[rotationIndex]);

    //     for (int i = 0; i<size;i++){
    //         int indexForGlobal = connected[i];
    //        joints.globalBody[indexForGlobal]= WorldBuilder.QuaternionClass.rotate(
    //             origin,joints.globalBody[indexForGlobal],quat
    //         );
    //         if (indexForGlobal != originIndex && rotationAxis !=1)
    //         joints.globalBody[indexForGlobal+1]= WorldBuilder.QuaternionClass.rotate(
    //             origin,joints.globalBody[indexForGlobal+1],quat
    //         );
    //         if (indexForGlobal != originIndex && rotationAxis !=2)
    //         joints.globalBody[indexForGlobal+2]= WorldBuilder.QuaternionClass.rotate(
    //             origin,joints.globalBody[indexForGlobal+2],quat
    //         );
    //         if (indexForGlobal != originIndex && rotationAxis !=3)
    //         joints.globalBody[indexForGlobal+3]= WorldBuilder.QuaternionClass.rotate(
    //             origin,joints.globalBody[indexForGlobal+3],quat
    //         );
    //     }
    // }

        //     int[] verticalLine = new int[]{
        //     0,
        //     2,
        //     1,
        //     1,
        //     3,0,0,
        //     3,0,0,
        //     2,
        //     1,0,
        //     1,0,0,0,0,0,0,0,
        //     1,0,0,0,0,0,
        //     2,0,
        //     3,0,
        //     3,0

        // };
