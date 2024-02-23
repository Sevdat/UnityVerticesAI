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
    public static List<int> ballLocations = new List<int>(); 
    public static Vector3[] worldDimensions;
    int arraySize;
    int arrayWidth;
    void Awake()
    {
        ballMesh = ball.GetComponent<MeshFilter>().mesh;
        binaryReader();
    }

    void binaryReader(){
        using (StreamReader reader = new StreamReader("Assets/v3/binaryWorld.txt"))
        {
            int check = 0;
            int zero = 48;
            int one = 49;
            while ((check = reader.Read()) != -1) {
                if (check == zero || check == one)
                    ballLocations.Add(check);
            }
            arraySize = ballLocations.Count;
            arrayWidth = (int)Math.Cbrt(arraySize);
            worldDimensions = new Vector3[arraySize];
            float x = 0;
            float y = 0;
            float z = 0;
            for (int i = 0; i<arraySize; i++){
                GameObject clone = Instantiate(ball);
                clone.name = $"{i}";
                worldDimensions[i] = new Vector3(x,y,z);
                x+=1;
                if (z > arrayWidth-2 && x > arrayWidth-1) {x = 0; z = 0; y += 1;}
                if (x > arrayWidth-1) {x = 0; z+=1;}; 
            }
        }
    }
    void binaryWriter(){
        using (StreamWriter writer = new StreamWriter("Assets/v3/binaryWorld.txt"))
        {
            string c = "";
            string p = "";
            int one = 49;
            int paragraph = 0;
            int newTile = 0;
            for (int i = 0; i < arraySize; i++){
                paragraph++;
                if (paragraph == arrayWidth) {
                    p = $"\n"; paragraph = 0; newTile += 1; 
                    } else p = "";
                if (newTile == arrayWidth) {
                    p = $"\n\n"; newTile = 0;
                        }
                if (ballLocations[i] == one) c = "1"; else c = "0";
                writer.Write($"{c}{p}");
            }
        }
    }

    int index = 0;
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ballLocations[index] = 48;
            print(ballLocations[index+1]);
            index +=1;
            if (index == 10) binaryWriter();
        }
    }
}
