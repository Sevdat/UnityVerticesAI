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
    int[] dimensions = new int[]{
        (int)Math.Pow(8,0f),//1x1x1
        (int)Math.Pow(8,1f),//2x2x2
        (int)Math.Pow(8,2f),//4x4x4
        (int)Math.Pow(8,3f),//8x8x8
        (int)Math.Pow(8,4f),//16x16x16
        (int)Math.Pow(8,5f),//32x32x32
        (int)Math.Pow(8,6f) //64x64x64
        };
    void Awake()
    {
        ballMesh = ball.GetComponent<MeshFilter>().mesh;
        begin(false);
        createBalls();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void begin(bool readAtBegin){
        if (readAtBegin) {
            binaryReader();
            } else {
                arraySize = dimensions[5];
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
            while (reader.Read() != -1) {
                arrayWidth++;
            }
            arraySize = arrayWidth*8;
            bitArray = new BitArray(arraySize);
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
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
            byte value = 0;
            byte bit = 128; //0x80
            for (int i = 0; i < arraySize; i++){
                if (bitArray[i]) value += bit;
                bit /= 2;
                if (bit == 0) {
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
        GameObject cloneHierarchy = new GameObject(){
            name = "cloneHierarchy"
        };
        for (int i = 0; i<arraySize; i++){
            GameObject clone = Instantiate(
                ball, cloneHierarchy.transform
                );
            if (!bitArray[i])
                clone.GetComponent<MeshFilter>().mesh.Clear();
            clone.name = $"{i}";
            Vector3 vec = new Vector3(x,y,z);
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
