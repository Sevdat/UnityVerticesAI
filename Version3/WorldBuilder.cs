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
    void Awake()
    {
        ballMesh = ball.GetComponent<MeshFilter>().mesh;
        using (StreamReader reader = new StreamReader("Assets/v3/binaryWorld.txt"))
        {
            int check = 0;
            int zero = 48;
            int one = 49;
            while ((check = reader.Read()) != -1) {
                if (check == zero || check == one)
                    ballLocations.Add(check);
            }
            int arraySize = ballLocations.Count;
            int arrayWidth = (int)Math.Cbrt(arraySize);
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

    int index = 0;
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ballLocations[index] = 48;
            print(ballLocations[index+1]);
            index +=1;
        }
    }
}
