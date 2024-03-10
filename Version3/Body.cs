using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3Int[] chest = new Vector3Int[]{
         new Vector3Int(4,4,5),new Vector3Int(4,4,4)
         };
    public static Vector3Int move = new Vector3Int(0,0,1); 
    public static Vector3Int[] tempChest;     
    void Start(){
    }

    // Update is called once per frame
    float time = 0;
    int i = 0;
    void Update(){
        time += Time.deltaTime;
        if (time > 0.1f){
            float cos = Mathf.Cos(i*Mathf.PI/180);
            float sin = Mathf.Sin(i*Mathf.PI/180);
            int y = (Mathf.Sign(cos)>0)?
                    (int)(2*cos +0.5f):
                        (int)(2*cos -0.5f);
            int x = (Mathf.Sign(sin)>0)?
                    (int)(2*sin +0.5f):
                        (int)(2*sin -0.5f);
            print($"{x}:{y}");
            WorldBuilder.createOrDelete(
                WorldBuilder.setVectorInBoundry(
                    chest[0],new Vector3Int(x,y,0)
                    ),true
                );
            time = 0;
            i++;
        }
    }
}
