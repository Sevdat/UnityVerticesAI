using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class RenderScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject verticesPoint;
    Vector3[] tempVertices;
    int[] triangles = new int[0];
    List<int[]> objectList = new List<int[]>();
    public GameObject meshOfObject;
    Ray ray;  
    RaycastHit hit;  

    void Start()
    {

        tempVertices = createVertices(1.0f,0.0f,0.0f,0.0f);
        renderVertices();
        
    }

    GameObject[] verticesPoints = new GameObject[8];
    float duration = 1.0f;
    string amountOfClicks = "";
    float click = -1;
    bool active  = true;
    void Update(){

        bool screenContact = 
        Input.touchCount > 0 && 
        Input.GetTouch(0).phase == TouchPhase.Began;

            if(screenContact){
                click += 1;
                
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                tempObject(ray);
        } 

        if (active) clickTracker(); else clickReset();
        active = true;
    }

    void clickTracker(){
        if (click == amountOfClicks.Length){
        amountOfClicks+="1";
        duration = 1.0f;
        print(amountOfClicks.Length);
        }
        if (duration>0) duration = duration - Time.deltaTime; 
        else {
            clickReset();
            };
    }

    void clickReset(){
        duration = 1.0f;
        amountOfClicks="";
        click = -1;
    }

    void tempObject(Ray ray){
        hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit,Mathf.Infinity)){
            if(hit.collider.tag == "verticesPoint"){
                active = false;
                clickReset();
                GameObject point = hit.collider.gameObject;
                Material pointColor = point.GetComponent<Renderer>().material;

                pointColor.color = (pointColor.color == Color.white) ?
                pointColor.color = Color.blue:
                pointColor.color = Color.white;

                string validConnect = "";
                int count = 1;
                int size = objectList.Count*8;
                int[] cubeConnect = new int[]{
                    0+size, 1+size, 2+size, 3+size,
                    4+size, 5+size, 6+size, 7+size
                    };
                foreach (GameObject i in verticesPoints){
                    if (i.GetComponent<Renderer>().material.color == Color.blue){
                        validConnect +=$"{count}";
                    }
                    count++;
                }

                chooseRule(cubeConnect,validConnect);

                renderTriangles(meshOfObject.GetComponent<MeshFilter>().mesh,tempVertices,triangles);

            }
        }
    }

                    //     tempTriangle.Add(triangles);
                    // print(triangles.Length);
    void chooseRule(int[] cubeConnect, string validConnect){
            switch (validConnect.Length){
            case 8:
            triangles = new int[36];
            applyRule(cubeConnect,LoadSides.sides8[validConnect]);
            break;
            case 7:
            triangles = new int[30];
            applyRule(cubeConnect,LoadSides.sides7[validConnect]);
            break;
            case 6:
            triangles = new int[24];
            applyRule(cubeConnect,LoadSides.sides6[validConnect]);
            break;
            case 5:
            triangles = new int[18];
            applyRule(cubeConnect,LoadSides.sides5[validConnect]);
            break;
            case 4:
            triangles = new int[12];
            applyRule(cubeConnect,LoadSides.sides4[validConnect]);
            break;
            case 3:
            triangles = new int[6];
            applyRule(cubeConnect,LoadSides.sides3[validConnect]);
            break;
            default:
            triangles = new int[0];
            break;
        }
    }

    void applyRule(int[] cubeConnect,string[] searchList){
        int count = 0;
        foreach(string i in searchList){
            int a = cubeConnect[(int)char.GetNumericValue(i[0])-1];
            int b = cubeConnect[(int)char.GetNumericValue(i[1])-1];
            int c = cubeConnect[(int)char.GetNumericValue(i[2])-1];
            int next = 3*count;
            triangles[0+next] += a;
            triangles[1+next] += b;
            triangles[2+next] += c;
            count++;  
        }
    }

    Vector3[] createVertices(float size, float x, float y, float z){

        Vector3 s1 = new Vector3(x,z,y);
        Vector3 s2 = new Vector3(size+x,z,y);
        Vector3 s3 = new Vector3(x,z,size+y);
        Vector3 s4 = new Vector3(size+x,z,size+y);
        
        Vector3 s5 = new Vector3(x,size+z,y);
        Vector3 s6 = new Vector3(size+x,size+z,y);
        Vector3 s7 = new Vector3(x,size+z,size+y);
        Vector3 s8 = new Vector3(size+x,size+z,size+y);

        Vector3[] vertices = 
        new Vector3[]{
            s1,s2,s3,s4,s5,s6,s7,s8
        };

        return vertices;
    }

    void renderVertices(){
        for(int i = 0; i<tempVertices.Length;i++){
        GameObject go = Instantiate(verticesPoint);
        go.transform.position = tempVertices[i];
        verticesPoints[i] = go;
        }
    }

    Vector4[] objectRGBA = new Vector4[]{
        new Vector4(255,255,255,0),
        new Vector4(255,255,255,0),
        new Vector4(255,255,255,0),
        new Vector4(255,255,255,0),
        new Vector4(0,0,0,0),
        new Vector4(0,0,0,0),
        new Vector4(0,0,0,0),
        new Vector4(0,0,0,0),
    };
    void renderTriangles(Mesh mesh, Vector3[] vertices, int[] triangles){
         mesh.Clear();
         mesh.vertices = vertices;
         mesh.triangles = triangles;
             
        Color[] colors = new Color[8];
        
        for (int i = 0; i < 8; i++){
            colors[i] = objectRGBA[i];
        }
            mesh.RecalculateNormals();
            mesh.colors = colors;

    }

}
