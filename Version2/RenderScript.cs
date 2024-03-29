using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RenderScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera cam;
    public GameObject player;
    public GameObject verticesPoint;
    public GameObject tempMesh;
    public GameObject savedMesh;
    public TMP_Text text;
    List<saveFormat> objectList = new List<saveFormat>();
    GameObject[] tempVerticesPoints = new GameObject[0];
    Vector3[] tempVertices = new Vector3[0];
    int[] triangles = new int[0];
    Color32[] tempColors = new Color32[0];
    Color32[] objectRGBA = new Color32[]{
        new Color32(25,25,25,0),
        new Color32(25,25,25,0),
        new Color32(25,25,25,0),
        new Color32(25,25,25,0),
        new Color32(25,25,25,0),
        new Color32(25,25,25,0),
        new Color32(25,25,25,0),
        new Color32(25,25,25,0),
    };
    Ray ray;  
    RaycastHit hit; 
    GameObject point; 
    float touchCount = 0;
    bool active  = true;

    string[] optionArray = new string[]{
         "create","save","delete","selectPoint","movePoint",
         "rotate","color",
    };
    public static string option = "lol";
    string oldOption = ""; 
    public static bool mobility = true;
    public static bool moveInCreate = false;
    bool timerBool;
    float duration = 0.5f;
    int amountOfClicks = 0;
    float click = -1;
    Vector3 oldCamPos;

    void Start()
    {
        oldCamPos = cam.transform.position;   
    }

    void Update(){
        touchCount = (Input.touchCount>0) ? Input.touchCount:0;
        bool screenContact = 
        touchCount > 0 && 
        Input.GetTouch(0).phase == TouchPhase.Began;

            if(screenContact){
                ray = 
                Camera.main.ScreenPointToRay(
                    Input.GetTouch(0).position
                    );
                active = true;
                timerBool = true;
                click +=1;
            } 
        if (option!="create"&& moveInCreate) moveInCreate = false;
        if (timerBool) clickTracker();
        if (!mobility && touchCount==1) chooseOption();
        if (!mobility) activeOption(option);
        if (mobility&& option!= "") option = "";
        if (oldOption != option) text.text = option;
        oldCamPos = cam.transform.position;
        oldOption = option;
    }



    void clickTracker(){
        if (click == amountOfClicks){
            amountOfClicks+=1;
            duration = (!mobility) ? 0.3f:0.2f;
        }
        duration = (duration>0)? 
            duration - Time.deltaTime : clickReset();
    }

    float clickReset(){
        if (amountOfClicks == 2) {
            point = null;
            mobility = !mobility;
            } else if (
                option == "create" && 
                    !mobility && 
                        amountOfClicks == 1 && touchCount<1) {
            moveInCreate = !moveInCreate;
            }
        amountOfClicks=0;
        click = -1;
        timerBool = false;
        return duration = 0.5f;
    }
    void chooseOption(){
        float xMove = (Movement.touchSingle.position.x -  Movement.singleOriginX)/100;
        float yMove = (Movement.touchSingle.position.y - Movement.singleOriginY)/100;
        int xSign = 0;
        int ySign = 0;
        if (xMove > 0.2f) xSign = 1;
           else if (xMove < -0.2f) xSign = -1;

        if (yMove > 0.2f) ySign = 1;
           else if (yMove < -0.2f) ySign = -1;

           if (xSign == 1 && ySign == 0) option = optionArray[0];
           if (xSign == -1 && ySign == 0) option = optionArray[2];

           if (xSign == 0 && ySign == 1) option = optionArray[1];
           if (xSign == 0 && ySign == -1) option = optionArray[6];
           
           if (xSign == 1 && ySign == -1) option = optionArray[3];
           if (xSign == -1 && ySign == 1) option = optionArray[5];
           
           if (xSign == 1 && ySign == 1) option = optionArray[4];
           if (xSign == -1 && ySign == -1) option = "-1-1";

           print(option);
    }

    void activeOption(string option){
        switch (option){
            case "create":
            create(active);
            break;

            case "save":
            save();
            break;

            case "delete":
            delete();
            break;

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
    struct saveFormat{
        public Vector3[] vertices;
        public int[] triangles;
        public Color32[] color;

        public saveFormat(Vector3[] vertices,int[] triangles,Color32[] color){
            this.vertices = vertices;
            this.triangles = triangles;
            this.color = color;
        }

    }

    void save(){
        if (touchCount<1){
            saveFormat saveTemp = 
                new saveFormat(tempVertices,triangles,tempColors);
            objectList.Add(saveTemp);
            List<Vector3> savedVertices = new List<Vector3>();
            List<int> savedTriangles = new List<int>();
            List<Color32> savedColors = new List<Color32>();
            for (int i = 0; i < objectList.Count;i++){
                Vector3[] temp1 = objectList[i].vertices;
                int[] temp2 = objectList[i].triangles;
                Color32[] temp3 = objectList[i].color;
                foreach(Vector3 vec in temp1) savedVertices.Add(vec);
                foreach(int trig in temp2) savedTriangles.Add(trig);
                foreach(Color32 col in temp3) savedColors.Add(col);
            }
        renderTriangles(
            savedMesh.GetComponent<MeshFilter>().mesh,
            savedVertices.ToArray(),
            savedTriangles.ToArray(),
            savedColors.ToArray()
            );
        
        tempVertices = new Vector3[0];
        foreach (GameObject i in tempVerticesPoints){
            Destroy(i);
        }
        tempVerticesPoints = new GameObject[0];
        triangles = new int[0];
        tempColors = objectRGBA;

        tempMesh.GetComponent<MeshFilter>().mesh.Clear();
        option = "";
        }
    }
    void delete(){
        if (touchCount<1 && objectList.Count != 0){
            objectList.RemoveAt(objectList.Count-1);
            List<Vector3> savedVertices = new List<Vector3>();
            List<int> savedTriangles = new List<int>();
            List<Color32> savedColors = new List<Color32>();
            for (int i = 0; i < objectList.Count;i++){
                    Vector3[] temp1 = objectList[i].vertices;
                    int[] temp2 = objectList[i].triangles;
                    Color32[] temp3 = objectList[i].color;
                    foreach(Vector3 vec in temp1) savedVertices.Add(vec);
                    foreach(int trig in temp2) savedTriangles.Add(trig);
                    foreach(Color32 col in temp3) savedColors.Add(col);
                }
            if (objectList.Count == 0) {
                savedMesh.GetComponent<MeshFilter>().mesh.Clear();
            } else
            renderTriangles(
                savedMesh.GetComponent<MeshFilter>().mesh,
                savedVertices.ToArray(),
                savedTriangles.ToArray(),
                savedColors.ToArray()
                );

            option = "";
        }
    }
    
    void create(bool active){
            
        float xMove = Movement.moveX/60;
        float yMove = Movement.moveY/60;
        float zMove = Movement.moveZ/60;
        float side = Movement.side/60;
        Vector3 direction = cam.transform.TransformDirection(new Vector3(xMove,yMove,zMove));
        Vector3 front = Camera.main.transform.position + Camera.main.transform.forward*5;
        tempVertices = createVertices(
            side,direction.x,direction.y,direction.z,
            front.x,front.y,front.z
            );
        tempColors = objectRGBA;
        renderVertices();
        renderTriangles(tempMesh.GetComponent<MeshFilter>().mesh,tempVertices,triangles,tempColors);

    }

    void rotateObject(){
        if (touchCount >1) { 
        Vector3 pos = new Vector3(
            (tempVerticesPoints[7].transform.position.x + tempVerticesPoints[0].transform.position.x)/2,
            (tempVerticesPoints[7].transform.position.y + tempVerticesPoints[0].transform.position.y)/2,
            (tempVerticesPoints[7].transform.position.z + tempVerticesPoints[0].transform.position.z)/2
            );
            float yMove = Movement.moveY;
            float xMove = -Movement.moveX;
            float zMove = -Movement.side;
            float side = Movement.moveZ;
            for (int i = 0; i<8; i++){
                tempVerticesPoints[i].transform.RotateAround(pos, cam.transform.TransformDirection(new Vector3(
                    yMove+side,xMove,zMove)), 4f
                );
                tempVertices[i] = tempVerticesPoints[i].transform.position;
            }
            renderTriangles(tempMesh.GetComponent<MeshFilter>().mesh,tempVertices,triangles,tempColors);
        }
    }

    void moveSelected(){
        if (Physics.Raycast(ray, out hit,Mathf.Infinity)){
            if (hit.collider.tag == "verticesPoint"){
            point = hit.collider.gameObject;
            }
            if (touchCount >1){
                float xMove =  Movement.moveX/150;
                float yMove =  Movement.moveY/150;
                float zMove =  Movement.moveZ/250;
                float side =  Movement.side/250;
                    for (int i = 0; i<8; i++){
                        if (tempVerticesPoints[i] == point) { 
                            tempVertices[i] += cam.transform.TransformDirection(new Vector3(
                            xMove+side,yMove,zMove
                            ));
                            tempVerticesPoints[i].transform.position = tempVertices[i];
                            renderTriangles(tempMesh.GetComponent<MeshFilter>().mesh,tempVertices,triangles,tempColors);
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

                foreach (GameObject i in tempVerticesPoints){
                    if (i.GetComponent<Renderer>().material.color == Color.blue){
                        validConnect +=$"{count}";

                    }
                    count++;
                }

                chooseRule(cubeConnect,validConnect);

                renderTriangles(tempMesh.GetComponent<MeshFilter>().mesh,tempVertices,triangles,tempColors);
            }
        }
    }

    void color(){
        if (Physics.Raycast(ray, out hit,Mathf.Infinity)){

            if (hit.collider.tag == "verticesPoint"){
                point = hit.collider.gameObject;
            }
            if (touchCount >1){

                float xMove = (Movement.touchRight.position.x -  Movement.rightOriginX)/100;
                float yMove = (Movement.touchRight.position.y - Movement.rightOriginY)/100;
                float zMove = (Movement.touchLeft.position.y - Movement.leftOriginY)/100;
                float side = (Movement.touchLeft.position.x - Movement.leftOriginX)/100;

                xMove = (xMove<0)? Mathf.Abs(xMove):0;
                for (int i = 0; i<8; i++){
                    if (tempVerticesPoints[i] == point) { 
                        objectRGBA[i] = new Color(
                        yMove,xMove,zMove,side
                        );
                        tempColors[i] =  Color32.Lerp(objectRGBA[i], objectRGBA[i] , tempVertices[i].y);
                        renderTriangles(tempMesh.GetComponent<MeshFilter>().mesh,tempVertices,triangles,tempColors);
                    }
                }
            }
        }
    }

    void chooseRule(int[] cubeConnect, string validConnect){
            switch (validConnect.Length){
            case 8:
            if (triangles.Length != 36) triangles = new int[36];
            applyRule(cubeConnect,LoadSides.sides8[validConnect]);
            break;
            case 7:
            if (triangles.Length != 30) triangles = new int[30];
            applyRule(cubeConnect,LoadSides.sides7[validConnect]);
            break;
            case 6:
            if (triangles.Length != 24) triangles = new int[24];
            applyRule(cubeConnect,LoadSides.sides6[validConnect]);
            break;
            case 5:
            if (triangles.Length != 18) triangles = new int[18];
            applyRule(cubeConnect,LoadSides.sides5[validConnect]);
            break;
            case 4:
            if (triangles.Length != 12) triangles = new int[12];
            applyRule(cubeConnect,LoadSides.sides4[validConnect]);
            break;
            case 3:
            if (triangles.Length != 6) triangles = new int[6];
            applyRule(cubeConnect,LoadSides.sides3[validConnect]);
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

    Vector3[] createVertices(
        float size, float x, float y, float z,
        float cx, float cy, float cz
        ){
        
        Vector3 s1Current,s2Current,s3Current,s4Current,
                s5Current,s6Current,s7Current,s8Current
                ;
        
        if (tempVerticesPoints.Length != 0){
            if (moveInCreate && touchCount >1){
                for (int i = 0; i<8; i++){
                    tempVerticesPoints[i].transform.RotateAround(
                        oldCamPos, cam.transform.up, Movement.rightTouchX/80
                    );
                    tempVerticesPoints[i].transform.RotateAround(
                        oldCamPos, cam.transform.right, -Movement.rightTouchY/80
                    );
                }
            }
            s1Current = tempVerticesPoints[0].transform.position;
            s2Current = tempVerticesPoints[1].transform.position;
            s3Current = tempVerticesPoints[2].transform.position;
            s4Current = tempVerticesPoints[3].transform.position;
            s5Current = tempVerticesPoints[4].transform.position;
            s6Current = tempVerticesPoints[5].transform.position;
            s7Current = tempVerticesPoints[6].transform.position;
            s8Current = tempVerticesPoints[7].transform.position;
        } else {
            s1Current = new Vector3(cx,cy,cz);
            s2Current = new Vector3(size+cx,cy,cz);
            s3Current = new Vector3(cx,size+cy,cz);
            s4Current = new Vector3(size+cx,size+cy,cz);
            s5Current = new Vector3(cx,cy,size+cz);
            s6Current = new Vector3(size+cx,cy,size+cz);
            s7Current = new Vector3(cx,size+cy,size+cz);
            s8Current = new Vector3(size+cx,size+cy,size+cz);
        }

        Vector3 s1,s2,s3,s4,
                s5,s6,s7,s8
                ;
                
        if (!moveInCreate){
            s1 = s1Current + new Vector3(x,y,z);
            s2 = s2Current + new Vector3(size+x,y,z);
            s3 = s3Current + new Vector3(x,size+y,z);
            s4 = s4Current + new Vector3(size+x,size+y,z);    
            s5 = s5Current + new Vector3(x,y,size+z);
            s6 = s6Current + new Vector3(size+x,y,size+z);
            s7 = s7Current + new Vector3(x,size+y,size+z);
            s8 = s8Current + new Vector3(size+x,size+y,size+z);
        } else {
            Vector3 move = cam.transform.position - oldCamPos;
            s1 = s1Current + move;
            s2 = s2Current + move;
            s3 = s3Current + move;
            s4 = s4Current + move;
            s5 = s5Current + move;
            s6 = s6Current + move;
            s7 = s7Current + move;
            s8 = s8Current + move;
        }

        Vector3[] vertices = 
            new Vector3[]{
                s1,s2,s3,s4,s5,s6,s7,s8
        };


        return vertices;
    }

    void renderVertices(){
        if (tempVerticesPoints.Length == 0){
            tempVerticesPoints = new GameObject[8];
            for(int i = 0; i<tempVertices.Length;i++){
                GameObject clone = Instantiate(verticesPoint.gameObject);
                tempVerticesPoints[i] = clone;
                clone.transform.position = tempVertices[i];
            }
        } else {
             for(int i = 0; i<tempVertices.Length;i++){
                tempVerticesPoints[i].transform.position = tempVertices[i];
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
