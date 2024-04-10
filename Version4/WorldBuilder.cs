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
    public static Vector3Int dimension = new Vector3Int(50,50,50);
    public static int dimensionX;
    public static int dimensionY;
    public static int dimensionZ;
    public static int cubeXZ;
    public static int cubeX;
    public static bool disablePlayerControls = false;
    int right = 0;
    int front = 0;
    int up = 0;
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
    public static Vector4 quatMul(Vector4 q1, Vector4 q2)
    {
        float w = q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z;
        float x = q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y;
        float y = q1.w * q2.y - q1.x * q2.z + q1.y * q2.w + q1.z * q2.x;
        float z = q1.w * q2.z + q1.x * q2.y - q1.y * q2.x + q1.z * q2.w;
        return new Vector4(x, y, z, w);
    }
    public static Vector3 rotate(
        float angle,Vector3 origin, Vector3 point,Vector3 rotationAxis
        ){
        Vector3 rotatedVec = origin;
        if (point != origin){
            Vector3 pointDirection = vectorDirections(origin,point);
            Vector3 perpendicular = normalizeVector3(
                    crossVector(rotationAxis, pointDirection)
                    );      

            float halfAngle = angle * 0.5f * (Mathf.PI/180.0f);
            float sinHalfAngle = Mathf.Sin(halfAngle);
            float w = Mathf.Cos(halfAngle);
            float x = perpendicular.x * sinHalfAngle;
            float y = perpendicular.y * sinHalfAngle;
            float z = perpendicular.z * sinHalfAngle;

            Vector4 quat = new Vector4(x,y,z,w);
            Vector4 axisQuat = new Vector4(pointDirection.x, pointDirection.y, pointDirection.z,0);
            Vector4 inverseQuat = new Vector4(-x,-y,-z,w);
            Vector4 rotatedQuaternion = 
                    quatMul(quatMul(quat,axisQuat), inverseQuat);
        
            rotatedVec = origin + new Vector3(
                    rotatedQuaternion.x,
                    rotatedQuaternion.y,
                    rotatedQuaternion.z
                    );
        }
        return rotatedVec;
    }
    public static float[] locatePoint(
        float radius,
        float constantOpposite, float axisAdjacent,
        float axisOpposite
        ){
        float currentTheta = constantOpposite/radius;
        float checkSin = Mathf.Asin(currentTheta);
        currentTheta = float.IsNaN(checkSin)? 
            Mathf.Asin(radius/constantOpposite) : checkSin;

        float adjacent = radius*Mathf.Cos(currentTheta);

        float currentAlpha = axisAdjacent/adjacent;
        float checkCos = Mathf.Acos(currentAlpha);
        currentAlpha = float.IsNaN(checkCos)? 
            Mathf.Acos(adjacent/axisAdjacent) : checkCos;

        float rotationSide = Mathf.Sign(axisOpposite);
        return new float[]{
            rotationSide*currentAlpha,
            adjacent
            };
    }
    public static Vector3 getAngles(Vector3 origin, Vector3 point){
        Vector3 alphaRotations = new Vector3(0,0,0);
        if (origin != point){
        Vector3 dir = vectorDirections(origin,point);
        float radius = vectorRadius(dir);

        float[] xValues = locatePoint(radius,dir.z,dir.y,dir.x);
        alphaRotations.x = xValues[0]*radianToAngle;

        float[] yValues = locatePoint(radius,dir.x,dir.y,dir.z);
        alphaRotations.y = yValues[0]*radianToAngle;

        float[] zValues = locatePoint(radius,dir.y,dir.z,dir.x);
        alphaRotations.z = zValues[0]*radianToAngle;
        }
        return alphaRotations;
    }
    public static Vector3[] rotateObject(
        float alpha, Vector3 origin,Vector3[] point,Vector3 rotationAxis
        ){
        createOrDeleteObject(point, false);
        int size = point.Length;
        Vector3[] rotatedVec = new Vector3[size];
        for (int i = 0; i < size; i++){
            Vector3 vec = rotate(alpha,origin,point[i],rotationAxis);
            rotatedVec[i] = vec;
            }
        return rotatedVec;
    }
    public static Vector3[] moveObject(
        Vector3 move, Vector3[] obj
        ){
        createOrDeleteObject(obj, false);
        Vector3[] movedObj = new Vector3[obj.Length];
        for (int i = 0; i < movedObj.Length; i++){
            movedObj[i] = obj[i]+move; 
            }
        return movedObj;
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