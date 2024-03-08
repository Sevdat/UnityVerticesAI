using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3Int[] chest = new Vector3Int[]{
         new Vector3Int(4,4,5),new Vector3Int(4,4,4),
         };
    void Start(){
            chest[0] = WorldBuilder.createVector(chest[0],new Vector3Int(0,0,0));
            chest[1] = WorldBuilder.createVector(chest[1],new Vector3Int(0,0,0));
    }

    // Update is called once per frame
    float time = 0;
    void Update(){
        time += Time.deltaTime;
        if (time > 2f){
            chest[0] = WorldBuilder.createVector(chest[0],new Vector3Int(0,0,1));
            chest[1] = WorldBuilder.createVector(chest[1],new Vector3Int(0,0,-1));
            time = 0;
        }
    }
}
