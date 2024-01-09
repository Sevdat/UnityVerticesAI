using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class ball : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject points;
    Vector3[] vertices;
    int[] triangles = new int[0];
    Mesh mesh;
    string sortedWorld;
    int xAmount;
    int xSquare;
    int zero = (int)'0';
    int minOne = -1;
    string bottom = "1342";
    string top = "5786";
    List<string[]> allCorners = new List<string[]>();
    

//Vector(x,y,z)  x = side, y = up/down, z = forward/backward 
//new Vector3(0,0,0), new Vector3(0,0,1), new Vector3(1,0,0), new Vector3(1,0,1)
//int[] triangles = new int[]{0,1,2};
    void Start()
    {

        loadFile(Resources.Load<TextAsset>("binaryWorld").text);
        mesh = new Mesh();
        xAmount = (int) Mathf.Pow(sortedWorld.Length,1f/3f);
        xSquare = (int)Mathf.Pow(xAmount,2f);
        vertices = createVertices(xAmount,10);

        renderVertices();
        cubeCorners(bottom,top);
        loadTriangles();
             
        renderTriangles(mesh,vertices,triangles);
        print($"Vertices: {vertices.Length}");
        print($"xMax: {vertices[xAmount-1].x}");
        printList(vertices);
        
         
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void renderTriangles(Mesh mesh, Vector3[] vertices, int[] triangles){
         mesh.Clear();
         mesh.vertices = vertices;
         mesh.triangles = triangles;
         mesh.RecalculateNormals();

    }

    void loadTriangles(){
    
    for(int i = 0; i<sortedWorld.Length-2*xAmount-xSquare;){

        int one1 = (sortedWorld[i]!=zero) ? i:minOne;
        int two1 = (sortedWorld[i+1]!=zero) ? i+1:minOne;
        int two2 = (sortedWorld[i+xAmount]!=zero) ? i+xAmount:minOne;
        int one2 = (sortedWorld[i+1+xAmount]!=zero) ? i+1+xAmount:minOne;

        int three1 = (sortedWorld[i+xSquare]!=zero) ? i+xSquare:minOne;
        int four1 = (sortedWorld[i+1+xSquare]!=zero) ? i+1+xSquare:minOne;
        int four2 = (sortedWorld[i+xAmount+xSquare]!=zero) ? i+xAmount+xSquare:minOne;
        int three2 = (sortedWorld[i+1+xAmount+xSquare]!=zero) ? i+1+xAmount+xSquare:minOne;

        int[] cubeConnect = new int[]{one1,one2,two1,two2,three1,three2,four1,four2};
        int count =0;
        foreach (int lol in cubeConnect){
            if (lol!=minOne) count++;
        }
        switch (count){
            case 8:          
            break;
            case 7:
            break;
            case 6:
            break;
            case 5:
            break;
            case 4:
            break;
            case 3:
            break;

        }


        if (i%xAmount == xAmount-1-1) i += 1;
        if (i!= 0 && i%(xSquare-xAmount-1)==0) {i += 2*xAmount+1;} else i+=1; 
        }
    }
    // string[] chosenCorner = new string[]{
    //     "1342","1573","1562",  "1782", "1584", "1386",
    //     "176","174","146",
    //      };
    void cubeCorners(string bottom,string top){

        for (int corner = 1;corner<(bottom+top).Length-1;){
        string down = (corner>4) ? top:bottom;
        string up = (corner>4) ? bottom:top;

        while (down[0] != $"{corner}"[0] && up[0] != $"{corner}"[0]){
            char first = down[0];
            char second = up[0];
            down = down.Replace($"{first}","") + first;
            up = up.Replace($"{second}","") + second;

        }
        string l = $"{down}{up}";
        string[] chosenCorner = new string[]{
        $"{l[0]}{l[1]}{l[2]}{l[3]}", //Straight
        $"{l[0]}{l[4]}{l[5]}{l[1]}", //Straight
        $"{l[0]}{l[4]}{l[7]}{l[3]}", //Straight
        $"{l[0]}{l[5]}{l[6]}{l[3]}", //Cross
        $"{l[0]}{l[4]}{l[6]}{l[2]}", //Cross
        $"{l[0]}{l[1]}{l[6]}{l[7]}", //Cross
        $"{l[0]}{l[5]}{l[7]}",  //CrossThree
        $"{l[0]}{l[5]}{l[2]}",  //CrossThree
        $"{l[0]}{l[2]}{l[7]}",  //CrossThree
         };

        allCorners.Add(chosenCorner);
        corner +=1;
        }

    }

    void loadFile(string binaryWorld){
        for(int i = 0; i<binaryWorld.Length;i++){
            char binaryChar = binaryWorld[i];
            bool zeroOrOne = binaryChar == '0' || binaryChar == '1';
            if (zeroOrOne) sortedWorld += binaryChar;
        }
    }

    int[] createTriangles(
        int[] triangleList, int a, int b, int c
        ){
        int size = triangleList.Length;
        int[] newList = new int[size + 3];
        for (int i=0; i<size; i++){
            newList[i] = triangleList[i];
        }
        if (size != 0) 
            newList[size] = a;  
        else 
            newList[0] = a;

        newList[size+1] = b;
        newList[size+2] = c;
        
        return newList;
    }

    Vector3[] createVertices(int xAmount, int distance){
        int worldSpace = (int) Mathf.Pow(xAmount,3);
        Vector3[] vertices = new Vector3[worldSpace];

        int index = 0;
        for (int y=0; y<xAmount; y++){
            for (int z=0; z<xAmount; z++){   
                for (int x=0; x<xAmount; x++){     
                    vertices[index] = 
                        new Vector3(x,y,z)*distance;
                    index +=1;
                }
            }
        }
        return vertices;
    }   

    void renderVertices(){
        for(int i = 0; i<vertices.Length;i++){
        GameObject go = GameObject.Instantiate(points);
        go.transform.position = vertices[i]/xAmount;
        }
    }

    void printList(Vector3[] list){
        string str = "";
        for (int i=0; i<list.Length; i++){
            str += $"\nid {i}: {list[i]}";
    }
    print($"allVertices: {str}");
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.W)){
            vertices[26].y -=1;
            print("lol");
         mesh.vertices = vertices;
        }
        

    }
}

//shader.GetComponent<MeshRenderer>().material.SetFloat("_Mode", 3); // Transparent
//        cube.transform.localScale = new Vector3(0.75f,0.75f,0.75f);

    // void initilizeCube(){
    //     cubeColor = new Material(Shader.Find("Standard"));
    //     cubeColor.SetOverrideTag("RenderType", "Transparent");
    //     cubeColor.SetFloat("_Mode", 3);
	// 	cubeColor.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
	// 	cubeColor.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
	// 	cubeColor.EnableKeyword("_ALPHABLEND_ON");
    //     cubeColor.SetColor("_Color", new Color(0,1,0,0.3f));
	// 	cubeColor.renderQueue = 3000;  
    //     cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //     cube.GetComponent<Renderer>().material = cubeColor;
    //     cube.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
    // }

        // vertices[0] = new Vector3(0,0,0);
        // vertices[1] = new Vector3(0,100,0);
        // vertices[2] = new Vector3(100,100,0);
        // vertices[3] = new Vector3(100,100,100);
        // vertices[4] = new Vector3(100,200,100);
        // vertices[5] = new Vector3(200,200,100);
        // triangles[0] = 0;
        // triangles[1] = 1;
        // triangles[2] = 2;
        // triangles[3] = 2;
        // triangles[4] = 1;
        // triangles[5] = 3;
        // uv[0] = new Vector2(0,0);
        // uv[1] = new Vector2(0,1);
        // uv[2] = new Vector2(1,1);

        
        // vertices[0] = new Vector3(0,0,0);
        // vertices[1] = new Vector3(0,100,0);
        // vertices[2] = new Vector3(100,100,0);
        // vertices[3] = new Vector3(100,100,100);
        // triangles[0] = 3;
        // triangles[1] = 0;
        // triangles[2] = 2;
        // triangles[3] = 3;
        // triangles[4] = 0;
        // triangles[5] = 2;

                // for (int index = 0; index < amount; index = index + 1) {
        //     float x = Random.Range(0f,200f);
        //     float y = Random.Range(0f,200f);
        //     float z = Random.Range(0f,200f);
        //     vertices[index] = new Vector3(x,y,0);
        // }

    //       void randomTriangle(
    //     int[] triangles,
    //     int amount,
    //     int aMin, int aMax,
    //     int bMin, int bMax,
    //     int cMin, int cMax
    //     ){
    //     for (int i = 0;i<amount; i++){

    //         triangles = createTriangles(
    //             triangles,
    //             UnityEngine.Random.Range(aMin,aMax),
    //             UnityEngine.Random.Range(bMin,bMax),
    //             UnityEngine.Random.Range(cMin,cMax)
    //                 );
    //         }
            
    // }

        // binaryList = new List<List<List<int>>>(){
        //     new List<List<int>>{  
        //         new List<int>{1},new List<int>{1},new List<int>{2},
        //         new List<int>{1},new List<int>{0},new List<int>{0},
        //         new List<int>{2},new List<int>{0},new List<int>{0}
        //     },
        //     new List<List<int>>{  
        //         new List<int>{0},new List<int>{0},new List<int>{0},
        //         new List<int>{0},new List<int>{0},new List<int>{0},
        //         new List<int>{0},new List<int>{0},new List<int>{0}
        //     },
        //     new List<List<int>>{  
        //         new List<int>{0},new List<int>{0},new List<int>{0},
        //         new List<int>{0},new List<int>{0},new List<int>{0},
        //         new List<int>{0},new List<int>{0},new List<int>{2}
        //     },
        //     };
      
        // string bottom = $"{a}{c}{d}{b}";
        // string top = $"{e}{f}{g}{h}";
        // string left = $"{a}{e}{c}{g}";
        // string right = $"{b}{f}{d}{h}";
        // string back = $"{b}{a}{e}{f}";
        // string front = $"{c}{d}{g}{h}";
        // string crossLeft = $"{a}{e}{d}{h}";
        // string crossRight = $"{b}{f}{g}{c}";

        // int a = (sortedWorld[i]-zero==1) ? i:0;
        // int b = (sortedWorld[i+1]-zero==1) ? i+1:0;
        // int c = (sortedWorld[i+xAmount-1]-zero==1) ? i+xAmount-1:0;
        // int d = (sortedWorld[i+1+xAmount-1]-zero==1) ? i+1+xAmount-1:0;

        // int e = (sortedWorld[i+xSquare-1]-zero==1) ? i+xSquare-1:0;
        // int f = (sortedWorld[i+1+xSquare-1]-zero==1) ? i+1+xSquare-1:0;
        // int g = (sortedWorld[i+xAmount-1+xSquare-1]-zero==1) ? i+xAmount-1+xSquare-1:0;
        // int h = (sortedWorld[i+1+xAmount-1+xSquare-1]-zero==1) ? i+1+xAmount-1+xSquare-1:0;
        
        // string top2 =    $"{7}{8}";
        // string top1 =    $"{5}{6}";

        // string bottom2 = $"{3}{4}";
        // string bottom1 = $"{1}{2}";

        //1342  zx
        //5786  zx
        //2376  zx   cross
        //1485  zx   cross

        //1573  yz
        //2684  yz
        //1782  yx   cross
        //3465  yx   cross

        //1562  yx
        //3784  yx
        //1386  yx   cross
        //2574  yx   cross

        //176
        //257
        //375
        //467

        //532
        //614
        //741
        //823

        // string[] sideOfCube = new string[]{
        //     "1386","1342","1584","1573","1562","1782", "176","174","164",
        //     };

    //         void loadTriangles(){
    
    // for(int i = 0; i<sortedWorld.Length-2*xAmount-xSquare;){

    //     int one1 = (sortedWorld[i]!=zero) ? i:minOne;
    //     int two1 = (sortedWorld[i+1]!=zero) ? i+1:minOne;
    //     int two2 = (sortedWorld[i+xAmount]!=zero) ? i+xAmount:minOne;
    //     int one2 = (sortedWorld[i+1+xAmount]!=zero) ? i+1+xAmount:minOne;

    //     int three1 = (sortedWorld[i+xSquare]!=zero) ? i+xSquare:minOne;
    //     int four1 = (sortedWorld[i+1+xSquare]!=zero) ? i+1+xSquare:minOne;
    //     int four2 = (sortedWorld[i+xAmount+xSquare]!=zero) ? i+xAmount+xSquare:minOne;
    //     int three2 = (sortedWorld[i+1+xAmount+xSquare]!=zero) ? i+1+xAmount+xSquare:minOne;

    //     int[] cubeConnect = new int[]{one1,one2,two1,two2,three1,three2,four1,four2};
        
    //     for (int current = 0; current <cubeConnect.Length;){
    //         int vertices1 = cubeConnect[current];
    //         int vertices2 = cubeConnect[current+1];

    //         if (vertices1 != minOne && vertices2 !=minOne){
    //             for (int check = current+2; check <cubeConnect.Length;){ 
    //                     int vertices3 = cubeConnect[check];
    //                     if (vertices3 !=minOne) {
    //                     triangles = createTriangles(
    //                         triangles,vertices1,vertices2,vertices3
    //                         );
    //                     }
    //                 check +=1;
    //             }
    //         }
    //             current+=2;
    //     }
    //     if (i%xAmount == xAmount-1-1) i += 1;
    //     if (i!= 0 && i%(xSquare-xAmount-1)==0) {i += 2*xAmount+1;} else i+=1; 
    //     }
    // }


                //1342
            //1573
            //1562

            //2134
            //2684
            //2156

            //3157
            //3421
            //3784

            //4268
            //4378
            //4213

            //5786
            //5731
            //5621

            //6578
            //6215
            //6842

            //7865
            //7834
            //7315
            
            //

                // string[] sideOfCube = new string[]{
    //     "1386","1342","1584","1573","1562","1782", "176","174","164",
    //     };

    //12563
    

        // 1532
        // 2614
        // 3741
        // 4823
        // 5176
        // 6258
        // 7385
        // 8467
