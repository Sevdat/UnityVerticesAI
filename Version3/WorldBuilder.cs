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
    public GameObject ball;
    public static Mesh ballMesh;
    public TextAsset textDoc;
    int arraySize;
    int arrayWidth;
    public static BitArray bitArray;
    Vector3 dimension = new Vector3(5f,9f,6f);
    GameObject cloneHierarchy;
    int index = 0;
    Transform ballCreator;
    void Awake()
    {
        ballMesh = ball.GetComponent<MeshFilter>().mesh;
        rewriteFile(true);
        createBalls();
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
                writer.Write((char)value);
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
            clone.name = $"{i}: ({x},{y},{z})";
            Vector3 vec = new Vector3(x,y,z);
            clone.transform.position = vec;
            }
            x+=1;
            if (z >dimension.z-2 && x > dimension.x-1) {x = 0; z = 0; y += 1;}
            if (x > dimension.x-1) {x = 0; z+=1;}; 
        }
        ballCreator = cloneHierarchy.transform.GetChild(0);
    } 
    void randomBallManipulator(int ballNumber){
        if (bitArray[ballNumber]){
            GameObject clone = Instantiate(
                ball, cloneHierarchy.transform
                );
            Vector3 vec = vectorFromInt(ballNumber,dimension);
            clone.name = $"{ballNumber}: {vec.x},{vec.y},{vec.z}";
            clone.transform.position = 
                vectorFromInt(ballNumber,dimension);
            }
        ballCreator = cloneHierarchy.transform.GetChild(0);
    } 

    Vector3 vectorFromInt(int location, Vector3 dimension){
        // right + (front)*(z) + (up)*(x*y) = int
        float dimensionXY = dimension.x*dimension.z;

        float[] up = amountOfFullDevision(
            location, dimensionXY
            );
        float[] front = amountOfFullDevision(
            up[0], dimension.z
            );

        return new Vector3(front[0],up[1],front[1]);
    }
    float[] amountOfFullDevision(float copyLocation,float dimension){
        for (int i =0;true;i++){
            if (copyLocation<0) {
                if (copyLocation<0) copyLocation += dimension;
                return new float[]{copyLocation,i-1};
            }
            copyLocation -= dimension;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("up")){
            ballCreator.GetComponent<MeshFilter>().mesh.Clear();
            ballCreator = cloneHierarchy.transform.GetChild(index);
            ballCreator.GetComponent<MeshFilter>().mesh = ballMesh;
            index +=1;
        }
        if (Input.GetKeyDown("down")){
            
        }
        if (Input.GetKeyDown("right")){
            
        }
        if (Input.GetKeyDown("left")){
            
        }
        if (Input.GetKeyDown("p")){
            binaryWriter();
        }
    }
}
