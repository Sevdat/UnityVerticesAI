using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Text;

public class WorldBuilder : MonoBehaviour
{
    public GameObject ball;
    public static Mesh ballMesh;
    public TextAsset textDoc;
    public static Vector3[] worldDimensions;
    int arraySize;
    int arrayWidth = 100;
    public static BitArray bitArray;
    void Awake()
    {
        ballMesh = ball.GetComponent<MeshFilter>().mesh;
        begin(false);
        createBalls();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void begin(bool readAtBegin){
        if (readAtBegin) binaryReader(); 
            else {
                arraySize = (int)Math.Pow(arrayWidth,3f);
                bitArray = new BitArray(arraySize);
                for (int i=0; i<arraySize;i++){
                    bitArray[i] = false;
                }
                binaryWriter();
            }
    }
    void binaryReader(){
        using (StreamReader reader = new StreamReader("Assets/v3/binaryWorld.txt"))
        {
            int check = 0;
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
            byte value = 0;
            byte bit = 128; //0x80
            for (int i = 0; i < arraySize; i++){
                if (bitArray[i]) value += bit;
                bit /= 2;
                if (bit == 0) {
                writer.Write((char)value);
                print((char)value);
                value = 0; bit = 128;
                }
            }
        }
    }

    void createBalls(){
        arraySize = bitArray.Count;
        arrayWidth = (int)Math.Cbrt(arraySize);
        worldDimensions = new Vector3[arraySize];
        float x = 0;
        float y = 0;
        float z = 0;
        for (int i = 0; i<arraySize; i++){
            GameObject clone = new GameObject{
                name = $"{i}" 
            };
            Vector3 vec = new Vector3(x,y,z);
            worldDimensions[i] = vec;
            clone.transform.position = vec;
            x+=1;
            if (z > arrayWidth-2 && x > arrayWidth-1) {x = 0; z = 0; y += 1;}
            if (x > arrayWidth-1) {x = 0; z+=1;}; 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("up"))
        {
            
        }
        if (Input.GetKeyDown("down"))
        {
            
        }
        if (Input.GetKeyDown("right"))
        {
            
        }
        if (Input.GetKeyDown("left"))
        {
            
        }
        if (Input.GetKeyDown("p"))
        {
            binaryWriter();
        }
    }
}
