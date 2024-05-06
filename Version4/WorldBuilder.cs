using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Text;
using UnityEngine.PlayerLoop;
using Unity.VisualScripting;

public class WorldBuilder : MonoBehaviour
{
    public GameObject originalObject;
    public static GameObject cloneHierarchy;
    public static GameObject dynamicClone;
    public static GameObject staticClone;
    public static BitArray bitArray;
    public static int arraySize;
    public static int arrayWidth;
    public static Vector3Int dimension = new Vector3Int(70,70,70);
    public static int dimensionX;
    public static int dimensionY;
    public static int dimensionZ;
    public static int cubeXZ;
    public static int cubeX;
    public static float angleToRadian = Mathf.PI/180f;
    public static float radianToAngle = 180f/Mathf.PI;
    public const int rotateX = 0;
    public const int rotateY = 1;
    public const int rotateZ = 2;
    public static float roundUp = 0.5f;
    void Awake()
    {
        dynamicClone = originalObject;
        staticClone = originalObject;
        CreateWorld.initilize();
        Cursor.lockState = CursorLockMode.Locked;
    }
    public class CreateWorld {
        public static void initilize(){
            staticClone.isStatic = true;
            cubeXZ = dimension.x*dimension.z;
            cubeX = dimension.x;
            dimensionX = dimension.x-1;
            dimensionY = dimension.y-1;
            dimensionZ = dimension.z-1;
            rewriteFile(false, false);
            fillEnviroment();
        }
        public static void rewriteFile(bool rewriteAtBegin, bool fillWithOne){
            if (rewriteAtBegin) {
                arraySize = dimension.x*dimension.y*dimension.z;
                bitArray = new BitArray(arraySize);
                for (int i=0; i<arraySize;i++) {
                    bitArray[i] = fillWithOne;
                    } 
                binaryWriter();
                } else {
                binaryReader();
                }
        }
        public static void binaryReader(){
            using (StreamReader reader = new StreamReader("Assets/v3/binaryWorld.txt"))
            {
                dimension.x = reader.Read();//x
                dimension.y = reader.Read();//y (first 3 values in text file)
                dimension.z = reader.Read();//z
                while (reader.Read() != -1) {
                    arrayWidth++;
                }
                arraySize = arrayWidth*8;
                bitArray = new BitArray(arraySize);
                reader.DiscardBufferedData();
                reader.BaseStream.Seek(3, SeekOrigin.Begin);
                int check;
                int index = 0;
                while ((check = reader.Read()) != -1) {
                    for(int i = 0; i<8;i++){
                        bitArray[i + index] = 
                            (check & 0x80) != 0 ? true:false; 
                        check = check << 1;
                    }
                    index += 8;
                }
            }
        }
        public static void binaryWriter(){
            using (StreamWriter writer = new StreamWriter("Assets/v3/binaryWorld.txt"))
            {
                writer.Write((char)dimension.x);
                writer.Write((char)dimension.y);
                writer.Write((char)dimension.z);
                byte value = 0;
                byte bit = 128; //0x80
                for (int i = 0; i < arraySize; i++){
                    if (bitArray[i]) value += bit;
                    bit /= 2;
                    if (bit == 0 || i == arraySize-1) {
                        writer.Write(Convert.ToChar(value));
                        value = 0; bit = 128;
                    }
                }
            }
        }
        public static void fillEnviroment(){
            arraySize = bitArray.Count;
            arrayWidth = (int)Math.Cbrt(arraySize);
            int x = 0;
            int y = 0;
            int z = 0;
            cloneHierarchy = new GameObject(){
                name = "cloneHierarchy",
                isStatic = true
            };
            for (int i = 0; i<arraySize; i++){
                if (bitArray[i]){
                GameObject clone = Instantiate(
                    staticClone, cloneHierarchy.transform
                    );
                Vector3Int vec = new Vector3Int(x,y,z);
                clone.name = $"{i}";
                clone.transform.position = vec;
                }
                x+=1;
                if (z >dimension.z-2 && x > dimension.x-1) {x = 0; z = 0; y += 1;}
                if (x > dimension.x-1) {x = 0; z+=1;} 
            }
        } 
    }

    public class BitArrayManipulator {
        public static int vecToInt(int right, int front, int up){
            return right + front*cubeX + up*cubeXZ;
        }
        public static void createOrDelete(Vector3Int vec, bool bitArrayBool){
            int ballNumber = vecToInt(vec.x, vec.y, vec.z);
            if (!bitArray[ballNumber] && bitArrayBool){
                    bitArray[ballNumber] = true;
                    GameObject clone = Instantiate(
                        dynamicClone, cloneHierarchy.transform
                        );
                    clone.name = $"{ballNumber}";
                    clone.transform.position = vec;
                } else if (bitArray[ballNumber] && !bitArrayBool) {
                    bitArray[ballNumber] = false;
                    Destroy(
                        cloneHierarchy.transform
                            .Find($"{ballNumber}").gameObject
                        );
                }
        } 
        public static int boundry(int location, int dimension){
            if (location > dimension) 
                    location = Math.Abs(location % (dimension+1));
                else if (location < 0) {
                    int size = dimension+1;
                    location = size-Math.Abs(location % size);
                }
            return location;
        }
        public static Vector3Int setVectorInBoundry(Vector3Int pos){
            int x = boundry(pos.x, dimensionX);
            int y = boundry(pos.y, dimensionY);
            int z = boundry(pos.z, dimensionZ);
            return new Vector3Int(x,y,z);
        }
        public static void createOrDeleteObject(
            Vector3[] obj, bool create
            ){ 
            for (int i = 0; i < obj.Length; i++){
                Vector3Int vector = setVectorInBoundry(
                    new Vector3Int(
                        Mathf.RoundToInt(obj[i].x),
                        Mathf.RoundToInt(obj[i].y),
                        Mathf.RoundToInt(obj[i].z)
                    )
                    );
                createOrDelete(vector,create);
            }
        }
    }

    public static class VectorManipulator{
        public static Vector3 vectorDirections(Vector3 origin, Vector3 point){
            float lineX = point.x-origin.x;
            float lineY = point.y-origin.y;
            float lineZ = point.z-origin.z;
            return new Vector3(lineX,lineY,lineZ);
            }
        public static float vectorRadius(Vector3 vectorDirections){
            float radius = MathF.Sqrt(
                Mathf.Pow(vectorDirections.x,2.0f)+
                Mathf.Pow(vectorDirections.y,2.0f)+
                Mathf.Pow(vectorDirections.z,2.0f)
            );
            return radius;
        }
        public static Vector3 crossVector(Vector3 a,Vector3 b){
            Vector3 perpendicular = new Vector3(){
                x = a.y * b.z - a.z * b.y,
                y = a.z * b.x - a.x * b.z,
                z = a.x * b.y - a.y * b.x
            };
            return perpendicular;
        }
        public static Vector3 normalizeVector3(Vector3 vec){    
            float length = Mathf.Sqrt(
                Mathf.Pow(vec.x,2.0f) + 
                Mathf.Pow(vec.y,2.0f) + 
                Mathf.Pow(vec.z,2.0f)
                );
            if (length > 0)
            {
                vec.x /= length;
                vec.y /= length;
                vec.z /= length;
            }
            return vec;
        }
        public static Vector3[] addToArray(Vector3[] vecArray,Vector3 add){
            Vector3[] clone = new Vector3[vecArray.Length];
            for (int i = 0; i< clone.Length; i++){
                Vector3 newVec = vecArray[i] +add;
                clone[i] = newVec;
            }
            return clone;
        }
        public static int localCrossIndex(Vector3 localRotationAxis){
            int index = 0;
            if (localRotationAxis.x == 0){
                index = (localRotationAxis.y != 0)? 1:2;
            }
            return index;
        }
    }
    public static class QuaternionClass {
        public static Vector4 quatMul(Vector4 q1, Vector4 q2) {
            float w = q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z;
            float x = q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y;
            float y = q1.w * q2.y - q1.x * q2.z + q1.y * q2.w + q1.z * q2.x;
            float z = q1.w * q2.z + q1.x * q2.y - q1.y * q2.x + q1.z * q2.w;
            return new Vector4(x, y, z, w);
        }
        public static Vector4 angledAxis(float angle,Vector3 rotationAxis){
                Vector3 perpendicular = VectorManipulator.normalizeVector3(rotationAxis); 
                float halfAngle = angle * 0.5f * (Mathf.PI/180.0f);
                float sinHalfAngle = Mathf.Sin(halfAngle);
                float w = Mathf.Cos(halfAngle);
                float x = perpendicular.x * sinHalfAngle;
                float y = perpendicular.y * sinHalfAngle;
                float z = perpendicular.z * sinHalfAngle;
                return new Vector4(x,y,z,w);
        }
        public static Vector3 rotate(
            Vector3 origin, Vector3 point,Vector4 angledAxis
            ){
            Vector3 rotatedVec = origin;
            if (point != origin){
                Vector3 pointDirection = VectorManipulator.vectorDirections(origin,point);     
                Vector4 rotatingVector = new Vector4(pointDirection.x, pointDirection.y, pointDirection.z,0);
                Vector4 inverseQuat = new Vector4(-angledAxis.x,-angledAxis.y,-angledAxis.z,angledAxis.w);
                Vector4 rotatedQuaternion = quatMul(quatMul(angledAxis,rotatingVector), inverseQuat);

                rotatedVec = origin + new Vector3(
                        rotatedQuaternion.x,
                        rotatedQuaternion.y,
                        rotatedQuaternion.z
                        );
            }
            return rotatedVec;
        }
    }
    
    public static class Movement{
        public static Vector3[] rotateObject(
            float alpha, Vector3 origin,Vector3[] point,Vector3 rotationAxis
            ){
            int size = point.Length;
            Vector4 angledAxis = QuaternionClass.angledAxis(alpha,rotationAxis);
            Vector3[] rotatedVec = new Vector3[size];
            for (int i = 0; i < size; i++){
                Vector3 vec = QuaternionClass.rotate(origin,point[i],angledAxis);
                rotatedVec[i] = vec;
                }
            return rotatedVec;
        }
        public static Vector3[] moveObject(
            Vector3 move, Vector3[] obj
            ){
            BitArrayManipulator.createOrDeleteObject(obj, false);
            Vector3[] movedObj = new Vector3[obj.Length];
            for (int i = 0; i < movedObj.Length; i++){
                movedObj[i] = obj[i]+move; 
                }
            return movedObj;
        }
    }


    public class bodyStructure {
        public List<index> jointList;
        public int[][][] bodyHierarchy;
        public Vector3[] localConnections;
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
        public void jointHierarchy(Vector3 startPoint, List<index> jointList){
            indexConnections[][] sortedJointArray = sortedConnections(jointList);
            int size = sortedJointArray.Length;
            HashSet<int> set = createSet(size);
            int[][][] indexParts = new int[size][][];
            for (int i = 0;i<size;i++){
                indexParts[i] = indexHierarchy(i, sortedJointArray, set);
            }
            Vector3[] vecWithAxis = createLine(startPoint,sortedJointArray,set);
            
                bodyHierarchy = indexParts;
                localConnections = vecWithAxis;
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
        public void rotate(float angle, int index, int rotationAxis){
            int originIndex = index*4;
            Vector3[] bodyVec = localConnections;
            Vector3 origin = bodyVec[originIndex];
            int rotationIndex = originIndex+rotationAxis;
            int[] connected = bodyHierarchy[index][0];
            int size = connected.Length;  
            Vector4 quat = QuaternionClass.angledAxis(angle,bodyVec[rotationIndex]-origin);
            for (int i = 0; i<size;i++){
                int indexForGlobal = connected[i];
                bodyVec[indexForGlobal]= QuaternionClass.rotate(
                    origin,bodyVec[indexForGlobal],quat
                );
                bodyVec[indexForGlobal+1]= QuaternionClass.rotate(
                    origin,bodyVec[indexForGlobal+1],quat
                );
                bodyVec[indexForGlobal+2]= QuaternionClass.rotate(
                    origin,bodyVec[indexForGlobal+2],quat
                );
                bodyVec[indexForGlobal+3]= QuaternionClass.rotate(
                    origin,bodyVec[indexForGlobal+3],quat
                );
            }
        }
        public void invertAxis(int indexHierarchy, bool x, bool y, bool z){
            Vector3[] vec = localConnections;
            int[] indexList = bodyHierarchy[indexHierarchy][0];
            for (int i = 0; i<indexList.Length; i++){
                int index = indexList[i];
                Vector3 origin = vec[index];
                if (x){
                vec[index+1] = origin - VectorManipulator.vectorDirections(origin,vec[index+1]);
                }
                if (y){
                vec[index+2] = origin - VectorManipulator.vectorDirections(origin,vec[index+2]);
                }
                if (z){
                vec[index+3] = origin - VectorManipulator.vectorDirections(origin,vec[index+3]);
                }
            }
        }
        public List<index> sortList(List<index> jointList){
            int size = jointList.Count;
            index[] sortedJointArray = new index[size];
            for (int i = 0; i<size; i++){
                index joint = jointList[i];
                int index = joint.currentIndex;
                sortedJointArray[index] = joint;
            }
            return new List<index>(sortedJointArray);
        }
        public List<index> renumberIndex(List<index> jointList){
            Dictionary<int,int> pairs = new Dictionary<int, int>();
            int size = jointList.Count;
            index[] changeList = new index[jointList.Count];
            for (int i = 0;i<size;i++){
                pairs.Add(jointList[i].currentIndex,i);
            }
            for (int i = 0;i<jointList.Count;i++){
                index index = jointList[i];
                index.currentIndex = pairs[index.currentIndex];
                for (int e = 0;e<index.connections.Length;e++){
                    indexConnections connected = index.connections[e];
                    connected.connectedIndex = pairs[connected.connectedIndex];
                }
                changeList[i] = index;
            }
            foreach (index i in changeList){
                print(i.currentIndex);
            }
            return new List<index>(changeList);
        }
    }



    public static class BodyCreator{
        public static Vector3[] loadParts(int[] bodyPart,Vector3[] globalBody){
            int size = bodyPart.Length;
            Vector3[] vec = new Vector3[size];
            for (int i = 0; i < size; i++){
                vec[i] = globalBody[bodyPart[i]];
            }
            return vec;
        }
        public static Vector3[] diagonal(
            Vector3[] points,
            float step
            ){
            float x0 = points[0].x;
            float x1 = points[1].x;
            float y0 = points[0].y;
            float y1 = points[1].y;
            float z0 = points[0].z;
            float z1 = points[1].z;
            float x = x1-x0;
            float y = y1-y0;
            float z = z1-z0;
            int size = (int)(1/step);
            Vector3[] diagonalArray = new Vector3[size];
            for (int i = 0; i < size; i++){ 
                float f = i*step;
                diagonalArray[i] = new Vector3(
                    x*f+x0,
                    y*f+y0,
                    z*f+z0
                    );
            }
            return diagonalArray;
        }
    }
    public Vector3[] tempConnections = new Vector3[]{new Vector3(0,0,0)};
    public void connectPoints(Vector3[] globalBody, float step){
        int size = globalBody.Length-1;
        int stepSize = (int)(1/step);
        BitArrayManipulator.createOrDeleteObject(tempConnections,false);
        tempConnections = new Vector3[(int)(1/step)*size];
        for (int i = 0; i < globalBody.Length-1; i++){
            Vector3[] t = BodyCreator.diagonal(
                new Vector3[]{globalBody[i],globalBody[i+1]},
                step);
            for(int e = 0; e < t.Length; e++){
                tempConnections[e + i*stepSize] = t[e];
            }
        }
    }

}


/* Find vector from Integer value*/

    // Vector3 vectorFromInt(int location, Vector3 dimension){
    //     // right + (front)*(z) + (up)*(x*z) = int
    //     float[] up = amountOfFullDevision(
    //         location, dimensionXZ
    //         );
    //     float[] front = amountOfFullDevision(
    //         up[0], dimension.x
    //         );
    //         print(front[0]);
    //     return new Vector3(front[0],up[1],front[1]);
    // }
    // float[] amountOfFullDevision(float copyLocation,float dimension){
    //     for (int i =0;true;i++){
    //         if (copyLocation<0) {
    //             if (copyLocation<0) copyLocation += dimension;
    //             return new float[]{copyLocation,i-1};
    //         }
    //         copyLocation -= dimension;
    //     }
    // }

    // public static Vector3 round(Vector3 vec){
    //     float x = vec.x;
    //     float y = vec.y;
    //     float z = vec.z;
    //     x = (x>0)? x +roundUp : x -roundUp;
    //     y = (y>0)? y +roundUp : y -roundUp;
    //     z = (z>0)? z +roundUp : z -roundUp;
    //     return new Vector3(x,y,z);
    // }

    // public static Vector3 rotate(
    //     float alpha,
    //     float radius, float[] vectorDirections,
    //     int rotationDirection
    //     ){
    //     float lineX = vectorDirections[0];
    //     float lineY = vectorDirections[1];
    //     float lineZ = vectorDirections[2];
    //     Vector3 rotatedVec;
    //     float x,y,z;
    //     if (lineX + lineY + lineZ != 0){
    //         switch(rotationDirection){
    //             case rotateX:
    //                 float[] xValues = locatePoint(radius,lineZ,lineY,lineX);
    //                 alpha = alpha*angleToRadian + xValues[0];
    //                 x = xValues[1]*Mathf.Sin(alpha);
    //                 y = xValues[1]*Mathf.Cos(alpha);
    //                 z = lineZ;
    //             break;
    //             case rotateY:
    //                 float[] yValues = locatePoint(radius,lineX,lineY,lineZ);
    //                 alpha = alpha*angleToRadian + yValues[0];
    //                 z = yValues[1]*Mathf.Sin(alpha);
    //                 y = yValues[1]*Mathf.Cos(alpha);
    //                 x = lineX;
    //             break;
    //             case rotateZ:
    //                 float[] zValues = locatePoint(radius,lineY,lineZ,lineX);
    //                 alpha = alpha*angleToRadian + zValues[0];
    //                 x = zValues[1]*Mathf.Sin(alpha);
    //                 z = zValues[1]*Mathf.Cos(alpha);
    //                 y = lineY;
    //             break;
    //             default:
    //             x= 0;
    //             y= 0;
    //             z= 0;
    //             break;
    //         }
    //         rotatedVec = new Vector3(x,y,z);
    //     } else rotatedVec = new Vector3(0,0,0);
    //     return rotatedVec;
    // }

    //  public static Vector3Int vecToVecInt(Vector3 vec){
    //     float x = vec.x;
    //     float y = vec.y;
    //     float z = vec.z;
    //     return new Vector3Int((int)x,(int)y,(int)z);
    // }

// verty strange behavior with floats

    //     public static float[] locatePoint2(
    //     float radius,
    //     float opposite, float axisAdjacent,
    //     float rotatingAxis
    //     ){
    //     float currentTheta = Mathf.Asin(opposite/radius);
        
    //     float adjacent = radius*Mathf.Cos(currentTheta);

    //     float alphaCorrection = axisAdjacent/adjacent;
    //     alphaCorrection = Mathf.Abs(alphaCorrection)>1? 
    //         MathF.Sign(alphaCorrection) : alphaCorrection;
    //     float currentAlpha = Mathf.Acos(alphaCorrection);
    //     float rotationSide = Mathf.Sign(rotatingAxis);
    //     return new float[]{
    //         rotationSide*currentAlpha,
    //         adjacent
    //         };
    // }

//         float[] xValues = locatePoint(radius,lineZ,lineY,lineX);
//         alpha = alphaAngles.x*angleToRadian + xValues[0];
//         x = xValues[1]*Mathf.Sin(alpha);
//         y = xValues[1]*Mathf.Cos(alpha);
//         z = lineZ;
//         rotatedVec = origin + new Vector3(x,y,z);
    
//     if (true) {
//         vectorDirection = vectorDirections(origin,rotatedVec);
//         lineX = vectorDirection[0];
//         lineY = vectorDirection[1];
//         lineZ = vectorDirection[2];
//         float[] yValues = locatePoint(radius,lineX,lineY,lineZ);
//         alpha = alphaAngles.y*angleToRadian + yValues[0];
//         z = yValues[1]*Mathf.Sin(alpha);
//         y = yValues[1]*Mathf.Cos(alpha);
//         x = lineX;
//         rotatedVec = origin + new Vector3(x,y,z);
//     }
//     if (true) {
//         vectorDirection = vectorDirections(origin,rotatedVec);
//         lineX = vectorDirection[0];
//         lineY = vectorDirection[1];
//         lineZ = vectorDirection[2];
//         float[] zValues = locatePoint(radius,lineY,lineZ,lineX);
//         alpha = alphaAngles.z*angleToRadian + zValues[0];
//         x = zValues[1]*Mathf.Sin(alpha);
//         z = zValues[1]*Mathf.Cos(alpha);
//         y = lineY;
//         rotatedVec = origin + new Vector3(x,y,z);
//     }
// }

        // public static Vector4 axisAngle(float angle, Vector3 axis) {
        //     float halfAngle = angle * 0.5f * (Mathf.PI/180.0f);
        //     float sina = Mathf.Sin(halfAngle);
        //     float cosa = Mathf.Cos(halfAngle);
        //     float w = cosa;
        //     float x = axis.x * sina;
        //     float y = axis.y * sina;
        //     float z =  axis.z * sina;
        //     return new Vector4(x, y, z, w);
        // }
        // public static Vector4 fromToRotation(Vector3 aFrom, Vector3 aTo)
        // {
        //     Vector3 axis = VectorManipulator.normalizeVector3(
        //         VectorManipulator.crossVector(aFrom, aTo)
        //         );
        //     float angle = Vector3.Angle(aFrom, aTo);
        //     return axisAngle(angle, axis.normalized);
        // }
        // public static Vector4 inverseQuat(Vector4 quat) {
        //     return new Vector4(-quat.x,-quat.y,-quat.z,quat.w);
        // }
        // public static Vector4 axisQuat(Vector3 quat) {
        //     return new Vector4(quat.x, quat.y, quat.z,0);
        // }
        // public static Vector3 rotate(
        //     float angle,Vector3 origin, Vector3 point,Vector3 rotationAxis
        //     ){
        //     Vector3 rotatedVec = origin;
        //     if (point != origin){
        //         Vector3 pointDirection = VectorManipulator.vectorDirections(origin,point);
        //         Vector4 quat = fromToRotation(origin,-point);
        //         Vector4 axis = axisQuat(pointDirection);
        //         Vector4 inverse = inverseQuat(quat);
        //         Vector4 rotatedQuaternion = quatMul(quatMul(quat,axis), inverse);
                
        //         rotatedVec = origin + new Vector3(
        //                 rotatedQuaternion.x,
        //                 rotatedQuaternion.y,
        //                 rotatedQuaternion.z
        //                 );
        //     }
        //     return rotatedVec;
        // }
