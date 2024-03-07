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
    public static Vector3Int dimension = new Vector3Int(10,10,10);
    public static int dimensionX;
    public static int dimensionY;
    public static int dimensionZ;
    public static int cubeXZ;
    public static int cubeX;
    public static bool disablePlayerControls = false;
    int right = 0;
    int front = 0;
    int up = 0;
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
            arraySize = (int)(dimension.x*dimension.y*dimension.z);
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
        float x = 0;
        float y = 0;
        float z = 0;
        cloneHierarchy = new GameObject(){
            name = "cloneHierarchy",
            isStatic = true
        };
        for (int i = 0; i<arraySize; i++){
            if (bitArray[i]){
            GameObject clone = Instantiate(
                staticClone, cloneHierarchy.transform
                );
            Vector3 vec = new Vector3(x,y,z);
            clone.name = $"{i}";
            clone.transform.position = vec;
            }
            x+=1;
            if (z >dimension.z-2 && x > dimension.x-1) {x = 0; z = 0; y += 1;}
            if (x > dimension.x-1) {x = 0; z+=1;}; 
        }
    } 
    public static void cloneCreator(int ballNumber, Vector3Int vec, bool bitArrayBool){
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
    public static int vecToInt(int right, int front, int up){
        return right + front*cubeX + up*cubeXZ;
    }
    public static int boundry(int direction,int add,int dimension){
        direction+=add;
        if (direction > dimension) 
                direction = Math.Abs(direction % (dimension+1));
            else if (direction < 0) {
                int size = dimension+1;
                direction = size-Math.Abs(direction % size);
            }
        return direction;
    }
    public static void move(
            Vector3Int intVector,
            int moveX,int moveY,int moveZ
            ){
            int x = boundry(intVector.x,moveX,dimensionX);
            int y = boundry(intVector.y,moveY,dimensionY);
            int z = boundry(intVector.z,moveZ,dimensionZ);
            int num = vecToInt(x,y,z);
            Vector3Int vec = new Vector3Int(x,y,z);
            cloneCreator(num,vec,true);
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
            cloneCreator(
                vecToInt(right, front, up)
                ,new Vector3Int(right,up,front), true
                );
        }

        if (Input.GetKey("return")){
            cloneCreator(
                vecToInt(right, front, up)
                ,new Vector3Int(right,up,front), false
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
