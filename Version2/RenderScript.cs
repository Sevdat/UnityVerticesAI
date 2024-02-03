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
    public GameObject player;
    public Camera cam;
    public GameObject verticesPoint;
    Vector3[] tempVertices = new Vector3[8];
    Color32[] colors = new Color32[8];
    int[] triangles;
    List<int[]> objectList = new List<int[]>();
    public GameObject meshOfObject;
    Ray ray;  
    RaycastHit hit;  
    float touchCount = 0;

    void Start()
    {

        tempVertices = createVertices(1.0f,0.0f,0.0f,0.0f);
        colors = objectRGBA;
        renderVertices();
        
    }

    GameObject[] verticesPoints = new GameObject[8];
    bool active  = true;

    string[] optionArray = new string[]{
        "selectPoint","movePoint","rotate","color"
    };
    public static string option = "select";
    public static bool mobility = true;
    bool timerBool;
    void Update(){
        touchCount = (Input.touchCount>0) ? Input.touchCount:0;
        bool screenContact = 
        touchCount > 0 && 
        Input.GetTouch(0).phase == TouchPhase.Began;

            if(screenContact){
                click +=1;
                timerBool = true;
                ray = 
                Camera.main.ScreenPointToRay(
                    Input.GetTouch(0).position
                    );
                    active = true;
            } 

        if (timerBool) clickTracker();
        if (!mobility && touchCount==1) chooseOption();
        if (!mobility) activeOption(option);

    }
    float duration = 0.5f;
    int amountOfClicks = 0;
    float click = -1;
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
    void chooseOption(){
        float xMove =  (Movement.touchSingle.position.x -  Movement.singleOriginX)/100;
        float yMove =  (Movement.touchSingle.position.y - Movement.singleOriginY)/100;
        int xSign = 0;
        int ySign = 0;
        if (xMove > 0.2f) xSign = 1;
           else if (xMove < -0.3f) xSign = -1;

        if (yMove > 0.2f) ySign = 1;
           else if (yMove < -0.2f) ySign = -1;

           if (xSign == 0 && ySign == 1) option = optionArray[0];
           if (xSign == 0 && ySign == -1) option = optionArray[3];

           if (xSign == 1 && ySign == 0) option = optionArray[1];
           if (xSign == -1 && ySign == 0) option = optionArray[2];
           
           if (xSign == 1 && ySign == -1) option ="1-1";
           if (xSign == -1 && ySign == 1) option = "-11";
           
           if (xSign == 1 && ySign == 1) option = "11";
           if (xSign == -1 && ySign == -1) option = "-1-1";

           print(option);
    }
    void activeOption(string option){
        switch (option){
            case "selectPoint":
            select(active,ray);
            active = false;
            break;
            
            case "movePoint":
            moveSelected();
            break;
            
            case "rotate":
            rotateObject();
            break;
            
            case "color":
            color();
            break;

        }
    }

    void rotateObject(){
        if (touchCount >1) { 
        Vector3 pos = new Vector3(
            (verticesPoints[7].transform.position.x + verticesPoints[0].transform.position.x)/2,
            (verticesPoints[7].transform.position.y + verticesPoints[0].transform.position.y)/2,
            (verticesPoints[7].transform.position.z + verticesPoints[0].transform.position.z)/2
            );
            float xMove = Movement.moveY;
            float yMove = -Movement.moveX;
            float zMove = -Movement.side;
            float side = Movement.moveZ;
            for (int i = 0; i<8; i++){
                verticesPoints[i].transform.RotateAround(pos, cam.transform.TransformDirection(new Vector3(
                    xMove+side,yMove,zMove)), 0.25f
                );
                tempVertices[i] = verticesPoints[i].transform.position;
            }
            renderTriangles(meshOfObject.GetComponent<MeshFilter>().mesh,tempVertices,triangles,colors);
        }
    }

    GameObject point;
    void moveSelected(){
        if (Physics.Raycast(ray, out hit,Mathf.Infinity)){
            if (hit.collider.tag == "verticesPoint"){
            point = hit.collider.gameObject;
            }
            if (touchCount >1){
                float xMove =  Movement.moveX/300;
                float yMove =  Movement.moveY/300;
                float zMove =  Movement.moveZ/400;
                float side =  Movement.side/400;
                    for (int i = 0; i<8; i++){
                        if (verticesPoints[i] == point) { 
                            tempVertices[i] += cam.transform.TransformDirection(new Vector3(
                            xMove+side,yMove,zMove
                            ));
                            verticesPoints[i].transform.position = tempVertices[i];
                            print(verticesPoints[i].transform.position);
                            renderTriangles(meshOfObject.GetComponent<MeshFilter>().mesh,tempVertices,triangles,colors);
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

                renderTriangles(meshOfObject.GetComponent<MeshFilter>().mesh,tempVertices,triangles,colors);
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

    Color32[] objectRGBA = new Color32[]{
        new Color(0.5f,0.5f,0.5f,1),
        new Color(0.5f,0.5f,0.5f,1),
        new Color(0.5f,0.5f,0.5f,1),
        new Color(0.5f,0.5f,0.5f,1),
        new Color(0.5f,0.5f,0.5f,1),
        new Color(0.5f,0.5f,0.5f,1),
        new Color(0.5f,0.5f,0.5f,1),
        new Color(0.5f,0.5f,0.5f,1),
    };

    void color(){
        if (Physics.Raycast(ray, out hit,Mathf.Infinity)){
        if (hit.collider.tag == "verticesPoint"){
        point = hit.collider.gameObject;
        }
        if (touchCount >1){
            print(touchCount);
            float xMove = (Movement.touchRight.position.x -  Movement.rightOriginX)/100;
            float yMove = (Movement.touchRight.position.y - Movement.rightOriginY)/100;
            float zMove = (Movement.touchLeft.position.y - Movement.leftOriginY)/100;
            float side = (Movement.touchLeft.position.x - Movement.leftOriginX)/100;

            xMove = (xMove<0)? Mathf.Abs(xMove):0;
                for (int i = 0; i<8; i++){
                    if (verticesPoints[i] == point) { 
                        objectRGBA[i] = new Color(
                        yMove,xMove,zMove,side
                        );
                        colors[i] =  Color32.Lerp(objectRGBA[i], objectRGBA[i] , tempVertices[i].y);
                        print(colors[i]);
                        renderTriangles(meshOfObject.GetComponent<MeshFilter>().mesh,tempVertices,triangles,colors);
                    }
                }
            }
        }
    }

    void renderTriangles(Mesh mesh, Vector3[] vertices, int[] triangles,Color32[] colors){
         mesh.Clear();
         mesh.vertices = vertices;
         mesh.triangles = triangles;
         mesh.colors32 = colors;

    }

}
