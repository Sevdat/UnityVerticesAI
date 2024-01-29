using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSides : MonoBehaviour
{
    string bottom = "1342";
    string top = "5786";
    List<string[]> allCorners;
    public static Dictionary<string,string[]> sides8;
    public static Dictionary<string,string[]> sides7;
    public static Dictionary<string,string[]> sides6;
    public static Dictionary<string,string[]> sides5;
    public static Dictionary<string,string[]> sides4;
    public static Dictionary<string,string[]> sides3;

    // Start is called before the first frame update
    void Awake()
    {
        allCorners = cubeCorners(bottom,top);
        createDictionary();
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

    // Update is called once per frame
    //      Botom      Left      Back     FrontCross
static int a=0,b=2,  c=4,d=6,  e=8,f=10,  g=12,h=14,
    //      DiagonalCross      RightCross
              i=16,j=18,       k=20,l=22,
    //UpperCrossThree  BottomCrossThree  RightCrossThree
           m=24,            n=26,             o=28;

string s(int index, int num){
    return allCorners[index][num];
}

void createDictionary(){

    sides8 = new Dictionary<string, string[]>(){
            // 8 connections (All connected)
            {"12345678",new string[]{
                s(0,a),s(0,b),s(0,c),s(0,d),s(0,e),s(0,f),
                s(7,a+1),s(7,b+1),s(7,c+1),s(7,d+1),s(7,e+1),s(7,f+1)
                }},
        };
    sides7 = new Dictionary<string, string[]>(){
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
    };

    sides6 = new Dictionary<string, string[]>(){
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
    };

    sides5 = new Dictionary<string, string[]>(){
        // 5 connections (Horizontal Top missing)
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
            // 5 connections (Horizontal Bottom missing)
            {"15678",new string[]{
                s(0,d),s(0,e),s(0,m+1),s(5,a),
                s(7,a+1),s(7,b+1)
                }},
            {"25678",new string[]{
                s(1,e),s(1,m+1),s(4,b),
                s(4,f+1),s(5,a+1),s(5,b+1)
                }},
            {"45678",new string[]{
                s(6,a),s(3,d),s(3,e),s(3,m+1),
                s(7,a+1),s(7,b+1)
                }},
            {"35678",new string[]{
                s(2,d),s(2,e),s(2,m+1),s(4,a),
                s(6,a+1),s(6,b+1)
                }},
            // 5 connections (Horizontal Left missing)
            {"12468",new string[]{
                s(0,a),s(0,o+1),s(0,f),s(1,e),s(1,f),
                s(5,f)
                }},
            {"23468",new string[]{
                s(1,a),s(3,c),s(3,d),
                s(7,m),s(7,c),s(7,e+1)
                }},  
            {"24678",new string[]{
                s(5,a+1),s(5,o),s(5,e),s(3,e),
                s(7,c+1),s(7,d+1)
                }},  
            {"24568",new string[]{
                s(1,f+1),s(3,c),s(3,d),s(4,o),s(4,a+1),
                s(4,f+1)
                }},  
            // 5 connections (Horizontal Right missing)
            {"12357",new string[]{
                s(0,c),s(0,d),s(1,b),s(1,n+1),s(1,c),s(2,e+1)
                }},  
            {"13457",new string[]{
                s(0,b),s(0,d+1),s(0,n+1),s(3,f),s(2,e),s(2,f)
                }},
            {"13567",new string[]{
                s(0,e),s(0,c+1),s(0,m+1),s(0,m+1),s(5,b+1),
                s(2,e),s(2,f)
                }},
            {"13578",new string[]{
                s(2,f+1),s(2,m+1),s(4,b+1),s(7,f+1),
                s(0,c),s(0,d)
                }},  
            // 5 connections (Bottom 2 left, Top 3)                
            {"13678",new string[]{
                s(0,m),s(0,c),s(2,d),s(2,g+1),
                s(5,a+1),s(5,g+1)
                }},
            {"13568",new string[]{
               s(0,e),s(2,f),s(2,g+1),s(2,h+1),s(2,m),
               s(4,a+1)
                }},
            {"24578",new string[]{
               s(1,m),s(1,f),s(3,e),
               s(4,b+1),s(4,k+1),s(4,l+1),
                }},
            {"24567",new string[]{
               s(3,m),s(1,d),s(3,c),
               s(4,k+1),s(4,l+1),s(6,a+1)
                }},
            // 5 connections (Bottom 2 up, Top 3)  
            {"12567",new string[]{
               s(0,e),s(0,f),s(0,d),s(1,i),s(1,k+1),
               s(6,a+1)
                }},                
            {"12578",new string[]{
               s(1,c),s(1,m),s(1,k+1),s(1,l+1),
               s(4,b+1),s(6,f+1)
                }},
            {"12568",new string[]{
               s(0,h+1),s(1,c),s(1,d),s(1,e),s(4,a+1),
               s(7,j+1)
                }},
            {"12678",new string[]{
               s(0,f),s(0,g+1),s(0,h+1),s(1,e),
               s(5,a+1),s(5,n+1)
                }},
            // 5 connections (Bottom 2 down, Top 3) 
            {"34678",new string[]{
               s(2,c),s(2,d),
               s(5,f+1),s(5,j),s(5,l+1),s(6,b+1)
                }},
            {"34578",new string[]{
               s(2,c),s(2,d),s(3,h+1),
               s(4,b+1),s(4,c+1),s(4,j+1)
                }},
            {"34567",new string[]{
               s(2,k+1),s(2,l+1),s(2,e),s(3,m),s(3,f),
               s(5,b+1)
                }},
            {"34568",new string[]{
               s(2,m),s(2,c),s(2,k+1),s(2,l+1),s(3,d),
               s(4,a+1)
                }},
            // 5 connections (Bottom 2 cross\, Top 3)
            {"14568",new string[]{
               s(0,e),s(0,i+1),s(0,j+1),s(0,o),
               s(3,d),s(4,a+1)
                }},
            {"14567",new string[]{
               s(0,o),s(0,d),s(0,e),s(0,n),s(3,m),
               s(5,b+1)
                }},
            {"14578",new string[]{
               s(0,i),s(0,j),s(0,n),s(0,d),s(3,e),
               s(4,b+1)
                }},
            {"14678",new string[]{
                s(0,m),s(0,n),s(0,o),s(3,e),s(3,d),
                s(6,b+1)
                }},
            // 5 connections (Bottom 2 cross/, Top 3)
            {"23678",new string[]{
                s(1,e),s(1,o),s(2,i),s(2,j),s(5,a+1),
                s(7,f+1)
                }},
            {"23578",new string[]{
                s(1,o),s(2,o),s(2,d),s(2,e),
                s(4,b+1),s(4,o+1),
                }},
            {"23568",new string[]{
                s(1,d),s(1,e),s(2,m),s(2,n),s(2,o),
                s(4,a+1)
                }},            
            {"23567",new string[]{
                s(1,n),s(2,i+1),s(2,j+1),s(2,e),s(1,d),
                s(5,b+1)
                }},    
            // 5 connections (Bottom 3, Top 2 left)
            {"23457",new string[]{
                s(1,g),s(1,h),s(1,a),s(2,o),s(2,e),
                s(3,f)
                }},
            {"12457",new string[]{
                s(0,a),s(1,g),s(1,h),s(3,o),
                s(6,f+1),s(4,e+1)
                }},
            {"13468",new string[]{
                s(0,b),s(0,o),s(0,k),s(0,l),s(2,c),s(3,d)
                }},            
            {"12368",new string[]{
                s(0,k),s(0,l),s(0,f),s(1,b),s(1,e),s(2,n)
                }},   
            // 5 connections (Bottom 3, Top 2 up) 
            {"23456",new string[]{
                s(1,a),s(1,d),s(2,k),s(2,l),s(2,o),
                s(5,e+1)
                }},               
            {"12456",new string[]{
                s(0,a),s(0,e),s(0,f),s(3,c),s(3,j),s(4,h)
                }},  
            {"13456",new string[]{
                s(0,b),s(0,e),s(0,o),s(2,f),s(2,k),s(2,l)
                }},
            {"12356",new string[]{
                s(0,e),s(0,f),s(1,b),s(2,f),s(2,l),
                s(5,i+1)
                }},
            // 5 connections (Bottom 3, Top 2 down)
            {"23478",new string[]{
                s(1,f),s(1,j+1),s(1,l),s(2,b),s(2,c),s(2,d)
                }},
            {"13478",new string[]{
                s(0,b),s(0,c),s(0,g),s(0,j),s(2,c),s(2,d)
                }},
            {"12478",new string[]{
                s(0,a),s(0,n),s(0,g),s(0,h),s(1,f),s(3,e)
                }},
            {"12378",new string[]{
                s(0,c),s(0,g),s(0,h),s(2,a),s(2,n),s(7,f+1)
                }},
            // 5 connections (Bottom 3, Top 2 cross)
            {"12358",new string[]{
                s(1,b),s(1,o),
                s(4,d+1),s(4,e+1),s(4,n+1),s(4,o+1)
                }},
            {"12458",new string[]{
                s(0,a),s(0,i+1),s(0,j+1),s(1,c),s(1,f),
                s(4,o+1)
                }},
            {"23458",new string[]{
                s(1,a),s(1,f),s(2,c),
                s(4,m+1),s(4,n+1),s(4,o+1)
                }},
            {"13458",new string[]{
                s(0,b),s(0,i),s(0,j),
                s(2,c),s(2,f),s(4,n+1)
                }},
            // 5 connections (Bottom 3, Top 2 cross)
            {"13467",new string[]{
               s(0,b),s(0,c),s(0,o),s(3,f),
               s(6,n+1),s(6,o+1)
                }},
            {"23467",new string[]{
               s(2,b),s(2,i),s(2,j),s(3,c),s(3,f),s(3,m)
                }},
            {"12467",new string[]{
               s(0,a),s(0,f),s(3,c),s(3,o),
               s(6,n+1),s(6,o+1)
                }},
            {"12367",new string[]{
               s(0,c),s(0,f),s(0,m),s(2,a),
               s(5,i+1),s(5,j+1)
                }},
    };

    sides4 = new Dictionary<string, string[]>(){
            // 4 connections (Bottom 4, Top 0)
            {"1234",new string[]{
                s(0,a),s(0,b),s(1,a+1),s(1,b+1)
                }},
            // 4 connections (Bottom 3, Top 1)
            {"1235",new string[]{
               s(1,b),s(1,c),s(1,n+1),s(2,f)
                }},
            {"1236",new string[]{
               s(0,f),s(1,b),s(2,j+1),s(5,g)
                }},                
            {"1238",new string[]{
               s(0,h),s(0,k),s(1,b),s(2,n)
                }},
            {"1237",new string[]{
               s(0,c),s(1,b),s(1,j),s(6,l)
                }},
                
            {"1245",new string[]{
               s(0,a),s(1,c),s(3,j),s(3,k)
                }},
            {"1246",new string[]{
               s(0,a),s(0,f),s(0,o+1),s(5,e+1)
                }},
            {"1248",new string[]{
               s(0,a),s(0,h),s(0,j+1),s(1,f)
                }},
            {"1247",new string[]{
               s(0,a),s(0,n),s(1,k),s(1,h)
                }},

            {"2347",new string[]{
               s(1,a),s(1,h),s(1,j+1),s(3,f)
                }},
            {"2348",new string[]{
               s(1,a),s(1,f),s(2,n+1),s(2,c)
                }},
            {"2346",new string[]{
               s(2,b),s(2,k),s(5,e+1),s(5,i)
                }},
            {"2345",new string[]{
               s(2,b),s(2,o),s(3,h),s(3,k)
                }},

            {"1345",new string[]{
               s(0,b),s(2,f),s(3,h),s(4,i+1)
                }},
            {"1346",new string[]{
               s(0,b),s(0,o),s(2,k),s(5,g)
                }},
            {"1348",new string[]{
               s(0,j),s(0,k),s(0,b),s(2,c)
                }},
            {"1347",new string[]{
               s(0,b),s(0,c),s(0,n+1),s(3,f)
                }},
            // 4 connections (Bottom 2, Top 2)
            {"1357",new string[]{
                s(0,c),s(0,d),s(4,c),s(4,d)
                }},
            {"2357",new string[]{
               s(1,g),s(1,j),s(2,e),s(2,o)
                }},
            {"3457",new string[]{
               s(2,e),s(3,l),s(3,h+1),s(6,d+1)
                }},
            {"1457",new string[]{
               s(0,d),s(0,n),s(3,j+1),s(3,l)
                }},
            {"2457",new string[]{
               s(1,g),s(1,h),s(3,k+1),s(3,l+1)
                }},
            {"1257",new string[]{
               s(1,c),s(1,g),s(1,k+1),s(6,f+1)
                }},
    
            {"1367",new string[]{
               s(0,c),s(0,m),s(2,h+1),s(2,i+1)
                }},
            {"2367",new string[]{
               s(1,i),s(1,j),s(2,i),s(2,j)
                }},
            {"2467",new string[]{
               s(1,h+1),s(1,i+1),s(3,c),s(3,m)
                }},
            {"3467",new string[]{
               s(2,i),s(2,k+1),s(3,f),s(3,m)
                }},
            {"1467",new string[]{
               s(0,n),s(0,o),s(6,n+1),s(6,o+1)
                }},
            {"1267",new string[]{
               s(0,f),s(0,m),s(1,i),s(1,k+1)
                }},

            {"1368",new string[]{
               s(0,k),s(0,l),s(2,g+1),s(2,h+1)
                }},
            {"2368",new string[]{
               s(1,e),s(1,o),s(2,g),s(2,j)
                }},
            {"1268",new string[]{
               s(0,f),s(0,l),s(1,e),s(7,g+1)
                }},
            {"1468",new string[]{
               s(0,j+1),s(0,l),s(3,d),s(5,m+1)
                }},
            {"3468",new string[]{
               s(2,c),s(2,g),s(3,d),s(5,l+1)
                }},
            {"2468",new string[]{
               s(1,e+1),s(1,f+1),s(3,c),s(3,d),
                }},

            {"1358",new string[]{
               s(0,i),s(0,k+1),s(2,f),s(2,m)
                }},
            {"1458",new string[]{
               s(0,i),s(0,j),s(3,i),s(3,j)
                }},
            {"2358",new string[]{
               s(1,n),s(1,m),s(2,n),s(2,m)
                }},
            {"2458",new string[]{
               s(1,f),s(4,o+1),s(3,k+1),s(3,i)
                }},
            {"1258",new string[]{
               s(0,h+1),s(1,c),s(4,o+1),s(7,j+1)
                }},
            {"3458",new string[]{
               s(2,c),s(2,m),s(3,h+1),s(3,i+1)
                }},

            {"3456",new string[]{
               s(2,k),s(2,l),s(4,g+1),s(4,h+1)
                }},
            {"1356",new string[]{
               s(0,e),s(2,f),s(2,h+1),s(2,l)
                }},
            {"2356",new string[]{
               s(2,l),s(2,o),s(4,f+1),s(2,j+1)
                }},
            {"1256",new string[]{
               s(0,e),s(0,f),s(1,c+1),s(1,d+1)
                }},
            {"1456",new string[]{
               s(0,e),s(0,o),s(3,g),s(3,j)
                }},
            {"2456",new string[]{
               s(1,d),s(3,c),s(3,g),s(3,k+1)
                }},                

            {"2478",new string[]{
               s(1,f),s(1,h+1),s(1,l),s(3,e)
                }},
            {"1278",new string[]{
               s(0,g),s(0,h),s(1,k+1),s(1,l+1)
                }},
            {"3478",new string[]{
               s(2,c),s(2,d),s(3,e+1),s(3,f+1)
                }},
            {"1478",new string[]{
               s(3,e),s(0,j),s(0,n),s(7,h)
                }},
            {"1378",new string[]{
               s(0,c),s(0,g),s(0,k+1),s(2,d)
                }},
            {"2378",new string[]{
               s(1,l),s(1,j+1),s(2,d),s(2,n),
                }},
            // 4 connections (Bottom 1, Top 3)
            {"3567",new string[]{
               s(2,e),s(2,i+1),s(2,l+1),s(6,a+1)
                }},
            {"4567",new string[]{
               s(3,g+1),s(3,l+1),s(6,a+1),s(6,n+1)
                }},
            {"1567",new string[]{
               s(0,d),s(0,e),s(0,m+1),s(5,b+1)
                }},
            {"2567",new string[]{
               s(1,d),s(1,g+1),s(1,i),s(5,b+1)
                }},

            {"1568",new string[]{
               s(0,e),s(0,i+1),s(0,l+1),s(4,a+1)
                }},
            {"2568",new string[]{
               s(1,d),s(1,e),s(1,m+1),s(4,a+1)
                }},
            {"3568",new string[]{
               s(2,m),s(2,l+1),s(5,h+1),s(4,a+1)
                }},
            {"4568",new string[]{
               s(3,g+1),s(4,a+1),s(4,j),s(5,f+1)
                }},

            {"1678",new string[]{
               s(0,m),s(0,g+1),s(0,l+1),s(5,a+1)
                }},
            {"2678",new string[]{
               s(1,e),s(1,i+1),s(1,l+1),s(5,a+1)
                }},
            {"3678",new string[]{
               s(2,d),s(2,g+1),s(2,i),s(5,a+1)
                }},
            {"4678",new string[]{
               s(3,d),s(3,e),s(3,m+1),s(5,a+1)
                }},

            {"1578",new string[]{
               s(0,d),s(0,g+1),s(0,i),s(4,b+1)
                }},
            {"2578",new string[]{
               s(1,g+1),s(1,l+1),s(1,m),s(4,b+1)
                }},
            {"3578",new string[]{
               s(2,e),s(2,d),s(2,m+1),s(4,b+1)
                }},
            {"4578",new string[]{
               s(3,e),s(3,i+1),s(3,l+1),s(4,b+1)
                }},
            // 4 connections (Bottom 1, Top 3)
            {"5678",new string[]{
                s(6,a),s(6,b),s(7,a+1),s(7,b+1)
                }},
    };
    sides3 = new Dictionary<string, string[]>(){
            {"124",new string[]{
                s(0,a),s(0,a+1),
                }},
            {"134",new string[]{
                s(0,b),s(0,b+1),
                }},
            {"137",new string[]{
                s(0,c),s(0,c+1),
                }},
            {"157",new string[]{
                s(0,d),s(0,d+1),
                }},
            {"156",new string[]{
                s(0,e),s(0,e+1),
                }},
            {"126",new string[]{
                s(0,f),s(0,f+1),
                }},
            {"178",new string[]{
                s(0,g),s(0,g+1),
                }},
            {"128",new string[]{
                s(0,h),s(0,h+1),
                }},
            {"158",new string[]{
                s(0,i),s(0,i+1),
                }},
            {"148",new string[]{
                s(0,j),s(0,j+1),
                }},
            {"138",new string[]{
                s(0,k),s(0,k+1),
                }},
            {"168",new string[]{
                s(0,l),s(0,l+1),
                }},
            {"167",new string[]{
                s(0,m),s(0,m+1),
                }},
            {"147",new string[]{
                s(0,n),s(0,n+1),
                }},
            {"146",new string[]{
                s(0,o),s(0,o+1),
                }},
            {"234",new string[]{
                s(1,a),s(1,a+1),
                }},
            {"123",new string[]{
                s(1,b),s(1,b+1),
                }},
            {"125",new string[]{
                s(1,c),s(1,c+1),
                }},
            {"256",new string[]{
                s(1,d),s(1,d+1),
                }},
            {"268",new string[]{
                s(1,e),s(1,e+1),
                }},
            {"248",new string[]{
                s(1,f),s(1,f+1),
                }},
            {"257",new string[]{
                s(1,g),s(1,g+1),
                }},
            {"247",new string[]{
                s(1,h),s(1,h+1),
                }},
            {"267",new string[]{
                s(1,i),s(1,i+1),
                }},
            {"237",new string[]{
                s(1,j),s(1,j+1),
                }},
            {"127",new string[]{
                s(1,k),s(1,k+1),
                }},
            {"278",new string[]{
                s(1,l),s(1,l+1),
                }},
            {"258",new string[]{
                s(1,m),s(1,m+1),
                }},
            {"235",new string[]{
                s(1,n),s(1,n+1),
                }},
            {"238",new string[]{
                s(1,o),s(1,o+1),
                }},
            {"348",new string[]{
                s(2,c),s(2,c+1),
                }},
            {"378",new string[]{
                s(2,d),s(2,d+1),
                }},
            {"357",new string[]{
                s(2,e),s(2,e+1),
                }},
            {"135",new string[]{
                s(2,f),s(2,f+1),
                }},
            {"368",new string[]{
                s(2,g),s(2,g+1),
                }},
            {"136",new string[]{
                s(2,h),s(2,h+1),
                }},
            {"367",new string[]{
                s(2,i),s(2,i+1),
                }},
            {"236",new string[]{
                s(2,j),s(2,j+1),
                }},
            {"346",new string[]{
                s(2,k),s(2,k+1),
                }},
            {"356",new string[]{
                s(2,l),s(2,l+1),
                }},
            {"358",new string[]{
                s(2,m),s(2,m+1),
                }},
            {"246",new string[]{
                s(3,c),s(3,c+1),
                }},
            {"468",new string[]{
                s(3,d),s(3,d+1),
                }},
            {"478",new string[]{
                s(3,e),s(3,e+1),
                }},
            {"347",new string[]{
                s(3,f),s(3,f+1),
                }},
            {"456",new string[]{
                s(3,g),s(3,g+1),
                }},
            {"345",new string[]{
                s(3,h),s(3,h+1),
                }},
            {"458",new string[]{
                s(3,i),s(3,i+1),
                }},
            {"145",new string[]{
                s(3,j),s(3,j+1),
                }},
            {"245",new string[]{
                s(3,k),s(3,k+1),
                }},
            {"457",new string[]{
                s(3,l),s(3,l+1),
                }},
            {"467",new string[]{
                s(3,m),s(3,m+1),
                }},
            {"568",new string[]{
                s(4,a),s(4,a+1),
                }},
            {"578",new string[]{
                s(4,b),s(4,b+1),
                }},
            {"678",new string[]{
                s(5,a),s(5,a+1),
                }},
            {"567",new string[]{
                s(5,b),s(5,b+1),
                }},

    };

}
}
