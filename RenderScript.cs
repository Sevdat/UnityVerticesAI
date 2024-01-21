using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEditor.PackageManager;
using UnityEngine;

public class RenderScript : MonoBehaviour
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
    List<string[]> allCorners;
    

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
        
        allCorners = cubeCorners(bottom,top);
        renderVertices();
        loadTriangles();  
        renderTriangles(mesh,vertices,triangles);

        printAll(allCorners);
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

        // 6 7 (Top)(Front) (6:Left) (7:Right)
        // 4 5 (Top)(Back)  (4:Left) (5:Right)

        // 2 3 (Bottom)(Front) (2:Left) (3:Right)
        // 0 1 (Bottom)(Back)  (0:Left) (1:Right) 

    //0  (c,d) = left\  (e,f) = back/  (g,h) = back/
    //1  (c,d) = back\  (e,f) = right/ (g,h) = right/
    //2  (c,d) = front\ (e,f) = left/  (g,h) = left/  
    //3  (c,d) = right\ (e,f) = front/ (g,h) = front/

    //      Botom      Left      Back     FrontCross
static int a=0,b=2,  c=4,d=6,  e=8,f=10,  g=12,h=14,
    //      DiagonalCross      RightCross
              i=16,j=18,       k=20,l=22,
    //UpperCrossThree  BottomCrossThree  RightCrossThree
           m=24,            n=26,             o=28;

string s(int index, int num){
    return allCorners[index][num];
}

    void loadTriangles(){
        Dictionary<string,string[]> sides = new Dictionary<string, string[]>(){
            // 8 connections (All connected)
            {"12345678",new string[]{
                s(0,a),s(0,b),s(0,c),s(0,d),s(0,e),s(0,f),
                s(7,a+1),s(7,b+1),s(7,c+1),s(7,d+1),s(7,e+1),s(7,f+1)
                }},
            // 7 connections (Top missing)
            {"1234678",new string[]{
                s(0,a),s(0,b),s(0,c),s(0,f),
                s(6,b+1),s(7,c+1),s(7,d+1),s(7,e+1),s(7,f+1),s(5,n+1)      
            }},
            {"1234578",new string[]{
                s(0,a),s(0,b),s(0,c),s(1,c),s(0,d),s(1,f),
                s(7,a+1),s(7,e+1),s(7,f+1),s(7,n+1)
            }},
            {"1234568",new string[]{
                s(0,a),s(0,b),s(0,e),s(0,f),s(2,f),
                s(7,b+1),s(7,c+1),s(7,d+1),s(7,e+1),s(7,o+1)            
            }},
            {"1234567",new string[]{
                s(0,a),s(0,b),s(0,c),s(0,d),s(0,e),s(0,f),
                s(5,b+1),s(5,e+1),s(6,d+1),s(6,n+1)
            }},
            // 7 connections (Bottom missing)
            {"2345678",new string[]{
                s(1,d),s(1,a),s(2,e),s(2,o),
                s(7,a+1),s(7,b+1),s(7,c+1),s(7,d+1),s(7,e+1),s(7,f+1)
                }},
            {"1345678",new string[]{
                s(0,b),s(0,c),s(0,d),s(0,e),s(0,o),
                s(7,a+1),s(7,b+1),s(7,e+1),s(7,f+1),s(5,f+1)
                }},
            {"1245678",new string[]{
                s(0,a),s(0,d),s(0,e),s(0,f),s(0,n),
                s(7,a+1),s(7,b+1),s(6,c+1),s(7,c+1),s(7,d+1)
                }},
            {"1235678",new string[]{
                s(1,b),s(0,c),s(0,d),s(0,e),s(0,f),
                s(7,a+1),s(7,b+1),s(7,c+1),s(7,f+1),s(7,m+1)
                }},
            // 6 connections (Top Horizontal missing)
            {"123478",new string[]{
                s(0,a),s(0,b),s(1,k),s(1,l),
                s(7,d+1),s(6,e+1),s(7,e+1),s(7,f+1)
                }},
            {"123456",new string[]{
                s(0,a),s(0,b),s(0,e),s(0,f),s(2,f),s(2,k),s(2,l),
                s(5,e+1)
                }},
            {"123457",new string[]{
                s(0,a),s(0,b),s(0,c),s(0,d),s(1,c),s(4,k),s(4,l),
                s(6,d+1)
                }},
            {"123468",new string[]{
                s(0,a),s(0,b),s(0,f),s(2,c),s(5,g),s(5,h),
                s(7,c+1),s(7,d+1)
                }},
            // 6 connections (Bottom Horizontal missing)
            {"345678",new string[]{
   /*k,l*/      s(2,k+1),s(2,l+1),s(3,d),s(2,e),
                s(7,a+1),s(7,b+1),s(7,e+1),s(7,f+1)
                }},
            {"125678",new string[]{
   /*k,l*/      s(0,d),s(0,e),s(1,e),s(0,f),s(1,k+1),s(1,l+1),
                s(7,a+1),s(7,b+1)
                }},
            {"135678",new string[]{
   /*g,h*/      s(0,c),s(0,d),s(2,g+1),s(2,h+1),s(0,e),
                s(7,a+1),s(7,b+1),s(7,f+1)
                }},
            {"245678",new string[]{
   /*k,l*/      s(1,d),s(3,k+1),s(3,l+1),
                s(6,c+1),s(7,a+1),s(7,b+1),s(7,c+1),s(7,d+1)
                }},
            // 6 connections (Cross Horizontal missing)
            {"123467",new string[]{ // s(0,a),s(0,b),s(0,c),s(0,f),s(3,c),s(3,f),s(0,n+1),s(0,o+1)
                s(0,a),s(0,b),s(0,c),s(0,f),s(3,c),s(3,f),
                s(6,o+1),s(6,n+1)
                }},
            {"123458",new string[]{
                s(0,a),s(0,b),s(1,c),s(1,m),s(2,m),
                s(4,d+1),s(1,f),s(7,e+1)
                }},
            {"235678",new string[]{
                s(1,d),s(1,e),s(2,d),s(2,e),s(2,n),s(2,o),
                s(7,a+1),s(7,b+1)
                }},
            {"145678",new string[]{
                s(0,d),s(0,e),s(0,n),s(0,o),s(3,e),
                s(5,f+1),s(7,a+1),s(7,b+1)
                }},
            // 6 connections (Cross Diagonal missing)
            {"134568",new string[]{
                s(0,b),s(0,e),s(0,o),s(2,f),s(2,m),s(2,c),s(3,d),
                s(4,a+1)
                }},
            {"123678",new string[]{
                s(0,c),s(0,f),s(0,m),s(1,b),s(1,e),s(1,o),
                s(6,b+1),s(7,f+1)
                }},
            {"124578",new string[]{
               s(0,a),s(0,d),s(0,n),s(1,c),
                s(4,b+1),s(4,o+1),s(6,c+1),s(7,d+1)
                }},
            {"234567",new string[]{
               s(1,d),s(2,b),s(2,e),s(3,f),s(2,o),s(3,c),s(3,m),
               s(5,b+1)
                }},
            // 6 connections (Vertiacal Cross missing)
            {"234578",new string[]{
               s(2,o),s(1,f),s(1,a),
                s(7,e+1),s(7,f+1),s(4,c+1),s(4,b+1),s(4,o+1),
                }},
            {"134678",new string[]{
               s(0,c),s(0,o),s(0,b),s(2,c),s(2,d),
               s(5,f+1),s(6,b+1),s(6,o+1)
                }},
            {"124567",new string[]{
                s(0,a),s(0,d),s(0,e),s(0,f),s(3,c),
                s(6,n+1),s(6,a+1),s(6,m+1),
                }},
            {"123568",new string[]{
                s(0,e),s(0,f),s(1,e),s(2,a),s(2,f),s(2,n),
                s(4,a+1),s(4,n+1)
                }},
            {"134567",new string[]{
                s(0,b),s(0,c),s(0,d),s(0,e),s(0,o),
                s(5,b+1),s(6,d+1),s(6,n+1)
                }},
            {"123578",new string[]{
                s(0,c),s(0,d),s(1,c),s(1,o),s(2,d),s(1,b),
                s(4,b+1),s(4,o+1)
                }},
            {"124678",new string[]{
                s(0,a),s(0,f),s(0,m),s(0,n),s(1,e),s(1,f),s(3,e),
                s(6,b+1)
                }},
            {"234568",new string[]{
                s(1,a),s(1,e),s(1,f),s(1,d),s(2,c),
                s(4,a+1),s(4,m+1),s(4,n+1)
                }},
            // 6 connections (Vertiacal missing)
            {"134578",new string[]{
                s(0,b),s(0,i),s(0,j),s(0,c),s(0,d),s(2,c),s(2,d),
                s(7,a+1)
                }},
            {"123567",new string[]{
                s(0,c),s(0,d),s(0,e),s(0,f),s(1,i),s(1,j),s(2,a),
                s(6,a+1)
                }},
            {"124568",new string[]{
   /*i,j*/      s(0,a),s(0,e),s(0,f),s(0,i+1),s(0,j+1),
                s(4,a+1),s(7,c+1),s(7,d+1)
                }},
            {"234678",new string[]{
   /*i,j*/      s(1,i+1),s(1,j+1),s(1,a),s(2,c),s(2,d),
                s(5,a+1),s(7,c+1),s(7,d+1)
                }},
            // 5 connections (Horizontal missing)
            {"12345",new string[]{
                s(0,a),s(0,b),s(2,b+1),s(2,o+1),
                s(4,d+1),s(4,e+1)
                }},
            {"12346",new string[]{
   /*b  */      s(2,a),s(2,b),s(0,b+1),s(0,f),s(3,c),
                s(5,m)
                }},
            {"12348",new string[]{
                s(0,a),s(0,b),s(2,+1),s(2,n+1),
                s(7,d+1),s(7,e+1)
                }},
            {"12347",new string[]{
                s(0,a+1),s(0,c),s(0,n+1),s(1,a),s(1,b),
                s(6,d+1)
                }},
        };
// 1100000000
// 0100000000

// 0100000000
// 1100000000
    for(int i = 0; i<sortedWorld.Length-2*xAmount-xSquare;){

       int s1 = (sortedWorld[i]!=zero) ? i:minOne;
       int s2 = (sortedWorld[i+1]!=zero) ? i+1:minOne;
       int s3 = (sortedWorld[i+xAmount]!=zero) ? i+xAmount:minOne;
       int s4 = (sortedWorld[i+1+xAmount]!=zero) ? i+1+xAmount:minOne;

       int s5 = (sortedWorld[i+xSquare]!=zero) ? i+xSquare:minOne;
       int s6 = (sortedWorld[i+1+xSquare]!=zero) ? i+1+xSquare:minOne;
       int s7 = (sortedWorld[i+xAmount+xSquare]!=zero) ? i+xAmount+xSquare:minOne;
       int s8 = (sortedWorld[i+1+xAmount+xSquare]!=zero) ? i+1+xAmount+xSquare:minOne;
        
        int[] cubeConnect = new int[]{s1,s2,s3,s4,s5,s6,s7,s8};

        string validConnect = "";
        for (int num = 0; num < cubeConnect.Length;num++){
            int value = cubeConnect[num];
            if (value != minOne) validConnect +=$"{num+1}";
        }
        if (validConnect.Length==5){
        print(sides[validConnect]);
        string[] searchList = sides[validConnect];
        applyRule(cubeConnect,searchList);
        }
        
        if (i%xAmount == xAmount-1-1) i += 1;
        if (i!= 0 && i%(xSquare-xAmount-1)==0) i += xAmount+1; else i+=1; 
        }
    }
    void applyRule(int[] cubeConnect,string[] searchList){
        foreach(string i in searchList){
            int a = cubeConnect[(int)char.GetNumericValue(i[0])-1];
            int b = cubeConnect[(int)char.GetNumericValue(i[1])-1];
            int c = cubeConnect[(int)char.GetNumericValue(i[2])-1];
            triangles = createTriangles(
                    triangles, a, b, c
                );       
        }
    }

    List<string[]> cubeCorners(string bottom,string top){

        string down = bottom;
        string up =  top;
        bool once = true;
        
        List<string[]> allCorners = new List<string[]>();
        for (int corner = 1;corner<(bottom+top).Length+1;){
        if (corner>4 && once){
        down = top; up = bottom; once = false;
        }

        while (down[0] != $"{corner}"[0] && up[0] != $"{corner}"[0]){
            char first = down[0];
            char second = up[0];
            down = down.Replace($"{first}","") + first;
            up = up.Replace($"{second}","") + second;

        }
        string l = $"{down}{up}";
        //0123 4567
        //1342 5786
        string[] chosenCorner = new string[]{
        $"{l[0]}{l[3]}{l[2]}",$"{l[0]}{l[2]}{l[3]}", //Bottom
        $"{l[0]}{l[2]}{l[1]}",$"{l[0]}{l[1]}{l[2]}", //Bottom

        $"{l[0]}{l[1]}{l[5]}",$"{l[0]}{l[5]}{l[1]}", //Left
        $"{l[0]}{l[5]}{l[4]}",$"{l[0]}{l[4]}{l[5]}", //Left

        $"{l[0]}{l[4]}{l[7]}",$"{l[0]}{l[7]}{l[4]}", //Back
        $"{l[0]}{l[7]}{l[3]}",$"{l[0]}{l[3]}{l[7]}", //Back

        $"{l[0]}{l[5]}{l[6]}",$"{l[0]}{l[6]}{l[5]}", //FrontCross
        $"{l[0]}{l[6]}{l[3]}",$"{l[0]}{l[3]}{l[6]}", //FrontCross

        $"{l[0]}{l[4]}{l[6]}",$"{l[0]}{l[6]}{l[4]}", //DiagonalCross
        $"{l[0]}{l[6]}{l[2]}",$"{l[0]}{l[2]}{l[6]}", //DiagonalCross

        $"{l[0]}{l[1]}{l[6]}",$"{l[0]}{l[6]}{l[1]}", //RightCross
        $"{l[0]}{l[6]}{l[7]}",$"{l[0]}{l[7]}{l[6]}", //RightCross

        $"{l[0]}{l[5]}{l[7]}",$"{l[0]}{l[7]}{l[5]}", //UpperCrossThree
        $"{l[0]}{l[2]}{l[5]}",$"{l[0]}{l[5]}{l[2]}", //BottomCrossThree
        $"{l[0]}{l[7]}{l[2]}",$"{l[0]}{l[2]}{l[7]}", //RightCrossThree
        };

        allCorners.Add(chosenCorner);
        corner +=1;
        }
        return allCorners;
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
    void printAll(List<string[]> allCorners){
        foreach (Array i in allCorners){
        string a= "";
        string[] am = new string[]{
            "a ","a1","b "," b1","c "," c1",
            "d ","d1","e ","e1","f "," f1",
            "g ","g1"," h "," h1"," i "," i1",
            " j "," j1 "," k "," k1"," l "," l1",
            "m","m1","n ","n1","o ","o1"
            };
        int count = 0;
     foreach (string e in i){
        if (count != 0 && count % 6 ==0) a+= "\n";
        a+=$"{am[count]}={e}  ";
        count++;
    }
    print(a);
    }
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
    // new Pair('1',a),new Pair('2',b),new Pair('3',c),new Pair('4',d),
    // new Pair('5',e),new Pair('6',f),new Pair('7',g),new Pair('8',h)
