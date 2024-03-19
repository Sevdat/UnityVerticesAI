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
    public static Vector3Int dimension = new Vector3Int(20,20,20);
    public static int dimensionX;
    public static int dimensionY;
    public static int dimensionZ;
    public static int cubeXZ;
    public static int cubeX;
    public static bool disablePlayerControls = false;
    int right = 0;
    int front = 0;
    int up = 0;
    public static float angleToRadian = Mathf.PI/180;
    public const int rotateX = 0;
    public const int rotateY = 1;
    public const int rotateZ = 2;
    public static float roundUp = 0.49f;
    void Awake()
    {
        dynamicClone = originalObject;
        staticClone = originalObject;
        staticClone.isStatic = true;
        cubeXZ = dimension.x*dimension.z;
        cubeX = dimension.x;
        dimensionX = dimension.x-1;
        dimensionY = dimension.y-1;
        dimensionZ = dimension.z-1;
        rewriteFile(false, false);
        createBalls();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void rewriteFile(bool rewriteAtBegin, bool fillWithOne){
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
    void binaryReader(){
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
    void createBalls(){
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
    public static float boundry(float location, float dimension){
        if (location > dimension) 
                location = Math.Abs(location % (dimension+1.0f));
            else if (location < 0.0f) {
                float size = dimension+1.0f;
                location = size-Math.Abs(location % size);
            }
        return location;
    }
    public static Vector3 setVectorInBoundry(Vector3 pos){
        float x = boundry(pos.x, dimensionX);
        float y = boundry(pos.y, dimensionY);
        float z = boundry(pos.z, dimensionZ);
        return new Vector3(x,y,z);
    }
    public static Vector3 round(Vector3 vec){
        float x = vec.x;
        float y = vec.y;
        float z = vec.z;
        x = (x>0)? x +roundUp : x -roundUp;
        y = (y>0)? y +roundUp : y -roundUp;
        z = (z>0)? z +roundUp : z -roundUp;
        return new Vector3(x,y,z);
    }
    public static Vector3Int vecToVecInt(Vector3 vec){
        float x = vec.x;
        float y = vec.y;
        float z = vec.z;
        return new Vector3Int((int)x,(int)y,(int)z);
    }
    public static float[] vectorDirections(Vector3 origin, Vector3 point){
        float lineX = point.x-origin.x;
        float lineY = point.y-origin.y;
        float lineZ = point.z-origin.z;
        return new float[]{lineX,lineY,lineZ};
        }
    public static float vectorRadius(float[] vectorDirections){
        float radius = MathF.Sqrt(
            Mathf.Pow(vectorDirections[0],2)+
            Mathf.Pow(vectorDirections[1],2)+
            Mathf.Pow(vectorDirections[2],2)
        );
        return radius;
    }
    public static float[] locatePoint(
        float radius,
        float opposite, float axisAdjacent,
        float rotatingAxis
        ){
        float currentTheta = Mathf.Asin(opposite/radius);
        float adjacent = radius*Mathf.Cos(currentTheta);
        float currentAlpha = Mathf.Acos(axisAdjacent/adjacent);
        float rotationSide = Mathf.Sign(rotatingAxis);
        return new float[]{
            rotationSide*currentAlpha,
            currentTheta,
            adjacent
            };
    }
    public static Vector3 rotate(
        float theta, float alpha,
        float radius, float[] vectorDirections,
        int rotationDirection
        ){
            float lineX = vectorDirections[0];
            float lineY = vectorDirections[1];
            float lineZ = vectorDirections[2];
            Vector3 rotatedVec = new Vector3();
            float x,y,z;
            switch(rotationDirection){
                case rotateX:
                    float[] xValues = locatePoint(radius,lineZ,lineY,lineX);
                    alpha = alpha*angleToRadian + xValues[0];
                    theta = theta*angleToRadian + xValues[1];
                    x = xValues[2]*Mathf.Sin(alpha);
                    y = xValues[2]*Mathf.Cos(alpha);
                    z = radius*Mathf.Sin(theta);
                    rotatedVec.x = float.IsNaN(x)? 0: x;
                    rotatedVec.y = float.IsNaN(y)? 0: y;
                    rotatedVec.z = float.IsNaN(z)? 0: z;
                break;
                case rotateY:
                    float[] yValues = locatePoint(radius,lineX,lineY,lineZ);
                    alpha = alpha*angleToRadian + yValues[0];
                    theta = theta*angleToRadian + yValues[1];
                    z = yValues[2]*Mathf.Sin(alpha);
                    y = yValues[2]*Mathf.Cos(alpha);
                    x = radius*Mathf.Sin(theta);
                    rotatedVec.z = float.IsNaN(z)? 0: z;
                    rotatedVec.y = float.IsNaN(y)? 0: y;
                    rotatedVec.x = float.IsNaN(x)? 0: x;
                break;
                case rotateZ:
                    float[] zValues = locatePoint(radius,lineY,lineZ,lineX);
                    alpha = alpha*angleToRadian + zValues[0];
                    theta = theta*angleToRadian + zValues[1];
                    x = zValues[2]*Mathf.Sin(alpha);
                    z = zValues[2]*Mathf.Cos(alpha);
                    y = radius*Mathf.Sin(theta);
                    rotatedVec.x = float.IsNaN(x)? 0: x;
                    rotatedVec.z = float.IsNaN(z)? 0: z;
                    rotatedVec.y = float.IsNaN(y)? 0: y;
                break;
            }
            return rotatedVec;
    }
    public static void createOrDeleteObject(
        Vector3[] obj, bool create
        ){ 
        for (int i = 0; i < obj.Length; i++){
            Vector3 vector = setVectorInBoundry(round(obj[i]));
            createOrDelete(vecToVecInt(vector),create);
        }
    }
    public static Vector3[] rotateObject(
        float theta, float alpha, int rotationDirection, 
        Vector3 origin,Vector3[] obj
        ){
        createOrDeleteObject(obj, false);
        Vector3[] rotatedObj = new Vector3[obj.Length];
        for (int i = 0; i < rotatedObj.Length; i++){
            float[] direc = vectorDirections(origin,obj[i]);
            rotatedObj[i] = 
                origin + rotate(
                            theta,alpha,vectorRadius(direc),
                            direc,rotationDirection
                            );
            }
        createOrDeleteObject(rotatedObj, true);
        return rotatedObj;
    }
    public static Vector3[] moveObject(
        Vector3 move, Vector3[] obj
        ){
        createOrDeleteObject(obj, false);
        Vector3[] movedObj = new Vector3[obj.Length];
        for (int i = 0; i < movedObj.Length; i++){
            movedObj[i] = obj[i]+move; 
            }
        createOrDeleteObject(movedObj, true);
        return movedObj;
    }

    void worldBuilderControls(){
        if (Input.GetKeyDown("w")){
            front = (front < dimensionZ) ? 
                front +=1: front = 0;
        }
        if (Input.GetKeyDown("s")){
            front = (front > 0) ? 
                front -=1: front = dimensionZ;
        }

        if (Input.GetKeyDown("d")){
            right =(right < dimensionX) ? 
                right+=1: right = 0;
        }
        if (Input.GetKeyDown("a")){
            right = (right > 0) ? 
                right-=1: right = dimensionX;
        }

        if (Input.GetKeyDown("e")){
            up = (up < dimensionY) ? 
                up+=1: up = 0;
        }
        if (Input.GetKeyDown("q")){
            up = (up > 0) ? 
                up-=1: up = dimensionY;
        }

        if (Input.GetKeyDown("p")){
            binaryWriter();
        }
        if (Input.GetKeyDown("space")){
            createOrDelete(
                new Vector3Int(right,up,front), true
                );
        }

        if (Input.GetKey("return")){
            createOrDelete(
                new Vector3Int(right,up,front), false
                );
        }
    }
    void Update()
    {
       if(disablePlayerControls) worldBuilderControls();
       if (Input.GetKeyDown("[")){
            disablePlayerControls = true;
        }
       if (Input.GetKeyDown("]")){
            disablePlayerControls = false;
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
