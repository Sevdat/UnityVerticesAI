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
    bool active  = true;

    string[] optionArray = new string[]{
        "select","move","rotate","color",
    };
    public static string option = "move";
    public static bool mobility = true;
    bool timerBool;
    float oldX = 0;
    float oldY = 0;
    float oldZ = 0;
    void Update(){

        bool screenContact = 
        Input.touchCount > 0 && 
        Input.GetTouch(0).phase == TouchPhase.Began;

            if(screenContact){
                click +=1;
                timerBool = true;
                ray = 
                Camera.main.ScreenPointToRay(
                    Input.GetTouch(0).position
                    );
                    active = true;
                if (Input.touchCount>1){
                    oneTouch = false;
                } else oneTouch = true;

            } 

        if (timerBool) clickTracker();

        if (!mobility) chooseOption(option);
        
        oldX = Movement.moveX;
        oldY = Movement.moveY;
        oldZ = Movement.moveZ;

    }
    float duration = 0.5f;
    int amountOfClicks = 0;
    float click = -1;
    bool pickOption = false;
    void clickTracker(){
        if (click == amountOfClicks){
            amountOfClicks+=1;
            duration = 0.2f;
        }
        duration = (duration>0)? 
            duration - Time.deltaTime : clickReset();
    }

    float clickReset(){
        if (amountOfClicks == 2) {
            point = new GameObject();
            mobility = !mobility;
            }
        amountOfClicks=0;
        click = -1;
        timerBool = false;
        return duration = 0.5f;
    }

    bool oneTouch = false;
    void chooseOption(string option){
        switch (option){
            case "select":
            select(active,ray);
            active = false;
            break;
            
            case "move":
            moveSelected(oldX,oldY,oldZ);
            break;
            
            case "rotate":
            rotateObject(oldX,oldY);
            break;
            
            case "color":
            break;

        }
    }

    void rotateObject(float x,float y){
        if (Movement.moveX != x || Movement.moveY != y){
        Vector3 pos = new Vector3(
            (verticesPoints[7].transform.position.x + verticesPoints[0].transform.position.x)/2,
            (verticesPoints[7].transform.position.y + verticesPoints[0].transform.position.y)/2,
            (verticesPoints[7].transform.position.z + verticesPoints[0].transform.position.z)/2
            );
            float yMove =  Movement.moveY-y;
            float xMove =  x - Movement.moveX;
            float zMove = 0;
            for (int i = 0; i<8; i++){
                verticesPoints[i].transform.RotateAround(pos, new Vector3(
                    yMove,xMove,zMove), 2
                );
                tempVertices[i] = verticesPoints[i].transform.position;
            }
            renderTriangles(meshOfObject.GetComponent<MeshFilter>().mesh,tempVertices,triangles);
        }
    }

    GameObject point;
    void moveSelected(float x, float y, float z){

        bool touchAmount = Input.touchCount!=0;
        if (Physics.Raycast(ray, out hit,Mathf.Infinity)){
            if (hit.collider.tag == "verticesPoint"){
            point = hit.collider.gameObject;
            }
                if (touchAmount && (Movement.moveX != x || Movement.moveY != y|| Movement.moveZ != z)){
                    float yMove = (Movement.moveY-y)*50;
                    float xMove = (Movement.moveX-x)*5;
                    float zMove = (Movement.moveZ-z)*20;
                        for (int i = 0; i<8; i++){ 
                            if (verticesPoints[i] == point) { 
                                if (Input.touchCount >1)
                                tempVertices[i] += new Vector3(
                                xMove,yMove,zMove
                                ); else tempVertices[i] += new Vector3(
                                xMove,yMove,0
                                );
                                verticesPoints[i].transform.position = tempVertices[i];
                                print(verticesPoints[i].transform.position);
                                renderTriangles(meshOfObject.GetComponent<MeshFilter>().mesh,tempVertices,triangles);
                        }
                    }
                }
        } 
    }


    void select(bool active, Ray ray){

        if (active && Physics.Raycast(ray, out hit,Mathf.Infinity)){
            if(hit.collider.tag == "verticesPoint"){
                GameObject point = hit.collider.gameObject;
                Material pointColor = point.GetComponent<Renderer>().material;

                pointColor.color = (pointColor.color == Color.white) ?
                pointColor.color = Color.blue:
                pointColor.color = Color.white;

                string validConnect = "";
                int count = 1;
                int[] cubeConnect = new int[]{
                    0, 1, 2, 3,
                    4, 5, 6, 7
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
        GameObject clone = Instantiate(verticesPoint);
        clone.transform.position = tempVertices[i];
        verticesPoints[i] = clone;
        }
    }

    Vector4[] objectRGBA = new Vector4[]{
        new Vector4(200,255,255,0),
        new Vector4(255,255,255,0),
        new Vector4(255,255,255,0),
        new Vector4(255,255,255,0),
        new Vector4(255,255,255,0),
        new Vector4(255,255,255,0),
        new Vector4(255,255,255,0),
        new Vector4(255,255,255,0),
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
