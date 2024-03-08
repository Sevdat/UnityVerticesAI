using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3Int[] chest = new Vector3Int[]{
         new Vector3Int(4,4,5),new Vector3Int(4,4,4)
         };
    public static Vector3Int[] move = new Vector3Int[]{
         new Vector3Int(0,0,1),new Vector3Int(0,0,1)
         }; 
    public static Vector3Int[] tempChest;     
    void Start(){
            tempChest = chest;
    }

    // Update is called once per frame
    float time = 0;
    void Update(){
        time += Time.deltaTime;
        if (time > 2f){
            WorldBuilder.createFromVector(chest,false);
            tempChest = WorldBuilder.createVector(chest,move);
            chest = tempChest;
            WorldBuilder.createFromVector(chest,true);
            time = 0;
        }
    }
}
