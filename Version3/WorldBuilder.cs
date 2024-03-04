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
    GameObject cloneHierarchy;
    public GameObject ball;
    public static Mesh ballMesh;
    public static BitArray bitArray;
    int arraySize;
    int arrayWidth;
    Vector3 dimension = new Vector3(5f,4f,3f);
    float dimensionX;
    float dimensionY;
    float dimensionZ;
    float cubeXZ;
    float cubeX;
    public static bool disablePlayerControls = false;
    int right = 0;
    int front = 0;
    int up = 0;
    void Awake()
    {
        ballMesh = ball.GetComponent<MeshFilter>().mesh;
        cubeXZ = dimension.x*dimension.z;
        cubeX = dimension.x;
        dimensionX = dimension.x-1;
        dimensionY = dimension.y-1;
        dimensionZ = dimension.z-1;
        rewriteFile(false);
        createBalls();
        print(cloneHierarchy.transform.GetChild(1).name);
        Cursor.lockState = CursorLockMode.Locked;
    }
    void rewriteFile(bool rewriteAtBegin){
        if (rewriteAtBegin) {
            arraySize = (int)(dimension.x*dimension.y*dimension.z);
            bitArray = new BitArray(arraySize);
            for (int i=0; i<arraySize;i++) {
                bitArray[i] = false;
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
    void binaryWriter(){
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
            name = "cloneHierarchy"
        };
        for (int i = 0; i<arraySize; i++){
            if (bitArray[i]){
            GameObject clone = Instantiate(
                ball, cloneHierarchy.transform
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
    void randomBallManipulator(int ballNumber, Vector3 vec, bool bitArrayBool){
        if (!bitArray[ballNumber] && bitArrayBool){
                bitArray[ballNumber] = true;
                GameObject clone = Instantiate(
                    ball, cloneHierarchy.transform
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
    void worldBuilderControls(){
        if (Input.GetKeyDown("w")){
            front = (front < dimensionZ) ? 
                front +=1: front = 0;
        }
        if (Input.GetKeyDown("s")){
            front = (front > 0) ? 
                front -=1: front = (int)dimensionZ;
        }

        if (Input.GetKeyDown("d")){
            right =(right < dimensionX) ? 
                right+=1: right = 0;
        }
        if (Input.GetKeyDown("a")){
            right = (right > 0) ? 
                right-=1: right = (int)dimensionX;
        }

        if (Input.GetKeyDown("e")){
            up = (up < dimensionY) ? 
                up+=1: up = 0;
        }
        if (Input.GetKeyDown("q")){
            up = (up > 0) ? 
                up-=1: up = (int)dimensionY;
        }

        if (Input.GetKeyDown("p")){
            binaryWriter();
        }
        if (Input.GetKeyDown("space")){
            randomBallManipulator(
                (int)(right + front*cubeX + up*cubeXZ)
                ,new Vector3(right,up,front), true
                );
        }

        if (Input.GetKey("return")){
            randomBallManipulator(
                (int)(right + front*cubeX + up*cubeXZ)
                ,new Vector3(right,up,front), false
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
